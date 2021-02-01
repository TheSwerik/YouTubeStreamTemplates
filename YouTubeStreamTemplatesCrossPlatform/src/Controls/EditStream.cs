using System;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditStream : EditComponent, IStyleable
    {
        private LiveStream? _currentLiveStream;
        Type IStyleable.StyleKey => typeof(EditComponent);

        #region Methods

        protected override LiveStream? GetLiveStream() { return _currentLiveStream; }

        private async Task CheckForStream()
        {
            //TODO make Custom Exception
            if (Service.LiveStreamService == null) throw new Exception("LiveStreamService is not Initialized.");

            while (true)
            {
                try
                {
                    _currentLiveStream = await Service.LiveStreamService.GetCurrentStreamAsVideo();
                    Logger.Debug("STREAM DETECTED: {0}", _currentLiveStream.Title);
                    InvokeOnRender(() => FillValues(_currentLiveStream));
                    return;
                }
                catch (NoCurrentStreamException)
                {
                    Logger.Debug("Not currently streaming...");
                }

                await Task.Delay(1000);
            }
        }

        #endregion

        #region Init

        private void Ignored() { AvaloniaXamlLoader.Load(this); }

        protected override async Task Init()
        {
            await base.Init();
            await Task.Run(CheckForStream);
        }

        #endregion
    }
}