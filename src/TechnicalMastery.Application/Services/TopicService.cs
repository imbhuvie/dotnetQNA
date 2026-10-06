using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Mappings;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Topic use cases.
/// </summary>
public class TopicService : ITopicService
{
    private readonly ITopicRepository topics;
    private readonly ICategoryRepository categories;

    public TopicService(ITopicRepository topics, ICategoryRepository categories)
    {
        this.topics = topics;
        this.categories = categories;
    }

    public async Task<IReadOnlyList<TopicDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Topic> result = await this.topics.GetAllAsync(cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<TopicDto>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        bool exists = await this.categories.ExistsAsync(categoryId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Category", categoryId);
        }

        IReadOnlyList<Topic> result = await this.topics.GetByCategoryAsync(categoryId, cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<TopicDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        Topic? topic = await this.topics.GetByIdAsync(id, cancellationToken);

        if (topic is null)
        {
            throw new NotFoundException("Topic", id);
        }

        return DtoMapper.ToDto(topic);
    }
}
