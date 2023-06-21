using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ChatGPTClient.Models;

using OpenAI.Client.AIClients;
using OpenAI.Client.Domain;
using OpenAI.Client.Models.Requests;

namespace ChatGPTClient
{
    /// <summary>
    /// Interaction logic for ChatGPTControl.xaml
    /// </summary>
    public partial class CompletionControl : UserControl
    {
        public CompletionControl()
        {
            InitializeComponent();
        }

        private readonly ICompletionAIClient completionClient;
        private readonly IModelRequestFactory requestFactory;
        private readonly CompletionViewModel ViewModel = new();
        private readonly ViewState viewState;

        public CompletionControl(
            ICompletionAIClient completionClient,
            IModelRequestFactory requestFactory,
            ViewState viewState
        )
        {
            this.viewState = viewState;
            this.completionClient = completionClient;
            this.requestFactory = requestFactory;
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.ViewState = viewState;
            ViewModel.Prompt.Text = "Translate the following English text to French: 'Hello, how are you?'";
            SetModels();
        }



        private void SetModels()
        {
            var models = new ObservableCollection<Model>();
            foreach (var model in requestFactory.GetModels("completions"))
            {
                models.Add(new Model() { ModelId = model.Trim() });
            }
            this.viewState.Models = models;

            this.viewState.SelectedModel = models[0];
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedModel = this.viewState.SelectedModel;
            var payload = requestFactory.CreateRequest<CompletionRequest>(() =>
                new CompletionRequest
                {
                    Model = selectedModel.ModelId,
                    Prompt = ViewModel.Prompt.Text,
                    MaxTokens = 100,
                    Temperature = 0.1f,
                    TopP = 1.0f,
                    NumChoicesPerPrompt = 1,
                    Stream = false,
                    Logprobs = null,
                    //Stop = "\n"
                });

            var completionsResponse = await completionClient.GetCompletionsAsync(payload, CancellationToken.None);
            if (completionsResponse!.Success)
            {
                var completions = completionsResponse.Value;
                foreach (var choice in completions.Choices)
                {
                    ViewModel.Result.Text.Add(choice.Text);
                }
            }
        }
    }
}
public class CompletionViewModel
{
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
    public ViewState ViewState { get; set; }
}
