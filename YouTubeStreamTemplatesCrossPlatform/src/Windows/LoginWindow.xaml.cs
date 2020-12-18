using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplatesCrossPlatform.Windows
{
    public class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Console.WriteLine(Directory.GetCurrentDirectory());
        }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }

        private void Login_OnPress(object? sender, PointerPressedEventArgs pointerPressedEventArgs)
        {
            var img = (Image) sender!;
            img.Opacity = .75;
        }

        private async void Login_OnClick(object? sender, PointerReleasedEventArgs e)
        {
            var img = (Image) sender!;
            img.Opacity = 1;
            // Service.LiveStreamService = await LiveStreamService.Init();

            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.Show();
            Close();
        }
    }
}