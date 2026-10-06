using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Study-progress tracking: read all entries or set one question's status.
/// </summary>
[ApiController]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly IStudyProgressService progress;

    public ProgressController(IStudyProgressService progress)
    {
        this.progress = progress;
    }

    /// <summary>Gets all tracked progress entries.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StudyProgressDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudyProgressDto>>>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<StudyProgressDto> result = await this.progress.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<StudyProgressDto>>.Ok(result, "Study progress retrieved successfully."));
    }

    /// <summary>Sets a question's study status (Learning, Completed or NeedsReview).</summary>
    [HttpPost("{questionId:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudyProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudyProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<StudyProgressDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<StudyProgressDto>>> SetStatus(
        int questionId,
        [FromBody] UpdateProgressRequest request,
        CancellationToken cancellationToken)
    {
        StudyProgressDto result = request.Status switch
        {
            StudyStatus.Learning => await this.progress.MarkViewedAsync(questionId, cancellationToken),
            StudyStatus.Completed => await this.progress.MarkCompletedAsync(questionId, cancellationToken),
            StudyStatus.NeedsReview => await this.progress.MarkNeedsReviewAsync(questionId, cancellationToken),
            _ => throw new ArgumentException("Status must be Learning, Completed or NeedsReview.", nameof(request))
        };

        return Ok(ApiResponse<StudyProgressDto>.Ok(result, "Study progress updated successfully."));
    }
}
