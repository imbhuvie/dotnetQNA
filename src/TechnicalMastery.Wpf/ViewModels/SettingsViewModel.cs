using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Settings screen: theme toggle and the configured API address (§31).
/// </summary>
public partial class SettingsViewModel : ViewModelBase
{
    private readonly ThemeService theme;

    [ObservableProperty]
    private bool isDarkTheme;

    public string ApiBaseUrl { get; }

    public SettingsViewModel(ThemeService theme, IConfiguration configuration)
    {
        this.theme = theme;
        ApiBaseUrl = configuration["Api:BaseUrl"] ?? "(not configured)";
        IsDarkTheme = theme.IsDark;
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        this.theme.ApplyTheme(IsDarkTheme);
    }
}
