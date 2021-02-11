using System;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplates.Templates
{
    public record Template : LiveStream
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