using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="INoteRepository"/>.
/// </summary>
public class NoteRepository : INoteRepository
{
    private readonly AppDbContext context;

    public NoteRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<QuestionNote>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken)
    {
        List<QuestionNote> notes = await this.context.QuestionNotes
            .AsNoTracking()
            .Where(note => note.QuestionId == questionId)
            .OrderByDescending(note => note.UpdatedAt)
            .ToListAsync(cancellationToken);

        return notes;
    }

    public Task<QuestionNote?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.QuestionNotes
            .AsNoTracking()
            .FirstOrDefaultAsync(note => note.Id == id, cancellationToken);
    }

    public async Task AddAsync(QuestionNote note, CancellationToken cancellationToken)
    {
        await this.context.QuestionNotes.AddAsync(note, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(QuestionNote note, CancellationToken cancellationToken)
    {
        this.context.QuestionNotes.Update(note);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        QuestionNote? note = await this.context.QuestionNotes
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (note is null)
        {
            return false;
        }

        this.context.QuestionNotes.Remove(note);
        await this.context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
