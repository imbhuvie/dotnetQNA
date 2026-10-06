using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Persists <see cref="StudyProgress"/> rows (exactly one per question).
/// </summary>
public interface IStudyProgressRepository
{
    Task<IReadOnlyList<StudyProgress>> GetAllAsync(CancellationToken cancellationToken);

    Task<StudyProgress?> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken);

    Task UpsertAsync(StudyProgress progress, CancellationToken cancellationToken);

    Task<int> CountByStatusAsync(StudyStatus status, CancellationToken cancellationToken);
}
