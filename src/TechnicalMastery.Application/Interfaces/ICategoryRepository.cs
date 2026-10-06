using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Reads <see cref="Category"/> rows. Categories are seeded content —
/// no create/update/delete operations are exposed.
/// </summary>
public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
}
