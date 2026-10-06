using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Browse screen (§12): category → topic → question list with difficulty filter,
/// search text and server-side pagination. Opening a question navigates to the
/// detail screen (Phase 21) with the selected id.
/// </summary>
public partial class BrowseViewModel : ViewModelBase
{
    private const int PageSize = 20;

    private readonly ICatalogApiClient catalog;
    private readonly IQuestionApiClient questions;
    private readonly NavigationService navigation;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private CategoryModel? selectedCategory;

    [ObservableProperty]
    private TopicModel? selectedTopic;

    [ObservableProperty]
    private DifficultyLevel? selectedDifficulty;

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private int totalPages;

    [ObservableProperty]
    private int totalCount;

    public ObservableCollection<CategoryModel> Categories { get; } = new ObservableCollection<CategoryModel>();

    public ObservableCollection<TopicModel> Topics { get; } = new ObservableCollection<TopicModel>();

    public ObservableCollection<QuestionSummaryModel> Questions { get; } = new ObservableCollection<QuestionSummaryModel>();

    public List<DifficultyLevel?> Difficulties { get; } = new List<DifficultyLevel?>
    {
        null,
        DifficultyLevel.Beginner,
        DifficultyLevel.Intermediate,
        DifficultyLevel.Advanced,
        DifficultyLevel.Production,
        DifficultyLevel.Architecture,
        DifficultyLevel.SystemDesign
    };

    public BrowseViewModel(ICatalogApiClient catalog, IQuestionApiClient questions, NavigationService navigation)
    {
        this.catalog = catalog;
        this.questions = questions;
        this.navigation = navigation;
    }

    public override async Task InitializeAsync()
    {
        await LoadCatalogAsync();
        await LoadQuestionsAsync(1);
    }

    [RelayCommand]
    private async Task CategoryChangedAsync()
    {
        await LoadTopicsAsync();
        await LoadQuestionsAsync(1);
    }

    [RelayCommand]
    private async Task FilterChangedAsync()
    {
        await LoadQuestionsAsync(1);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadQuestionsAsync(1);
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (Page > 1)
        {
            await LoadQuestionsAsync(Page - 1);
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (Page < TotalPages)
        {
            await LoadQuestionsAsync(Page + 1);
        }
    }

    [RelayCommand]
    private async Task OpenQuestionAsync(QuestionSummaryModel? question)
    {
        if (question is null)
        {
            return;
        }

        await this.navigation.NavigateToAsync<QuestionDetailViewModel>(detail => detail.QuestionId = question.Id);
    }

    private async Task LoadCatalogAsync()
    {
        try
        {
            IReadOnlyList<CategoryModel> categories = await this.catalog.GetCategoriesAsync(CancellationToken.None);

            Categories.Clear();

            foreach (CategoryModel category in categories)
            {
                Categories.Add(category);
            }

            await LoadTopicsAsync();
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    private async Task LoadTopicsAsync()
    {
        try
        {
            int? categoryId = SelectedCategory?.Id;

            IReadOnlyList<TopicModel> topics = await this.catalog.GetTopicsAsync(categoryId, CancellationToken.None);

            Topics.Clear();
            SelectedTopic = null;

            foreach (TopicModel topic in topics)
            {
                Topics.Add(topic);
            }
        }
        catch (Exception ex)
        {
            ShowError(ApiException.UserMessage(ex));
        }
    }

    private async Task LoadQuestionsAsync(int page)
    {
        IsBusy = true;
        ClearError();

        try
        {
            PagedResult<QuestionSummaryModel> result = await this.questions.GetPagedAsync(
                page, PageSize, SelectedCategory?.Id, SelectedTopic?.Id, SelectedDifficulty,
                string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim(),
                null, false, CancellationToken.None);

            Page = result.Page;
            TotalPages = result.TotalPages;
            TotalCount = result.TotalCount;

            Questions.Clear();

            foreach (QuestionSummaryModel question in result.Items)
            {
                Questions.Add(question);
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
}
