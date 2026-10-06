using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Topic use cases (seeded read-only content).
/// </summary>
public interface ITopicService
{
    Task<IReadOnlyList<Topic>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Topic>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken);

    Task<Topic> GetByIdAsync(int id, CancellationToken cancellationToken);
}
