using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Services;
using TechnicalMastery.Application.Validators;
using TechnicalMastery.Domain.Enums;
using TechnicalMastery.Infrastructure.Repositories;

namespace TechnicalMastery.Tests;

public class QuestionServiceTests
{
    private static QuestionService CreateService(TestDatabase database)
    {
        return new QuestionService(
            new QuestionRepository(database.Context),
            new CategoryRepository(database.Context),
            new TopicRepository(database.Context),
            new BookmarkRepository(database.Context),
            new StudyProgressRepository(database.Context),
            new QuestionsQueryValidator());
    }

    [Fact]
    public async Task GetById_ReturnsDetailWithTags()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);
        int id = database.QuestionId("What is boxing?");

        QuestionDetailDto detail = await service.GetByIdAsync(id, CancellationToken.None);

        Assert.Equal("TestCat", detail.CategoryName);
        Assert.Equal("T1", detail.TopicName);
        Assert.Contains("boxing", detail.Tags);
        Assert.Equal(DifficultyLevel.Intermediate, detail.DifficultyLevel);
    }

    [Fact]
    public async Task GetById_Missing_ThrowsNotFound()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);

        await Assert.ThrowsAsync<Application.Common.Exceptions.NotFoundException>(
            () => service.GetByIdAsync(999999, CancellationToken.None));
    }

    [Fact]
    public async Task GetPaged_FiltersByDifficulty()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);

        PagedResult<QuestionSummaryDto> result = await service.GetPagedAsync(
            new QuestionsQuery { Page = 1, PageSize = 20, Difficulty = DifficultyLevel.Beginner },
            CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("What is a variable?", result.Items[0].QuestionText);
    }

    [Fact]
    public async Task GetPaged_Search_FindsByTag()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);

        PagedResult<QuestionSummaryDto> result = await service.GetPagedAsync(
            new QuestionsQuery { Page = 1, PageSize = 20, Search = "threads" },
            CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetPaged_InvalidCategory_ThrowsNotFound()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);

        await Assert.ThrowsAsync<Application.Common.Exceptions.NotFoundException>(
            () => service.GetPagedAsync(
                new QuestionsQuery { Page = 1, PageSize = 20, CategoryId = 999999 },
                CancellationToken.None));
    }

    [Fact]
    public async Task GetPaged_InvalidQuery_ThrowsValidation()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => service.GetPagedAsync(
                new QuestionsQuery { Page = 0, PageSize = 500 },
                CancellationToken.None));
    }

    [Fact]
    public async Task GetRandom_ReturnsRequestedCount()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);

        IReadOnlyList<QuestionSummaryDto> result = await service.GetRandomAsync(2, null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetRelated_ReturnsSameTopicOthers()
    {
        using TestDatabase database = new TestDatabase();
        QuestionService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        IReadOnlyList<QuestionSummaryDto> result = await service.GetRelatedAsync(id, 5, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("What is boxing?", result[0].QuestionText);
    }
}
