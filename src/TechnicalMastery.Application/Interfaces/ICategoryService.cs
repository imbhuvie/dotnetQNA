using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Category use cases (seeded read-only content).
/// </summary>
public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);

    Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken);
}
