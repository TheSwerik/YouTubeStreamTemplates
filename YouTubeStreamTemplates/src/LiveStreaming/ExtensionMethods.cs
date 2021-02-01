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
                       Thumbnails = liveBroadcast.Snippet.Thumbnails,
                       StartTime = liveBroadcast.Snippet.ScheduledStartTime ?? DateTime.MinValue,
                       EndTime = liveBroadcast.Snippet.ScheduledEndTime ?? DateTime.MinValue
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
                                     // Thumbnails = liveStream.Thumbnails,
                                     Tags = liveStream.Tags
                                     // DefaultLanguage = liveStream.Language,
                                     // DefaultAudioLanguage = liveStream.Language
                                 },
                       // Localizations = liveStream.Localizations,
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
                       Thumbnails = video.Snippet.Thumbnails,
                       Tags = (List<string>) (video.Snippet.Tags ?? new List<string>()),
                       Language = video.Snippet.DefaultLanguage,
                       StartTime = video.LiveStreamingDetails?.ScheduledStartTime ?? DateTime.MinValue,
                       EndTime = video.LiveStreamingDetails?.ScheduledEndTime ?? DateTime.MinValue
                   };
        }
    }
}