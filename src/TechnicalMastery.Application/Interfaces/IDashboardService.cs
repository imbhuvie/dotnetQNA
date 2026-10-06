using TechnicalMastery.Application.DTOs;

namespace TechnicalMastery.Application.Interfaces;

/// <summary>
/// Dashboard aggregation use case: headline totals plus per-category progress (§11).
/// </summary>
public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);
}
