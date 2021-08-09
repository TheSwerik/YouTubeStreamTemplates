using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using YouTubeStreamTemplates.LiveStreaming;
using YouTubeStreamTemplatesCrossPlatform.Windows;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class AutoCompleteSelectComboBox : UserControl
    {
        private readonly Grid _comboBoxGrid;
        private readonly Popup _resultPopup;
        private readonly TextBox _searchInputBox;
        private readonly StackPanel _searchResultPanel;
        private readonly TextBlock _selectedNumberText;

        private readonly Stopwatch _stopwatch;
        private readonly Grid _textGrid;

        private IEnumerable<Playlist> _items;

        public AutoCompleteSelectComboBox()
        {
            AvaloniaXamlLoader.Load(this);
            _textGrid = this.Find<Grid>("TextGrid");
            _comboBoxGrid = this.Find<Grid>("ComboBoxGrid");
            _searchInputBox = this.Find<TextBox>("SearchInputBox");
            _resultPopup = this.Find<Popup>("ResultPopup");
            _searchResultPanel = this.Find<StackPanel>("ResultStackPanel");
            _selectedNumberText = this.Find<TextBlock>("SelectedNumberText");
            _stopwatch = new Stopwatch();
            _items = new List<Playlist>();
            SelectedItems = new List<Playlist>();
            OnLostFocus(null, null);
            MainWindow.Instance.PositionChanged += OnWindowPositionChanged;
        }

        public IEnumerable<Playlist> Items
        {
            set => SetItems(value);
        }

        public List<Playlist> SelectedItems { get; }

        public event EventHandler<EventArgs> OnChanged = null!;

        private (List<CheckBoxSearchResult>, int) GetSortedResults()
        {
            var allResults = _searchResultPanel.Children
                                               .OfType<CheckBoxSearchResult>()
                                               .ToList();

            if (string.IsNullOrWhiteSpace(_searchInputBox.Text))
            {
                allResults.Sort((c1, c2) => string.CompareOrdinal(c1.Text, c2.Text));
                return (allResults, allResults.Count);
            }

            var results = new List<CheckBoxSearchResult>();
            foreach (var token in _searchInputBox.Text.Split(' '))
                results.AddRange(allResults
                                     .Where(c => c.Text.Contains(token, StringComparison.InvariantCultureIgnoreCase)));

            allResults.Sort((c1, c2) =>
                            {
                                var c2Count = results.Count(r => r.Equals(c2));
                                var c1Count = results.Count(r => r.Equals(c1));
                                if (c1Count == c2Count) return string.CompareOrdinal(c1.Text, c2.Text);
                                return c2Count - c1Count;
                            });
            return (allResults, results.Distinct().Count());
        }

        private void SetItems(IEnumerable<Playlist> items)
        {
            var itemList = new List<Playlist>();
            foreach (var item in items)
            {
                itemList.Add(item);
                var checkBoxSearchResult = new CheckBoxSearchResult(item);
                _searchResultPanel.Children.Add(checkBoxSearchResult);
                checkBoxSearchResult.OnChanged += SearchResult_OnClick;
            }

            _items = itemList;
        }

        public void SetSelectedItems(IEnumerable<Playlist> items)
        {
            SelectedItems.Clear();
            foreach (var item in items)
            {
                SelectedItems.Add(item);
                foreach (var checkBoxSearchResult in _searchResultPanel.Children
                                                                       .OfType<CheckBoxSearchResult>()
                                                                       .Where(c => c.Playlist.Equals(item)))
                    checkBoxSearchResult.Check();
            }

            _selectedNumberText.Text = SelectedItems.Count + " Selected";
        }

        #region EventHandlers

        private void OnClick(object? sender, PointerPressedEventArgs e)
        {
            if (_comboBoxGrid.IsEnabled && !_textGrid.IsEnabled) return;
            _textGrid.IsEnabled = false;
            _textGrid.IsVisible = false;
            _comboBoxGrid.IsEnabled = true;
            _comboBoxGrid.IsVisible = true;
            _searchInputBox.Focus();
            var a = new Popup();
        }

        private void OnLostFocus(object? sender, RoutedEventArgs? e)
        {
            if (_textGrid.IsEnabled && !_comboBoxGrid.IsEnabled ||
                _searchResultPanel.Children.OfType<CheckBoxSearchResult>().Any(c => c.IsPointerOver) ||
                _comboBoxGrid.IsFocused || _comboBoxGrid.Children.Any(c => c.IsFocused))
                return;


            _textGrid.IsEnabled = true;
            _textGrid.IsVisible = true;
            _comboBoxGrid.IsEnabled = false;
            _comboBoxGrid.IsVisible = false;
            _resultPopup.Close();
        }

        private void SearchResult_OnClick(object? sender, Playlist playlist)
        {
            var checkBoxSearchResult = (CheckBoxSearchResult)sender!;
            if (checkBoxSearchResult.IsChecked) SelectedItems.Add(playlist);
            else SelectedItems.Remove(playlist);
            _searchInputBox.Focus();
            _selectedNumberText.Text = SelectedItems.Count + " Selected";
            OnChanged.Invoke(this, null!);
        }

        private void SearchInputBox_OnTextInput(object? sender, KeyEventArgs keyEventArgs)
        {
            if (!_resultPopup.IsOpen) _resultPopup.Open();
            var (sortedResults, matching) = GetSortedResults();
            _searchResultPanel.Children.Clear();
            _searchResultPanel.Children.AddRange(sortedResults);
            if (matching > 40) matching = 40;
            foreach (var checkBoxSearchResult in sortedResults.Take(matching)) checkBoxSearchResult.IsVisible = true;
            foreach (var checkBoxSearchResult in sortedResults.Skip(matching)) checkBoxSearchResult.IsVisible = false;
            if (matching <= 0) _resultPopup.Close();
        }

        private void SearchInputBox_OnGotFocus(object? sender, GotFocusEventArgs e)
        {
            _searchInputBox.SelectAll();
            SearchInputBox_OnTextInput(null, new KeyEventArgs());
        }

        private async void OnWindowPositionChanged(object? sender, PixelPointEventArgs e)
        {
            if (!_resultPopup.IsOpen) return;
            var isRunning = _stopwatch.IsRunning;
            _stopwatch.Restart();
            if (!isRunning) await ChangePopupPosition();
        }

        private async Task ChangePopupPosition()
        {
            while (_stopwatch.ElapsedMilliseconds < 500) await Task.Delay(50);
            _resultPopup.Close();
            _resultPopup.Open();
            _stopwatch.Reset();
        }

        #endregion
    }
}