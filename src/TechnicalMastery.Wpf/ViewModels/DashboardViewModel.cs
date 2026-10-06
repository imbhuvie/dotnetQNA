using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Dashboard screen (§11): headline totals + per-category progress + resume entry.
/// Loads its data in <see cref="InitializeAsync"/> after navigation.
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardApiClient dashboard;
    private readonly NavigationService navigation;

    [ObservableProperty]
    private int totalQuestions;

    [ObservableProperty]
    private int completedCount;

    [ObservableProperty]
    private int remainingCount;

    [ObservableProperty]
    private int bookmarkCount;

    [ObservableProperty]
    private int needsReviewCount;

    public ObservableCollection<CategoryProgressModel> Categories { get; } = new ObservableCollection<CategoryProgressModel>();

    public DashboardViewModel(IDashboardApiClient dashboard, NavigationService navigation)
    {
        this.dashboard = dashboard;
        this.navigation = navigation;
    }

    public override async Task InitializeAsync()
    {
        IsBusy = true;
        ClearError();

        try
        {
            DashboardSummaryModel summary = await this.dashboard.GetSummaryAsync(CancellationToken.None);

            TotalQuestions = summary.TotalQuestions;
            CompletedCount = summary.CompletedCount;
            RemainingCount = summary.RemainingCount;
            BookmarkCount = summary.BookmarkCount;
            NeedsReviewCount = summary.NeedsReviewCount;

            Categories.Clear();

            foreach (CategoryProgressModel row in summary.Categories)
            {
                Categories.Add(row);
            }
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ContinueLearningAsync()
    {
        await this.navigation.NavigateToAsync<BrowseViewModel>();
    }
}
