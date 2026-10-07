using System.Collections.ObjectModel;
using System.Windows.Input;
using TechnicalMastery.Wpf.Commands;
using TechnicalMastery.Wpf.Models;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Browse screen (§12): category → topic → question list with difficulty filter,
/// search text and server-side pagination. Opening a question navigates to the
/// detail screen with the selected id.
/// </summary>
public class BrowseViewModel : ViewModelBase
{
    private const int PageSize = 20;

    private readonly ICatalogApiClient catalog;
    private readonly IQuestionApiClient questions;
    private readonly NavigationService navigation;

    private string searchText = string.Empty;

    private CategoryModel? selectedCategory;

    private TopicModel? selectedTopic;

    private DifficultyLevel? selectedDifficulty;

    private int page = 1;

    private int totalPages;

    private int totalCount;

    private string selectedSort = "Default";

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

    /// <summary>
    /// Display labels mapped to API (sortBy, descending) in <see cref="LoadQuestionsAsync"/>.
    /// </summary>
    public List<string> SortOptions { get; } = new List<string>
    {
        "Default",
        "Newest first",
        "Oldest first",
        "Hardest first",
        "Easiest first"
    };

    public BrowseViewModel(ICatalogApiClient catalog, IQuestionApiClient questions, NavigationService navigation)
    {
        this.catalog = catalog;
        this.questions = questions;
        this.navigation = navigation;

        CategoryChangedCommand = new RelayCommand(CategoryChangedAsync);
        FilterChangedCommand = new RelayCommand(FilterChangedAsync);
        SearchCommand = new RelayCommand(SearchAsync);
        PreviousPageCommand = new RelayCommand(PreviousPageAsync);
        NextPageCommand = new RelayCommand(NextPageAsync);
        OpenQuestionCommand = new RelayCommand<QuestionSummaryModel>(OpenQuestionAsync);
    }

    public string SearchText
    {
        get { return this.searchText; }
        set { SetProperty(ref this.searchText, value); }
    }

    public CategoryModel? SelectedCategory
    {
        get { return this.selectedCategory; }
        set { SetProperty(ref this.selectedCategory, value); }
    }

    public TopicModel? SelectedTopic
    {
        get { return this.selectedTopic; }
        set { SetProperty(ref this.selectedTopic, value); }
    }

    public DifficultyLevel? SelectedDifficulty
    {
        get { return this.selectedDifficulty; }
        set { SetProperty(ref this.selectedDifficulty, value); }
    }

    public int Page
    {
        get { return this.page; }
        set { SetProperty(ref this.page, value); }
    }

    public int TotalPages
    {
        get { return this.totalPages; }
        set { SetProperty(ref this.totalPages, value); }
    }

    public int TotalCount
    {
        get { return this.totalCount; }
        set { SetProperty(ref this.totalCount, value); }
    }

    public string SelectedSort
    {
        get { return this.selectedSort; }
        set { SetProperty(ref this.selectedSort, value); }
    }

    public ICommand CategoryChangedCommand { get; }

    public ICommand FilterChangedCommand { get; }

    public ICommand SearchCommand { get; }

    public ICommand PreviousPageCommand { get; }

    public ICommand NextPageCommand { get; }

    public ICommand OpenQuestionCommand { get; }

    public override async Task InitializeAsync()
    {
        await LoadCatalogAsync();
        await LoadQuestionsAsync(1);
    }

    private async Task CategoryChangedAsync()
    {
        await LoadTopicsAsync();
        await LoadQuestionsAsync(1);
    }

    private async Task FilterChangedAsync()
    {
        await LoadQuestionsAsync(1);
    }

    private async Task SearchAsync()
    {
        await LoadQuestionsAsync(1);
    }

    private async Task PreviousPageAsync()
    {
        if (Page > 1)
        {
            await LoadQuestionsAsync(Page - 1);
        }
    }

    private async Task NextPageAsync()
    {
        if (Page < TotalPages)
        {
            await LoadQuestionsAsync(Page + 1);
        }
    }

    private async Task OpenQuestionAsync(QuestionSummaryModel? question)
    {
        if (question is null)
        {
            return;
        }

        await this.navigation.NavigateToAsync<QuestionDetailViewModel>(detail => detail.QuestionId = question.Id);
    }

    private static (string? SortBy, bool Descending) MapSort(string selected)
    {
        if (selected == "Newest first")
        {
            return ("newest", true);
        }

        if (selected == "Oldest first")
        {
            return ("newest", false);
        }

        if (selected == "Hardest first")
        {
            return ("difficulty", true);
        }

        if (selected == "Easiest first")
        {
            return ("difficulty", false);
        }

        return (null, false);
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
            (string? sortBy, bool descending) = MapSort(SelectedSort);

            PagedResult<QuestionSummaryModel> result = await this.questions.GetPagedAsync(
                page, PageSize, SelectedCategory?.Id, SelectedTopic?.Id, SelectedDifficulty,
                string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim(),
                sortBy, descending, CancellationToken.None);

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
