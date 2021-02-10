using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplates.Exceptions
{
    public class ThumbnailTooLargeException : YouTubeStreamTemplateException
    {
        public ThumbnailTooLargeException(long fileSize) : base(
            $"Thumbnail is too large. {fileSize} bytes > {LiveStream.MaxThumbnailSize} bytes")
        {
        }
    }
}