using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// Holds the active theme and swaps the app's resource dictionary.
/// Views reference theme colors via {DynamicResource …Brush} so they
/// update instantly with no code-behind (§26, §32).
/// </summary>
public class ThemeService : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool isDark;

    public bool IsDark
    {
        get { return this.isDark; }
        private set
        {
            if (this.isDark != value)
            {
                this.isDark = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDark)));
            }
        }
    }

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
