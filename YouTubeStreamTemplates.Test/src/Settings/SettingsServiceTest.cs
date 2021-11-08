using NUnit.Framework;
using YouTubeStreamTemplates.Settings;

namespace YouTubeStreamTemplates.Test.Settings
{
    public class SettingsServiceTest
    {
        [Test]
        [Ignore("Creates Folders and Files")]
        public void TestCreateInstance()
        {
            var serviceInstance = SettingsService.Instance;
            Assert.NotNull(serviceInstance, "SettingsService.Instance is null");
            Assert.NotNull(serviceInstance.Settings, "Settings is null");
            Assert.Greater(serviceInstance.Settings.Keys.Count, 0, "Settings are empty");
        }
    }
}