using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Question query use cases (seeded read-only content).
/// </summary>
public interface IQuestionService
{
    Task<Question> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Question> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? categoryId,
        int? topicId,
        DifficultyLevel? difficulty,
        QuestionType? type,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Question>> GetRandomAsync(
        int count,
        int? categoryId,
        DifficultyLevel? difficulty,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Question>> GetRelatedAsync(int questionId, int count, CancellationToken cancellationToken);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);
}
