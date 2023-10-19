using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ChatGPT.Wpf.App.Models;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

using Serilog;

namespace ChatGPT.Wpf.App.TabPages.Completions;


/// <summary>
/// Interaction logic for ChatGPTControl.xaml
/// </summary>
public partial class CompletionControl : UserControl
{
    public CompletionControl()
    {
        InitializeComponent();
    }

    private readonly ICompletionAIClient aiClient;
    private readonly IModelRequestFactory requestFactory;
    private readonly ILogger logger;
    private readonly CompletionViewModel viewModel = new();
    private readonly ViewState viewState;
    private readonly ObservableCollection<Model> models = new();

    public CompletionControl(
        ICompletionAIClient aiClient,
        IModelRequestFactory requestFactory,
        IOptions<OpenAIOptions> options,
        ILogger logger,
        ViewState viewState
    )
    {
        this.viewState = viewState;
        this.aiClient = aiClient;
        this.requestFactory = requestFactory;
        this.logger = logger;
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
        logger.Information("Setting OpenAI models");
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
        logger.Information("Submitting");
        var prompt = viewModel!.Prompt!.Text!.Trim();
        var payload = CreatePayload(prompt);
        UpdateUIStatus(prompt);
        if (payload.Stream.HasValue && !payload.Stream.Value)
        {
            var response = await aiClient.GetCompletionsAsync(payload, CancellationToken.None);
            ProcessAnswer(response);
        }
        else
        {
            var responseCollection = aiClient.GetCompletionsStreamAsync(payload, CancellationToken.None);
            await ProcessStreamedAnswer(responseCollection);
        }
        viewModel.IsReady = true;
    }

    private void ProcessAnswer(OneOf<OpenAI.Client.OpenAI.Models.ChatCompletion.Completions, ErrorResponse> completionsResponse)
    {
        completionsResponse.Switch(
            completions =>
            {
                foreach (var choice in completions.Choices)
                {
                    viewModel.Result.Reply.Add(new RichResultViewModel()
                    {
                        Text = choice.Text.Trim(),
                        Kind = "A",
                        Success = true,
                    });
                }
            },
            error =>
            {
                viewModel.Result.Reply.Add(new RichResultViewModel()
                {
                    Text = error.Error,
                    Role = "error",
                    Kind = "F",
                    Success = false,
                });
            }
        );
    }
    private async Task ProcessStreamedAnswer(IAsyncEnumerable<OneOf<OpenAI.Client.OpenAI.Models.ChatCompletion.Completions, ErrorResponse>> responseCollection)
    {
        await foreach (var response in responseCollection)
        {
            response.Switch(
                completions =>
                {
                    foreach (var choice in completions.Choices)
                    {
                        viewModel.Result.Reply.Add(new RichResultViewModel()
                        {
                            Text = choice.Text.Trim(),
                            Kind = "A",
                            Success = true,
                        });
                    }
                },
                error =>
                {
                    viewModel.Result.Reply.Add(new RichResultViewModel()
                    {
                        Text = error.Error,
                        Role = "error",
                        Kind = "F",
                        Success = false,
                    });
                }
            );
        }
    }
    private CompletionRequest CreatePayload(string prompt)
    {
        var selectedModel = this.viewState.SelectedModel!;
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
        return payload;
    }

    private void UpdateUIStatus(string prompt)
    {
        viewModel.IsReady = false;
        viewModel.Result.Reply.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Success = true,
        });
    }

    private void Copy_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var result = button?.Tag as string;
        Clipboard.SetText(result);
    }

    private void Clear_OnClick(object sender, RoutedEventArgs e)
    {
        viewModel.Result.Reply.Clear();
    }
}
