using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Question query use cases. Summaries and details carry the current user's
/// bookmark/progress state. Returns DTOs only (Rule 9).
/// </summary>
public interface IQuestionService
{
    Task<QuestionDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<PagedResult<QuestionSummaryDto>> GetPagedAsync(QuestionsQuery query, CancellationToken cancellationToken);

    Task<IReadOnlyList<QuestionSummaryDto>> GetRandomAsync(
        int count,
        int? categoryId,
        DifficultyLevel? difficulty,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<QuestionSummaryDto>> GetRelatedAsync(int questionId, int count, CancellationToken cancellationToken);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);
}
