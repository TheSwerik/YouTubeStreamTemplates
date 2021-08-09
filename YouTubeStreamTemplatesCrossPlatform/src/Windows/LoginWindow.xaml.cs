using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;

namespace YouTubeStreamTemplatesCrossPlatform.Windows
{
    public class LoginWindow : Window
    {
        public LoginWindow() { InitializeComponent(); }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }

        private void Login_OnPress(object sender, PointerPressedEventArgs pointerPressedEventArgs)
        {
            ((Image)sender).Opacity = .75;
        }

        private async void Login_OnClick(object sender, PointerReleasedEventArgs e)
        {
            ((Image)sender).Opacity = 1;
            await SettingsService.Init();
            await LiveStreamService.Init();

            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.Show();
            Close();
        }
    }
}