namespace YouTubeStreamTemplates.Exceptions
{
    public class OhPleaseNeverHappenException : YouTubeStreamTemplateException
    {
        public OhPleaseNeverHappenException(string error) : base(
            $"{error} somehow, if you see this, something really bad happened, pls never see this.")
        {
        }
    }
}