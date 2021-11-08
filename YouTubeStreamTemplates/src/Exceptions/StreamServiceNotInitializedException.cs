namespace YouTubeStreamTemplates.Exceptions
{
    public class StreamServiceNotInitializedException : YouTubeStreamTemplateException
    {
        public StreamServiceNotInitializedException() : base("The StreamService is not Initialized yet.") { }
    }
}