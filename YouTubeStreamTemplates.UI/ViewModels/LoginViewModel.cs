using System;
using System.Reactive;
using ReactiveUI;

namespace YouTubeStreamTemplates.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            LoginCommand = ReactiveCommand.CreateFromTask(async () =>
                                                          {
                                                              Console.WriteLine("LOGIN");
                                                              return true;
                                                          });
        }

        public ReactiveCommand<Unit, bool> LoginCommand { get; }
    }
}