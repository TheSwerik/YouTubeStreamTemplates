using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeStreamTemplates.LiveStream
{
    public static class ExtensionMethods
    {
        public static Stream ToStream(this LiveBroadcast liveBroadcast)
        {
            return new Stream
                   {
                       Id = liveBroadcast.Id,
                       Title = liveBroadcast.Snippet.Title,
                       Description = liveBroadcast.Snippet.Description,
                       StartTime = liveBroadcast.Snippet.ActualStartTime ?? DateTime.MinValue,
                       EndTime = liveBroadcast.Snippet.ActualEndTime ?? DateTime.MinValue
                   };
        }

        public static Stream ToStream(this Video video)
        {
            return new Stream
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

        public static LiveBroadcast ToLiveBroadcast(this Stream stream)
        {
            return new LiveBroadcast
                   {
                       Id = stream.Id,
                       Kind = "youtube#liveBroadcast",
                       Snippet = new LiveBroadcastSnippet
                                 {
                                     Title = stream.Title,
                                     Description = stream.Description,
                                     ScheduledStartTime = stream.StartTime.ToUniversalTime(),
                                     ScheduledEndTime = stream.EndTime.ToUniversalTime()
                                 }
                   };
        }

        public static Video ToVideo(this Stream stream)
        {
            return new Video
                   {
                       Id = stream.Id,
                       Snippet = new VideoSnippet
                                 {
                                     Title = stream.Title,
                                     Description = stream.Description,
                                     CategoryId = stream.Category,
                                     Tags = stream.Tags,
                                     DefaultLanguage = stream.TextLanguage,
                                     DefaultAudioLanguage = stream.AudioLanguage
                                 },
                       LiveStreamingDetails = new VideoLiveStreamingDetails
                                              {
                                                  ScheduledStartTime = stream.StartTime.ToUniversalTime(),
                                                  ScheduledEndTime = stream.EndTime.ToUniversalTime()
                                              },
                       Status = new VideoStatus()
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