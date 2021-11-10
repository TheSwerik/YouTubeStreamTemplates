using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using NUnit.Framework;
using YouTubeStreamTemplates.LiveStream;

namespace YouTubeStreamTemplates.Test.LiveStream
{
    public class ExtensionMethodsTest
    {
        [Test]
        public void TestLiveBroadcastToLiveStream()
        {
            var liveBroadCast = new LiveBroadcast
                                {
                                    Id = "testID",
                                    Snippet = new LiveBroadcastSnippet
                                              {
                                                  Title = "testTITLE",
                                                  Description = "testDESC",
                                                  ActualStartTime = DateTime.Today,
                                                  ActualEndTime = DateTime.Today.AddDays(1)
                                              }
                                };
            var stream = liveBroadCast.ToStream();

            Assert.AreEqual(liveBroadCast.Id, stream.Id, "Does not copy ID");
            Assert.AreEqual(liveBroadCast.Snippet.Title, stream.Title, "Does not copy Title");
            Assert.AreEqual(liveBroadCast.Snippet.Description, stream.Description, "Does not copy Description");
            Assert.AreEqual(liveBroadCast.Snippet.ActualStartTime, stream.StartTime, "Does not copy StartTime");
            Assert.AreEqual(liveBroadCast.Snippet.ActualEndTime, stream.EndTime, "Does not copy EndTime");
        }

        [Test]
        public void TestVideoToLiveStream()
        {
            var video = new Video
                        {
                            Id = "testID",
                            Snippet = new VideoSnippet
                                      {
                                          Title = "testTITLE",
                                          Description = "testDESC",
                                          CategoryId = "testCat",
                                          Tags = new List<string> { "testTag1", "testTag2" },
                                          DefaultLanguage = "testLANG",
                                          DefaultAudioLanguage = "testAUIDOLANG"
                                      },
                            LiveStreamingDetails = new VideoLiveStreamingDetails
                                                   {
                                                       ScheduledStartTime = DateTime.Today,
                                                       ScheduledEndTime = DateTime.Today.AddDays(1)
                                                   }
                        };
            var stream = video.ToStream();

            Assert.AreEqual(video.Id, stream.Id, "Does not copy ID");
            Assert.AreEqual(video.Snippet.Title, stream.Title, "Does not copy Title");
            Assert.AreEqual(video.Snippet.Description, stream.Description, "Does not copy Description");
            Assert.AreEqual(video.Snippet.CategoryId, stream.Category, "Does not copy Category");
            Assert.AreEqual(video.Snippet.DefaultLanguage, stream.TextLanguage, "Does not copy TextLanguage");
            Assert.AreEqual(video.Snippet.DefaultAudioLanguage, stream.AudioLanguage, "Does not copy AudioLanguage");
            Assert.AreEqual(video.LiveStreamingDetails.ScheduledStartTime, stream.StartTime, "Does not copy StartTime");
            Assert.AreEqual(video.LiveStreamingDetails.ScheduledEndTime, stream.EndTime, "Does not copy EndTime");
            Assert.AreEqual(video.Snippet.Tags.Count, stream.Tags.Count, "Does not copy Tags");
            Assert.AreEqual(video.Snippet.Tags[0], stream.Tags[0], "Does not copy Tags");
            Assert.AreEqual(video.Snippet.Tags[1], stream.Tags[1], "Does not copy Tags");
        }

        [Test]
        public void TestToLiveBroadcast()
        {
            var stream = new Stream
                         {
                             Id = "testID",
                             Title = "testTITLE",
                             Description = "testDESC",
                             StartTime = DateTime.Today,
                             EndTime = DateTime.Today.AddDays(1)
                         };
            var liveBroadCast = stream.ToLiveBroadcast();

            Assert.AreEqual(stream.Id, liveBroadCast.Id, "Does not copy ID");
            Assert.AreEqual(stream.Title, liveBroadCast.Snippet.Title, "Does not copy Title");
            Assert.AreEqual(stream.Description, liveBroadCast.Snippet.Description, "Does not copy Description");
            Assert.AreEqual(stream.StartTime, liveBroadCast.Snippet.ScheduledStartTime, "Does not copy StartTime");
            Assert.AreEqual(stream.EndTime, liveBroadCast.Snippet.ScheduledEndTime, "Does not copy EndTime");
            Assert.NotNull(liveBroadCast.Kind, "Does not create Kind");
        }

        [Test]
        public void TestToVideo()
        {
            var stream = new Stream
                         {
                             Id = "testID",
                             Title = "testTITLE",
                             Description = "testDESC",
                             Category = "testCat",
                             Tags = new List<string> { "testTag1", "testTag2" },
                             TextLanguage = "testLANG",
                             AudioLanguage = "testAUIDOLANG",
                             StartTime = DateTime.Today,
                             EndTime = DateTime.Today.AddDays(1)
                         };
            var video = stream.ToVideo();

            Assert.AreEqual(stream.Id, video.Id, "Does not copy ID");
            Assert.AreEqual(stream.Title, video.Snippet.Title, "Does not copy Title");
            Assert.AreEqual(stream.Description, video.Snippet.Description, "Does not copy Description");
            Assert.AreEqual(stream.Category, video.Snippet.CategoryId, "Does not copy Category");
            Assert.AreEqual(stream.TextLanguage, video.Snippet.DefaultLanguage, "Does not copy TextLanguage");
            Assert.AreEqual(stream.AudioLanguage, video.Snippet.DefaultAudioLanguage, "Does not copy AudioLanguage");
            Assert.AreEqual(stream.StartTime, video.LiveStreamingDetails.ScheduledStartTime, "Does not copy StartTime");
            Assert.AreEqual(stream.EndTime, video.LiveStreamingDetails.ScheduledEndTime, "Does not copy EndTime");
            Assert.AreEqual(stream.Tags.Count, video.Snippet.Tags.Count, "Does not copy Tags");
            Assert.AreEqual(stream.Tags[0], video.Snippet.Tags[0], "Does not copy Tags");
            Assert.AreEqual(stream.Tags[1], video.Snippet.Tags[1], "Does not copy Tags");
            Assert.NotNull(video.Status, "Does not create Status");
        }

        [Test]
        public void TestToDistinctDictionary()
        {
            var playlists = new List<PlaylistItem>
                            {
                                new()
                                {
                                    Snippet = new PlaylistItemSnippet
                                              { ResourceId = new ResourceId { VideoId = "test" } }
                                },
                                new()
                                {
                                    Snippet = new PlaylistItemSnippet
                                              { ResourceId = new ResourceId { VideoId = "test1" } }
                                },
                                new()
                                {
                                    Snippet = new PlaylistItemSnippet
                                              { ResourceId = new ResourceId { VideoId = "test2" } }
                                },
                                new()
                                {
                                    Snippet = new PlaylistItemSnippet
                                              { ResourceId = new ResourceId { VideoId = "test" } }
                                }
                            };

            var distinctPlaylists = playlists.ToDistinctDictionary();

            Assert.AreNotEqual(playlists.Count, distinctPlaylists.Count);
        }
    }
}