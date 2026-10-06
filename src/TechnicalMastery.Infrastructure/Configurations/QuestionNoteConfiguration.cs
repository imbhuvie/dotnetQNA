using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Infrastructure.Configurations;

/// <summary>
/// Table, column and relationship rules for <see cref="QuestionNote"/>.
/// </summary>
public class QuestionNoteConfiguration : IEntityTypeConfiguration<QuestionNote>
{
    public void Configure(EntityTypeBuilder<QuestionNote> builder)
    {
        builder.ToTable("QuestionNotes");

        builder.HasKey(note => note.Id);

        builder.Property(note => note.NoteText)
            .IsRequired();

        builder.HasIndex(note => note.QuestionId);
    }
}
