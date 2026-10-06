using FluentValidation;
using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Validators;

/// <summary>Rules for note updates — identical to creation: a note always has text.</summary>
public class UpdateNoteRequestValidator : AbstractValidator<UpdateNoteRequest>
{
    public UpdateNoteRequestValidator()
    {
        RuleFor(request => request.NoteText)
            .NotEmpty()
            .WithMessage("Note text must not be empty.")
            .MaximumLength(4000)
            .WithMessage("Note text must be 4000 characters or fewer.");
    }
}
