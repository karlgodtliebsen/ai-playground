using System;
using System.Collections.Generic;
using System.Windows;

using AI.Library.Configuration;

using ChatGPTClient.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatGPTClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    const string applicationName = "ChatGPT Client ";

    private IServiceProvider serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        IHostBuilder builder = Host.CreateDefaultBuilder();
        builder.AddHostLogging();
        builder.AddSecrets<App>();
        builder.ConfigureServices((context, services) =>
        {
            services.AddAppConfiguration(context.Configuration);
            services.AddSingleton<MainWindow>();
            services.AddSingleton<CompletionControl>();
            services.AddSingleton<ChatCompletionControl>();
        });

        IHost host = builder.Build();
        serviceProvider = host.Services;
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

        mainWindow.SetChildViews(new Dictionary<string, UIElement>() {
            { "Completion", serviceProvider.GetRequiredService<CompletionControl>() },
            { "Chat Completion", serviceProvider.GetRequiredService<ChatCompletionControl>() }
        });

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        base.OnExit(e);
    }


}
