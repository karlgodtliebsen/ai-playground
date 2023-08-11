using System;
using System.Collections.Generic;
using System.Windows;

using AI.Library.Configuration;

using ChatGPT.Wpf.App.Configuration;
using ChatGPT.Wpf.App.TabPages.ChatCompletions;
using ChatGPT.Wpf.App.TabPages.Completions;
using ChatGPT.Wpf.App.TabPages.Edits;
using ChatGPT.Wpf.App.TabPages.Images;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatGPT.Wpf.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

#pragma warning disable CS8618
    private IServiceProvider serviceProvider;
#pragma warning restore CS8618

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var builder = Host.CreateDefaultBuilder();
        builder.WithLogging();
        builder.AddSecrets<App>();
        builder.ConfigureServices((context, services) =>
        {
            services.AddAppConfiguration(context.Configuration);
            services.AddSingleton<MainWindow>();
            //TODO: consider->these could be found using reflection
            services.AddSingleton<CompletionControl>();
            services.AddSingleton<ChatCompletionControl>();
            services.AddSingleton<CreateImageControl>();
            services.AddSingleton<EditsControl>();
        });

        IHost host = builder.Build();
        serviceProvider = host.Services;
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

        mainWindow.SetChildViews(new Dictionary<string, UIElement>() {
            { "Completion", serviceProvider.GetRequiredService<CompletionControl>() },
            { "Chat Completion", serviceProvider.GetRequiredService<ChatCompletionControl>() },
            { "Images", serviceProvider.GetRequiredService<CreateImageControl>() } ,
            { "Edit", serviceProvider.GetRequiredService<EditsControl>() }
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
