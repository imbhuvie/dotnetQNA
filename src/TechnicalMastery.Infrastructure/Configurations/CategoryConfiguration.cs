using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="Category"/>.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(category => category.Name)
            .IsUnique();

        builder.Property(category => category.Description)
            .HasMaxLength(1000);

        builder.Property(category => category.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(category => category.Topics)
            .WithOne(topic => topic.Category)
            .HasForeignKey(topic => topic.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
