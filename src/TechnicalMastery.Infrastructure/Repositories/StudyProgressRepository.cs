using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IStudyProgressRepository"/>.
/// </summary>
public class StudyProgressRepository : IStudyProgressRepository
{
    private readonly AppDbContext context;

    public StudyProgressRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<StudyProgress>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<StudyProgress> entries = await this.context.StudyProgressEntries
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entries;
    }

    public Task<StudyProgress?> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken)
    {
        return this.context.StudyProgressEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(progress => progress.QuestionId == questionId, cancellationToken);
    }

    public async Task UpsertAsync(StudyProgress progress, CancellationToken cancellationToken)
    {
        StudyProgress? existing = await this.context.StudyProgressEntries
            .FirstOrDefaultAsync(item => item.QuestionId == progress.QuestionId, cancellationToken);

        if (existing is null)
        {
            await this.context.StudyProgressEntries.AddAsync(progress, cancellationToken);
        }
        else
        {
            existing.Status = progress.Status;
            existing.LastViewedAt = progress.LastViewedAt;
            existing.CompletedAt = progress.CompletedAt;
            existing.ReviewCount = progress.ReviewCount;
        }

        await this.context.SaveChangesAsync(cancellationToken);
    }
}
