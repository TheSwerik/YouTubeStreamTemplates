using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NLog;

namespace YouTubeStreamTemplatesCrossPlatform
{
    public static class ImageHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string TempFolderPath => Path.GetTempPath() + @"\YouTubeStreamTemplates\";
        private static string TempThumbnailFolderPath => @$"{TempFolderPath}Thumbnails\";
        private static string TempTemplateThumbnailFolderPath => @$"{TempThumbnailFolderPath}Template\";
        private static string TempStreamThumbnailFolderPath => @$"{TempThumbnailFolderPath}LiveStream\";

        public static IImage PathToImage(string path, bool template, string id, int timeout = 1000)
        {
            var task = PathToImageAsync(path, template, id);
            task.Wait(timeout);
            if (!task.IsCompleted) Logger.Error($"Image Loading timeout: {path}");
            if (task.Exception != null) Logger.Error(task.Exception.Message);
            return PathToImageAsync(path, template, id).Result;
        }

        public static async Task<IImage> PathToImageAsync(string path, bool template, string id)
        {
            if (!path.StartsWith("http")) return new Bitmap(path);
            var savePath = $"{(template ? TempTemplateThumbnailFolderPath : TempStreamThumbnailFolderPath)}{id}.png";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(path + "?random=" + new Random().Next(), savePath);
            return new Bitmap(savePath);
        }

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(TempTemplateThumbnailFolderPath);
            Directory.CreateDirectory(TempStreamThumbnailFolderPath);
        }
    }
}