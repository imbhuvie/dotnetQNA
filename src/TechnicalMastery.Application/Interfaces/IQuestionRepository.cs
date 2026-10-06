using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Reads <see cref="Question"/> rows with filtering, sorting, searching
/// and pagination. Questions are seeded content — no write operations.
/// <para>
/// <c>sortBy</c> accepts <c>"id"</c>, <c>"difficulty"</c> or <c>"newest"</c>;
/// anything else falls back to <c>"id"</c>.
/// </para>
/// </summary>
public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(int id, CancellationToken cancellationToken);

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

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
}
