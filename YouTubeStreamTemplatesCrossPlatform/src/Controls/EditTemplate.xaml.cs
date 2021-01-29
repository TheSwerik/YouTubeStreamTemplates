using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditTemplate : EditComponent, IStyleable
    {
        private readonly GenericComboBox<Template> _templateComboBox;
        Type IStyleable.StyleKey => typeof(TextBox);

        #region EventListener

        private void TemplateComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //TODO Check if there are unsaved changes
            FillValues(_templateComboBox.SelectedItem!);
        }

        #endregion

        #region Methods

        protected override void Refresh()
        {
            _templateComboBox.Items = Service.TemplateService!.Templates;
            _templateComboBox.SelectedIndex = 0;
            if (_templateComboBox.SelectedItem != null) FillValues(_templateComboBox.SelectedItem);
        }

        protected override LiveStream? GetLiveStream() { return _templateComboBox.SelectedItem; }

        #endregion

        #region Init

        public EditTemplate() { _templateComboBox = this.Find<GenericComboBox<Template>>("TemplateComboBox"); }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var grid = this.Find<Grid>("ContentGrid");
            grid.RowDefinitions = RowDefinitions.Parse("*,*,*,*,*,10*,*,10*,*,*,*");

            foreach (var control in grid.Children.Cast<Control?>()) Grid.SetRow(control, Grid.GetRow(control) + 2);
            grid.Children.Add(_templateComboBox);
        }

        protected override async Task Init()
        {
            await base.Init();
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
            _templateComboBox.SelectedIndex = 0;
            Refresh();
        }

        #endregion
    }
}