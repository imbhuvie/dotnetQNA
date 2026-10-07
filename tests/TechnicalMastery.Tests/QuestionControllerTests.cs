using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Api.Controllers;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Services;
using TechnicalMastery.Application.Validators;
using TechnicalMastery.Infrastructure.Repositories;

namespace TechnicalMastery.Tests;

public class QuestionControllerTests
{
    private static QuestionsController CreateController(TestDatabase database)
    {
        QuestionService service = new QuestionService(
            new QuestionRepository(database.Context),
            new CategoryRepository(database.Context),
            new TopicRepository(database.Context),
            new BookmarkRepository(database.Context),
            new StudyProgressRepository(database.Context),
            new QuestionsQueryValidator());

        return new QuestionsController(service);
    }

    [Fact]
    public async Task GetById_ReturnsOkEnvelope()
    {
        using TestDatabase database = new TestDatabase();
        QuestionsController controller = CreateController(database);
        int id = database.QuestionId("What is boxing?");

        ActionResult<ApiResponse<QuestionDetailDto>> action = await controller.GetById(id, CancellationToken.None);
        OkObjectResult ok = Assert.IsType<OkObjectResult>(action.Result);
        ApiResponse<QuestionDetailDto> envelope = Assert.IsType<ApiResponse<QuestionDetailDto>>(ok.Value);

        Assert.True(envelope.Success);
        Assert.Equal(id, envelope.Data!.Id);
    }

    [Fact]
    public async Task GetById_Missing_ThrowsNotFound()
    {
        using TestDatabase database = new TestDatabase();
        QuestionsController controller = CreateController(database);

        await Assert.ThrowsAsync<Application.Common.Exceptions.NotFoundException>(
            () => controller.GetById(999999, CancellationToken.None));
    }

    [Fact]
    public async Task GetPaged_ReturnsOkWithTotals()
    {
        using TestDatabase database = new TestDatabase();
        QuestionsController controller = CreateController(database);

        ActionResult<ApiResponse<PagedResult<QuestionSummaryDto>>> action = await controller.GetPaged(
            new QuestionsQuery { Page = 1, PageSize = 20 }, CancellationToken.None);
        OkObjectResult ok = Assert.IsType<OkObjectResult>(action.Result);
        ApiResponse<PagedResult<QuestionSummaryDto>> envelope =
            Assert.IsType<ApiResponse<PagedResult<QuestionSummaryDto>>>(ok.Value);

        Assert.True(envelope.Success);
        Assert.Equal(3, envelope.Data!.TotalCount);
    }
}
