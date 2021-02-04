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

        private async Task CheckForStream(int delay = 1000)
        {
            if (Service.LiveStreamService == null) throw new ServiceNotInitializedException(typeof(LiveStreamService));
            var longDelay = delay * 20;
            while (true)
            {
                await Task.Delay(_currentLiveStream == null ? delay : longDelay);
                try
                {
                    var stream = await Service.LiveStreamService.GetCurrentStreamAsVideo();
                    // TODO check for unsaved Changes
                    if (_currentLiveStream == null)
                        Logger.Debug("Stream Detected:\tid: {0} \tTitle: {1}", stream.Id, stream.Title);
                    _currentLiveStream = stream;
                    InvokeOnRender(() => FillValues(_currentLiveStream));
                }
                catch (NoCurrentStreamException)
                {
                    Logger.Debug("Not currently streaming...");
                    _currentLiveStream = null;
                }
            }
        }

        #endregion

        #region Init

        private void Ignored() { AvaloniaXamlLoader.Load(this); }

        protected override async Task Init()
        {
            await base.Init();
            await Task.Run(() => CheckForStream());
        }

        #endregion
    }
}