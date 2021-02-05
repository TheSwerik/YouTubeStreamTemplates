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
            Grid.SetRowSpan(templateEditor, 3);
            Grid.SetColumnSpan(templateEditor, 2);
            grid.Children.Add(templateEditor);

            var streamEditor = new ViewStream();
            Grid.SetColumn(streamEditor, 2);
            Grid.SetRowSpan(streamEditor, 3);
            Grid.SetColumnSpan(streamEditor, 2);
            grid.Children.Add(streamEditor);

            var updateControl = new UpdateControl(templateEditor, streamEditor);
            Grid.SetColumn(updateControl, 1);
            Grid.SetColumnSpan(updateControl, 2);
            Grid.SetRow(updateControl, 1);
            grid.Children.Add(updateControl);
        }
    }
}