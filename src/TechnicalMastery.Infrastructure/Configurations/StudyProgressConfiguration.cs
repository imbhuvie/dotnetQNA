using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="StudyProgress"/>.
/// Exactly one row per question (unique QuestionId).
/// </summary>
public class StudyProgressConfiguration : IEntityTypeConfiguration<StudyProgress>
{
    public void Configure(EntityTypeBuilder<StudyProgress> builder)
    {
        builder.ToTable("StudyProgress");

        builder.HasKey(progress => progress.Id);

        builder.HasIndex(progress => progress.QuestionId)
            .IsUnique();

        builder.Property(progress => progress.Status)
            .HasConversion<int>()
            .HasDefaultValue(Domain.Enums.StudyStatus.NotStarted);

        builder.HasOne(progress => progress.Question)
            .WithOne(question => question.Progress)
            .HasForeignKey<StudyProgress>(progress => progress.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
