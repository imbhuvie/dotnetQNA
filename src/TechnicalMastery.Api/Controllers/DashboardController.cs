using Microsoft.AspNetCore.Mvc;
using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Application.Interfaces;

namespace TechnicalMastery.Api.Controllers;

/// <summary>
/// Aggregated study statistics for the dashboard screen.
/// </summary>
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService dashboard;

    public DashboardController(IDashboardService dashboard)
    {
        this.dashboard = dashboard;
    }

    /// <summary>Gets headline totals plus per-category progress rows.</summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary(CancellationToken cancellationToken)
    {
        DashboardSummaryDto result = await this.dashboard.GetSummaryAsync(cancellationToken);

        return Ok(ApiResponse<DashboardSummaryDto>.Ok(result, "Dashboard summary retrieved successfully."));
    }
}
