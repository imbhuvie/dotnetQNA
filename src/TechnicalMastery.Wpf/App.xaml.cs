using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechnicalMastery.Wpf.Services;
using TechnicalMastery.Wpf.ViewModels;

namespace TechnicalMastery.Wpf;

/// <summary>
/// Application entry point. Builds a generic host (configuration + DI) and
/// opens the main shell window. Views and view models are registered in
/// later phases; this file only owns host lifetime.
/// </summary>
public partial class App : Application
{
    private IHost? host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // §20 on the client: no silent process deaths. UI-thread faults show a
        // friendly message and are logged with full stacks (including inners);
        // the app survives. Non-UI faults are logged before exit.
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // One typed HttpClient from the factory (never newed per request, §18).
        // The base address comes from appsettings — a single configurable place.
        string apiBaseUrl = builder.Configuration["Api:BaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Api:BaseUrl' is missing.");

        builder.Services.AddHttpClient<StudyApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddTransient<ICatalogApiClient>(provider => provider.GetRequiredService<StudyApiClient>());
        builder.Services.AddTransient<IQuestionApiClient>(provider => provider.GetRequiredService<StudyApiClient>());
        builder.Services.AddTransient<IBookmarkApiClient>(provider => provider.GetRequiredService<StudyApiClient>());
        builder.Services.AddTransient<IProgressApiClient>(provider => provider.GetRequiredService<StudyApiClient>());
        builder.Services.AddTransient<INotesApiClient>(provider => provider.GetRequiredService<StudyApiClient>());
        builder.Services.AddTransient<IDashboardApiClient>(provider => provider.GetRequiredService<StudyApiClient>());

        // View/view-model registrations arrive in Phase 17+.
        builder.Services.AddSingleton<NavigationService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<BrowseViewModel>();
        builder.Services.AddTransient<BookmarksViewModel>();
        builder.Services.AddTransient<ProgressViewModel>();
        builder.Services.AddTransient<QuestionDetailViewModel>();
        builder.Services.AddTransient<NotesViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<AboutViewModel>();

        this.host = builder.Build();
        this.host.Start();

        MainWindow mainWindow = this.host.Services.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (this.host is not null)
        {
            await this.host.StopAsync();
            this.host.Dispose();
        }

        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogCrash(e.Exception);

        MessageBox.Show(
            "Something went wrong, but the app is still running.\n\nDetails were written to the logs folder.",
            ".NET Technical Mastery",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        e.Handled = true;
    }

    private void OnDomainUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            LogCrash(exception);
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogCrash(e.Exception);
        e.SetObserved();
    }

    private static void LogCrash(Exception exception)
    {
        try
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(directory);

            string path = Path.Combine(directory, "wpf-" + DateTime.Now.ToString("yyyyMMdd") + ".txt");

            StringBuilder text = new StringBuilder();
            text.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Unhandled exception:");

            Exception? current = exception;
            while (current is not null)
            {
                text.AppendLine(current.GetType().FullName + ": " + current.Message);
                text.AppendLine(current.StackTrace);
                current = current.InnerException;
            }

            File.AppendAllText(path, text.ToString());
        }
        catch (IOException)
        {
            // Logging must never crash the app a second time.
        }
    }
}
