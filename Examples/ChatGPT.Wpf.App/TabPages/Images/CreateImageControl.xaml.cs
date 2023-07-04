using ChatGPTClient.Models;

using Microsoft.Extensions.Options;
using Microsoft.Win32;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.Images;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

    public CreateImageControl(
        IImagesAIClient imagesClient,
        IModelRequestFactory requestFactory,
        IOptions<OpenAIOptions> options,
        ViewState viewState
    )
    {
        this.imagesClient = imagesClient;
        this.requestFactory = requestFactory;
        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.ViewState = viewState;
        ViewModel.Prompt.Text = "A garden with Red Tulips";
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
        var prompt = ViewModel!.Prompt!.Text!.Trim();

        var payload = CreatePayload(prompt);
        UpdateUIStatus(prompt);
        var response = await imagesClient.CreateImageAsync(payload, CancellationToken.None);
        ProcessAnswer(response);
    }

    private void ProcessAnswer(OneOf<GeneratedImage, ErrorResponse> response)
    {
        response.Switch(
            generatedImage =>
            {
                var index = 1;
                foreach (var model in generatedImage.Data)
                {
                    //TODO: use a usercontrol

                    var border = new Border()
                    {
                        Margin = new Thickness(5),
                        CornerRadius = new CornerRadius(10),
                        BorderBrush = System.Windows.Media.Brushes.White,
                        BorderThickness = new Thickness(1),
                        Child = CreateImage(model)
                    };
                    var tabItem = new TabItem
                    {
                        Margin = new Thickness(5),
                        Header = $"Image {index++}",
                        Content = border
                    };
                    TabControl.Items.Add(tabItem);
                    tabItem.Focus();
                }
            },
            error =>
            {
                //ViewModel.Result.Reply.Add(new RichResultViewModel()
                //{
                //Text = error.Error,
                //Role = "error",
                //    Kind = "F",
                //    Success = false,
                //});
            }
        );

        ViewModel.IsReady = true;
    }

    private ImageGenerationRequest CreatePayload(string prompt)
    {
        var options = ViewModel!.Options!;
        var payload = new ImageGenerationRequest
        {
            Prompt = prompt,
            NumberOfImagesToGenerate = options.NumberOfImagesToGenerate,
            ImageSize = options.ImageSize,
            ImageResponseFormat = options.ImageResponseFormat,
        };
        return payload;
    }

    private void UpdateUIStatus(string prompt)
    {
        ViewModel.Question.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Success = true,
        });
        TabControl.Items.Clear();
        ViewModel.IsReady = false;
    }

    private UIElement? CreateImage(ImageData imageData)
    {
        if (imageData.Url is not null)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri(imageData.Url)),
                Stretch = System.Windows.Media.Stretch.Uniform,
            };
        }
        if (imageData.Data is not null)
        {
            var image = GetBitmapImageFromBase64Json(imageData.Data);
            return new Image()
            {
                Source = image,
                Stretch = System.Windows.Media.Stretch.Uniform,
            };
        }
        return default;
    }

    private BitmapImage GetBitmapImageFromBase64Json(string base64JsonImage)
    {
        // Remove the "data:image/jpeg;base64," prefix from the base64 string
        string base64Image = base64JsonImage.Replace("data:image/jpeg;base64,", "");
        byte[] imageData = Convert.FromBase64String(base64Image);
        BitmapImage bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = new System.IO.MemoryStream(imageData);
        bitmap.EndInit();
        return bitmap;
    }

    private void Save_Image(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Image Files (*.png)|*.png";
        saveFileDialog.FileName = "image.png";
        saveFileDialog.DefaultExt = ".png";
        if (saveFileDialog.ShowDialog() == true)
        {
            var image = (Image)((TabItem)TabControl.SelectedItem).Content;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            using var stream = saveFileDialog.OpenFile();
            encoder.Save(stream);
        }
    }
}
