namespace TechnicalMastery.Application.DTOs;

/// <summary>
/// Per-category progress bar row plus the headline totals for the dashboard (§11).
/// </summary>
public class DashboardSummaryDto
{
    public int TotalQuestions { get; set; }

    public int CompletedCount { get; set; }

    public int RemainingCount { get; set; }

    public int BookmarkCount { get; set; }

    public int NeedsReviewCount { get; set; }

    public List<CategoryProgressDto> Categories { get; set; } = new List<CategoryProgressDto>();
}

public class CategoryProgressDto
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int Total { get; set; }

    public int Completed { get; set; }

    public int PercentComplete { get; set; }
}
