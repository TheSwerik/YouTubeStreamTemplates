using NUnit.Framework;
using YouTubeStreamTemplates.Helpers;

namespace YouTubeStreamTemplates.Test.Helpers
{
    public class ExtensionGetterTest
    {
        [Test]
        public void TestGetPNG()
        {
            var filename = "test.png";
            Assert.AreEqual(
                "image/png",
                ExtensionGetter.GetJsonExtension(filename),
                "ExtensionGetter can't parse .png"
            );

            filename = "test.PNG";
            Assert.AreEqual(
                "image/png",
                ExtensionGetter.GetJsonExtension(filename),
                "ExtensionGetter can't parse .PNG"
            );
        }

        [Test]
        public void TestGetJPG()
        {
            var filename = "test.jpg";
            Assert.AreEqual(
                "image/jpeg",
                ExtensionGetter.GetJsonExtension(filename),
                "ExtensionGetter can't parse .jpg"
            );

            filename = "test.JPG";
            Assert.AreEqual(
                "image/jpeg",
                ExtensionGetter.GetJsonExtension(filename),
                "ExtensionGetter can't parse .JPG"
            );
        }

        [Test]
        public void TestGetJPEG()
        {
            var filename = "test.jpeg";
            Assert.AreEqual(
                "image/jpeg",
                ExtensionGetter.GetJsonExtension(filename),
                "ExtensionGetter can't parse .jpeg"
            );

            filename = "test.JPEG";
            Assert.AreEqual(
                "image/jpeg",
                ExtensionGetter.GetJsonExtension(filename),
                "ExtensionGetter can't parse .JPEG"
            );
        }
    }
}