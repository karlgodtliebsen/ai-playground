﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ChatGPT.Wpf.App.Models;
using Microsoft.Extensions.Options;
using OpenAI.Client.Configuration;

namespace ChatGPT.Wpf.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow(IOptions<OpenAIConfiguration> options)
    {
        var viewModel = new ViewModel()
        {
            ApiKey = new ApiKeyViewModel() { ApiKey = options.Value.ApiKey },
        };
        InitializeComponent();
        DataContext = viewModel;
    }

    public void SetChildViews(IDictionary<string, UIElement> userControls)
    {
        TabControl.Items.Clear();
        foreach (var uiElement in userControls)
        {
            AddTabPage(uiElement.Key, uiElement.Value);
        }
    }


    private void AddTabPage(string header, UIElement content)
    {
        var tabItem = new TabItem
        {
            Header = header,
            Content = content
        };
        TabControl.Items.Add(tabItem);
    }

    private void ChangeWindow_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as ToggleButton;
        WindowStyle = button!.IsChecked == true ? WindowStyle.ThreeDBorderWindow : WindowStyle.ToolWindow;
    }
}
