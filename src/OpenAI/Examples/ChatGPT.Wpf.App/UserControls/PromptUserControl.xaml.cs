﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatGPT.Wpf.App.Models;

namespace ChatGPT.Wpf.App.UserControls
{
    /// <summary>
    /// Interaction logic for PromptUserControl.xaml
    /// </summary>
    public partial class PromptUserControl : UserControl
    {
        public PromptUserControl()
        {
            InitializeComponent();
        }

        public new event KeyEventHandler KeyUp;

        public event EventHandler ButtonClicked;

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(nameof(Prompt), typeof(PromptTextModel), typeof(PromptUserControl));
        public PromptTextModel Prompt
        {
            get => (PromptTextModel)GetValue(PromptProperty);
            set => SetValue(PromptProperty, value);
        }

        public static readonly DependencyProperty IsReadyProperty = DependencyProperty.Register(nameof(IsReady), typeof(bool), typeof(PromptUserControl));
        public bool IsReady
        {
            get => (bool)GetValue(IsReadyProperty);
            set => SetValue(IsReadyProperty, value);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(sender, e);
        }

        private void Key_OnUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(sender, e);
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            Prompt.Text = "";
        }
    }
}
