using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using NLog;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditStream : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GenericComboBox<Category> _categoryComboBox;
        private readonly TextBox _descriptionTextBox;
        private readonly TagEditor _tagEditor;
        private readonly TextBox _titleTextBox;

        #region EventListener

        private void OnChanged(object? sender, RoutedEventArgs args) { Logger.Info("SOMETHING CHANGED"); }

        #endregion

        #region Methods

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        private async Task FillValues()
        {
            var liveStream = await Service.LiveStreamService!.GetCurrentStream();
            Logger.Debug(liveStream);
            _descriptionTextBox.Text = liveStream.Description;
            _categoryComboBox.SelectedItem = liveStream.Category;
            _tagEditor.RefreshTags(liveStream.Tags);
        }

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
                    await FillValues();
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

        public EditStream()
        {
            _tagEditor = new TagEditor(); //TODO

            InitializeComponent();
            _categoryComboBox = this.Find<GenericComboBox<Category>>("CategoryComboBox");
            _titleTextBox = this.Find<TextBox>("TitleTextBox");
            _descriptionTextBox = this.Find<TextBox>("DescriptionTextBox");

            InvokeOnRender(async () => await Init());
        }

        private async Task Init()
        {
            //TODO REMOVE THIS:
            Service.LiveStreamService = await LiveStreamService.Init();
            //-------- Until here -------------------

            while (Service.LiveStreamService == null)
            {
                Logger.Debug("Waiting for LiveStreamService to initialize...");
                await Task.Delay(25);
            }

            _categoryComboBox.SelectionChanged += OnChanged;
            _titleTextBox.TextInput += OnChanged;
            _descriptionTextBox.TextInput += OnChanged;

            _categoryComboBox.Items = Enum.GetValues(typeof(Category));

            await Task.Run(CheckForStream);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Grid.SetRow(_tagEditor, 5);
            Grid.SetColumn(_tagEditor, 2);
            this.Find<Grid>("ContentGrid").Children.Add(_tagEditor);
        }

        #endregion
    }
}