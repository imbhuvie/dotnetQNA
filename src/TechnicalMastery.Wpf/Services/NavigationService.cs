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
        TViewModel viewModel = this.services.GetRequiredService<TViewModel>();
        CurrentViewModel = viewModel;
        await viewModel.InitializeAsync();
    }
}
