using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using NLog;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditTemplate : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GenericComboBox<Category> _categoryComboBox;
        private readonly TextBox _descriptionTextBox;
        private readonly TagEditor _tagEditor;
        private readonly GenericComboBox<Template> _templateComboBox;
        private readonly TextBox _titleTextBox;

        #region EventListener

        private void OnChanged(object? sender, RoutedEventArgs args) { Logger.Info("SOMETHING CHANGED"); }

        #endregion

        #region Methods

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        private void FillValues()
        {
            _templateComboBox.Items = Service.TemplateService!.Templates;
            var template = Service.TemplateService!.Templates[0];
            Logger.Debug(template);
            _descriptionTextBox.Text = template.Description;
            _categoryComboBox.SelectedItem = template.Category;
            _tagEditor.RefreshTags(template.Tags);
        }

        #endregion

        #region Init

        public EditTemplate()
        {
            _tagEditor = new TagEditor(); //TODO

            InitializeComponent();
            _templateComboBox = this.Find<GenericComboBox<Template>>("TemplateComboBox");
            _categoryComboBox = this.Find<GenericComboBox<Category>>("CategoryComboBox");
            _titleTextBox = this.Find<TextBox>("TitleTextBox");
            _descriptionTextBox = this.Find<TextBox>("DescriptionTextBox");

            InvokeOnRender(async () => await Init());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Grid.SetRow(_tagEditor, 7);
            Grid.SetColumn(_tagEditor, 2);
            this.Find<Grid>("ContentGrid").Children.Add(_tagEditor);
        }

        private async Task Init()
        {
            //TODO REMOVE THIS:
            Service.TemplateService = new TemplateService();
            await Service.TemplateService.LoadAllTemplates(SettingsService.Instance.Settings[Settings.SavePath]);
            //-------- Until here -------------------

            while (Service.TemplateService == null)
            {
                Logger.Debug("Waiting for TemplateService to initialize...");
                await Task.Delay(25);
            }

            _templateComboBox.SelectionChanged += OnChanged;
            _categoryComboBox.SelectionChanged += OnChanged;
            _titleTextBox.TextInput += OnChanged;
            _descriptionTextBox.TextInput += OnChanged;

            _categoryComboBox.Items = Enum.GetValues(typeof(Category));
            _templateComboBox.SelectedIndex = 0;
            FillValues();
        }

        #endregion
    }
}