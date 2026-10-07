using FluentValidation;
using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Mappings;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Personal-note use cases with automatic audit timestamps.
/// </summary>
public class NoteService : INoteService
{
    private readonly INoteRepository notes;
    private readonly IQuestionRepository questions;
    private readonly IValidator<CreateNoteRequest> createValidator;
    private readonly IValidator<UpdateNoteRequest> updateValidator;

    public NoteService(
        INoteRepository notes,
        IQuestionRepository questions,
        IValidator<CreateNoteRequest> createValidator,
        IValidator<UpdateNoteRequest> updateValidator)
    {
        this.notes = notes;
        this.questions = questions;
        this.createValidator = createValidator;
        this.updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<QuestionNoteDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<QuestionNote> result = await this.notes.GetAllAsync(cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<QuestionNoteDto>> GetByQuestionAsync(int questionId, CancellationToken cancellationToken)
    {
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        IReadOnlyList<QuestionNote> result = await this.notes.GetByQuestionIdAsync(questionId, cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<QuestionNoteDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        QuestionNote? note = await this.notes.GetByIdAsync(id, cancellationToken);

        if (note is null)
        {
            throw new NotFoundException("Note", id);
        }

        return DtoMapper.ToDto(note);
    }

    public async Task<QuestionNoteDto> CreateAsync(int questionId, CreateNoteRequest request, CancellationToken cancellationToken)
    {
        await this.createValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        DateTime now = DateTime.UtcNow;

        QuestionNote note = new QuestionNote
        {
            QuestionId = questionId,
            NoteText = request.NoteText.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await this.notes.AddAsync(note, cancellationToken);

        return DtoMapper.ToDto(note);
    }

    public async Task<QuestionNoteDto> UpdateAsync(int id, UpdateNoteRequest request, CancellationToken cancellationToken)
    {
        await this.updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        QuestionNote? note = await this.notes.GetByIdAsync(id, cancellationToken);

        if (note is null)
        {
            throw new NotFoundException("Note", id);
        }

        note.NoteText = request.NoteText.Trim();
        note.UpdatedAt = DateTime.UtcNow;

        await this.notes.UpdateAsync(note, cancellationToken);

        return DtoMapper.ToDto(note);
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
