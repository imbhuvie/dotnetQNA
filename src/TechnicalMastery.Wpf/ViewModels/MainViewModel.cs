using System.Windows.Input;
using TechnicalMastery.Wpf.Commands;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Shell view model: sidebar navigation commands, global search box, and the
/// hosted <see cref="NavigationService"/>. Contains zero business logic —
/// every screen loads its own data after navigation.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly NavigationService navigation;

    private string searchText = string.Empty;

    public MainViewModel(NavigationService navigation)
    {
        this.navigation = navigation;

        NavigateToDashboardCommand = new RelayCommand(NavigateToDashboardAsync);
        NavigateToBrowseCommand = new RelayCommand(NavigateToBrowseAsync);
        NavigateToBookmarksCommand = new RelayCommand(NavigateToBookmarksAsync);
        NavigateToProgressCommand = new RelayCommand(NavigateToProgressAsync);
        NavigateToNotesCommand = new RelayCommand(NavigateToNotesAsync);
        NavigateToSettingsCommand = new RelayCommand(NavigateToSettingsAsync);
        NavigateToAboutCommand = new RelayCommand(NavigateToAboutAsync);
        SearchCommand = new RelayCommand(SearchAsync);
    }

    public string SearchText
    {
        get { return this.searchText; }
        set { SetProperty(ref this.searchText, value); }
    }

    public NavigationService Navigation
    {
        get { return this.navigation; }
    }

    public ICommand NavigateToDashboardCommand { get; }

    public ICommand NavigateToBrowseCommand { get; }

    public ICommand NavigateToBookmarksCommand { get; }

    public ICommand NavigateToProgressCommand { get; }

    public ICommand NavigateToNotesCommand { get; }

    public ICommand NavigateToSettingsCommand { get; }

    public ICommand NavigateToAboutCommand { get; }

    public ICommand SearchCommand { get; }

    public async Task ShowDashboardAsync()
    {
        await this.navigation.NavigateToAsync<DashboardViewModel>();
    }

    private async Task NavigateToDashboardAsync()
    {
        await this.navigation.NavigateToAsync<DashboardViewModel>();
    }

    private async Task NavigateToBrowseAsync()
    {
        await this.navigation.NavigateToAsync<BrowseViewModel>();
    }

    private async Task NavigateToBookmarksAsync()
    {
        await this.navigation.NavigateToAsync<BookmarksViewModel>();
    }

    private async Task NavigateToProgressAsync()
    {
        await this.navigation.NavigateToAsync<ProgressViewModel>();
    }

    private async Task NavigateToNotesAsync()
    {
        await this.navigation.NavigateToAsync<NotesViewModel>();
    }

    private async Task NavigateToSettingsAsync()
    {
        await this.navigation.NavigateToAsync<SettingsViewModel>();
    }

    private async Task NavigateToAboutAsync()
    {
        await this.navigation.NavigateToAsync<AboutViewModel>();
    }

    private async Task SearchAsync()
    {
        // Configure-before-initialize: the query is set before Browse loads,
        // so the first load already applies it (server-side search, §14).
        string query = SearchText;

        await this.navigation.NavigateToAsync<BrowseViewModel>(browse => browse.SearchText = query);
    }
}
