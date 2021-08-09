using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class CheckBoxSearchResult : UserControl
    {
        private readonly CheckBox _checkBox;
        private readonly Grid _grid;
        public readonly string Text;

        public CheckBoxSearchResult(string text) : this()
        {
            ((TextBlock)_grid.Children.First(c => c is TextBlock)).Text = text;
            Text = text;
        }

        public CheckBoxSearchResult(Playlist playlist) : this(playlist.Title) { Playlist = playlist; }

        // ReSharper disable once MemberCanBePrivate.Global
        public CheckBoxSearchResult()
        {
            Text = "";
            AvaloniaXamlLoader.Load(this);
            _grid = this.Find<Grid>("Grid");
            _checkBox = this.Find<CheckBox>("CheckBox");
        }

        public Playlist Playlist { get; }
        public bool IsChecked => _checkBox.IsChecked ?? false;
        public event EventHandler<Playlist> OnChanged = null!;

        private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            _checkBox.IsChecked = !_checkBox.IsChecked;
            OnChanged.Invoke(this, Playlist);
        }

        public void Check() { _checkBox.IsChecked = true; }

        private void CheckBox_OnClick(object sender, RoutedEventArgs e) { OnChanged.Invoke(this, Playlist); }
    }
}