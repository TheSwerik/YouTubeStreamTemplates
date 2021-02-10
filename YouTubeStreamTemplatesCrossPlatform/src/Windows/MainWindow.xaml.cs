using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplatesCrossPlatform.Windows
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            Closing += Dispose;
        }

        private void Dispose(object? sender, CancelEventArgs e) { Service.LiveStreamService!.Dispose(); }
    }
}