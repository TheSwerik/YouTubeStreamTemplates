using System.Collections.Generic;
using System.Linq;
using YouTubeStreamTemplates.Exceptions;

namespace YouTubeStreamTemplates.Templates
{
    public static class ExtensionMethods
    {
        public static string GetValue(this IEnumerable<string> lines, string key)
        {
            var line = lines.FirstOrDefault(l => l.Trim().StartsWith(key + ":"));
            if (line == null) throw new LineMissingKeyException(key);
            return line[(key.Length + 1)..].Trim();
        }
    }
}