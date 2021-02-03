using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EditComponent : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly GenericComboBox<KeyValuePair<string, string>> CategoryComboBox;
        protected readonly TextBox DescriptionTextBox;
        protected readonly TagEditor TagEditor;
        protected readonly TextBox TitleTextBox;
        protected bool Edited { get; private set; }

        #region EventListener

        protected void OnChanged(object? sender, RoutedEventArgs args)
        {
            Edited = HasDifference();
            if (Edited) Logger.Info("SOMETHING CHANGED");
        }

        #endregion

        #region Methods

        protected static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        protected virtual void Refresh() { throw new NotImplementedException(); }
        protected virtual LiveStream? GetLiveStream() { throw new NotImplementedException(); }

        protected void FillValues(LiveStream liveStream)
        {
            // Logger.Debug($"Fill Values with:\n{liveStream}");
            TitleTextBox.Text = liveStream.Title;
            DescriptionTextBox.Text = liveStream.Description;
            CategoryComboBox.SelectedItem =
                Service.LiveStreamService!.Category.First(kp => kp.Key.Equals(liveStream.Category));
            TagEditor.RefreshTags(liveStream.Tags);
        }

        private bool HasDifference()
        {
            var live = GetLiveStream();
            return live == null ||
                   live.Title.Equals(TitleTextBox.Text) &&
                   live.Description.Equals(DescriptionTextBox.Text) &&
                   live.Category.Equals(CategoryComboBox.SelectedItem.Value) &&
                   live.Tags.Count == TagEditor.Tags.Count &&
                   live.Tags.All(t => TagEditor.Tags.Contains(t));
        }

        #endregion

        #region Init

        public EditComponent()
        {
            TagEditor = new TagEditor();

            InitializeComponent();
            CategoryComboBox = this.Find<GenericComboBox<KeyValuePair<string, string>>>("CategoryComboBox");
            TitleTextBox = this.Find<TextBox>("TitleTextBox");
            DescriptionTextBox = this.Find<TextBox>("DescriptionTextBox");

            InvokeOnRender(async () => await Init());
        }

        protected virtual async Task Init()
        {
            CategoryComboBox.SelectionChanged += OnChanged;
            TitleTextBox.TextInput += OnChanged;
            DescriptionTextBox.TextInput += OnChanged;

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

            CategoryComboBox.Items = Service.LiveStreamService.Category;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Grid.SetRow(TagEditor, 5);
            Grid.SetColumn(TagEditor, 2);
            this.Find<Grid>("ContentGrid").Children.Add(TagEditor);
        }

        #endregion
    }
}