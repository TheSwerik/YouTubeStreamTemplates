using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace YouTubeStreamTemplatesCrossPlatform
{
    public static class ImageHelper
    {
        private static string TempFolderPath => Path.GetTempPath() + @"\YouTubeStreamTemplates\";
        private static string TempThumbnailFolderPath => @$"{TempFolderPath}Thumbnails\";
        private static string TempTemplateThumbnailFolderPath => @$"{TempThumbnailFolderPath}Template\";
        private static string TempStreamThumbnailFolderPath => @$"{TempThumbnailFolderPath}LiveStream\";

        public static IImage PathToImage(string path, bool template, string id)
        {
            if (!path.StartsWith("http")) return new Bitmap(path);
            var savePath = $"{(template ? TempTemplateThumbnailFolderPath : TempStreamThumbnailFolderPath)}{id}.png";
            using var client = new WebClient();
            client.DownloadFile(path, savePath);
            return new Bitmap(savePath);
        }

        public static async Task<IImage> PathToImageAsync(string path, bool template, string id)
        {
            if (!path.StartsWith("http")) return new Bitmap(path);
            var savePath = $"{(template ? TempTemplateThumbnailFolderPath : TempStreamThumbnailFolderPath)}{id}.png";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(path, savePath);
            return new Bitmap(savePath);
        }

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(TempTemplateThumbnailFolderPath);
            Directory.CreateDirectory(TempStreamThumbnailFolderPath);
        }
    }
}