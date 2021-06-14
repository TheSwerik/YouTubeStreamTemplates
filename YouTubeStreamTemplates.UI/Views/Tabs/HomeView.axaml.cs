using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplates.UI.Views.Tabs
{
    public class HomeView : Avalonia.Controls.UserControl
    {
        public HomeView() { InitializeComponent(); }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
    }
}