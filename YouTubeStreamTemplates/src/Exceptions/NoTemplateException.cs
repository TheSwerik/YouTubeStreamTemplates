namespace YouTubeStreamTemplates.Exceptions
{
    public class NoTemplateException : YouTubeStreamTemplateException
    {
        public NoTemplateException() : base("There is no current Template loaded.") { }
    }
}