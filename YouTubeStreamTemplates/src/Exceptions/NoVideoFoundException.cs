namespace YouTubeStreamTemplates.Exceptions
{
    public class NoVideoFoundException : YouTubeStreamTemplateException
    {
        public NoVideoFoundException(string id) : base($"Could not find Video with ID {id}.") { }
    }
}