using System.Threading;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.Options;

using OpenAI.Client.AIClients;
using OpenAI.Client.Domain;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;

namespace ChatGPTClient
{
    /// <summary>
    /// Interaction logic for ChatGPTControl.xaml
    /// </summary>
    public partial class ChatCompletionControl : UserControl
    {
        public ChatCompletionControl()
        {
            InitializeComponent();
        }

        private readonly IChatCompletionAIClient aiClient;
        private readonly IModelRequestFactory requestFactory;

        private readonly ChatCompletionViewModel ViewModel = new();
        private readonly ViewState viewState;

        public ChatCompletionControl(
            IChatCompletionAIClient chatCompletionClient,
            IModelRequestFactory requestFactory,
            IOptions<ViewState> viewState
        )
        {
            this.viewState = viewState.Value;
            this.aiClient = chatCompletionClient;
            this.requestFactory = requestFactory;
            InitializeComponent();
            DataContext = ViewModel;

            //ViewModel.Prompt.Text = "Hello, I'm a helpful assistant. What can I help you with today?";

            //this.viewState.Models.Clear();
            //var models = new ObservableCollection<Model>();
            //foreach (var model in requestFactory.GetModels("chat/completions"))
            //{
            //    models.Add(new Model() { ModelId = model });
            //}
            //this.viewState.Models = models;

        }


        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string deploymentName = "gpt-3.5-turbo";

            var messages = new[]
            {
                new ChatCompletionMessage {Role = "system", Content = "You are a helpful assistant.!" },
                new ChatCompletionMessage { Role = "user", Content = ViewModel.Prompt.Text }
            };

            var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
                new ChatCompletionRequest
                {
                    Model = deploymentName, //viewState.Model.ModelId,
                    Messages = messages,
                    MaxTokens = 100,
                    Temperature = 0.0f,
                    TopP = 1.0f,
                    NumChoicesPerPrompt = 1,
                });

            var response = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
            if (response!.Success)
            {
                var completions = response.Value;
                foreach (var choice in completions.Choices)
                {
                    ViewModel.Result.Text.Add(choice.Message.Content.Trim());
                }
            }
        }
    }
}
public class ChatCompletionViewModel
{
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
}
