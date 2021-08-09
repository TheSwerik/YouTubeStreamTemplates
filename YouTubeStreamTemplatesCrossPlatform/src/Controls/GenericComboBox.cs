using System;
using Avalonia.Controls;
using Avalonia.Styling;

namespace YouTubeStreamTemplatesCrossPlatform.Controls
{
    public class GenericComboBox<T> : ComboBox, IStyleable
    {
        public new T? SelectedItem
        {
            get => (T?)(base.SelectedItem ?? default(T));
            set => base.SelectedItem = value;
        }

        Type IStyleable.StyleKey => typeof(ComboBox);
    }
}