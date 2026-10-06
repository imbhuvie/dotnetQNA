using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Persists the user's personal <see cref="QuestionNote"/> rows.
/// </summary>
public interface INoteRepository
{
    Task<IReadOnlyList<QuestionNote>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken);

    Task<QuestionNote?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(QuestionNote note, CancellationToken cancellationToken);

    Task UpdateAsync(QuestionNote note, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
