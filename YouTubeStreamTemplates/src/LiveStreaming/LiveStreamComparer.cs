using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using static YouTubeStreamTemplates.LiveStreaming.LiveStreamSortMode;

namespace YouTubeStreamTemplates.LiveStreaming
{
    public class LiveBroadcastComparer : IComparer<LiveBroadcast>
    {
        public static readonly LiveBroadcastComparer ByDateDesc = new(DateDesc);
        public static readonly LiveBroadcastComparer ByDateAsc = new(DateAsc);
        public static readonly LiveBroadcastComparer ByTitleDesc = new(TitleDesc);
        public static readonly LiveBroadcastComparer ByTitleAsc = new(TitleAsc);

        public static readonly LiveBroadcastComparer ByDateDescPlanned =
            new(DateDesc, LiveStreamTimeCompareMode.Planned);

        public static readonly LiveBroadcastComparer ByDateAscPlanned = new(DateAsc, LiveStreamTimeCompareMode.Planned);

        public static readonly LiveBroadcastComparer ByTitleDescPlanned =
            new(TitleDesc, LiveStreamTimeCompareMode.Planned);

        public static readonly LiveBroadcastComparer ByTitleAscPlanned =
            new(TitleAsc, LiveStreamTimeCompareMode.Planned);

        private readonly LiveStreamSortMode _sortMode;
        private readonly LiveStreamTimeCompareMode _timeCompareMode;

        private LiveBroadcastComparer(LiveStreamSortMode sortMode,
                                      LiveStreamTimeCompareMode timeCompareMode = LiveStreamTimeCompareMode.Actual)
        {
            _sortMode = sortMode;
            _timeCompareMode = timeCompareMode;
        }

        public int Compare(LiveBroadcast? x, LiveBroadcast? y)
        {
            if (y == null) return 1;
            if (x == null) return -1;
            var xSnippet = x.Snippet;
            var xStartTime = _timeCompareMode == LiveStreamTimeCompareMode.Actual
                                 ? xSnippet.ActualStartTime
                                 : xSnippet.ScheduledStartTime;
            var ySnippet = y.Snippet;
            var yStartTime = _timeCompareMode == LiveStreamTimeCompareMode.Actual
                                 ? ySnippet.ActualStartTime
                                 : ySnippet.ScheduledStartTime;
            return _sortMode switch
                   {
                       DateDesc => -xStartTime?.CompareTo(yStartTime) ?? 1,
                       DateAsc => xStartTime?.CompareTo(yStartTime) ?? -1,
                       TitleDesc => -string.Compare(xSnippet.Title, ySnippet.Title, StringComparison.InvariantCulture),
                       TitleAsc => string.Compare(xSnippet.Title, ySnippet.Title, StringComparison.InvariantCulture),
                       _ => throw new ArgumentOutOfRangeException(nameof(_sortMode), _sortMode + "", "wrong")
                   };
        }
    }
}