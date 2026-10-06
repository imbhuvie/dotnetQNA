using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Persists <see cref="Bookmark"/> rows (one per question until multi-user support).
/// </summary>
public interface IBookmarkRepository
{
    Task<IReadOnlyList<Bookmark>> GetAllAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int questionId, CancellationToken cancellationToken);

    Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(int questionId, CancellationToken cancellationToken);
}
