using Microsoft.Extensions.DependencyInjection;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Services;

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
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<IStudyProgressService, StudyProgressService>();
        services.AddScoped<INoteService, NoteService>();

        return services;
    }
}
