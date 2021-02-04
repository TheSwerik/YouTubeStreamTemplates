using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class TagCard : UserControl
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public TagCard() { AvaloniaXamlLoader.Load(this); }

        public TagCard(TagEditor tagEditor, string text, bool readOnly = false) : this()
        {
            this.Find<TextBlock>("TextBlock").Text = text;
            var closeButton = this.Find<Button>("CloseButton");
            if (readOnly) closeButton.IsVisible = false;
            else closeButton.Click += (s, e) => { tagEditor.Remove(this); };
        }
    }
}