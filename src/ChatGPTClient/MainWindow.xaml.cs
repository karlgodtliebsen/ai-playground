using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.Options;

using OpenAI.Client.AIClients;
using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;

namespace ChatGPTClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IChatCompletionAIClient chatCompletionClient;
        private readonly ICompletionAIClient completionClient;
        private readonly IModelRequestFactory requestFactory;
        private readonly ViewState viewState;
        private readonly OpenAIOptions options;

        private readonly ViewModel ViewModel = new();

        public MainWindow(
            IChatCompletionAIClient chatCompletionClient,
            ICompletionAIClient completionClient,
            IModelRequestFactory requestFactory,
            IOptions<ViewState> viewState,
            IOptions<OpenAIOptions> options
            )
        {
            this.chatCompletionClient = chatCompletionClient;
            this.completionClient = completionClient;
            this.requestFactory = requestFactory;
            this.viewState = viewState.Value;
            this.options = options.Value;
            InitializeComponent();
            WindowState = WindowState.Maximized;
            DataContext = ViewModel;
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
            TabItem tabItem = new TabItem();
            tabItem.Header = header;
            tabItem.Content = content;
            TabControl.Items.Add(tabItem);
        }
    }
}
