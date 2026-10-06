using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Domain.Enums;

namespace TechnicalMastery.Application.Services;

/// <summary>
/// Dashboard aggregation: headline totals plus one progress row per category (§11).
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly ICategoryRepository categories;
    private readonly IQuestionRepository questions;
    private readonly IBookmarkRepository bookmarks;
    private readonly IStudyProgressRepository progressEntries;

    public DashboardService(
        ICategoryRepository categories,
        IQuestionRepository questions,
        IBookmarkRepository bookmarks,
        IStudyProgressRepository progressEntries)
    {
        this.categories = categories;
        this.questions = questions;
        this.bookmarks = bookmarks;
        this.progressEntries = progressEntries;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken)
    {
        int total = await this.questions.CountAsync(cancellationToken);
        int completed = await this.progressEntries.CountByStatusAsync(StudyStatus.Completed, cancellationToken);
        int needsReview = await this.progressEntries.CountByStatusAsync(StudyStatus.NeedsReview, cancellationToken);
        int bookmarkCount = await this.bookmarks.CountAsync(cancellationToken);

        IReadOnlyList<Category> allCategories = await this.categories.GetAllAsync(cancellationToken);
        IReadOnlyList<(int CategoryId, int Total, int Completed)> stats = await this.questions.GetCategoryStatsAsync(cancellationToken);

        Dictionary<int, (int Total, int Completed)> byCategory = stats.ToDictionary(
            stat => stat.CategoryId,
            stat => (stat.Total, stat.Completed));

        List<CategoryProgressDto> rows = allCategories
            .Select(category =>
            {
                byCategory.TryGetValue(category.Id, out (int Total, int Completed) counts);

                return new CategoryProgressDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    Total = counts.Total,
                    Completed = counts.Completed,
                    PercentComplete = counts.Total == 0 ? 0 : counts.Completed * 100 / counts.Total
                };
            })
            .ToList();

        return new DashboardSummaryDto
        {
            TotalQuestions = total,
            CompletedCount = completed,
            RemainingCount = Math.Max(total - completed, 0),
            BookmarkCount = bookmarkCount,
            NeedsReviewCount = needsReview,
            Categories = rows
        };
    }
}
