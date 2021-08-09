using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NLog;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Settings;

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
            _autoUpdateCheckBox.IsChecked = bool.Parse(SettingsService.Settings[Setting.AutoUpdate]);
        }

        private static SettingsService SettingsService => SettingsService.Instance;

        private bool CheckBoxIsChecked() { return _autoUpdateCheckBox.IsChecked ?? false; }

        #region EventListeners

        private async void AutoUpdateCheckBox_OnChecked(object? sender, RoutedEventArgs e)
        {
            await SettingsService.UpdateSetting(Setting.AutoUpdate, CheckBoxIsChecked() + "");
        }

        private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
        {
            await LiveStreamService.Instance.CheckedUpdate();
        }

        private async void OnlySavedTemplatesCheckBox_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var checkBox = (CheckBox)sender;
            await SettingsService.UpdateSetting(Setting.OnlyUpdateSavedTemplates, (checkBox.IsChecked ?? false) + "");
        }

        #endregion
    }
}