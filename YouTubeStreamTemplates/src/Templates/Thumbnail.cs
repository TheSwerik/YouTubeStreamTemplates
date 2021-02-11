using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YouTubeStreamTemplates.Templates
{
    public record Thumbnail
    {
        public string Source { get; set; } = "";
        public byte[] Result { get; set; }

        public bool HasSameResult(string otherPath) { return HasSameResult(File.ReadAllBytes(otherPath)); }
        public bool HasSameResult(IEnumerable<byte> other) { return Result.SequenceEqual(other); }
    }
}