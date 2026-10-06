using FluentValidation;
using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Validators;

/// <summary>Rules for note creation (shared text rules live here, not in the service).</summary>
public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator()
    {
        RuleFor(request => request.NoteText)
            .NotEmpty()
            .WithMessage("Note text must not be empty.")
            .MaximumLength(4000)
            .WithMessage("Note text must be 4000 characters or fewer.");
    }
}
