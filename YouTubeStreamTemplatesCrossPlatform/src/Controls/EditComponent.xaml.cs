using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using NLog;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public abstract class EditComponent : UserControl
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly GenericComboBox<Category> CategoryComboBox;
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

        protected abstract void Refresh();
        protected abstract LiveStream? GetLiveStream();

        protected void FillValues(LiveStream template)
        {
            Logger.Debug($"Fill Values with:\n{template}");
            TitleTextBox.Text = template.Title;
            DescriptionTextBox.Text = template.Description;
            CategoryComboBox.SelectedItem = template.Category;
            TagEditor.RefreshTags(template.Tags);
        }

        private bool HasDifference()
        {
            var live = GetLiveStream();
            return live == null ||
                   live.Category == CategoryComboBox.SelectedItem &&
                   live.Title.Equals(TitleTextBox.Text) &&
                   live.Description.Equals(DescriptionTextBox.Text) &&
                   live.Tags.Count == TagEditor.Tags.Count &&
                   live.Tags.All(t => TagEditor.Tags.Contains(t));
        }

        #endregion

        #region Init

        public EditComponent()
        {
            TagEditor = new TagEditor(); //TODO

            InitializeComponent();
            CategoryComboBox = this.Find<GenericComboBox<Category>>("CategoryComboBox");
            TitleTextBox = this.Find<TextBox>("TitleTextBox");
            DescriptionTextBox = this.Find<TextBox>("DescriptionTextBox");

            InvokeOnRender(async () => await Init());
        }

        protected virtual async Task Init()
        {
            CategoryComboBox.SelectionChanged += OnChanged;
            TitleTextBox.TextInput += OnChanged;
            DescriptionTextBox.TextInput += OnChanged;

            CategoryComboBox.Items = Enum.GetValues(typeof(Category));
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