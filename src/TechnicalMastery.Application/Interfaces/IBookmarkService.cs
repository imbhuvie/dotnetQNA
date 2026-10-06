using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Bookmark use cases. Bookmarking requires the question to exist;
/// double-bookmarking is a conflict, removing a missing bookmark is not found.
/// </summary>
public interface IBookmarkService
{
    Task<IReadOnlyList<Bookmark>> GetAllAsync(CancellationToken cancellationToken);

    Task<Bookmark> AddAsync(int questionId, CancellationToken cancellationToken);

    Task RemoveAsync(int questionId, CancellationToken cancellationToken);
}
