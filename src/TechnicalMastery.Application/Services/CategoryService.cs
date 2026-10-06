using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Category use cases.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository categories;

    public CategoryService(ICategoryRepository categories)
    {
        this.categories = categories;
    }

    public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return this.categories.GetAllAsync(cancellationToken);
    }

    public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        Category? category = await this.categories.GetByIdAsync(id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category", id);
        }

        return category;
    }
}
