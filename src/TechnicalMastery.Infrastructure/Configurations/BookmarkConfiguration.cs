using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="Bookmark"/>.
/// One bookmark per question (unique QuestionId) until multi-user support
/// adds a UserId column and widens the rule to (UserId, QuestionId).
/// </summary>
public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("Bookmarks");

        builder.HasKey(bookmark => bookmark.Id);

        builder.HasIndex(bookmark => bookmark.QuestionId)
            .IsUnique();
    }
}
