using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TechnicalMastery.Wpf.ViewModels;

/// <summary>
/// Base for all view models, written by hand: INotifyPropertyChanged plus a
/// SetProperty helper, a busy flag, user-facing error text, and an async
/// initialization hook that <see cref="Services.NavigationService"/> calls
/// after navigation (API loading happens there, never in constructors).
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool isBusy;

    private string? errorMessage;

    public bool IsBusy
    {
        get { return this.isBusy; }
        set { SetProperty(ref this.isBusy, value); }
    }

    public string? ErrorMessage
    {
        get { return this.errorMessage; }
        set { SetProperty(ref this.errorMessage, value); }
    }

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

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        return true;
    }
}
