using CommunityToolkit.Mvvm.ComponentModel;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Base for all view models: busy flag, user-facing error text, and an
/// async initialization hook that <see cref="Services.NavigationService"/>
/// calls after navigation (API loading happens there, never in constructors).
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected void ShowError(string message)
    {
        ErrorMessage = message;
    }

    protected void ClearError()
    {
        ErrorMessage = null;
    }
}
