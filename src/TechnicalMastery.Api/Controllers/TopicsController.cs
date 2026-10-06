using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Read-only catalog of topics, optionally scoped to a category.
/// </summary>
[ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly ITopicService topics;

    public TopicsController(ITopicService topics)
    {
        this.topics = topics;
    }

    /// <summary>Gets topics, optionally filtered by category (e.g. /api/topics?categoryId=3).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TopicDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TopicDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TopicDto>>>> GetAll(
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TopicDto> result = categoryId.HasValue
            ? await this.topics.GetByCategoryAsync(categoryId.Value, cancellationToken)
            : await this.topics.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<TopicDto>>.Ok(result, "Topics retrieved successfully."));
    }

    /// <summary>Gets a single topic by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TopicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TopicDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TopicDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        TopicDto result = await this.topics.GetByIdAsync(id, cancellationToken);

        return Ok(ApiResponse<TopicDto>.Ok(result, "Topic retrieved successfully."));
    }
}
