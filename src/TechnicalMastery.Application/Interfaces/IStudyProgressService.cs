using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Study-progress use cases. All transitions stamp times automatically:
/// viewing records LastViewedAt, completing records CompletedAt,
/// flagging for review increments ReviewCount. Returns DTOs only (Rule 9).
/// </summary>
public interface IStudyProgressService
{
    Task<IReadOnlyList<StudyProgressDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<StudyProgressDto> MarkViewedAsync(int questionId, CancellationToken cancellationToken);

    Task<StudyProgressDto> MarkCompletedAsync(int questionId, CancellationToken cancellationToken);

    Task<StudyProgressDto> MarkNeedsReviewAsync(int questionId, CancellationToken cancellationToken);
}
