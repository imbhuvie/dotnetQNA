using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.Interfaces;
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

    public Task<IReadOnlyList<Topic>> GetAllAsync(CancellationToken cancellationToken)
    {
        return this.topics.GetAllAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Topic>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        bool exists = await this.categories.ExistsAsync(categoryId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Category", categoryId);
        }

        return await this.topics.GetByCategoryAsync(categoryId, cancellationToken);
    }

    public async Task<Topic> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        Topic? topic = await this.topics.GetByIdAsync(id, cancellationToken);

        if (topic is null)
        {
            throw new NotFoundException("Topic", id);
        }

        return topic;
    }
}
