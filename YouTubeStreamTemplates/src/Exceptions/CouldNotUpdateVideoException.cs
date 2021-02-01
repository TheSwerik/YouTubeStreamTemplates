using Google.Apis.Requests;

namespace YouTubeStreamTemplates.Exceptions
{
    public class CouldNotUpdateVideoException : YouTubeStreamTemplateException
    {
        public CouldNotUpdateVideoException(RequestError error) : base($"Could not update Video:\n{error}") { }
    }
}