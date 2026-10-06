using FluentValidation;
using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Mappings;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Question query use cases. Every outward shape is enriched with the
/// current user's bookmark/progress state via two bulk lookups.
/// </summary>
public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository questions;
    private readonly ICategoryRepository categories;
    private readonly ITopicRepository topics;
    private readonly IBookmarkRepository bookmarks;
    private readonly IStudyProgressRepository progressEntries;
    private readonly IValidator<QuestionsQuery> queryValidator;

    public QuestionService(
        IQuestionRepository questions,
        ICategoryRepository categories,
        ITopicRepository topics,
        IBookmarkRepository bookmarks,
        IStudyProgressRepository progressEntries,
        IValidator<QuestionsQuery> queryValidator)
    {
        this.questions = questions;
        this.categories = categories;
        this.topics = topics;
        this.bookmarks = bookmarks;
        this.progressEntries = progressEntries;
        this.queryValidator = queryValidator;
    }

    public async Task<QuestionDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        Question? question = await this.questions.GetByIdAsync(id, cancellationToken);

        if (question is null)
        {
            throw new NotFoundException("Question", id);
        }

        bool isBookmarked = await this.bookmarks.ExistsAsync(id, cancellationToken);
        StudyProgress? progress = await this.progressEntries.GetByQuestionIdAsync(id, cancellationToken);

        return DtoMapper.ToDetailDto(question, isBookmarked, progress?.Status ?? StudyStatus.NotStarted);
    }

    public async Task<PagedResult<QuestionSummaryDto>> GetPagedAsync(QuestionsQuery query, CancellationToken cancellationToken)
    {
        await this.queryValidator.ValidateAndThrowAsync(query, cancellationToken);

        await ValidateFiltersAsync(query.CategoryId, query.TopicId, cancellationToken);

        (IReadOnlyList<Question> items, int totalCount) = await this.questions.GetPagedAsync(
            query.Page, query.PageSize, query.CategoryId, query.TopicId, query.Difficulty,
            query.Type, query.Search, query.SortBy, query.Descending, cancellationToken);

        List<QuestionSummaryDto> summaries = await ToSummariesAsync(items, cancellationToken);

        return DtoMapper.ToPagedResult(summaries, totalCount, query.Page, query.PageSize);
    }

    public async Task<IReadOnlyList<QuestionSummaryDto>> GetRandomAsync(
        int count,
        int? categoryId,
        DifficultyLevel? difficulty,
        CancellationToken cancellationToken)
    {
        ValidateCount(count);

        await ValidateFiltersAsync(categoryId, null, cancellationToken);

        IReadOnlyList<Question> items = await this.questions.GetRandomAsync(count, categoryId, difficulty, cancellationToken);

        return await ToSummariesAsync(items, cancellationToken);
    }

    public async Task<IReadOnlyList<QuestionSummaryDto>> GetRelatedAsync(int questionId, int count, CancellationToken cancellationToken)
    {
        ValidateCount(count);

        bool exists = await this.questions.ExistsAsync(questionId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Question", questionId);
        }

        IReadOnlyList<Question> items = await this.questions.GetRelatedAsync(questionId, count, cancellationToken);

        return await ToSummariesAsync(items, cancellationToken);
    }

    public Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        return this.questions.CountAsync(cancellationToken);
    }

    private static void ValidateCount(int count)
    {
        if (count is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be between 1 and 100.");
        }
    }

    private async Task<List<QuestionSummaryDto>> ToSummariesAsync(IReadOnlyList<Question> items, CancellationToken cancellationToken)
    {
        IReadOnlyList<int> bookmarkedIds = await this.bookmarks.GetBookmarkedQuestionIdsAsync(cancellationToken);
        IReadOnlyDictionary<int, StudyStatus> statusMap = await this.progressEntries.GetStatusMapAsync(cancellationToken);

        HashSet<int> bookmarked = new HashSet<int>(bookmarkedIds);

        List<QuestionSummaryDto> summaries = items
            .Select(question => DtoMapper.ToSummaryDto(
                question,
                bookmarked.Contains(question.Id),
                statusMap.TryGetValue(question.Id, out StudyStatus status) ? status : StudyStatus.NotStarted))
            .ToList();

        return summaries;
    }

    private async Task ValidateFiltersAsync(int? categoryId, int? topicId, CancellationToken cancellationToken)
    {
        if (categoryId.HasValue)
        {
            bool exists = await this.categories.ExistsAsync(categoryId.Value, cancellationToken);

            if (!exists)
            {
                throw new NotFoundException("Category", categoryId.Value);
            }
        }

        if (topicId.HasValue)
        {
            bool exists = await this.topics.ExistsAsync(topicId.Value, cancellationToken);

            if (!exists)
            {
                throw new NotFoundException("Topic", topicId.Value);
            }
        }
    }
}
