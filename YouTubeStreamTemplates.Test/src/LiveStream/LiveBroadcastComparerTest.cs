using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using NUnit.Framework;
using YouTubeStreamTemplates.LiveStream;

namespace YouTubeStreamTemplates.Test.LiveStream
{
    public class LiveBroadcastComparerTest
    {
        private List<LiveBroadcast> _liveBroadcasts;

        [SetUp]
        public void SetUp()
        {
            _liveBroadcasts = new List<LiveBroadcast>
                              {
                                  new()
                                  {
                                      Snippet = new LiveBroadcastSnippet
                                                {
                                                    Title = "5",
                                                    ActualStartTime = DateTime.Today.AddDays(5),
                                                    ScheduledStartTime = DateTime.Today.AddDays(5)
                                                }
                                  },
                                  new()
                                  {
                                      Snippet = new LiveBroadcastSnippet
                                                {
                                                    Title = "1",
                                                    ActualStartTime = DateTime.Today.AddDays(1),
                                                    ScheduledStartTime = DateTime.Today.AddDays(1)
                                                }
                                  },
                                  new()
                                  {
                                      Snippet = new LiveBroadcastSnippet
                                                {
                                                    Title = "0",
                                                    ActualStartTime = DateTime.Today.AddDays(0),
                                                    ScheduledStartTime = DateTime.Today.AddDays(0)
                                                }
                                  },
                                  new()
                                  {
                                      Snippet = new LiveBroadcastSnippet
                                                {
                                                    Title = "4",
                                                    ActualStartTime = DateTime.Today.AddDays(4),
                                                    ScheduledStartTime = DateTime.Today.AddDays(4)
                                                }
                                  },
                                  new()
                                  {
                                      Snippet = new LiveBroadcastSnippet
                                                {
                                                    Title = "2",
                                                    ActualStartTime = DateTime.Today.AddDays(2),
                                                    ScheduledStartTime = DateTime.Today.AddDays(2)
                                                }
                                  },
                                  new()
                                  {
                                      Snippet = new LiveBroadcastSnippet
                                                {
                                                    Title = "3",
                                                    ActualStartTime = DateTime.Today.AddDays(3),
                                                    ScheduledStartTime = DateTime.Today.AddDays(3)
                                                }
                                  }
                              };
        }

        [Test]
        public void TestCreateAllPremadeComparers()
        {
            Assert.NotNull(LiveBroadcastComparer.ByDateDesc);
            Assert.NotNull(LiveBroadcastComparer.ByDateAsc);
            Assert.NotNull(LiveBroadcastComparer.ByTitleDesc);
            Assert.NotNull(LiveBroadcastComparer.ByTitleAsc);
            Assert.NotNull(LiveBroadcastComparer.ByDateDescPlanned);
            Assert.NotNull(LiveBroadcastComparer.ByDateAscPlanned);
            Assert.NotNull(LiveBroadcastComparer.ByTitleDescPlanned);
            Assert.NotNull(LiveBroadcastComparer.ByTitleAscPlanned);
        }

