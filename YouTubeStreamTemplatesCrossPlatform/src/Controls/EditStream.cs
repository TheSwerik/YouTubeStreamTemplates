using System;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using YouTubeStreamTemplates.Exceptions;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditStream : EditComponent, IStyleable
    {
        Type IStyleable.StyleKey => typeof(EditComponent);

        #region Methods

        private async Task CheckForStream()
        {
            //TODO make Custom Exception
            if (Service.LiveStreamService == null) throw new Exception("LiveStreamService is not Initialized.");

            while (true)
            {
                try
                {
                    var result = await Service.LiveStreamService.GetCurrentStream();
                    Logger.Debug("STREAM DETECTED: {0}", result.Title);
                    InvokeOnRender(() => FillValues(result));
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