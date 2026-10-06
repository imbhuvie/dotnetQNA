using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Personal notes attached to questions.
/// </summary>
[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly INoteService notes;

    public NotesController(INoteService notes)
    {
        this.notes = notes;
    }

    /// <summary>Gets all notes for one question.</summary>
    [HttpGet("question/{questionId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuestionNoteDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuestionNoteDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<QuestionNoteDto>>>> GetByQuestion(
        int questionId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<QuestionNoteDto> result = await this.notes.GetByQuestionAsync(questionId, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<QuestionNoteDto>>.Ok(result, "Notes retrieved successfully."));
    }

    /// <summary>Gets a single note by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<QuestionNoteDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        QuestionNoteDto result = await this.notes.GetByIdAsync(id, cancellationToken);

        return Ok(ApiResponse<QuestionNoteDto>.Ok(result, "Note retrieved successfully."));
    }

    /// <summary>Adds a note to a question.</summary>
    [HttpPost("question/{questionId:int}")]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<QuestionNoteDto>>> Create(
        int questionId,
        [FromBody] CreateNoteRequest request,
        CancellationToken cancellationToken)
    {
        QuestionNoteDto result = await this.notes.CreateAsync(questionId, request, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<QuestionNoteDto>.Ok(result, "Note created successfully."));
    }

    /// <summary>Replaces a note's text.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<QuestionNoteDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<QuestionNoteDto>>> Update(
        int id,
        [FromBody] UpdateNoteRequest request,
        CancellationToken cancellationToken)
    {
        QuestionNoteDto result = await this.notes.UpdateAsync(id, request, cancellationToken);

        return Ok(ApiResponse<QuestionNoteDto>.Ok(result, "Note updated successfully."));
    }

    /// <summary>Deletes a note.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await this.notes.DeleteAsync(id, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new object(), "Note deleted successfully."));
    }
}
