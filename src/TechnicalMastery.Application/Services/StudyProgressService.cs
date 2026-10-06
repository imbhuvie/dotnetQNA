using FluentValidation;
using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Mappings;
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
    private readonly IValidator<UpdateProgressRequest> statusValidator;

    public StudyProgressService(
        IStudyProgressRepository progressEntries,
        IQuestionRepository questions,
        IValidator<UpdateProgressRequest> statusValidator)
    {
        this.progressEntries = progressEntries;
        this.questions = questions;
        this.statusValidator = statusValidator;
    }

    public async Task<IReadOnlyList<StudyProgressDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<StudyProgress> result = await this.progressEntries.GetAllAsync(cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<StudyProgressDto> MarkViewedAsync(int questionId, CancellationToken cancellationToken)
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

            return DtoMapper.ToDto(created);
        }

        existing.LastViewedAt = now;

        if (existing.Status == StudyStatus.NotStarted)
        {
            existing.Status = StudyStatus.Learning;
        }

        await this.progressEntries.UpsertAsync(existing, cancellationToken);

        return DtoMapper.ToDto(existing);
    }

    public async Task<StudyProgressDto> MarkCompletedAsync(int questionId, CancellationToken cancellationToken)
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

            return DtoMapper.ToDto(created);
        }

        existing.Status = StudyStatus.Completed;
        existing.LastViewedAt = now;
        existing.CompletedAt = now;

        await this.progressEntries.UpsertAsync(existing, cancellationToken);

        return DtoMapper.ToDto(existing);
    }

    public async Task<StudyProgressDto> MarkNeedsReviewAsync(int questionId, CancellationToken cancellationToken)
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

            return DtoMapper.ToDto(created);
        }

        existing.Status = StudyStatus.NeedsReview;
        existing.LastViewedAt = now;
        existing.ReviewCount += 1;

        await this.progressEntries.UpsertAsync(existing, cancellationToken);

        return DtoMapper.ToDto(existing);
    }

    private async Task EnsureQuestionExistsAsync(int questionId, CancellationToken cancellationToken)
    {
        bool exists = await this.questions.ExistsAsync(questionId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Question", questionId);
        }
    }

    public async Task<StudyProgressDto> SetStatusAsync(int questionId, UpdateProgressRequest request, CancellationToken cancellationToken)
    {
        await this.statusValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (request.Status == StudyStatus.Learning)
        {
            return await MarkViewedAsync(questionId, cancellationToken);
        }

        if (request.Status == StudyStatus.Completed)
        {
            return await MarkCompletedAsync(questionId, cancellationToken);
        }

        return await MarkNeedsReviewAsync(questionId, cancellationToken);
    }
}
