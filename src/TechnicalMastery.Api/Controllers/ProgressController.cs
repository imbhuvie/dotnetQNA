using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;

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
        StudyProgressDto result = await this.progress.SetStatusAsync(questionId, request, cancellationToken);

        return Ok(ApiResponse<StudyProgressDto>.Ok(result, "Study progress updated successfully."));
    }
}
