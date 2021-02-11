using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace YouTubeStreamTemplates.Helpers
{
    public static class ImageHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string TempFolderPath => Path.GetTempPath() + @"\YouTubeStreamTemplates\";
        private static string TempThumbnailFolderPath => @$"{TempFolderPath}Thumbnails\";
        private static string TempTemplateThumbnailFolderPath => @$"{TempThumbnailFolderPath}Template\";
        private static string TempStreamThumbnailFolderPath => @$"{TempThumbnailFolderPath}LiveStream\";

        public static string GetImagePath(string path, bool template, string id, int timeout = 1000)
        {
            var task = GetImagePathAsync(path, template, id);
            task.Wait(timeout);
            if (!task.IsCompleted) Logger.Error($"Image Loading timeout: {path}");
            if (task.Exception != null) Logger.Error(task.Exception.Message);
            return task.Result;
        }

        public static async Task<string> GetImagePathAsync(string path, bool template, string id)
        {
            if (!path.StartsWith("http")) return path;
            var savePath = $"{(template ? TempTemplateThumbnailFolderPath : TempStreamThumbnailFolderPath)}{id}.png";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(path + "?random=" + new Random().Next(), savePath);
            return savePath;
        }

        public static async Task<byte[]> GetStreamThumbnailBytesAsync(string id)
        {
            var path = $"{TempStreamThumbnailFolderPath}{id}.png";
            if (!File.Exists(path)) await GetLiveStreamImagePathAsync(id);
            return await File.ReadAllBytesAsync(path);
        }

        public static async Task<string> GetLiveStreamImagePathAsync(string id)
        {
            var savePath = $"{TempStreamThumbnailFolderPath}{id}.png";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(GetThumbnailUrlFromId(id) + "?random=" + new Random().Next(), savePath);
            return savePath;
        }

        public static string GetThumbnailUrlFromId(string id)
        {
            return $"https://i.ytimg.com/vi/{id}/maxresdefault_live.jpg";
        }

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(TempTemplateThumbnailFolderPath);
            Directory.CreateDirectory(TempStreamThumbnailFolderPath);
        }
    }
}