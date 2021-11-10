namespace YouTubeStreamTemplates.Exceptions
{
    public class NoClientIdOrSecretException : YouTubeStreamTemplateException
    {
        public NoClientIdOrSecretException() : base("There is no clientId or clientSecret provided.") { }
    }
}