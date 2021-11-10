using System.Collections.Generic;
using NUnit.Framework;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.Test.Templates
{
    public class ExtensionMethodsTest
    {
        [Test]
        public void TestGetValue()
        {
            var keys = new string[2] { "A", "B" };
            var values = new string[2] { "187", "test" };
            var lines = new List<string> { $"{keys[0]}:{values[0]}", $"{keys[1]}: {values[1]}" };

            Assert.AreEqual(values[0], lines.GetValue(keys[0]), "Can not read Value");
            Assert.AreEqual(values[1], lines.GetValue(keys[1]), "Does not trim Value");
            Assert.Throws<LineMissingKeyException>(() => lines.GetValue("C"), "Does not throw when key is missing");
        }
    }
}