using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class TagCard : UserControl
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public TagCard()
        {
            AvaloniaXamlLoader.Load(this);
            Text = "";
        }

        public TagCard(TagEditor tagEditor, string text) : this()
        {
            this.Find<Button>("CloseButton").Click += (s, e) => { tagEditor.Remove(this); };
            this.Find<TextBlock>("TextBlock").Text = text;
            Text = text;
        }

        public string Text { get; }
    }
}