using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using YouTubeStreamTemplatesCrossPlatform.Controls;

namespace YouTubeStreamTemplatesCrossPlatform.Windows.Tabs
{
    public class Home : UserControl
    {
        public Home() { InitializeComponent(); }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var grid = this.Find<Grid>("Grid");

            var templateEditor = new EditTemplate();
            Grid.SetColumn(templateEditor, 0);
            grid.Children.Add(templateEditor);

            var streamEditor = new ViewStream();
            Grid.SetColumn(streamEditor, 1);
            grid.Children.Add(streamEditor);
        }
    }
}