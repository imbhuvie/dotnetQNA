using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Question browsing: paged/filtered list, detail, search, random and related.
/// All list endpoints return summaries enriched with bookmark/progress state.
/// </summary>
[ApiController]
[Route("api/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService questions;

    public QuestionsController(IQuestionService questions)
    {
        this.questions = questions;
    }

    /// <summary>Gets a paged, filtered, sorted question list (e.g. ?page=1&amp;pageSize=20&amp;difficulty=Intermediate).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionSummaryDto>>>> GetPaged(
        [FromQuery] QuestionsQuery query,
        CancellationToken cancellationToken)
    {
        PagedResult<QuestionSummaryDto> result = await this.questions.GetPagedAsync(query, cancellationToken);

        return Ok(ApiResponse<PagedResult<QuestionSummaryDto>>.Ok(result, "Questions retrieved successfully."));
    }

    /// <summary>Full-text search across question/answer text, topic, category and tags.</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionSummaryDto>>>> Search(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        QuestionsQuery questionsQuery = new QuestionsQuery { Search = query, Page = page, PageSize = pageSize };

        PagedResult<QuestionSummaryDto> result = await this.questions.GetPagedAsync(questionsQuery, cancellationToken);

        return Ok(ApiResponse<PagedResult<QuestionSummaryDto>>.Ok(result, "Search completed successfully."));
    }

    /// <summary>Gets random questions, optionally scoped (e.g. ?count=10&amp;difficulty=Beginner).</summary>
    [HttpGet("random")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuestionSummaryDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<QuestionSummaryDto>>>> GetRandom(
        [FromQuery] int count = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] DifficultyLevel? difficulty = null,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<QuestionSummaryDto> result = await this.questions.GetRandomAsync(count, categoryId, difficulty, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<QuestionSummaryDto>>.Ok(result, "Random questions retrieved successfully."));
    }

    /// <summary>Gets questions in a single category.</summary>
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionSummaryDto>>>> GetByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        QuestionsQuery questionsQuery = new QuestionsQuery { CategoryId = categoryId, Page = page, PageSize = pageSize };

        PagedResult<QuestionSummaryDto> result = await this.questions.GetPagedAsync(questionsQuery, cancellationToken);

        return Ok(ApiResponse<PagedResult<QuestionSummaryDto>>.Ok(result, "Questions retrieved successfully."));
    }

    /// <summary>Gets questions in a single topic.</summary>
    [HttpGet("topic/{topicId:int}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionSummaryDto>>>> GetByTopic(
        int topicId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        QuestionsQuery questionsQuery = new QuestionsQuery { TopicId = topicId, Page = page, PageSize = pageSize };

        PagedResult<QuestionSummaryDto> result = await this.questions.GetPagedAsync(questionsQuery, cancellationToken);

        return Ok(ApiResponse<PagedResult<QuestionSummaryDto>>.Ok(result, "Questions retrieved successfully."));
    }

    /// <summary>Gets questions of one difficulty (e.g. /api/questions/difficulty/Intermediate).</summary>
    [HttpGet("difficulty/{difficulty}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionSummaryDto>>>> GetByDifficulty(
        DifficultyLevel difficulty,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        QuestionsQuery questionsQuery = new QuestionsQuery { Difficulty = difficulty, Page = page, PageSize = pageSize };

        PagedResult<QuestionSummaryDto> result = await this.questions.GetPagedAsync(questionsQuery, cancellationToken);

        return Ok(ApiResponse<PagedResult<QuestionSummaryDto>>.Ok(result, "Questions retrieved successfully."));
    }

    /// <summary>Gets the full reading payload for one question.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<QuestionDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QuestionDetailDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<QuestionDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        QuestionDetailDto result = await this.questions.GetByIdAsync(id, cancellationToken);

        return Ok(ApiResponse<QuestionDetailDto>.Ok(result, "Question retrieved successfully."));
    }

    /// <summary>Gets questions from the same topic (excluding the question itself).</summary>
    [HttpGet("{id:int}/related")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuestionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuestionSummaryDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<QuestionSummaryDto>>>> GetRelated(
        int id,
        [FromQuery] int count = 5,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<QuestionSummaryDto> result = await this.questions.GetRelatedAsync(id, count, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<QuestionSummaryDto>>.Ok(result, "Related questions retrieved successfully."));
    }
}
