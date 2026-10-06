using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Services;
using TechnicalMastery.Application.Validators;

namespace TechnicalMastery.Application;

/// <summary>
/// Registers all Application services (business logic) with the host's
/// dependency injection container. Controllers and clients depend on the
/// <c>I*</c> interfaces, never on these implementations.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Validators are registered explicitly (one line each) so the full
        // validation set is visible here with no assembly scanning.
        services.AddScoped<IValidator<QuestionsQuery>, QuestionsQueryValidator>();
        services.AddScoped<IValidator<CreateNoteRequest>, CreateNoteRequestValidator>();
        services.AddScoped<IValidator<UpdateNoteRequest>, UpdateNoteRequestValidator>();
        services.AddScoped<IValidator<UpdateProgressRequest>, UpdateProgressRequestValidator>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<IStudyProgressService, StudyProgressService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
