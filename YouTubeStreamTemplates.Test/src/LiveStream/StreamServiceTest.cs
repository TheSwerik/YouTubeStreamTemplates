using System;
using NUnit.Framework;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.LiveStream;

namespace YouTubeStreamTemplates.Test.LiveStream
{
    public class StreamServiceTest
    {
        [Test]
        [Ignore("Creates Files and Folders")]
        public void TestInitialize()
        {
            Assert.Throws<StreamServiceNotInitializedException>(() => Console.WriteLine(StreamService.Instance));
            Assert.False(StreamService.IsInitialized);

            StreamService.Init().Wait();

            Assert.NotNull(StreamService.Instance, "Init does not work");
            Assert.True(StreamService.IsInitialized, "Init does not set IsInitialized");

            Assert.ThrowsAsync<AlreadyInitializedException>(async () => await StreamService.Init(),
                                                            "Does not throw on second init");
        }
    }
}