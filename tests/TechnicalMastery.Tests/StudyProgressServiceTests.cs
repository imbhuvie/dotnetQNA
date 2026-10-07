using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Services;
using TechnicalMastery.Application.Validators;
using TechnicalMastery.Domain.Enums;
using TechnicalMastery.Infrastructure.Repositories;

namespace TechnicalMastery.Tests;

public class StudyProgressServiceTests
{
    private static StudyProgressService CreateService(TestDatabase database)
    {
        return new StudyProgressService(
            new StudyProgressRepository(database.Context),
            new QuestionRepository(database.Context),
            new UpdateProgressRequestValidator());
    }

    [Fact]
    public async Task MarkCompleted_SetsStatusAndTimestamps()
    {
        using TestDatabase database = new TestDatabase();
        StudyProgressService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        StudyProgressDto result = await service.MarkCompletedAsync(id, CancellationToken.None);

        Assert.Equal(StudyStatus.Completed, result.Status);
        Assert.NotNull(result.CompletedAt);
        Assert.NotNull(result.LastViewedAt);
    }

    [Fact]
    public async Task MarkNeedsReview_IncrementsReviewCount()
    {
        using TestDatabase database = new TestDatabase();
        StudyProgressService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        StudyProgressDto first = await service.MarkNeedsReviewAsync(id, CancellationToken.None);
        StudyProgressDto second = await service.MarkNeedsReviewAsync(id, CancellationToken.None);

        Assert.Equal(1, first.ReviewCount);
        Assert.Equal(2, second.ReviewCount);
    }

    [Fact]
    public async Task MarkViewed_CreatesLearningEntry()
    {
        using TestDatabase database = new TestDatabase();
        StudyProgressService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        StudyProgressDto result = await service.MarkViewedAsync(id, CancellationToken.None);

        Assert.Equal(StudyStatus.Learning, result.Status);
    }

    [Fact]
    public async Task SetStatus_InvalidStatus_ThrowsValidation()
    {
        using TestDatabase database = new TestDatabase();
        StudyProgressService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => service.SetStatusAsync(id, new UpdateProgressRequest { Status = StudyStatus.NotStarted }, CancellationToken.None));
    }

    [Fact]
    public async Task MarkCompleted_MissingQuestion_ThrowsNotFound()
    {
        using TestDatabase database = new TestDatabase();
        StudyProgressService service = CreateService(database);

        await Assert.ThrowsAsync<Application.Common.Exceptions.NotFoundException>(
            () => service.MarkCompletedAsync(999999, CancellationToken.None));
    }
}
