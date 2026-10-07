using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using TechnicalMastery.Wpf.Commands;
using TechnicalMastery.Wpf.Services;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Settings screen: theme toggle and the configured API address (§31).
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private readonly ThemeService theme;

    private bool isDarkTheme;

    public string ApiBaseUrl { get; }

    public SettingsViewModel(ThemeService theme, IConfiguration configuration)
    {
        this.theme = theme;
        ApiBaseUrl = configuration["Api:BaseUrl"] ?? "(not configured)";
        IsDarkTheme = theme.IsDark;

        ToggleThemeCommand = new RelayCommand(ToggleTheme);
    }

    public bool IsDarkTheme
    {
        get { return this.isDarkTheme; }
        set { SetProperty(ref this.isDarkTheme, value); }
    }

    public ICommand ToggleThemeCommand { get; }

    private void ToggleTheme()
    {
        this.theme.ApplyTheme(IsDarkTheme);
    }
}
