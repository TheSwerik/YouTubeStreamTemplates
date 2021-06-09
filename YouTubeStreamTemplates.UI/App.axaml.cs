using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using YouTubeStreamTemplates.UI.ViewModels;
using YouTubeStreamTemplates.UI.Views;

namespace YouTubeStreamTemplates.UI
{
    public class App : Application
    {
        public override void Initialize() { AvaloniaXamlLoader.Load(this); }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow {DataContext = new MainWindowViewModel()};

            base.OnFrameworkInitializationCompleted();
        }
    }
}