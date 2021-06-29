using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplates.UI.Views.UserControl
{
    public class TemplateView : Avalonia.Controls.UserControl
    {
        public TemplateView() { InitializeComponent(); }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
    }
}