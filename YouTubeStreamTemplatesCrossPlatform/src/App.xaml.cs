using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using YouTubeStreamTemplatesCrossPlatform.Themes;
using YouTubeStreamTemplatesCrossPlatform.Windows;

namespace YouTubeStreamTemplatesCrossPlatform
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            LoadTheme();
        }

        private void LoadTheme()
        {
            Styles.Add(new Styling());
            Styles.Add(new DarkTheme());
            // Styles.Add(new LightTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new LoginWindow();
            base.OnFrameworkInitializationCompleted();
        }
    }
}