using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Personal-note use cases. Notes always belong to an existing question.
/// Returns DTOs only (Rule 9).
/// </summary>
public interface INoteService
{
    Task<IReadOnlyList<QuestionNoteDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<QuestionNoteDto>> GetByQuestionAsync(int questionId, CancellationToken cancellationToken);

    Task<QuestionNoteDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<QuestionNoteDto> CreateAsync(int questionId, CreateNoteRequest request, CancellationToken cancellationToken);

    Task<QuestionNoteDto> UpdateAsync(int id, UpdateNoteRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
