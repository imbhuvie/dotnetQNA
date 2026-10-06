using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ITopicRepository"/>.
/// </summary>
public class TopicRepository : ITopicRepository
{
    private readonly AppDbContext context;

    public TopicRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<Topic>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<Topic> topics = await this.context.Topics
            .AsNoTracking()
            .Where(topic => topic.IsActive)
            .OrderBy(topic => topic.Category.DisplayOrder)
            .ThenBy(topic => topic.DisplayOrder)
            .ToListAsync(cancellationToken);

        return topics;
    }

    public async Task<IReadOnlyList<Topic>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        List<Topic> topics = await this.context.Topics
            .AsNoTracking()
            .Where(topic => topic.CategoryId == categoryId && topic.IsActive)
            .OrderBy(topic => topic.DisplayOrder)
            .ToListAsync(cancellationToken);

        return topics;
    }

    public Task<Topic?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.Topics
            .AsNoTracking()
            .FirstOrDefaultAsync(topic => topic.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.Topics.AnyAsync(topic => topic.Id == id, cancellationToken);
    }
}
