using NUnit.Framework;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.Test.Templates
{
    public class TemplateTest
    {
        [Test]
        public void TestCreateTemplate()
        {
            const string name = "TestTemplate";
            var template = new Template(name);

            Assert.AreEqual(name, template.Name);

            var template2 = new Template(name);

            Assert.AreEqual(name, template2.Name);
            Assert.AreNotEqual(template.Id, template2.Id);
        }

        [Test]
        public void TestCompareTemplates()
        {
            var template = new Template("Test1");
            var template2 = new Template("Test2");

            Assert.AreNotEqual(template.Id, template2.Id);
            Assert.AreNotEqual(template.Name, template2.Name);
        }
    }
}