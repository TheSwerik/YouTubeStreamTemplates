using System;
using System.Windows.Input;
using ReactiveUI;

namespace YouTubeStreamTemplates.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            LoginCommand = ReactiveCommand.CreateFromTask(async () => { Console.WriteLine("LOGIN"); });
        }

        public ICommand LoginCommand { get; }
    }
}