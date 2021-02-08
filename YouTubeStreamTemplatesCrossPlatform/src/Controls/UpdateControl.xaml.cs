using System.Linq;
using System.Threading.Tasks;
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

        private async Task CheckIfShouldUpdate()
        {
            while (CheckBoxIsChecked())
            {
                Logger.Debug("Checking If Should Update..");
                LiveStream? stream;
                while ((stream = _viewStream.CurrentLiveStream) == null) await Task.Delay(2000);
                if (!CheckBoxIsChecked()) return;
                var template = bool.Parse(SettingsService.Instance.Settings[Settings.OnlyUpdateSavedTemplates])
                                   ? _editTemplate.SelectedTemplate
                                   : _editTemplate.ChangedTemplate();
                if (HasDifference(stream, template)) await Service.LiveStreamService!.UpdateStream(template);
                await Task.Delay(20000);
            }
        }

        private bool CheckBoxIsChecked() { return _autoUpdateCheckBox.IsChecked ?? false; }

        private static bool HasDifference(LiveStream stream, Template template)
        {
            return !(template.Title.Equals(stream.Title) &&
                     template.Description.Equals(stream.Description) &&
                     template.Category.Equals(stream.Category) &&
                     template.Tags.Count == stream.Tags.Count &&
                     template.Tags.All(t => stream.Tags.Contains(t)));
        }

        #region EventListeners

        private void AutoUpdateCheckBox_OnChecked(object? sender, RoutedEventArgs e)
        {
            SettingsService.Instance.Settings[Settings.AutoUpdate] = (_autoUpdateCheckBox.IsChecked ?? false) + "";
            SettingsService.Instance.Save();
            if (CheckBoxIsChecked()) Task.Run(CheckIfShouldUpdate);
        }

        private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
        {
            Logger.Debug("Clicked on Update.");
            var stream = _viewStream.CurrentLiveStream;
            if (stream == null) return;
            Logger.Debug("Stream Found.");
            var template = bool.Parse(SettingsService.Instance.Settings[Settings.OnlyUpdateSavedTemplates])
                               ? _editTemplate.SelectedTemplate
                               : _editTemplate.ChangedTemplate();
            if (HasDifference(stream, template)) await Service.LiveStreamService!.UpdateStream(template);
        }

        private void OnlySavedTemplatesCheckBox_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var checkBox = (CheckBox) sender;
            SettingsService.Instance.Settings[Settings.OnlyUpdateSavedTemplates] = "" + (checkBox.IsChecked ?? false);
            SettingsService.Instance.Save();
        }

        #endregion
    }
}