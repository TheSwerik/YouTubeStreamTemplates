using System;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplates.Templates
{
    public class Template : LiveStream
    {
        public Template(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Name { get; set; }
    }
}