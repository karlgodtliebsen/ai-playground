using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ChatGPTClient.Models;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.AIClients;
using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace ChatGPTClient;


/// <summary>
/// Interaction logic for ChatGPTControl.xaml
/// </summary>
public partial class EditsControl : UserControl
{
    public EditsControl()
    {
        InitializeComponent();
    }

    private readonly IEditsAIClient aiClient;
    private readonly IModelRequestFactory requestFactory;
    private readonly EditsViewModel viewModel = new();
    private readonly ViewState viewState;
    private readonly ObservableCollection<Model> models = new();

    public EditsControl(
        IEditsAIClient aiClient,
        IModelRequestFactory requestFactory,
        IOptions<OpenAIOptions> options,
        ViewState viewState
    )
    {
        this.viewState = viewState;
        this.aiClient = aiClient;
        this.requestFactory = requestFactory;
        InitializeComponent();
        viewModel.ViewState = viewState;
        DataContext = viewModel;
        viewModel.Prompt.Text = "What day of the wek is it?";
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        SetOpenAIModels();

        viewModel.Instruction = new InstructionTextModel()
        {
            Text = "Fix the spelling mistakes"
        };
        viewModel.Hint = new HintTextModel()
        {
            Text = "The Hint"
        };
    }

    private void SetOpenAIModels()
    {
        models.Clear();
        foreach (var model in requestFactory.GetModels("edits"))
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
        var prompt = viewModel!.Prompt!.Text!.Trim();
        var instructions = viewModel!.Instruction!.Text!.Trim();
        var payload = CreatePayload(prompt, instructions);
        //UpdateUIStatus(prompt);
        //var response = await aiClient.GetEditsAsync(payload, CancellationToken.None);
        //ProcessAnswer(response);
        viewModel.IsReady = true;
    }

    private void ProcessAnswer(OneOf<Completions, ErrorResponse> completionsResponse)
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

    private EditsRequest CreatePayload(string prompt, string instructions)
    {
        var selectedModel = this.viewState.SelectedModel!;
        var options = viewModel.Options;

        var payload = requestFactory.CreateRequest<EditsRequest>(() =>
            new EditsRequest
            {
                //ID of the model to use. You can use the text-davinci-edit-001 or code-davinci-edit-001 model with this endpoint.
                Model = selectedModel.ModelId,
                Input = prompt,
                Instruction = instructions,
                //MaxTokens = options.MaxTokens,
                //Temperature = options.Temperature,
                //TopP = options.TopP,
                //NumChoicesPerPrompt = options.NumChoicesPerPrompt,
                //Stream = options.Stream,
                //Logprobs = options.Logprobs,
                //Stop = options.Stop,

                //Model = "text-davinci-edit-001",
                //Input = "What day of the wek is it?",
                //Instruction = "Fix the spelling mistakes",
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
