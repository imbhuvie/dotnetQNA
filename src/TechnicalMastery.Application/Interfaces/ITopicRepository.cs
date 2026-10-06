using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Reads <see cref="Topic"/> rows. Topics are seeded content —
/// no create/update/delete operations are exposed.
/// </summary>
public interface ITopicRepository
{
    Task<IReadOnlyList<Topic>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Topic>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken);

    Task<Topic?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
}
