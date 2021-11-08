using System;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeStreamTemplates.LiveStream
{
    //TODO rename
    public record Stream : IComparable<Stream>
    {
        public const int MaxThumbnailSize = 2097152;
        public string Id { get; init; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<string> Tags { get; set; } = new();
        public string TextLanguage { get; set; } = "en";
        public string AudioLanguage { get; set; } = "en";
        public string Category { get; set; } = null!;

        /// <summary>DOESN'T WORK IN THE CURRENT YOUTUBE API VERSION</summary>
        public string? Game { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> PlaylistIDs { get; set; } = new();

        #region Methods

        public bool HasDifference(Stream other)
        {
            return !(Title.Equals(other.Title) &&
                     Description.Equals(other.Description) &&
                     Category.Equals(other.Category) &&
                     Tags.Count == other.Tags.Count &&
                     Tags.All(t => other.Tags.Contains(t)));
        }

        public int CompareTo(Stream? other) { return StartTime.CompareTo(other?.StartTime); }

        public override string ToString()
        {
            return Id + "\n" +
                   Title + "\n" +
                   string.Join(", ", Tags) + "\n" +
                   Category + "\n" +
                   StartTime + "\n" +
                   EndTime;
        }

        #endregion
    }
}