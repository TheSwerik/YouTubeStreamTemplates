using System;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using NLog;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplatesCrossPlatform.Exceptions;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditStream : EditComponent, IStyleable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private LiveStream? _currentLiveStream;
        Type IStyleable.StyleKey => typeof(EditComponent);

        #region Methods

        protected override LiveStream? GetLiveStream() { return _currentLiveStream; }

        private async Task CheckForStream()
        {
            if (Service.LiveStreamService == null) throw new ServiceNotInitializedException(typeof(LiveStreamService));

            while (true)
            {
                try
                {
                    _currentLiveStream = await Service.LiveStreamService.GetCurrentStreamAsVideo();
                    Logger.Debug("Stream Detected:\tid: {0} \tTitle: {1}", _currentLiveStream.Id,
                                 _currentLiveStream.Title);
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