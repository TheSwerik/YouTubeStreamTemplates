using System;

namespace YouTubeStreamTemplates.Helpers
{
    public static class ExtensionGetter
    {
        public static string GetJsonExtension(string filename)
        {
            if (filename.EndsWithIgnoreCase(".png")) return "image/png";
            if (filename.EndsWithIgnoreCase(".jpg") || filename.EndsWithIgnoreCase(".jpeg")) return "image/jpeg";
            return "";
        }

        private static bool EndsWithIgnoreCase(this string text, string end)
        {
            return text.EndsWith(end, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}