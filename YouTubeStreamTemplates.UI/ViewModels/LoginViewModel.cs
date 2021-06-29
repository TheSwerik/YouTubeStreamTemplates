using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;

namespace YouTubeStreamTemplates.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel() { LoginCommand = ReactiveCommand.CreateFromTask(InitLiveStreamService); }

        public ReactiveCommand<Unit, bool> LoginCommand { get; }

        private static async Task<bool> InitLiveStreamService()
        {
            await SettingsService.Init();
            await LiveStreamService.Init();
            return true;
        }
    }
}