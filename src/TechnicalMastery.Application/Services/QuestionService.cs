using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Question query use cases.
/// </summary>
public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository questions;
    private readonly ICategoryRepository categories;
    private readonly ITopicRepository topics;

    public QuestionService(
        IQuestionRepository questions,
        ICategoryRepository categories,
        ITopicRepository topics)
    {
        this.questions = questions;
        this.categories = categories;
        this.topics = topics;
    }

    public async Task<Question> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        Question? question = await this.questions.GetByIdAsync(id, cancellationToken);

        if (question is null)
        {
            throw new NotFoundException("Question", id);
        }

        return question;
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
        await ValidateFiltersAsync(categoryId, topicId, cancellationToken);

        return await this.questions.GetPagedAsync(
            page, pageSize, categoryId, topicId, difficulty, type,
            search, sortBy, descending, cancellationToken);
    }

    public async Task<IReadOnlyList<Question>> GetRandomAsync(
        int count,
        int? categoryId,
        DifficultyLevel? difficulty,
        CancellationToken cancellationToken)
    {
        await ValidateFiltersAsync(categoryId, null, cancellationToken);

        return await this.questions.GetRandomAsync(count, categoryId, difficulty, cancellationToken);
    }

    public async Task<IReadOnlyList<Question>> GetRelatedAsync(int questionId, int count, CancellationToken cancellationToken)
    {
        bool exists = await this.questions.ExistsAsync(questionId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Question", questionId);
        }

        return await this.questions.GetRelatedAsync(questionId, count, cancellationToken);
    }

    public Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        return this.questions.CountAsync(cancellationToken);
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
