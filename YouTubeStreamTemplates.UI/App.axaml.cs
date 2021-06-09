using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using NLog;
using YouTubeStreamTemplates.UI.ViewModels;
using YouTubeStreamTemplates.UI.Views;

namespace YouTubeStreamTemplates.UI
{
    public class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Initialize()
        {
            Console.WriteLine(0);
            AvaloniaXamlLoader.Load(this);
            LoadTheme();
        }

        private void LoadTheme()
        {
            Styles.Add(new FluentTheme(new Uri("avares://YouTubeStreamTemplates.UI/App.axaml", UriKind.Absolute))
                       {
                           Mode = FluentThemeMode.Dark
                           // Mode = FluentThemeMode.Light
                       });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new LoginWindow {DataContext = new LoginViewModel()};
            // desktop.MainWindow = new MainWindow {DataContext = new MainWindowViewModel()};

            base.OnFrameworkInitializationCompleted();
        }

        public static void ChangeMainWindow(Window mainWindow)
        {
            if (Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            desktop.MainWindow = mainWindow;
            desktop.MainWindow.Show();
        }
    }
}