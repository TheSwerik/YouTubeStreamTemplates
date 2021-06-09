using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using YouTubeStreamTemplates.UI.ViewModels;

namespace YouTubeStreamTemplates.UI.Views
{
    public class LoginWindow : ReactiveWindow<LoginViewModel>
    {
        public LoginWindow()
        {
            InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
        }

        private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
    }
}