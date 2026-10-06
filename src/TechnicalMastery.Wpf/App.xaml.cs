using System.IO;
using System.Windows;
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
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<BrowseViewModel>();
        builder.Services.AddTransient<BookmarksViewModel>();
        builder.Services.AddTransient<ProgressViewModel>();
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
}
