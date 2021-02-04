using System;
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
        private readonly TextBlock _descriptionTextBlock;
        private readonly TagEditor _tagEditor;
        private readonly TextBlock _titleTextBlock;
        private LiveStream? _currentLiveStream;

        #region Methods

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

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        private void FillValues(LiveStream liveStream)
        {
            // Logger.Debug($"Fill Values with:\n{liveStream}");
            _titleTextBlock.Text = liveStream.Title;
            _descriptionTextBlock.Text = liveStream.Description;
            // CategoryComboBox.SelectedItem =
            // Service.LiveStreamService!.Category.First(kp => kp.Key.Equals(liveStream.Category));

            _tagEditor.Tags = liveStream.Tags.ToHashSet();
            _tagEditor.RefreshTags();
        }

        #endregion

        #region Init

        public ViewStream()
        {
            _tagEditor = new TagEditor();

            InitializeComponent();
            _categoryTextBlock = this.Find<TextBlock>("CategoryTextBlock");
            _titleTextBlock = this.Find<TextBlock>("TitleTextBlock");
            _descriptionTextBlock = this.Find<TextBlock>("DescriptionTextBlock");

            InvokeOnRender(async () => await Init());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Grid.SetRow(_tagEditor, 5);
            Grid.SetColumn(_tagEditor, 2);
            this.Find<Grid>("ContentGrid").Children.Add(_tagEditor);
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

            // CategoryComboBox.Items = Service.LiveStreamService.Category;
            // CategoryComboBox.IsReadOnly = true;
            // TagEditor.IsReadOnly = true;
            await Task.Run(() => CheckForStream());
        }

        #endregion
    }
}