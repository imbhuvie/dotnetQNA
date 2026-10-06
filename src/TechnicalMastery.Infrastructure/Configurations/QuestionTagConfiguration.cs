using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="QuestionTag"/>.
/// </summary>
public class QuestionTagConfiguration : IEntityTypeConfiguration<QuestionTag>
{
    public void Configure(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.ToTable("QuestionTags");

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Tag)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(tag => new { tag.QuestionId, tag.Tag })
            .IsUnique();
    }
}
