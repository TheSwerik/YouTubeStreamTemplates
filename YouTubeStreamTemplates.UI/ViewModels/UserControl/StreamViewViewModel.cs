using System;
using System.Diagnostics;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplates.UI.ViewModels.UserControl
{
    public class StreamViewViewModel : ViewModelBase
    {
        public StreamViewViewModel() { Console.WriteLine("VIEW MODEL"); }

        public string Categories => "Test";

        public void OpenStream()
        {
            if (!LiveStreamService.IsInitialized || LiveStreamService.Instance.CurrentLiveStream == null) return;
            Process.Start(new ProcessStartInfo
                          {
                              UseShellExecute = true,
                              FileName = "https://www.youtube.com/watch?v=" +
                                         LiveStreamService.Instance.CurrentLiveStream.Id
                          })
                   ?.Dispose();
        }
    }
}