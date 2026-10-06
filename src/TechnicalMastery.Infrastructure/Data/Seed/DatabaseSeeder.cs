using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Infrastructure.Data.Seed;

/// <summary>
/// Loads the embedded seed JSON (categories, topics, question batches) into
/// SQLite. Idempotent: categories/topics upsert by name, questions load only
/// when the table is empty — re-running never duplicates content.
/// <para>Runs pending EF migrations first (the migrations strategy from Phase 5,
/// applied automatically so any fresh checkout just works).</para>
/// </summary>
public class DatabaseSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly AppDbContext context;
    private readonly ILogger<DatabaseSeeder> logger;

    public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await this.context.Database.MigrateAsync(cancellationToken);

        int categoryCount = await SeedCategoriesAsync(cancellationToken);
        int topicCount = await SeedTopicsAsync(cancellationToken);
        int questionCount = await SeedQuestionsAsync(cancellationToken);

        this.logger.LogInformation(
            "Seeding complete: {Categories} categories, {Topics} topics, {Questions} questions total.",
            categoryCount, topicCount, questionCount);
    }

    private async Task<int> SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        List<SeedCategory> seeds = Load<List<SeedCategory>>("categories.json");
        DateTime now = DateTime.UtcNow;

        foreach (SeedCategory seed in seeds)
        {
            Category? existing = await this.context.Categories
                .FirstOrDefaultAsync(category => category.Name == seed.Name, cancellationToken);

            if (existing is null)
            {
                await this.context.Categories.AddAsync(new Category
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    DisplayOrder = seed.DisplayOrder,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }, cancellationToken);
            }
            else
            {
                existing.Description = seed.Description;
                existing.DisplayOrder = seed.DisplayOrder;
            }
        }

        await this.context.SaveChangesAsync(cancellationToken);

        return await this.context.Categories.CountAsync(cancellationToken);
    }

    private async Task<int> SeedTopicsAsync(CancellationToken cancellationToken)
    {
        List<SeedTopic> seeds = Load<List<SeedTopic>>("topics.json");

        Dictionary<string, int> categoryIds = await this.context.Categories
            .ToDictionaryAsync(category => category.Name, category => category.Id, cancellationToken);

        foreach (SeedTopic seed in seeds)
        {
            if (!categoryIds.TryGetValue(seed.Category, out int categoryId))
            {
                throw new InvalidOperationException("Seed topic '" + seed.Name + "' references unknown category '" + seed.Category + "'.");
            }

            int capturedCategoryId = categoryId;
            string capturedName = seed.Name;

            Topic? existing = await this.context.Topics.FirstOrDefaultAsync(
                topic => topic.CategoryId == capturedCategoryId && topic.Name == capturedName,
                cancellationToken);

            if (existing is null)
            {
                await this.context.Topics.AddAsync(new Topic
                {
                    CategoryId = categoryId,
                    Name = seed.Name,
                    Description = seed.Description,
                    DisplayOrder = seed.DisplayOrder,
                    IsActive = true
                }, cancellationToken);
            }
            else
            {
                existing.Description = seed.Description;
                existing.DisplayOrder = seed.DisplayOrder;
            }
        }

        await this.context.SaveChangesAsync(cancellationToken);

        return await this.context.Topics.CountAsync(cancellationToken);
    }

    private async Task<int> SeedQuestionsAsync(CancellationToken cancellationToken)
    {
        bool hasQuestions = await this.context.Questions.AnyAsync(cancellationToken);

        if (hasQuestions)
        {
            this.logger.LogInformation("Questions already seeded — skipping question batches.");
            return await this.context.Questions.CountAsync(cancellationToken);
        }

        // Any file under Seed/questions/*.json is a batch; batches accumulate toward 1000+.
        List<string> batchFiles = ListBatchFiles();
        DateTime now = DateTime.UtcNow;
        int inserted = 0;

        foreach (string batchFile in batchFiles)
        {
            List<SeedQuestion> seeds = LoadFromFile<List<SeedQuestion>>(batchFile);

            foreach (SeedQuestion seed in seeds)
            {
                Topic? topic = await this.context.Topics
                    .Include(item => item.Category)
                    .FirstOrDefaultAsync(
                        item => item.Name == seed.Topic && item.Category.Name == seed.Category,
                        cancellationToken);

                if (topic is null)
                {
                    throw new InvalidOperationException("Seed question '" + seed.QuestionText + "' references unknown topic '" + seed.Topic + "' in category '" + seed.Category + "'.");
                }

                Question question = new Question
                {
                    TopicId = topic.Id,
                    QuestionText = seed.QuestionText,
                    ShortAnswer = seed.ShortAnswer,
                    DetailedAnswer = seed.DetailedAnswer,
                    DifficultyLevel = ParseDifficulty(seed.DifficultyLevel, seed.QuestionText),
                    QuestionType = ParseType(seed.QuestionType, seed.QuestionText),
                    CodeExample = seed.CodeExample,
                    InternalWorking = seed.InternalWorking,
                    RealWorldUsage = seed.RealWorldUsage,
                    CommonMistake = seed.CommonMistake,
                    TechnicalConversation = seed.TechnicalConversation,
                    InterviewFollowUp = seed.InterviewFollowUp,
                    KeyTakeaway = seed.KeyTakeaway,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now,
                    Tags = seed.Tags
                        .Select(tag => tag.Trim().ToLowerInvariant())
                        .Where(tag => tag.Length > 0)
                        .Distinct()
                        .Select(tag => new QuestionTag { Tag = tag })
                        .ToList()
                };

                await this.context.Questions.AddAsync(question, cancellationToken);
                inserted++;
            }

            await this.context.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Seeded batch {Batch} ({Count} questions).", Path.GetFileName(batchFile), seeds.Count);
        }

        return inserted;
    }

    private static DifficultyLevel ParseDifficulty(string value, string questionText)
    {
        if (Enum.TryParse<DifficultyLevel>(value, ignoreCase: true, out DifficultyLevel level))
        {
            return level;
        }

        throw new InvalidOperationException("Seed question '" + questionText + "' has invalid difficulty '" + value + "'.");
    }

    private static QuestionType ParseType(string value, string questionText)
    {
        if (Enum.TryParse<QuestionType>(value, ignoreCase: true, out QuestionType type))
        {
            return type;
        }

        throw new InvalidOperationException("Seed question '" + questionText + "' has invalid type '" + value + "'.");
    }

    private static T Load<T>(string fileName)
    {
        Assembly assembly = typeof(DatabaseSeeder).Assembly;
        string resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Embedded seed file '" + fileName + "' was not found.");

        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Embedded seed file '" + fileName + "' could not be opened.");

        return JsonSerializer.Deserialize<T>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Embedded seed file '" + fileName + "' is empty or invalid.");
    }

    private static List<string> ListBatchFiles()
    {
        Assembly assembly = typeof(DatabaseSeeder).Assembly;

        return assembly.GetManifestResourceNames()
            .Where(name => name.Contains(".Seed.questions.", StringComparison.OrdinalIgnoreCase) && name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name)
            .ToList();
    }

    private static T LoadFromFile<T>(string resourceName)
    {
        Assembly assembly = typeof(DatabaseSeeder).Assembly;

        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Embedded seed batch '" + resourceName + "' could not be opened.");

        return JsonSerializer.Deserialize<T>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Embedded seed batch '" + resourceName + "' is empty or invalid.");
    }
}
