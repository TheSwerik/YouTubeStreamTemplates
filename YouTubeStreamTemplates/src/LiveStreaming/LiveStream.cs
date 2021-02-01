using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeStreamTemplates.LiveStreaming
{
    //TODO rename
    public class LiveStream : IComparable<LiveStream>
    {
        public string Id { get; init; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ThumbnailDetails Thumbnails { get; set; } = null!;
        public List<string> Tags { get; set; } = new();
        public string TextLanguage { get; set; } = "en";
        public string AudioLanguage { get; set; } = "en";
        public string Category { get; set; } = null!;

        /// <summary>DOESN'T WORK IN THE CURRENT YOUTUBE API VERSION</summary>
        public string? Game { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CompareTo(LiveStream? other) { return StartTime.CompareTo(other?.StartTime); }

        public LiveBroadcast ToLiveBroadcast()
        {
            return new()
                   {
                       Id = Id,
                       Snippet = new LiveBroadcastSnippet
                                 {
                                     Title = Title,
                                     Description = Description,
                                     Thumbnails = Thumbnails,
                                     ScheduledStartTime = StartTime.ToUniversalTime()
                                 }
// Kind = 
                   };
        }

        public override string ToString()
        {
            return Id + "\n" +
                   Title + "\n" +
                   string.Join(", ", Tags) + "\n" +
                   Category + "\n" +
                   StartTime + "\n" +
                   EndTime;
        }
    }
}