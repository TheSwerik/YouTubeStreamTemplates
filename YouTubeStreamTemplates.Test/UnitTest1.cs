using NUnit.Framework;
using YouTubeStreamTemplates.Settings;

namespace YouTubeStreamTemplates.Test
{
    public class Tests
    {
        [SetUp] public void Setup() { }

        [Test] public void Test1() { Assert.Pass(); }

        [Test] public void TestLibraryWorking() { Assert.NotNull(SettingsService.Instance); }
    }
}