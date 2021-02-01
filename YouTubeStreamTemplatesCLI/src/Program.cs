using System;
using System.Threading.Tasks;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplatesCLI
{
    internal static class Program
    {
        internal static async Task Main(string[] args)
        {
            try
            {
                var liveStreamService = LiveStreamService.Init().Result;

                await liveStreamService.GetCategories();
            }
            catch (AggregateException e)
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    Console.WriteLine(innerException.GetType() + ": " + innerException.Message);
                    Console.WriteLine(innerException.StackTrace);
                }
            }
        }
    }
}