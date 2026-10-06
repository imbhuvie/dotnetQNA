using FluentValidation;
using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Validators;

/// <summary>
/// Guards the question-listing query string. The repository clamps defensively,
/// but invalid input is a client error (400) — not something to silently fix.
/// </summary>
public class QuestionsQueryValidator : AbstractValidator<QuestionsQuery>
{
    public QuestionsQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be 1 or greater.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(query => query.Search)
            .MaximumLength(200)
            .WithMessage("Search must be 200 characters or fewer.")
            .When(query => query.Search is not null);

        RuleFor(query => query.SortBy)
            .Must(value => value is null || value.Trim().ToLowerInvariant() is "id" or "difficulty" or "newest")
            .WithMessage("SortBy must be one of: id, difficulty, newest.");
    }
}
