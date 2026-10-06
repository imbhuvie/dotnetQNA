using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Mappings;
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

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Category> result = await this.categories.GetAllAsync(cancellationToken);

        return result.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        Category? category = await this.categories.GetByIdAsync(id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category", id);
        }

        return DtoMapper.ToDto(category);
    }
}
