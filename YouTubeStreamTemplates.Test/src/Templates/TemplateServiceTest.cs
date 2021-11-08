using NUnit.Framework;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.Test.Templates
{
    public class TemplateServiceTest
    {
        [Test]
        [Ignore("Creates Files and Folders")]
        public void TestGetInstance()
        {
            var templateService = TemplateService.Instance;

            Assert.NotNull(templateService);
            Assert.NotNull(templateService.Templates);
        }

        [Test]
        [Ignore("Creates Files and Folders")]
        public void TestSaveAndDeleteTemplate()
        {
            var templateService = TemplateService.Instance;
            var template = new Template("test");
            templateService.SaveTemplate(template);

            Assert.True(templateService.Templates.Contains(template));

            templateService.DeleteTemplate(template);

            Assert.False(templateService.Templates.Contains(template));
        }
    }
}