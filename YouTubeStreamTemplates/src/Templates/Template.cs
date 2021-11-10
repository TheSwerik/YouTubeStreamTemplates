using System;
using YouTubeStreamTemplates.LiveStream;

namespace YouTubeStreamTemplates.Templates
{
    public record Template : Stream
    {
        public Template(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Thumbnail = new Thumbnail();
        }

        public string Name { get; set; }
        public Thumbnail Thumbnail { get; set; }
    }
}