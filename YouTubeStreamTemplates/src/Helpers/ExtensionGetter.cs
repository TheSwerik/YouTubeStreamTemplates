namespace YouTubeStreamTemplates.Helpers
{
    public static class ExtensionGetter
    {
        public static string GetJsonExtension(string filename)
        {
            return filename.EndsWith(".png") ? "image/png" : "image/jpeg";
        }
    }
}