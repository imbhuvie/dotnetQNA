using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Tests;

/// <summary>
/// Isolated SQLite database per test (temp file + real migrations + tiny seed).
/// Each test gets a fresh file, so tests never interfere and the dev database
/// is never touched. File is deleted on dispose.
/// </summary>
public sealed class TestDatabase : IDisposable
{
    private readonly string filePath;

    public AppDbContext Context { get; }

    public TestDatabase()
    {
        this.filePath = Path.Combine(Path.GetTempPath(), "tm_test_" + Guid.NewGuid().ToString("N") + ".db");

        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=" + this.filePath)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.Migrate();
        Seed(Context);
    }

    public int QuestionId(string text)
    {
        return Context.Questions.First(question => question.QuestionText == text).Id;
    }

    public void Dispose()
    {
        Context.Dispose();

        try
        {
            File.Delete(this.filePath);
        }
        catch (IOException)
        {
            // Best effort: the OS cleans temp files eventually.
        }
    }

    private static void Seed(AppDbContext context)
    {
        DateTime now = DateTime.UtcNow;

        Category category = new Category
        {
            Name = "TestCat",
            Description = "Test category",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        context.Categories.Add(category);
        context.SaveChanges();

        Topic first = new Topic
        {
            CategoryId = category.Id,
            Name = "T1",
            Description = "First topic",
            DisplayOrder = 1,
            IsActive = true
        };
        Topic second = new Topic
        {
            CategoryId = category.Id,
            Name = "T2",
            Description = "Second topic",
            DisplayOrder = 2,
            IsActive = true
        };
        context.Topics.AddRange(first, second);
        context.SaveChanges();

        context.Questions.Add(new Question
        {
            TopicId = first.Id,
            QuestionText = "What is a variable?",
            ShortAnswer = "Named storage.",
            DetailedAnswer = "Detailed variables.",
            DifficultyLevel = DifficultyLevel.Beginner,
            QuestionType = QuestionType.Conceptual,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            Tags = new List<QuestionTag> { new QuestionTag { Tag = "basics" } }
        });
        context.Questions.Add(new Question
        {
            TopicId = first.Id,
            QuestionText = "What is boxing?",
            ShortAnswer = "Heap wrap.",
            DetailedAnswer = "Detailed boxing.",
            DifficultyLevel = DifficultyLevel.Intermediate,
            QuestionType = QuestionType.CodeBased,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            Tags = new List<QuestionTag> { new QuestionTag { Tag = "boxing" }, new QuestionTag { Tag = "performance" } }
        });
        context.Questions.Add(new Question
        {
            TopicId = second.Id,
            QuestionText = "How do you debug a deadlock?",
            ShortAnswer = "Dumps first.",
            DetailedAnswer = "Detailed deadlock.",
            DifficultyLevel = DifficultyLevel.Advanced,
            QuestionType = QuestionType.Debugging,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            Tags = new List<QuestionTag> { new QuestionTag { Tag = "threads" } }
        });
        context.SaveChanges();
    }
}
