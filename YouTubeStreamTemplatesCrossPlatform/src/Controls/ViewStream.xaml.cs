using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using NLog;
using YouTubeStreamTemplates.Helpers;
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
            while (!LiveStreamService.IsInitialized)
            {
                Logger.Debug("Waiting for LiveStreamService to initialize...");
                await Task.Delay(100);
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
            if (LiveStreamService.Instance == null) throw new ServiceNotInitializedException(typeof(LiveStreamService));
            await foreach (var stream in LiveStreamService.Instance.CheckForStream(delay))
                if (stream == null) InvokeOnRender(ClearValues);
                else InvokeOnRender(() => FillValues(stream));
        }

        private async void FillValues(LiveStream liveStream)
        {
            // Logger.Debug($"Fill Values with:\n{liveStream}");
            _titleTextBlock.Text = liveStream.Title;
            _descriptionTextBlock.Text = liveStream.Description;
            _categoryTextBlock.Text = LiveStreamService.Instance
                                                       .Category
                                                       .First(kp => kp.Key.Equals(liveStream.Category))
                                                       .Value;
            _tagEditor.Tags = liveStream.Tags.ToHashSet();
            _tagEditor.RefreshTags();
            _contentGrid.IsVisible = !(_noStreamGrid.IsVisible = false);
            _thumbnail.Source = new Bitmap(await ImageHelper.GetLiveStreamImagePathAsync(liveStream.Id));
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