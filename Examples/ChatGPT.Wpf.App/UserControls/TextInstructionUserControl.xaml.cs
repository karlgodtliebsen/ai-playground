using System.Windows;
using System.Windows.Controls;

using ChatGPTClient.Models;

namespace ChatGPTClient.UserControls
{
    /// <summary>
    /// Interaction logic for the UserControl.xaml
    /// </summary>
    public partial class TextInstructionUserControl : UserControl
    {
        public TextInstructionUserControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HintProperty = DependencyProperty.Register(nameof(Hint), typeof(HintTextModel), typeof(TextInstructionUserControl));

        public HintTextModel Hint
        {
            get { return (HintTextModel)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(InstructionTextModel), typeof(TextInstructionUserControl));
        public InstructionTextModel Text
        {
            get { return (InstructionTextModel)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            Text.Text = "";
        }
    }
}
