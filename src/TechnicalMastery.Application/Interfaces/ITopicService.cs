using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Topic use cases (seeded read-only content). Returns DTOs only (Rule 9).
/// </summary>
public interface ITopicService
{
    Task<IReadOnlyList<TopicDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TopicDto>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken);

    Task<TopicDto> GetByIdAsync(int id, CancellationToken cancellationToken);
}
