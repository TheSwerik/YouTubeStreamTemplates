using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeStreamTemplates.LiveStreaming
{
    public static class ExtensionMethods
    {
        public static LiveStream ToLiveStream(this LiveBroadcast liveBroadcast)
        {
            return new LiveStream
                   {
                       Id = liveBroadcast.Id,
                       Title = liveBroadcast.Snippet.Title,
                       Description = liveBroadcast.Snippet.Description,
                       StartTime = liveBroadcast.Snippet.ActualStartTime ?? DateTime.MinValue,
                       EndTime = liveBroadcast.Snippet.ActualEndTime ?? DateTime.MinValue
                   };
        }

        public static LiveBroadcast ToLiveBroadcast(this LiveStream liveStream)
        {
            return new LiveBroadcast
                   {
                       Id = liveStream.Id,
                       Kind = "youtube#liveBroadcast",
                       Snippet = new LiveBroadcastSnippet
                                 {
                                     Title = liveStream.Title,
                                     Description = liveStream.Description,
                                     ScheduledStartTime = liveStream.StartTime.ToUniversalTime()
                                 }
                   };
        }

        public static Video ToVideo(this LiveStream liveStream)
        {
            return new Video
                   {
                       Id = liveStream.Id,
                       Snippet = new VideoSnippet
                                 {
                                     Title = liveStream.Title,
                                     Description = liveStream.Description,
                                     CategoryId = liveStream.Category,
                                     Tags = liveStream.Tags,
                                     DefaultLanguage = liveStream.TextLanguage,
                                     DefaultAudioLanguage = liveStream.AudioLanguage
                                 },
                       LiveStreamingDetails = new VideoLiveStreamingDetails
                                              {
                                                  ScheduledStartTime = liveStream.StartTime.ToUniversalTime(),
                                                  ScheduledEndTime = liveStream.EndTime.ToUniversalTime()
                                              },
                       Status = new VideoStatus()
                   };
        }

        public static LiveStream ToLiveStream(this Video video)
        {
            return new LiveStream
                   {
                       Id = video.Id,
                       Title = video.Snippet.Title,
                       Description = video.Snippet.Description,
                       Category = video.Snippet.CategoryId,
                       Tags = (List<string>)(video.Snippet.Tags ?? new List<string>()),
                       TextLanguage = video.Snippet.DefaultLanguage,
                       AudioLanguage = video.Snippet.DefaultAudioLanguage,
                       StartTime = video.LiveStreamingDetails?.ScheduledStartTime ?? DateTime.MinValue,
                       EndTime = video.LiveStreamingDetails?.ScheduledEndTime ?? DateTime.MinValue
                   };
        }

        public static Dictionary<string, string> ToDistinctDictionary(this IEnumerable<PlaylistItem> items)
        {
            return items.GroupBy(p => p.Snippet.ResourceId.VideoId)
                        .Select(g => g.First())
                        .ToDictionary(i => i.Snippet.ResourceId.VideoId, i => i.Id);
        }
    }
}