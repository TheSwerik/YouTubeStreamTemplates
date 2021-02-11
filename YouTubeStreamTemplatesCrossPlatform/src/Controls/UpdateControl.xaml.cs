using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NLog;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class UpdateControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CheckBox _autoUpdateCheckBox;
        private readonly EditTemplate _editTemplate = null!;
        private readonly ViewStream _viewStream = null!;

        // ReSharper disable once MemberCanBePrivate.Global
        public UpdateControl()
        {
            AvaloniaXamlLoader.Load(this);
            _autoUpdateCheckBox = this.Find<CheckBox>("AutoUpdateCheckBox");
        }

        public UpdateControl(EditTemplate editTemplate, ViewStream viewStream) : this()
        {
            _editTemplate = editTemplate;
            _viewStream = viewStream;
            _autoUpdateCheckBox.IsChecked = bool.Parse(SettingsService.Instance.Settings[Settings.AutoUpdate]);
        }

        private bool CheckBoxIsChecked() { return _autoUpdateCheckBox.IsChecked ?? false; }

        #region EventListeners

        private void AutoUpdateCheckBox_OnChecked(object? sender, RoutedEventArgs e)
        {
            SettingsService.Instance.UpdateSetting(Settings.AutoUpdate, CheckBoxIsChecked() + "");
        }

        private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
        {
            Logger.Debug("Clicked on Update.");
            await LiveStreamService.Instance.CheckedUpdate(TemplateService.Instance.GetCurrentTemplate,
                                                           _editTemplate.ChangedTemplate);
        }

        private void OnlySavedTemplatesCheckBox_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var checkBox = (CheckBox) sender;
            SettingsService.Instance.UpdateSetting(Settings.OnlyUpdateSavedTemplates,
                                                   (checkBox.IsChecked ?? false) + "");
        }

        #endregion
    }
}