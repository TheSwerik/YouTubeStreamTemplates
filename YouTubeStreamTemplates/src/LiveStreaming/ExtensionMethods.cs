using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeStreamTemplates.LiveStreaming
{
    public static class ExtensionMethods
    {
        public static LiveStream ToLiveStream(this LiveBroadcast liveBroadcast)
        {
            return new()
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
            return new()
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
            return new()
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
                                              }
                   };
        }

        public static LiveStream ToLiveStream(this Video video)
        {
            return new()
                   {
                       Id = video.Id,
                       Title = video.Snippet.Title,
                       Description = video.Snippet.Description,
                       Category = video.Snippet.CategoryId,
                       Tags = (List<string>) (video.Snippet.Tags ?? new List<string>()),
                       TextLanguage = video.Snippet.DefaultLanguage,
                       AudioLanguage = video.Snippet.DefaultAudioLanguage,
                       StartTime = video.LiveStreamingDetails?.ScheduledStartTime ?? DateTime.MinValue,
                       EndTime = video.LiveStreamingDetails?.ScheduledEndTime ?? DateTime.MinValue
                   };
        }
    }
}