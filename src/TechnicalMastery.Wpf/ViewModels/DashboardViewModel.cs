using System.Collections.ObjectModel;
using System.Windows.Input;
using TechnicalMastery.Wpf.Commands;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Dashboard screen (§11): headline totals + per-category progress + resume entry.
/// Loads its data in <see cref="InitializeAsync"/> after navigation.
/// </summary>
public class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardApiClient dashboard;
    private readonly NavigationService navigation;

    private int totalQuestions;

    private int completedCount;

    private int remainingCount;

    private int bookmarkCount;

    private int needsReviewCount;

    public ObservableCollection<CategoryProgressModel> Categories { get; } = new ObservableCollection<CategoryProgressModel>();

    public DashboardViewModel(IDashboardApiClient dashboard, NavigationService navigation)
    {
        this.dashboard = dashboard;
        this.navigation = navigation;

        ContinueLearningCommand = new RelayCommand(ContinueLearningAsync);
    }

    public int TotalQuestions
    {
        get { return this.totalQuestions; }
        set { SetProperty(ref this.totalQuestions, value); }
    }

    public int CompletedCount
    {
        get { return this.completedCount; }
        set { SetProperty(ref this.completedCount, value); }
    }

    public int RemainingCount
    {
        get { return this.remainingCount; }
        set { SetProperty(ref this.remainingCount, value); }
    }

    public int BookmarkCount
    {
        get { return this.bookmarkCount; }
        set { SetProperty(ref this.bookmarkCount, value); }
    }

    public int NeedsReviewCount
    {
        get { return this.needsReviewCount; }
        set { SetProperty(ref this.needsReviewCount, value); }
    }

    public ICommand ContinueLearningCommand { get; }

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

    private async Task ContinueLearningAsync()
    {
        await this.navigation.NavigateToAsync<BrowseViewModel>();
    }
}
