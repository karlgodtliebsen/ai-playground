using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ChatGPTClient.Models;

using Microsoft.Extensions.Options;

using OpenAI.Client.AIClients;
using OpenAI.Client.Configuration;

using OpenAI.Client.Domain;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;

namespace ChatGPTClient.TabPages;

/// <summary>
/// Interaction logic for CreateImage.xaml
/// </summary>
public partial class CreateImageControl : UserControl
{
    public CreateImageControl()
    {
        InitializeComponent();
    }

    private readonly IImagesAIClient imagesClient;
    private readonly IModelRequestFactory requestFactory;
    private readonly CreateImageViewModel ViewModel = new();
    private readonly ViewState viewState;

    public CreateImageControl(
        IImagesAIClient imagesClient,
        IModelRequestFactory requestFactory,
        IOptions<OpenAIOptions> options,
        ViewState viewState
    )
    {
        this.viewState = viewState;
        this.imagesClient = imagesClient;
        this.requestFactory = requestFactory;
        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.PopupViewModel.ApiKey = options.Value.ApiKey;
        ViewModel.ViewState = viewState;
        ViewModel.Prompt.Text = "A cute baby sea otter";
    }


    private async void Key_OnUp(object sender, RoutedEventArgs e)
    {
        var ev = e as KeyEventArgs;
        if (ev?.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
        {
            await Submit();
        }
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        await Submit();
    }

    private async Task Submit()
    {
        var prompt = ViewModel!.Prompt!.Text!.Trim();
        var payload =

            new ImageGenerationRequest
            {
                Prompt = prompt,
                NumberOfImagesToGenerate = 1,
                ImageSize = ImageSize.Size1024
            };


        ViewModel.Question.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Success = true,
        });

        var response = await imagesClient.CreateImageAsync(payload, CancellationToken.None);
        if (response!.Success)
        {
            foreach (var model in response!.Value.Data)
            {
                //model.Data
                ViewModel.Result.ImagePath = model.Url;
            }
        }
        else
        {
            //ViewModel.Result.Reply.Add(new RichResultViewModel()
            //{
            //    Text = "Error",
            //    Kind = "F",
            //    Success = false,
            //});
        }
    }
}


public class CreateImagePopupViewModel
{
    public string ApiKey { get; set; }
}

public class CreateImageViewModel
{
    public ObservableCollection<RichResultViewModel> Question { get; } = new() { };

    public CreateImagePopupViewModel PopupViewModel { get; } = new();
    public ImageResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
    public ViewState ViewState { get; set; }
}




public sealed class ImageResultViewModel : INotifyPropertyChanged
{
    private string? imagePath = default!;

    public string? ImagePath
    {
        get { return imagePath; }
        set
        {
            if (imagePath != value)
            {
                imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
