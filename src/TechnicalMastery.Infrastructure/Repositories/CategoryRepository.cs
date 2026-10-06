using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ICategoryRepository"/>.
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext context;

    public CategoryRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<Category> categories = await this.context.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.DisplayOrder)
            .ToListAsync(cancellationToken);

        return categories;
    }

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return this.context.Categories.AnyAsync(category => category.Id == id, cancellationToken);
    }
}
