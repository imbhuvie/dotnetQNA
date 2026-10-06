using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Shell view model: sidebar navigation commands, global search box, and the
/// hosted <see cref="NavigationService"/>. Contains zero business logic —
/// every screen loads its own data after navigation.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly NavigationService navigation;

    [ObservableProperty]
    private string searchText = string.Empty;

    public MainViewModel(NavigationService navigation)
    {
        this.navigation = navigation;
    }

    public NavigationService Navigation
    {
        get { return this.navigation; }
    }

    public async Task ShowDashboardAsync()
    {
        await this.navigation.NavigateToAsync<DashboardViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToDashboardAsync()
    {
        await this.navigation.NavigateToAsync<DashboardViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToBrowseAsync()
    {
        await this.navigation.NavigateToAsync<BrowseViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToBookmarksAsync()
    {
        await this.navigation.NavigateToAsync<BookmarksViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToProgressAsync()
    {
        await this.navigation.NavigateToAsync<ProgressViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToNotesAsync()
    {
        await this.navigation.NavigateToAsync<NotesViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await this.navigation.NavigateToAsync<SettingsViewModel>();
    }

    [RelayCommand]
    private async Task NavigateToAboutAsync()
    {
        await this.navigation.NavigateToAsync<AboutViewModel>();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await this.navigation.NavigateToAsync<BrowseViewModel>();

        if (this.navigation.CurrentViewModel is BrowseViewModel browse)
        {
            browse.SearchText = SearchText;
        }
    }
}
