using System.Threading;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.Options;

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
            IOptions<ViewState> viewState
        )
        {
            this.viewState = viewState.Value;
            this.completionClient = completionClient;
            this.requestFactory = requestFactory;
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Prompt.Text = "Translate the following English text to French: 'Hello, how are you?'";

            //this.viewState.Models.Clear();
            //var models = new ObservableCollection<Model>();
            //foreach (var model in requestFactory.GetModels("completions"))
            //{
            //    this.viewState.Models.Add(new Model() { ModelId = model });
            //}
            //// this.viewState.Models = models;
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    this.viewState.Models.Clear();
        //    var models = new ObservableCollection<Model>();
        //    foreach (var model in requestFactory.GetModels("completions"))
        //    {
        //        this.viewState.Models.Add(new Model() { ModelId = model });
        //    }
        //    // this.viewState.Models = models;
        //    base.OnRender(drawingContext);
        //}

        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    base.OnGotFocus(e);
        //}

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var payload = requestFactory.CreateRequest<CompletionRequest>(() =>
                new CompletionRequest
                {
                    Model = viewState.SelectedModel.ModelId,
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
}
