using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ChatGPTClient.Models;

using Microsoft.Extensions.Options;

using OpenAI.Client.AIClients;
using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.Models.Requests;

namespace ChatGPTClient;


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
    private readonly CompletionViewModel viewModel = new();
    private readonly ViewState viewState;
    private readonly ObservableCollection<Model> models = new();

    public CompletionControl(
        ICompletionAIClient completionClient,
        IModelRequestFactory requestFactory,
        IOptions<OpenAIOptions> options,
        ViewState viewState
    )
    {
        this.viewState = viewState;
        this.completionClient = completionClient;
        this.requestFactory = requestFactory;
        InitializeComponent();
        viewModel.ViewState = viewState;
        DataContext = viewModel;
        viewModel.Prompt.Text = "Translate the following English text to French: 'Hello, how are you?'";
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        SetOpenAIModels();
    }

    private void SetOpenAIModels()
    {
        models.Clear();
        foreach (var model in requestFactory.GetModels("completions"))
        {
            models.Add(new Model() { ModelId = model.Trim() });
        }
        this.viewState.Models = models;
        this.viewState.SelectedModel = models.FirstOrDefault();
    }

    private async void Key_OnUp(object sender, KeyEventArgs ev)
    {
        if (ev?.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
        {
            await Submit();
        }
    }

    private async void ButtonBase_OnClick(object sender, EventArgs e)
    {
        await Submit();
    }

    private async Task Submit()
    {
        var selectedModel = this.viewState.SelectedModel;
        var prompt = viewModel!.Prompt!.Text!.Trim();
        var options = viewModel.Options;
        var payload = requestFactory.CreateRequest<CompletionRequest>(() =>
            new CompletionRequest
            {
                Model = selectedModel.ModelId,
                Prompt = prompt,
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                NumChoicesPerPrompt = options.NumChoicesPerPrompt,
                Stream = options.Stream,
                Logprobs = options.Logprobs,
                Stop = options.Stop,
            });

        viewModel.Result.Reply.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Success = true,
        });
        viewModel.IsReady = false;
        var completionsResponse = await completionClient.GetCompletionsAsync(payload, CancellationToken.None);
        if (completionsResponse!.Success)
        {
            var completions = completionsResponse.Value;
            foreach (var choice in completions.Choices)
            {
                viewModel.Result.Reply.Add(new RichResultViewModel()
                {
                    Text = choice.Text.Trim(),
                    Kind = "A",
                    Success = true,
                });
            }
        }
        else
        {
            viewModel.Result.Reply.Add(new RichResultViewModel()
            {
                Text = "Error",
                Kind = "F",
                Success = false,
            });
        }
        viewModel.IsReady = true;

    }

    private void Copy_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var result = button?.Tag as string;
        Clipboard.SetText(result);
    }
}
