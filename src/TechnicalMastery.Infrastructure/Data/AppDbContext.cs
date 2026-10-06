using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the Technical Mastery platform.
/// <para>
/// The connection string is supplied by the host (ASP.NET Core API) via
/// <c>DbContextOptions</c> — this class never contains connection details itself.
/// </para>
/// <para>
/// Table mappings come from the <c>Configurations</c> folder and are applied
/// automatically by <see cref="OnModelCreating"/>.
/// </para>
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<QuestionTag> QuestionTags => Set<QuestionTag>();

    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    public DbSet<StudyProgress> StudyProgressEntries => Set<StudyProgress>();

    public DbSet<QuestionNote> QuestionNotes => Set<QuestionNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
