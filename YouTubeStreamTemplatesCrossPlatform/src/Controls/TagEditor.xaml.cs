using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class TagEditor : UserControl
    {
        private readonly TextBox _inputTextBox;
        private readonly WrapPanel _tagsPanel;

        public List<TagCard> TagCards => _tagsPanel.Children.OfType<TagCard>().ToList();
        public List<string> Tags => _tagsPanel.Children.OfType<TagCard>().Select(c => c.Text).ToList();
        public void Remove(TagCard tagCard) { _tagsPanel.Children.Remove(tagCard); }

        #region EventListener

        //TODO
        // private void InputTextBox_OnTextInput(object? sender, TextInputEventArgs e)
        // {
        //     InputTextBox_FinishWriting();
        //     Console.WriteLine("ALARM");
        // }

        private void InputTextBox_OnLostFocus(object? sender, RoutedEventArgs e) { InputTextBox_FinishWriting(); }

        private void InputTextBox_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.OemComma) InputTextBox_FinishWriting();
            InvokeOnRender(_inputTextBox.Focus);
        }

        private void InputTextBox_OnTextEntered(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.Property.Name.Equals("Text") || e.NewValue.ToString() == null) return;
            var tags = e.NewValue.ToString()!.Split(",");
            for (var i = 0; i < tags.Length - 1; i++) InputTextBox_FinishWriting(tags[i]);
            _inputTextBox.Text = tags[^1];
            InvokeOnRender(_inputTextBox.Focus);
        }

        private void InputTextBox_FinishWriting(string? text = null)
        {
            text ??= _inputTextBox.Text.Replace(",", "");
            if (string.IsNullOrWhiteSpace(text)) return;
            if (text.Length > 100) throw new ArgumentException("text is too long");
            // if (string.Join(",", SelectedLivestream.CurrentLiveStream.Tags).Length + text.Length + 1 > 500)
            // throw new ArgumentException("tags are too long");

            // SelectedLivestream.CurrentLiveStream.Tags.Add(text);
            // SelectedLivestream.OnNext();
            _inputTextBox.Text = "";
        }

        #endregion

        #region Init

        // ReSharper disable once MemberCanBePrivate.Global
        public TagEditor()
        {
            AvaloniaXamlLoader.Load(this);
            _tagsPanel = this.Find<WrapPanel>("TagsPanel")!;
            _inputTextBox = this.Find<TextBox>("InputTextBox")!;
            _inputTextBox.PropertyChanged += InputTextBox_OnTextEntered;
        }

        public TagEditor(List<string> tags) : this() { RefreshTags(tags); }

        public void RefreshTags(List<string> tags)
        {
            InvokeOnRender(() =>
                           {
                               var controls = _tagsPanel.Children;
                               controls.Clear();
                               controls.AddRange(tags.Select(tag => new TagCard(this, tag)));
                               InvokeOnRender(ResizeInputBox);
                               controls.Add(_inputTextBox);
                           });
        }

        #endregion

        #region Helper Methods

        private void ResizeInputBox()
        {
            var maxWidth = _tagsPanel.Bounds.Width;
            var allWithoutTextBox = _tagsPanel.Children.Where(c => c is not TextBox).ToList();
            var highestY = allWithoutTextBox.Max(c => c.Bounds.Y);
            var lineWidth = allWithoutTextBox.Where(c => Math.Abs(c.Bounds.Y - highestY) < 1).Sum(c => c.Bounds.Width);
            var desiredWidth = maxWidth - lineWidth;
            _inputTextBox.Width = desiredWidth < _inputTextBox.MinWidth ? maxWidth : desiredWidth;
        }

        private static void InvokeOnRender(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
        }

        #endregion
    }
}