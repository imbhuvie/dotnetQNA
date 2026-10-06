using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        // View/view-model registrations arrive in Phase 17+.
        builder.Services.AddSingleton<MainWindow>();

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
