using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Study-progress use cases. All transitions stamp times automatically:
/// viewing records LastViewedAt, completing records CompletedAt,
/// flagging for review increments ReviewCount.
/// </summary>
public interface IStudyProgressService
{
    Task<IReadOnlyList<StudyProgress>> GetAllAsync(CancellationToken cancellationToken);

    Task<StudyProgress> MarkViewedAsync(int questionId, CancellationToken cancellationToken);

    Task<StudyProgress> MarkCompletedAsync(int questionId, CancellationToken cancellationToken);

    Task<StudyProgress> MarkNeedsReviewAsync(int questionId, CancellationToken cancellationToken);
}
