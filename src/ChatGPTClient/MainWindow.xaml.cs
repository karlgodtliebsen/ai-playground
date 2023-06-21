using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ChatGPTClient.Models;

using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;

namespace ChatGPTClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly IChatCompletionAIClient chatCompletionClient;
        //private readonly ICompletionAIClient completionClient;
        //private readonly IModelRequestFactory requestFactory;
        private readonly OpenAIOptions options;

        private readonly ViewModel ViewModel;

        public MainWindow(
            //IChatCompletionAIClient chatCompletionClient,
            //ICompletionAIClient completionClient,
            //IModelRequestFactory requestFactory,
            ViewState viewState,
            IOptions<OpenAIOptions> options
            )
        {
            this.options = options.Value;
            //this.chatCompletionClient = chatCompletionClient;
            //this.completionClient = completionClient;
            //this.requestFactory = requestFactory;
            //this.viewState = viewState.Value;
            ViewModel = new ViewModel()
            {
                ApiKey = new ApiKeyViewModel() { ApiKey = this.options.ApiKey },
                ViewState = viewState
            };


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
            var tabItem = new TabItem
            {
                Header = header,
                Content = content
            };
            TabControl.Items.Add(tabItem);
        }
    }
}
