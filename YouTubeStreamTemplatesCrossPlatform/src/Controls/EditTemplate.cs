using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using NLog;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class EditTemplate : EditComponent, IStyleable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Grid _contentGrid = null!;
        private GenericComboBox<Template> _templateComboBox = null!;
        Type IStyleable.StyleKey => typeof(EditComponent);

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

        private void Ignored() { AvaloniaXamlLoader.Load(this); }

        protected override async Task Init()
        {
            await base.Init();

            while (Service.TemplateService == null)
            {
                Logger.Debug("Waiting for TemplateService to initialize...");
                await Task.Delay(100);

                //TODO REMOVE THIS:
                Service.TemplateService = new TemplateService();
                await Service.TemplateService.LoadAllTemplates(SettingsService.Instance.Settings[Settings.SavePath]);
                //-------- Until here -------------------
            }

            InitializeComponent();
            Refresh();
        }

        private void InitializeComponent()
        {
            _contentGrid = this.Find<Grid>("ContentGrid");
            _contentGrid.RowDefinitions = RowDefinitions.Parse("*,*,*,*,*,10*,*,10*,*,*,*");
            foreach (var control in _contentGrid.Children.Cast<Control?>())
                Grid.SetRow(control, Grid.GetRow(control) + 2);

            var templateBlock = new TextBlock {Text = "Template:"};
            Grid.SetRow(templateBlock, 1);
            Grid.SetColumn(templateBlock, 1);
            _contentGrid.Children.Add(templateBlock);

            _templateComboBox = new GenericComboBox<Template>
                                {
                                    ItemTemplate = new FuncDataTemplate(typeof(Template), (o, s) =>
                                                                        {
                                                                            var template = (Template) o;
                                                                            return new TextBlock
                                                                                {
                                                                                    Text = template.Name
                                                                                };
                                                                        })
                                };
            Grid.SetRow(_templateComboBox, 1);
            Grid.SetColumn(_templateComboBox, 2);
            _templateComboBox.SelectionChanged += TemplateComboBox_OnSelectionChanged;
            _templateComboBox.SelectedIndex = 0;
            _contentGrid.Children.Add(_templateComboBox);

            //TODO Remove this Button:
            var testButton = new Button {Content = "Update Stream"};
            testButton.Click += async (sender, args) =>
                                    await Service.LiveStreamService!.UpdateStream(_templateComboBox.SelectedItem!);
            Grid.SetRow(testButton, 9);
            Grid.SetColumn(testButton, 1);
            _contentGrid.Children.Add(testButton);
        }

        #endregion
    }
}