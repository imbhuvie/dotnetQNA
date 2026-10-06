using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Bookmark management: list, add and remove.
/// </summary>
[ApiController]
[Route("api/bookmarks")]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService bookmarks;

    public BookmarksController(IBookmarkService bookmarks)
    {
        this.bookmarks = bookmarks;
    }

    /// <summary>Gets all bookmarks, newest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookmarkDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BookmarkDto>>>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<BookmarkDto> result = await this.bookmarks.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<BookmarkDto>>.Ok(result, "Bookmarks retrieved successfully."));
    }

    /// <summary>Bookmarks a question.</summary>
    [HttpPost("{questionId:int}")]
    [ProducesResponseType(typeof(ApiResponse<BookmarkDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<BookmarkDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<BookmarkDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<BookmarkDto>>> Add(int questionId, CancellationToken cancellationToken)
    {
        BookmarkDto result = await this.bookmarks.AddAsync(questionId, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<BookmarkDto>.Ok(result, "Question bookmarked successfully."));
    }

    /// <summary>Removes a bookmark.</summary>
    [HttpDelete("{questionId:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Remove(int questionId, CancellationToken cancellationToken)
    {
        await this.bookmarks.RemoveAsync(questionId, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new object(), "Bookmark removed successfully."));
    }
}