        [Test]
        public void TestCompareByDateDesc()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByDateDesc);
            Assert.Greater(_liveBroadcasts[0].Snippet.ActualStartTime, _liveBroadcasts[1].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[1].Snippet.ActualStartTime, _liveBroadcasts[2].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[2].Snippet.ActualStartTime, _liveBroadcasts[3].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[3].Snippet.ActualStartTime, _liveBroadcasts[4].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[4].Snippet.ActualStartTime, _liveBroadcasts[5].Snippet.ActualStartTime);
        }

        [Test]
        public void TestCompareByDateAsc()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByDateAsc);
            Assert.Greater(_liveBroadcasts[5].Snippet.ActualStartTime, _liveBroadcasts[4].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[4].Snippet.ActualStartTime, _liveBroadcasts[3].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[3].Snippet.ActualStartTime, _liveBroadcasts[2].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[2].Snippet.ActualStartTime, _liveBroadcasts[1].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[1].Snippet.ActualStartTime, _liveBroadcasts[0].Snippet.ActualStartTime);
        }

        [Test]
        public void TestCompareByTitleDesc()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByTitleDesc);
            Assert.Greater(_liveBroadcasts[0].Snippet.Title, _liveBroadcasts[1].Snippet.Title);
            Assert.Greater(_liveBroadcasts[1].Snippet.Title, _liveBroadcasts[2].Snippet.Title);
            Assert.Greater(_liveBroadcasts[2].Snippet.Title, _liveBroadcasts[3].Snippet.Title);
            Assert.Greater(_liveBroadcasts[3].Snippet.Title, _liveBroadcasts[4].Snippet.Title);
            Assert.Greater(_liveBroadcasts[4].Snippet.Title, _liveBroadcasts[5].Snippet.Title);
        }

        [Test]
        public void TestCompareByTitleAsc()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByTitleAsc);
            Assert.Greater(_liveBroadcasts[5].Snippet.Title, _liveBroadcasts[4].Snippet.Title);
            Assert.Greater(_liveBroadcasts[4].Snippet.Title, _liveBroadcasts[3].Snippet.Title);
            Assert.Greater(_liveBroadcasts[3].Snippet.Title, _liveBroadcasts[2].Snippet.Title);
            Assert.Greater(_liveBroadcasts[2].Snippet.Title, _liveBroadcasts[1].Snippet.Title);
            Assert.Greater(_liveBroadcasts[1].Snippet.Title, _liveBroadcasts[0].Snippet.Title);
        }

        [Test]
        public void TestCompareByDateDescPlanned()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByDateDescPlanned);
            Assert.Greater(_liveBroadcasts[0].Snippet.ActualStartTime, _liveBroadcasts[1].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[1].Snippet.ActualStartTime, _liveBroadcasts[2].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[2].Snippet.ActualStartTime, _liveBroadcasts[3].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[3].Snippet.ActualStartTime, _liveBroadcasts[4].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[4].Snippet.ActualStartTime, _liveBroadcasts[5].Snippet.ActualStartTime);
        }

        [Test]
        public void TestCompareByDateAscPlanned()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByDateAscPlanned);
            Assert.Greater(_liveBroadcasts[5].Snippet.ActualStartTime, _liveBroadcasts[4].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[4].Snippet.ActualStartTime, _liveBroadcasts[3].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[3].Snippet.ActualStartTime, _liveBroadcasts[2].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[2].Snippet.ActualStartTime, _liveBroadcasts[1].Snippet.ActualStartTime);
            Assert.Greater(_liveBroadcasts[1].Snippet.ActualStartTime, _liveBroadcasts[0].Snippet.ActualStartTime);
        }

        [Test]
        public void TestCompareByTitleDescPlanned()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByTitleDescPlanned);
            Assert.Greater(_liveBroadcasts[0].Snippet.Title, _liveBroadcasts[1].Snippet.Title);
            Assert.Greater(_liveBroadcasts[1].Snippet.Title, _liveBroadcasts[2].Snippet.Title);
            Assert.Greater(_liveBroadcasts[2].Snippet.Title, _liveBroadcasts[3].Snippet.Title);
            Assert.Greater(_liveBroadcasts[3].Snippet.Title, _liveBroadcasts[4].Snippet.Title);
            Assert.Greater(_liveBroadcasts[4].Snippet.Title, _liveBroadcasts[5].Snippet.Title);
        }

        [Test]
        public void TestCompareByTitleAscPlanned()
        {
            _liveBroadcasts.Sort(LiveBroadcastComparer.ByTitleAscPlanned);
            Assert.Greater(_liveBroadcasts[5].Snippet.Title, _liveBroadcasts[4].Snippet.Title);
            Assert.Greater(_liveBroadcasts[4].Snippet.Title, _liveBroadcasts[3].Snippet.Title);
            Assert.Greater(_liveBroadcasts[3].Snippet.Title, _liveBroadcasts[2].Snippet.Title);
            Assert.Greater(_liveBroadcasts[2].Snippet.Title, _liveBroadcasts[1].Snippet.Title);
            Assert.Greater(_liveBroadcasts[1].Snippet.Title, _liveBroadcasts[0].Snippet.Title);
        }
    }
}