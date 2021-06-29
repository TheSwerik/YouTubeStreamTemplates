using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using YouTubeStreamTemplates.UI.ViewModels.UserControl;

namespace YouTubeStreamTemplates.UI.Views.UserControl
{
    public class StreamView : ReactiveUserControl<StreamViewViewModel>
    {
        public StreamView() { InitializeComponent(); }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }

        private void ThumbnailImage_OnClick(object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
        {
            ViewModel.OpenStream();
        }
    }
}