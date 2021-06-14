using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplates.UI.Views.UserControl
{
    public class StreamView : Avalonia.Controls.UserControl
    {
        public StreamView() { InitializeComponent(); }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
    }
}