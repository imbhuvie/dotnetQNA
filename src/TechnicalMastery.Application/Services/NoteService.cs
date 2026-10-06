using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Personal-note use cases with automatic audit timestamps.
/// </summary>
public class NoteService : INoteService
{
    private readonly INoteRepository notes;
    private readonly IQuestionRepository questions;

    public NoteService(INoteRepository notes, IQuestionRepository questions)
    {
        this.notes = notes;
        this.questions = questions;
    }

    public async Task<IReadOnlyList<QuestionNote>> GetByQuestionAsync(int questionId, CancellationToken cancellationToken)
    {
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        return await this.notes.GetByQuestionIdAsync(questionId, cancellationToken);
    }

    public async Task<QuestionNote> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        QuestionNote? note = await this.notes.GetByIdAsync(id, cancellationToken);

        if (note is null)
        {
            throw new NotFoundException("Note", id);
        }

        return note;
    }

    public async Task<QuestionNote> CreateAsync(int questionId, string noteText, CancellationToken cancellationToken)
    {
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        if (string.IsNullOrWhiteSpace(noteText))
        {
            throw new ArgumentException("Note text must not be empty.", nameof(noteText));
        }

        DateTime now = DateTime.UtcNow;

        QuestionNote note = new QuestionNote
        {
            QuestionId = questionId,
            NoteText = noteText.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await this.notes.AddAsync(note, cancellationToken);

        return note;
    }

    public async Task<QuestionNote> UpdateAsync(int id, string noteText, CancellationToken cancellationToken)
    {
        QuestionNote note = await GetByIdAsync(id, cancellationToken);

        if (string.IsNullOrWhiteSpace(noteText))
        {
            throw new ArgumentException("Note text must not be empty.", nameof(noteText));
        }

        note.NoteText = noteText.Trim();
        note.UpdatedAt = DateTime.UtcNow;

        await this.notes.UpdateAsync(note, cancellationToken);

        return note;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        bool deleted = await this.notes.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Note", id);
        }
    }

    private async Task EnsureQuestionExistsAsync(int questionId, CancellationToken cancellationToken)
    {
        bool exists = await this.questions.ExistsAsync(questionId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Question", questionId);
        }
    }
}
