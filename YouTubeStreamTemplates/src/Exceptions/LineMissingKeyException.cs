namespace YouTubeStreamTemplates.Exceptions
{
    public class LineMissingKeyException : YouTubeStreamTemplateException
    {
        public LineMissingKeyException(string key) : base($"No line contains the key {key}.") { }
    }
}