using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;

namespace YouTubeStreamTemplates.UI
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                File.WriteAllText(
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\YouTubeStreamTemplates\error logs {DateTime.Now.ToFileTime()}.txt",
                    e.GetType() + ": " + e.Message + "\n" + e.StackTrace);
                throw;
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                             .UsePlatformDetect()
                             .LogToTrace()
                             .UseReactiveUI();
        }
    }
}