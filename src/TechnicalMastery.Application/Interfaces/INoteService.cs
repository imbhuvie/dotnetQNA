using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Personal-note use cases. Notes always belong to an existing question.
/// </summary>
public interface INoteService
{
    Task<IReadOnlyList<QuestionNote>> GetByQuestionAsync(int questionId, CancellationToken cancellationToken);

    Task<QuestionNote> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<QuestionNote> CreateAsync(int questionId, string noteText, CancellationToken cancellationToken);

    Task<QuestionNote> UpdateAsync(int id, string noteText, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
