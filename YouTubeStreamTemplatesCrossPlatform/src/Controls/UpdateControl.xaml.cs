using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NLog;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class UpdateControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CheckBox _checkBox;
        private readonly EditTemplate _editTemplate = null!;
        private readonly ViewStream _viewStream = null!;

        public UpdateControl()
        {
            AvaloniaXamlLoader.Load(this);
            _checkBox = this.Find<CheckBox>("CheckBox");
        }

        public UpdateControl(EditTemplate editTemplate, ViewStream viewStream) : this()
        {
            _editTemplate = editTemplate;
            _viewStream = viewStream;
        }

        private async Task CheckIfShouldUpdate()
        {
            while (CheckBoxIsChecked())
            {
                Logger.Debug("Checking If Should Update..");
                LiveStream? stream;
                while ((stream = _viewStream.CurrentLiveStream) == null) await Task.Delay(2000);
                if (!CheckBoxIsChecked()) return;
                var template = _editTemplate.SelectedTemplate;
                if (HasDifference(stream, template)) await Service.LiveStreamService!.UpdateStream(template);
                await Task.Delay(20000);
            }
        }

        private bool CheckBoxIsChecked() { return _checkBox.IsChecked ?? false; }

        private static bool HasDifference(LiveStream stream, Template template)
        {
            return !(template.Title.Equals(stream.Title) &&
                     template.Description.Equals(stream.Description) &&
                     template.Category.Equals(stream.Category) &&
                     template.Tags.Count == stream.Tags.Count &&
                     template.Tags.All(t => stream.Tags.Contains(t)));
        }

        #region EventListeners

        private void CheckBox_OnChecked(object? sender, RoutedEventArgs e)
        {
            if (CheckBoxIsChecked()) Task.Run(CheckIfShouldUpdate);
        }

        #endregion

        private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var stream = _viewStream.CurrentLiveStream;
            if (stream == null) return;
            var template = _editTemplate.SelectedTemplate;
            if (HasDifference(stream, template)) await Service.LiveStreamService!.UpdateStream(template);
        }
    }
}