using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="Question"/>.
/// Long-text content uses TEXT columns (SQLite stores them efficiently;
/// length limits are enforced by validation, not the schema).
/// </summary>
public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(question => question.Id);

        builder.Property(question => question.QuestionText)
            .IsRequired();

        builder.Property(question => question.ShortAnswer)
            .IsRequired();

        builder.Property(question => question.DetailedAnswer)
            .IsRequired();

        builder.Property(question => question.DifficultyLevel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(question => question.QuestionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(question => question.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(question => question.TopicId);

        builder.HasIndex(question => question.DifficultyLevel);

        builder.HasMany(question => question.Tags)
            .WithOne(tag => tag.Question)
            .HasForeignKey(tag => tag.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(question => question.Bookmarks)
            .WithOne(bookmark => bookmark.Question)
            .HasForeignKey(bookmark => bookmark.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(question => question.Notes)
            .WithOne(note => note.Question)
            .HasForeignKey(note => note.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
