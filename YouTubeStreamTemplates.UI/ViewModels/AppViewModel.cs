using Avalonia.Themes.Fluent;

namespace YouTubeStreamTemplates.UI.ViewModels
{
    public class AppViewModel : ViewModelBase

    {
        public AppViewModel() { FLuentMode = FluentThemeMode.Dark; }

        public FluentThemeMode FLuentMode { get; }
    }
}