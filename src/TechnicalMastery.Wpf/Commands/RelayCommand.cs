using System.Windows.Input;

namespace TechnicalMastery.Wpf.Commands;

/// <summary>
/// Hand-written ICommand for parameterless actions (async or sync).
/// Replaces the toolkit's generated commands: every line here is visible,
/// no source generator involved.
/// <para>
/// Async handlers run as async void (the only legal shape for
/// <see cref="ICommand.Execute"/>). View models catch their own exceptions;
/// anything escaping reaches the global handler in App.xaml.cs.
/// </para>
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Func<Task> executeAsync;
    private readonly Action? executeSync;
    private readonly Func<bool>? canExecute;

    public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        this.executeAsync = execute;
        this.canExecute = canExecute;
    }

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.executeSync = execute;
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return this.canExecute?.Invoke() ?? true;
    }

    public async void Execute(object? parameter)
    {
        if (this.executeSync is not null)
        {
            this.executeSync();
            return;
        }

        await this.executeAsync();
    }
}
