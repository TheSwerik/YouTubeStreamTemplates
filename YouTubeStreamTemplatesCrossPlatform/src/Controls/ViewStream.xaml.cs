using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using NLog;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplatesCrossPlatform.Exceptions;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class ViewStream : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly TextBlock _categoryTextBlock;
        private readonly Grid _contentGrid;
        private readonly TextBlock _descriptionTextBlock;
        private readonly Grid _noStreamGrid;
        private readonly TagEditor _tagEditor;
        private readonly Image _thumbnail;

        private readonly TextBlock _titleTextBlock;
        // public LiveStream? CurrentLiveStream { get; private set; }

        #region Init

        public ViewStream()
        {
            _tagEditor = new TagEditor(true);

            InitializeComponent();
            _categoryTextBlock = this.Find<TextBlock>("CategoryTextBlock");
            _titleTextBlock = this.Find<TextBlock>("TitleTextBlock");
            _thumbnail = this.Find<Image>("ThumbnailImage");
            _descriptionTextBlock = this.Find<TextBlock>("DescriptionTextBlock");
            _contentGrid = this.Find<Grid>("ContentGrid");
            _noStreamGrid = this.Find<Grid>("NoStreamGrid");
            _contentGrid.Children.Add(_tagEditor);

            InvokeOnRender(async () => await Init());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Grid.SetRow(_tagEditor, 10);
            Grid.SetColumn(_tagEditor, 1);
        }

        private async Task Init()
        {
            while (Service.LiveStreamService == null)
            {
                Logger.Debug("Waiting for LiveStreamService to initialize...");
                await Task.Delay(100);

                //TODO REMOVE THIS:
                try
                {
                    Service.LiveStreamService ??= await LiveStreamService.Init();
                }
                catch (AlreadyInitializingException)
                {
                }

                //-------- Until here -------------------
            }

            await Task.Run(() => CheckForStream());
        }

        #endregion

        #region Methods

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        private async Task CheckForStream(int delay = 1000)
        {
            if (Service.LiveStreamService == null) throw new ServiceNotInitializedException(typeof(LiveStreamService));
            await foreach (var stream in Service.LiveStreamService.CheckForStream(delay))
                if (stream == null) InvokeOnRender(ClearValues);
                else InvokeOnRender(() => FillValues(stream));
        }

        private async void FillValues(LiveStream liveStream)
        {
            // Logger.Debug($"Fill Values with:\n{liveStream}");
            _titleTextBlock.Text = liveStream.Title;
            _descriptionTextBlock.Text = liveStream.Description;
            _categoryTextBlock.Text = Service.LiveStreamService!.Category
                                                                .First(kp => kp.Key.Equals(liveStream.Category))
                                                                .Value;
            _tagEditor.Tags = liveStream.Tags.ToHashSet();
            _tagEditor.RefreshTags();
            _contentGrid.IsVisible = !(_noStreamGrid.IsVisible = false);
            _thumbnail.Source = await ImageHelper.PathToImageAsync(liveStream.ThumbnailPath, false, liveStream.Id);
        }

        private void ClearValues()
        {
            _contentGrid.IsVisible = !(_noStreamGrid.IsVisible = true);
            _titleTextBlock.Text = "";
            _descriptionTextBlock.Text = "";
            _categoryTextBlock.Text = "";
            _tagEditor.Tags = new HashSet<string>();
            _tagEditor.RefreshTags();
        }

        #endregion
    }
}