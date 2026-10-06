using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TechnicalMastery.Wpf.ViewModels;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// View-model-first navigation. Views are resolved by WPF data templates;
/// this service only swaps the current view model and initializes it.
/// No view, page, or control type is referenced here — navigation targets
/// are view models, keeping code-behind free of logic (Rule: MVVM).
/// </summary>
public partial class NavigationService : ObservableObject
{
    private readonly IServiceProvider services;

    [ObservableProperty]
    private ViewModelBase? currentViewModel;

    public NavigationService(IServiceProvider services)
    {
        this.services = services;
    }

    public async Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
    {
        await NavigateToAsync<TViewModel>(_ => { });
    }

    /// <summary>
    /// Navigates and configures the view model (e.g. setting the question id)
    /// before it initializes — so detail screens load the right data.
    /// </summary>
    public async Task NavigateToAsync<TViewModel>(Action<TViewModel> configure) where TViewModel : ViewModelBase
    {
        TViewModel viewModel = this.services.GetRequiredService<TViewModel>();
        configure(viewModel);
        CurrentViewModel = viewModel;
        await viewModel.InitializeAsync();
    }
}
