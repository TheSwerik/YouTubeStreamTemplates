using System;
using System.Collections.Generic;
using NUnit.Framework;
using YouTubeStreamTemplates.LiveStream;

namespace YouTubeStreamTemplates.Test.LiveStream
{
    public class StreamTest
    {
        [Test]
        public void TestSort()
        {
            var streams = new List<Stream>
                          {
                              new() { StartTime = DateTime.Today.AddDays(1) },
                              new() { StartTime = DateTime.Today },
                              new() { StartTime = DateTime.Today.AddDays(3) },
                              new() { StartTime = DateTime.Today.AddDays(2) },
                              new() { StartTime = DateTime.Today.AddDays(4) }
                          };

            var sortedStreams = new List<Stream>(streams);
            sortedStreams.Sort();

            Assert.AreNotEqual(streams[0].StartTime, sortedStreams[0].StartTime);
            Assert.AreNotEqual(streams[1].StartTime, sortedStreams[1].StartTime);
            Assert.AreNotEqual(streams[2].StartTime, sortedStreams[2].StartTime);
            Assert.AreNotEqual(streams[3].StartTime, sortedStreams[3].StartTime);
            Assert.AreEqual(streams[4].StartTime, sortedStreams[4].StartTime);
        }

        [TestFixture]
        public class StreamTestEqual : StreamTest
        {
            [Test]
            public void TestEquals()
            {
                var stream1 = new Stream
                              {
                                  Title = "Test1",
                                  Description = "Test1",
                                  Category = "Test1",
                                  Tags = new List<string> { "Test1", "Test2" }
                              };
                var stream2 = new Stream
                              {
                                  Title = "Test1",
                                  Description = "Test1",
                                  Category = "Test1",
                                  Tags = new List<string> { "Test1", "Test2" }
                              };

                Assert.False(stream1.HasDifference(stream2), "HasDifference is True eventhough they are the same");
            }
        }

        [TestFixture]
        public class StreamTestNotEqual : StreamTest
        {
            [SetUp]
            public void SetupBeforeEachTest()
            {
                _stream1 = new Stream
                           {
                               Title = "Test1",
                               Description = "Test1",
                               Category = "Test1",
                               Tags = new List<string> { "Test1", "Test2" }
                           };
                _stream2 = new Stream
                           {
                               Title = "Test1",
                               Description = "Test1",
                               Category = "Test1",
                               Tags = new List<string> { "Test1", "Test2" }
                           };
            }

            private Stream _stream1;
            private Stream _stream2;

            [Test]
            public void TestTitleNotEqual()
            {
                _stream2.Title = "Test2";
                Assert.True(_stream1.HasDifference(_stream2), "HasDifference doesn't detect Title");
            }

            [Test]
            public void TestDescriptionNotEqual()
            {
                _stream2.Description = "Test2";
                Assert.True(_stream1.HasDifference(_stream2), "HasDifference doesn't detect Description");
            }

            [Test]
            public void TestCategoryNotEqual()
            {
                _stream2.Category = "Test2";
                Assert.True(_stream1.HasDifference(_stream2), "HasDifference doesn't detect Category");
            }

            [Test]
            public void TestTagsLengthNotEqual()
            {
                _stream2.Tags.Add("Test3");
                Assert.True(_stream1.HasDifference(_stream2), "HasDifference doesn't detect Tag-length");
            }

            [Test]
            public void TestTagsNotEqual()
            {
                _stream2.Tags[1] = "Test3";
                Assert.True(_stream1.HasDifference(_stream2), "HasDifference doesn't detect Tags");
            }
        }
    }
}