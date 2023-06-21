using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ChatGPTClient.Models;

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
        private IList<ChatCompletionMessage> messages = new List<ChatCompletionMessage>();
        public ChatCompletionControl(
            IChatCompletionAIClient chatCompletionClient,
            IModelRequestFactory requestFactory,
            ViewState viewState
        )
        {
            this.viewState = viewState;
            this.aiClient = chatCompletionClient;
            this.requestFactory = requestFactory;
            InitializeComponent();
            ViewModel.ViewState = viewState;
            DataContext = ViewModel;
            messages.Add(new ChatCompletionMessage { Role = "system", Content = "You are a helpful assistant that provides information." });
            SetModels();
        }



        private void SetModels()
        {
            var models = new ObservableCollection<Models.Model>();
            foreach (var model in requestFactory.GetModels("chat/completions"))
            {
                models.Add(new Models.Model() { ModelId = model.Trim() });
            }
            this.viewState.Models = models;
            this.viewState.SelectedModel = models[0];
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedModel = this.viewState.SelectedModel;
            messages.Add(new ChatCompletionMessage { Role = "user", Content = ViewModel.Prompt.Text });

            var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
                new ChatCompletionRequest
                {
                    Model = selectedModel.ModelId,
                    Messages = messages.ToArray(),
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
                    messages.Add(new ChatCompletionMessage { Role = "assistant", Content = choice.Message.Content.Trim() });
                }
                //catch the assistant roles answer and use it as the next prompt
            }
        }
    }
}
public class ChatCompletionViewModel
{
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
    public ViewState ViewState { get; set; }
}
