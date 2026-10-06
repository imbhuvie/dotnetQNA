using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Study-progress use cases with automatic timestamp rules.
/// </summary>
public class StudyProgressService : IStudyProgressService
{
    private readonly IStudyProgressRepository progressEntries;
    private readonly IQuestionRepository questions;

    public StudyProgressService(IStudyProgressRepository progressEntries, IQuestionRepository questions)
    {
        this.progressEntries = progressEntries;
        this.questions = questions;
    }

    public Task<IReadOnlyList<StudyProgress>> GetAllAsync(CancellationToken cancellationToken)
    {
        return this.progressEntries.GetAllAsync(cancellationToken);
    }

    public async Task<StudyProgress> MarkViewedAsync(int questionId, CancellationToken cancellationToken)
    {
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        StudyProgress? existing = await this.progressEntries.GetByQuestionIdAsync(questionId, cancellationToken);
        DateTime now = DateTime.UtcNow;

        if (existing is null)
        {
            StudyProgress created = new StudyProgress
            {
                QuestionId = questionId,
                Status = StudyStatus.Learning,
                LastViewedAt = now,
                ReviewCount = 0
            };

            await this.progressEntries.UpsertAsync(created, cancellationToken);

            return created;
        }

        existing.LastViewedAt = now;

        if (existing.Status == StudyStatus.NotStarted)
        {
            existing.Status = StudyStatus.Learning;
        }

        await this.progressEntries.UpsertAsync(existing, cancellationToken);

        return existing;
    }

    public async Task<StudyProgress> MarkCompletedAsync(int questionId, CancellationToken cancellationToken)
    {
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        StudyProgress? existing = await this.progressEntries.GetByQuestionIdAsync(questionId, cancellationToken);
        DateTime now = DateTime.UtcNow;

        if (existing is null)
        {
            StudyProgress created = new StudyProgress
            {
                QuestionId = questionId,
                Status = StudyStatus.Completed,
                LastViewedAt = now,
                CompletedAt = now,
                ReviewCount = 0
            };

            await this.progressEntries.UpsertAsync(created, cancellationToken);

            return created;
        }

        existing.Status = StudyStatus.Completed;
        existing.LastViewedAt = now;
        existing.CompletedAt = now;

        await this.progressEntries.UpsertAsync(existing, cancellationToken);

        return existing;
    }

    public async Task<StudyProgress> MarkNeedsReviewAsync(int questionId, CancellationToken cancellationToken)
    {
        await EnsureQuestionExistsAsync(questionId, cancellationToken);

        StudyProgress? existing = await this.progressEntries.GetByQuestionIdAsync(questionId, cancellationToken);
        DateTime now = DateTime.UtcNow;

        if (existing is null)
        {
            StudyProgress created = new StudyProgress
            {
                QuestionId = questionId,
                Status = StudyStatus.NeedsReview,
                LastViewedAt = now,
                ReviewCount = 1
            };

            await this.progressEntries.UpsertAsync(created, cancellationToken);

            return created;
        }

        existing.Status = StudyStatus.NeedsReview;
        existing.LastViewedAt = now;
        existing.ReviewCount += 1;

        await this.progressEntries.UpsertAsync(existing, cancellationToken);

        return existing;
    }

    private async Task EnsureQuestionExistsAsync(int questionId, CancellationToken cancellationToken)
    {
        bool exists = await this.questions.ExistsAsync(questionId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Question", questionId);
        }
    }
}
