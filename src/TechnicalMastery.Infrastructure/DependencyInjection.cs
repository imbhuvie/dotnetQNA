using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Infrastructure.Data;
using TechnicalMastery.Infrastructure.Repositories;

namespace TechnicalMastery.Infrastructure;

/// <summary>
/// Registers all Infrastructure services (EF Core, repositories, seeders)
/// with the host's dependency injection container.
/// <para>
/// Lives in Infrastructure so the API project gets database access through
/// one line, without referencing EF Core itself (Rule 8: database logic
/// belongs in Infrastructure).
/// </para>
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IBookmarkRepository, BookmarkRepository>();
        services.AddScoped<IStudyProgressRepository, StudyProgressRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();

        return services;
    }
}
