using System.Windows.Input;

namespace TechnicalMastery.Wpf.Commands;

/// <summary>
/// Hand-written ICommand for actions taking a parameter (e.g. the clicked
/// question). A null or wrongly-typed parameter arrives as default(T).
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Func<T?, Task> executeAsync;
    private readonly Action<T?>? executeSync;

    public RelayCommand(Func<T?, Task> execute)
    {
        this.executeAsync = execute;
    }

    public RelayCommand(Action<T?> execute)
    {
        this.executeSync = execute;
        this.executeAsync = _ => Task.CompletedTask;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        T? value = parameter is T typed ? typed : default;

        if (this.executeSync is not null)
        {
            this.executeSync(value);
            return;
        }

        await this.executeAsync(value);
    }
}
