using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="Topic"/>.
/// </summary>
public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable("Topics");

        builder.HasKey(topic => topic.Id);

        builder.Property(topic => topic.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(topic => new { topic.CategoryId, topic.Name })
            .IsUnique();

        builder.Property(topic => topic.Description)
            .HasMaxLength(1000);

        builder.Property(topic => topic.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(topic => topic.Questions)
            .WithOne(question => question.Topic)
            .HasForeignKey(question => question.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
