using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Bookmark use cases. Bookmarking requires the question to exist;
/// double-bookmarking is a conflict, removing a missing bookmark is not found.
/// Returns DTOs only (Rule 9).
/// </summary>
public interface IBookmarkService
{
    Task<IReadOnlyList<BookmarkDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<BookmarkDto> AddAsync(int questionId, CancellationToken cancellationToken);

    Task RemoveAsync(int questionId, CancellationToken cancellationToken);
}
