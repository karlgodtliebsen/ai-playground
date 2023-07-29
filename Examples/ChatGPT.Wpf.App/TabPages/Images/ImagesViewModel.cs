using ChatGPT.Wpf.App.Models;

namespace ChatGPT.Wpf.App.TabPages.Images;

public sealed class ImagesViewModel : BaseViewModel
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
}
