using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Read-only catalog of question categories.
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService categories;

    public CategoriesController(ICategoryService categories)
    {
        this.categories = categories;
    }

    /// <summary>Gets all active categories in display order.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<CategoryDto> result = await this.categories.GetAllAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(result, "Categories retrieved successfully."));
    }

    /// <summary>Gets a single category by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        CategoryDto result = await this.categories.GetByIdAsync(id, cancellationToken);

        return Ok(ApiResponse<CategoryDto>.Ok(result, "Category retrieved successfully."));
    }
}
