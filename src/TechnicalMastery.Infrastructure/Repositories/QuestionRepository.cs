using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IQuestionRepository"/>.
/// All queries run server-side — callers never receive an untracked
/// <c>IQueryable</c>, so no database logic can leak into upper layers.
/// </summary>
public class QuestionRepository : IQuestionRepository
{
    private const int MaxPageSize = 100;

    private readonly AppDbContext context;

    public QuestionRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Task<Question?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.Questions
            .AsNoTracking()
            .Include(question => question.Topic)
                .ThenInclude(topic => topic.Category)
            .Include(question => question.Tags)
            .FirstOrDefaultAsync(question => question.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Question> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? categoryId,
        int? topicId,
        DifficultyLevel? difficulty,
        QuestionType? type,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken)
    {
        int safePage = Math.Max(page, 1);
        int safePageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        IQueryable<Question> query = this.context.Questions
            .AsNoTracking()
            .Include(question => question.Topic)
            .Where(question => question.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(question => question.Topic.CategoryId == categoryId.Value);
        }

        if (topicId.HasValue)
        {
            query = query.Where(question => question.TopicId == topicId.Value);
        }

        if (difficulty.HasValue)
        {
            query = query.Where(question => question.DifficultyLevel == difficulty.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(question => question.QuestionType == type.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = "%" + search.Trim() + "%";
            query = query.Where(question =>
                EF.Functions.Like(question.QuestionText, pattern) ||
                EF.Functions.Like(question.ShortAnswer, pattern) ||
                EF.Functions.Like(question.DetailedAnswer, pattern) ||
                EF.Functions.Like(question.Topic.Name, pattern) ||
                EF.Functions.Like(question.Topic.Category.Name, pattern) ||
                question.Tags.Any(tag => EF.Functions.Like(tag.Tag, pattern)));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        query = ApplyOrdering(query, sortBy, descending);

        List<Question> items = await query
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Question>> GetRandomAsync(
        int count,
        int? categoryId,
        DifficultyLevel? difficulty,
        CancellationToken cancellationToken)
    {
        int safeCount = Math.Clamp(count, 1, MaxPageSize);

        IQueryable<Question> query = this.context.Questions
            .AsNoTracking()
            .Include(question => question.Topic)
            .Where(question => question.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(question => question.Topic.CategoryId == categoryId.Value);
        }

        if (difficulty.HasValue)
        {
            query = query.Where(question => question.DifficultyLevel == difficulty.Value);
        }

        // SQLite has no efficient TABLESAMPLE; ORDER BY RANDOM() is fine at this scale.
        List<Question> questions = await query
            .OrderBy(question => EF.Functions.Random())
            .Take(safeCount)
            .ToListAsync(cancellationToken);

        return questions;
    }

    public async Task<IReadOnlyList<Question>> GetRelatedAsync(int questionId, int count, CancellationToken cancellationToken)
    {
        int safeCount = Math.Clamp(count, 1, MaxPageSize);

        Question? source = await this.context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(question => question.Id == questionId, cancellationToken);

        if (source is null)
        {
            return Array.Empty<Question>();
        }

        List<Question> related = await this.context.Questions
            .AsNoTracking()
            .Include(question => question.Topic)
            .Where(question => question.IsActive &&
                question.Id != questionId &&
                question.TopicId == source.TopicId)
            .OrderBy(question => question.Id)
            .Take(safeCount)
            .ToListAsync(cancellationToken);

        return related;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return this.context.Questions.CountAsync(question => question.IsActive, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.Questions.AnyAsync(question => question.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<(int CategoryId, int Total, int Completed)>> GetCategoryStatsAsync(CancellationToken cancellationToken)
    {
        var rows = await this.context.Questions
            .AsNoTracking()
            .Where(question => question.IsActive)
            .Select(question => new
            {
                CategoryId = question.Topic.CategoryId,
                Completed = question.Progress != null && question.Progress.Status == StudyStatus.Completed
            })
            .ToListAsync(cancellationToken);

        List<(int CategoryId, int Total, int Completed)> stats = rows
            .GroupBy(row => row.CategoryId)
            .Select(group => (group.Key, group.Count(), group.Count(row => row.Completed)))
            .OrderBy(stat => stat.Item1)
            .ToList();

        return stats;
    }

    private static IQueryable<Question> ApplyOrdering(IQueryable<Question> query, string? sortBy, bool descending)
    {
        string key = (sortBy ?? "id").Trim().ToLowerInvariant();

        if (key == "difficulty" && !descending)
        {
            return query.OrderBy(question => question.DifficultyLevel).ThenBy(question => question.Id);
        }

        if (key == "difficulty" && descending)
        {
            return query.OrderByDescending(question => question.DifficultyLevel).ThenBy(question => question.Id);
        }

        if (key == "newest" && !descending)
        {
            return query.OrderBy(question => question.CreatedAt).ThenBy(question => question.Id);
        }

        if (key == "newest" && descending)
        {
            return query.OrderByDescending(question => question.CreatedAt).ThenBy(question => question.Id);
        }

        if (descending)
        {
            return query.OrderByDescending(question => question.Id);
        }

        return query.OrderBy(question => question.Id);
    }
}
