﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
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
        private string _thumbnailPath;
        public Template SelectedTemplate => _templateComboBox.SelectedItem!;

        public Template ChangedTemplate()
        {
            return SelectedTemplate with
                   {
                       Title = _titleTextBox.Text,
                       Description = _descriptionTextBox.Text,
                       Category = _categoryComboBox.SelectedItem.Key,
                       Tags = _tagEditor.Tags.ToList(),
                       ThumbnailPath = _thumbnailPath
                   };
        }

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
            _thumbnailPath = "";
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

        #region Methods

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        private void Refresh()
        {
            _templateComboBox.Items = Service.TemplateService!.Templates;
            _templateComboBox.SelectedItem = Service.TemplateService!.GetCurrentTemplate();
            if (_templateComboBox.SelectedItem == null) _templateComboBox.SelectedIndex = 0;
        }

        private async void FillValues(Template template)
        {
            // Logger.Debug($"Fill Values with:\n{template}");
            _titleTextBox.Text = template.Title;
            _descriptionTextBox.Text = template.Description;
            _categoryComboBox.SelectedItem = Service.LiveStreamService!.Category.FirstMatching(template.Category);
            _tagEditor.RefreshTags(template.Tags);
            await SettingsService.Instance.UpdateSetting(Settings.CurrentTemplate, template.Id);
            if (string.IsNullOrWhiteSpace(template.ThumbnailPath))
            {
                _thumbnail.Source = new Bitmap("res/Overlay.png");
            }
            else
            {
                _thumbnail.Source = await ImageHelper.PathToImageAsync(template.ThumbnailPath, true, template.Id);
                _thumbnailPath = template.ThumbnailPath;
            }
        }

        private bool HasDifference(Template? template = null)
        {
            template ??= SelectedTemplate;
            return !(template.Title.Equals(_titleTextBox.Text) &&
                     template.Description.Equals(_descriptionTextBox.Text) &&
                     template.Category.Equals(_categoryComboBox.SelectedItem.Key) &&
                     template.ThumbnailPath.Equals(_thumbnailPath) &&
                     template.Tags.Count == _tagEditor.Tags.Count &&
                     template.Tags.All(t => _tagEditor.Tags.Contains(t)));
        }

        #region EventListeners

        #region Change Template

        private bool _ignoreDifferenceCheck;

        private void OnChanged(object? sender, EventArgs e) { _saveButton.IsEnabled = HasDifference(); }

        private async void TemplateComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
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

        #endregion

        #region Save

        public void OnHotKeyPressed(object? sender, KeyEventArgs keyEventArgs)
        {
            if (_saveButton.IsEnabled &&
                (keyEventArgs.KeyModifiers & KeyModifiers.Control) != 0 &&
                keyEventArgs.Key == Key.S)
                OnSaveButtonClicked(null, new RoutedEventArgs());
        }

        public async void OnSaveButtonClicked(object? sender, RoutedEventArgs routedEventArgs)
        {
            await Service.TemplateService!.SaveTemplate(ChangedTemplate());
            OnChanged(null, null);
        }

        #endregion

        #region Thumbnail

        private async void ThumbnailImage_OnClick(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left) FileContextButton_OnClick(null, new RoutedEventArgs());
        }

        private async void FileContextButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
                             {
                                 AllowMultiple = false,
                                 Title = "Select a Thumbnail",
                                 Filters = new List<FileDialogFilter>
                                           {
                                               new()
                                               {
                                                   Extensions = new List<string> {"png", "jpg", "jpeg"},
                                                   Name = "Image"
                                               }
                                           }
                             };
            var strings = await fileDialog.ShowAsync((Window) Parent.Parent.Parent.Parent.Parent);
            if (strings == null || strings.Length == 0) return;
            _thumbnailPath = strings[0];
            _thumbnail.Source = await ImageHelper.PathToImageAsync(_thumbnailPath, true, SelectedTemplate.Id);
            OnChanged(null, null);
        }

        private async void URLContextButton_OnClick(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("URl");
            // TODO Paste Link and get Image
            // _thumbnailPath = URL;
            // _thumbnail.Source = await ImageHelper.PathToImageAsync(_thumbnailPath, true, SelectedTemplate.Id);
        }

        #endregion

        #endregion

        #endregion
    }
}