using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using NLog;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditTemplate : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GenericComboBox<KeyValuePair<string, string>> _categoryComboBox;
        private readonly TextBox _descriptionTextBox;
        private readonly TagEditor _tagEditor;
        private readonly TextBox _titleTextBox;
        private bool _edited;
        private GenericComboBox<Template> _templateComboBox;

        #region EventListener

        private void TemplateComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_edited)
            {
                Logger.Info("You have unsaved changes!");
                return;
            }

            FillValues(_templateComboBox.SelectedItem!);
        }

        private void OnChanged(object? sender, RoutedEventArgs args)
        {
            _edited = HasDifference();
            if (_edited) Logger.Info("SOMETHING CHANGED");
        }

        #endregion

        #region Methods

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        private void Refresh()
        {
            var template =
                Service.TemplateService!.Templates.FirstOrDefault(
                    t => t.Id.Equals(SettingsService.Instance.Settings[Settings.CurrentTemplate]));
            _templateComboBox.Items = Service.TemplateService!.Templates;
            _templateComboBox.SelectedItem = Service.TemplateService
                                                    .Templates
                                                    .FirstOrDefault(t => t.Id.Equals(SettingsService.Instance.Settings
                                                                        [Settings.CurrentTemplate]));
            if (_templateComboBox.SelectedItem == null) _templateComboBox.SelectedIndex = 0;
        }

        private void FillValues(Template liveStream)
        {
            // Logger.Debug($"Fill Values with:\n{liveStream}");
            _titleTextBox.Text = liveStream.Title;
            _descriptionTextBox.Text = liveStream.Description;
            _categoryComboBox.SelectedItem =
                Service.LiveStreamService!.Category.First(kp => kp.Key.Equals(liveStream.Category));

            _tagEditor.Tags = liveStream.Tags.ToHashSet();
            _tagEditor.RefreshTags();
            SettingsService.Instance.Settings[Settings.CurrentTemplate] = liveStream.Id;
            Task.Run(SettingsService.Instance.Save);
        }

        private bool HasDifference()
        {
            var live = _templateComboBox?.SelectedItem;
            return live == null ||
                   live.Title.Equals(_titleTextBox.Text) &&
                   live.Description.Equals(_descriptionTextBox.Text) &&
                   live.Category.Equals(_categoryComboBox.SelectedItem.Value) &&
                   live.Tags.Count == _tagEditor.Tags.Count &&
                   live.Tags.All(t => _tagEditor.Tags.Contains(t));
        }

        #endregion

        #region Init

        public EditTemplate()
        {
            _tagEditor = new TagEditor();

            InitializeComponent();
            _categoryComboBox = this.Find<GenericComboBox<KeyValuePair<string, string>>>("CategoryComboBox");
            _titleTextBox = this.Find<TextBox>("TitleTextBox");
            _descriptionTextBox = this.Find<TextBox>("DescriptionTextBox");

            InvokeOnRender(async () => await Init());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var contentGrid = this.Find<Grid>("ContentGrid");
            _templateComboBox = this.Find<GenericComboBox<Template>>("TemplateComboBox");

            Grid.SetRow(_tagEditor, 7);
            Grid.SetColumn(_tagEditor, 2);
            contentGrid.Children.Add(_tagEditor);

            //TODO Remove this Button:
            var testButton = new Button {Content = "Update Stream"};
            testButton.Click += async (sender, args) =>
                                    await Service.LiveStreamService!.UpdateStream(_templateComboBox.SelectedItem!);
            Grid.SetRow(testButton, 9);
            Grid.SetColumn(testButton, 1);
            contentGrid.Children.Add(testButton);
        }

        private async Task Init()
        {
            _categoryComboBox.SelectionChanged += OnChanged;
            _titleTextBox.TextInput += OnChanged;
            _descriptionTextBox.TextInput += OnChanged;

            while (Service.LiveStreamService == null)
            {
                Logger.Debug("Waiting for LiveStreamService to initialize...");
                await Task.Delay(100);
            }

            _categoryComboBox.Items = Service.LiveStreamService.Category;

            while (Service.TemplateService == null)
            {
                Logger.Debug("Waiting for TemplateService to initialize...");
                await Task.Delay(100);

                //TODO REMOVE THIS:
                Service.TemplateService = new TemplateService();
                await Service.TemplateService.LoadAllTemplates(SettingsService.Instance.Settings[Settings.SavePath]);
                //-------- Until here -------------------
            }

            Refresh();
        }

        #endregion
    }
}