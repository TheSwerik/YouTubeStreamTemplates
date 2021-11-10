using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using static YouTubeStreamTemplates.LiveStream.LiveBroadcastComparer.LiveStreamSortMode;

namespace YouTubeStreamTemplates.LiveStream
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
            return _sortMode switch
                   {
                       DateDesc or DateAsc => CompareDate(x, y) == 0 ? CompareTitle(x, y) : CompareDate(x, y),
                       TitleDesc or TitleAsc => CompareTitle(x, y) == 0 ? CompareDate(x, y) : CompareTitle(x, y),
                       _ => throw new ArgumentOutOfRangeException(nameof(_sortMode), _sortMode + "", "wrong")
                   };
        }

        private int CompareDate(LiveBroadcast x, LiveBroadcast y)
        {
            var xSnippet = x.Snippet;
            var xStartTime = _timeCompareMode == LiveStreamTimeCompareMode.Actual
                                 ? xSnippet.ActualStartTime
                                 : xSnippet.ScheduledStartTime;
            var ySnippet = y.Snippet;
            var yStartTime = _timeCompareMode == LiveStreamTimeCompareMode.Actual
                                 ? ySnippet.ActualStartTime
                                 : ySnippet.ScheduledStartTime;
            var sortValue = xStartTime?.CompareTo(yStartTime) ?? -1;
            return _sortMode is DateDesc or TitleDesc ? -sortValue : sortValue;
        }

        private int CompareTitle(LiveBroadcast x, LiveBroadcast y)
        {
            var xSnippet = x.Snippet;
            var ySnippet = y.Snippet;
            var sortValue = string.Compare(xSnippet.Title, ySnippet.Title, StringComparison.InvariantCulture);
            return _sortMode is TitleDesc or DateDesc ? -sortValue : sortValue;
        }

        internal enum LiveStreamSortMode
        {
            DateDesc,
            DateAsc,
            TitleDesc,
            TitleAsc
        }

        private enum LiveStreamTimeCompareMode
        {
            Actual,
            Planned
        }
    }
}