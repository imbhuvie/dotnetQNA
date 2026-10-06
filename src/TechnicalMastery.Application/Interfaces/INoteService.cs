using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Personal-note use cases. Notes always belong to an existing question.
/// Returns DTOs only (Rule 9).
/// </summary>
public interface INoteService
{
    Task<IReadOnlyList<QuestionNoteDto>> GetByQuestionAsync(int questionId, CancellationToken cancellationToken);

    Task<QuestionNoteDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<QuestionNoteDto> CreateAsync(int questionId, string noteText, CancellationToken cancellationToken);

    Task<QuestionNoteDto> UpdateAsync(int id, string noteText, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
