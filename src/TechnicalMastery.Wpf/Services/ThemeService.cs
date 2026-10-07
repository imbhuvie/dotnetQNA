using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// Holds the active theme and swaps the app's resource dictionary.
/// Views reference theme colors via {DynamicResource …Brush} so they
/// update instantly with no code-behind (§26, §32).
/// </summary>
public partial class ThemeService : ObservableObject
{
    [ObservableProperty]
    private bool isDark;

    public void ApplyTheme(bool dark)
    {
        IsDark = dark;

        ResourceDictionary theme = new ResourceDictionary
        {
            Source = new Uri(
                dark ? "Resources/DarkTheme.xaml" : "Resources/LightTheme.xaml",
                UriKind.Relative)
        };

        System.Collections.ObjectModel.Collection<ResourceDictionary> merged =
            Application.Current.Resources.MergedDictionaries;
        merged.Clear();
        merged.Add(theme);
    }
}
