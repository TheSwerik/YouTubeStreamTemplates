using System.Collections.Generic;

namespace YouTubeStreamTemplates.LiveStream
{
    public struct Playlist
    {
        public string Id { get; set; }
        public string Title { get; set; }

        /// <summary>
        ///     Key:    VideoID
        ///     Value: PlaylistItemID
        /// </summary>
        public Dictionary<string, string> Videos { get; set; }

        public Playlist(string id, string title, Dictionary<string, string> videos)
        {
            Id = id;
            Title = title;
            Videos = videos;
        }

        // public override string ToString() { return $"{{{Id}, {Title}}}"; }
        public override string ToString() { return $"{Title}"; }
    }
}