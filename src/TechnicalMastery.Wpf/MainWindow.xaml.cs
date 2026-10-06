using System.Windows;
using TechnicalMastery.Wpf.ViewModels;

namespace TechnicalMastery.Wpf;

/// <summary>
/// Shell window. The only code-behind in the project: assigns the injected
/// view model and navigates to the dashboard. No API calls, no logic.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await this.viewModel.ShowDashboardAsync();
    }
}
