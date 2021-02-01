using System;
using YouTubeStreamTemplates.LiveStreaming;

namespace YouTubeStreamTemplatesCLI
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            try
            {
                var liveStreamService = LiveStreamService.Init().Result;
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