using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using TechnicalMastery.Wpf.ViewModels;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// View-model-first navigation. Views are resolved by WPF data templates;
/// this service only swaps the current view model and initializes it.
/// No view, page, or control type is referenced here — navigation targets
/// are view models, keeping code-behind free of logic (Rule: MVVM).
/// Implements INotifyPropertyChanged directly (same pattern as ViewModelBase).
/// </summary>
public class NavigationService : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly IServiceProvider services;

    private ViewModelBase? currentViewModel;

    public NavigationService(IServiceProvider services)
    {
        this.services = services;
    }

    public ViewModelBase? CurrentViewModel
    {
        get { return this.currentViewModel; }
        private set
        {
            if (!EqualityComparer<ViewModelBase?>.Default.Equals(this.currentViewModel, value))
            {
                this.currentViewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
            }
        }
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
