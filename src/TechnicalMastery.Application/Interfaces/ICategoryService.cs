using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Category use cases (seeded read-only content). Returns DTOs only —
/// entities never leave the Application layer (Rule 9).
/// </summary>
public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken);
}
