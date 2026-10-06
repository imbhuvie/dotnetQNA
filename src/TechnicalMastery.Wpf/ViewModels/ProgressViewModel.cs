using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Progress screen (§16): list all study progress entries with status badges,
/// filter by status, and navigate to question detail.
/// </summary>
public partial class ProgressViewModel : ViewModelBase
{
    private readonly IProgressApiClient progress;
    private readonly NavigationService navigation;

    [ObservableProperty]
    private StudyStatus? filterStatus;

    public ObservableCollection<StudyProgressModel> Entries { get; } = new ObservableCollection<StudyProgressModel>();

    public List<StudyStatus?> StatusFilters { get; } = new List<StudyStatus?>
    {
        null,
        StudyStatus.NotStarted,
        StudyStatus.Learning,
        StudyStatus.Completed,
        StudyStatus.NeedsReview
    };

    public ProgressViewModel(IProgressApiClient progress, NavigationService navigation)
    {
        this.progress = progress;
        this.navigation = navigation;
    }

    public override async Task InitializeAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task FilterChangedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        ClearError();

        try
        {
            IReadOnlyList<StudyProgressModel> result = await this.progress.GetAllProgressAsync(CancellationToken.None);
            Entries.Clear();

            foreach (StudyProgressModel entry in result)
            {
                if (FilterStatus is null || entry.Status == FilterStatus)
                {
                    Entries.Add(entry);
                }
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
    private async Task OpenQuestionAsync(StudyProgressModel? entry)
    {
        if (entry is null)
        {
            return;
        }

        await this.navigation.NavigateToAsync<QuestionDetailViewModel>(detail => detail.QuestionId = entry.QuestionId);
    }
}