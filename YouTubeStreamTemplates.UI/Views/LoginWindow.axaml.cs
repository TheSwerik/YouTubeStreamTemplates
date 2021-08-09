using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using YouTubeStreamTemplates.UI.ViewModels;

namespace YouTubeStreamTemplates.UI.Views
{
    public class LoginWindow : ReactiveWindow<LoginViewModel>
    {
        public LoginWindow()
        {
            InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
            this.WhenActivated(d => d(ViewModel.LoginCommand.Subscribe(ChangeToMainWindow)));
        }

        private void ChangeToMainWindow(bool b)
        {
            if (!b) return;

            App.ChangeMainWindow(new MainWindow { DataContext = new MainWindowViewModel() });
            Close();
        }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
    }
}