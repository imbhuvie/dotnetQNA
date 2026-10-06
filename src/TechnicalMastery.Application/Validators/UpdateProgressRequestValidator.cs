using FluentValidation;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Validators;

/// <summary>
/// Only transitions a user can choose are allowed — NotStarted is a system
/// state (no entry yet), never something a client sets explicitly.
/// </summary>
public class UpdateProgressRequestValidator : AbstractValidator<UpdateProgressRequest>
{
    public UpdateProgressRequestValidator()
    {
        RuleFor(request => request.Status)
            .Must(status => status is StudyStatus.Learning or StudyStatus.Completed or StudyStatus.NeedsReview)
            .WithMessage("Status must be Learning, Completed or NeedsReview.");
    }
}
