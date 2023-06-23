using System.Windows.Controls;

namespace ChatGPTClient.Dialogs
{
    /// <summary>
    /// Interaction logic for CompletionUserControl.xaml
    /// </summary>
    public partial class CompletionDialogUserControl : UserControl
    {
        public CompletionDialogUserControl()
        {
            InitializeComponent();
        }

        //public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(Models.CompletionOptionsViewModel), typeof(CompletionDialogUserControl));

        //public Models.CompletionOptionsViewModel Options
        //{
        //    get { return (Models.CompletionOptionsViewModel)GetValue(OptionsProperty); }
        //    set { SetValue(OptionsProperty, value); }
        //}

    }
}
