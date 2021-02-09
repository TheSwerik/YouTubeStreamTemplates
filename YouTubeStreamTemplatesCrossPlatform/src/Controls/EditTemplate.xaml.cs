using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
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

        private readonly Button _saveButton;
        private readonly TagEditor _tagEditor;
        private readonly GenericComboBox<Template> _templateComboBox;
        private readonly Image _thumbnail;
        private readonly TextBox _titleTextBox;
        public Template SelectedTemplate => _templateComboBox.SelectedItem!;

        public Template ChangedTemplate()
        {
            return SelectedTemplate with
                   {
                       Title = _titleTextBox.Text,
                       Description = _descriptionTextBox.Text,
                       Category = _categoryComboBox.SelectedItem.Key,
                       Tags = _tagEditor.Tags.ToList()
                   };
        }

        #region EventListener

        private bool _ignoreDifferenceCheck;

        private void OnChanged(object? sender, EventArgs e) { _saveButton.IsEnabled = HasDifference(); }

        private void TemplateComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_ignoreDifferenceCheck)
            {
                _ignoreDifferenceCheck = false;
                return;
            }

            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null && HasDifference((Template?) e.RemovedItems[0]))
            {
                _ignoreDifferenceCheck = true;
                _templateComboBox.SelectedItem = (Template?) e.RemovedItems[0];
                Logger.Info("You have unsaved changes!");
                //TODO MessageBox
                return;
            }

            FillValues(SelectedTemplate);
        }

        public async void OnSaveButtonClicked(object? sender, RoutedEventArgs routedEventArgs)
        {
            await Service.TemplateService!.SaveTemplate(ChangedTemplate());
            OnChanged(null, null);
        }

        public void OnHotKeyPressed(object? sender, KeyEventArgs keyEventArgs)
        {
            if (_saveButton.IsEnabled &&
                (keyEventArgs.KeyModifiers & KeyModifiers.Control) != 0 &&
                keyEventArgs.Key == Key.S)
                OnSaveButtonClicked(null, new RoutedEventArgs());
        }

        private async void ThumbnailImage_OnClick(object? sender, PointerReleasedEventArgs e)
        {
            // var fileDialog = new OpenFileDialog {AllowMultiple = false, Title = "Select a Thumbnail"};
            // // fileDialog.Filters=false;
            // var strings = await fileDialog.ShowAsync((Window) this.Parent.Parent.Parent.Parent.Parent);
            // Console.WriteLine(string.Join("\n", strings));
            // _thumbnail.Source =
            //     await ImageHelper.PathToImageAsync(SelectedTemplate.ThumbnailPath, true, SelectedTemplate.Id);
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

        private void FillValues(Template template)
        {
            // Logger.Debug($"Fill Values with:\n{template}");
            _titleTextBox.Text = template.Title;
            _descriptionTextBox.Text = template.Description;
            _categoryComboBox.SelectedItem =
                Service.LiveStreamService!.Category.First(kp => kp.Key.Equals(template.Category));

            _tagEditor.Tags = template.Tags.ToHashSet();
            _tagEditor.RefreshTags();
            SettingsService.Instance.Settings[Settings.CurrentTemplate] = template.Id;
            Task.Run(SettingsService.Instance.Save);
        }

        private bool HasDifference(Template? template = null)
        {
            template ??= SelectedTemplate;
            return !(template.Title.Equals(_titleTextBox.Text) &&
                     template.Description.Equals(_descriptionTextBox.Text) &&
                     template.Category.Equals(_categoryComboBox.SelectedItem.Key) &&
                     template.Tags.Count == _tagEditor.Tags.Count &&
                     template.Tags.All(t => _tagEditor.Tags.Contains(t)));
        }

        #endregion

        #region Init

        public EditTemplate()
        {
            _tagEditor = new TagEditor();

            InitializeComponent();
            _templateComboBox = this.Find<GenericComboBox<Template>>("TemplateComboBox");
            _saveButton = this.Find<Button>("SaveButton");
            _titleTextBox = this.Find<TextBox>("TitleTextBox");
            _thumbnail = this.Find<Image>("ThumbnailImage");
            _descriptionTextBox = this.Find<TextBox>("DescriptionTextBox");
            _categoryComboBox = this.Find<GenericComboBox<KeyValuePair<string, string>>>("CategoryComboBox");
            AddEventListeners();
            InvokeOnRender(async () => await Init());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var contentGrid = this.Find<Grid>("ContentGrid");

            Grid.SetRow(_tagEditor, 11);
            Grid.SetColumn(_tagEditor, 1);
            contentGrid.Children.Add(_tagEditor);
        }

        private void AddEventListeners()
        {
            _templateComboBox.SelectionChanged += OnChanged;
            _categoryComboBox.SelectionChanged += OnChanged;
            _titleTextBox.KeyUp += OnChanged;
            _titleTextBox.LostFocus += OnChanged;
            _descriptionTextBox.KeyUp += OnChanged;
            _descriptionTextBox.LostFocus += OnChanged;
            _tagEditor.OnChanged += OnChanged;
        }

        private async Task Init()
        {
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