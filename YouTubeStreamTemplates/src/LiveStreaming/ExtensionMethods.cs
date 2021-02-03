﻿using System;
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
                       ThumbnailPath = liveBroadcast.Snippet.Thumbnails.Maxres.Url,
                       //TODO Change to Actual:
                       StartTime = liveBroadcast.Snippet.ScheduledStartTime ?? DateTime.MinValue,
                       EndTime = liveBroadcast.Snippet.ScheduledEndTime ?? DateTime.MinValue
                       // StartTime = liveBroadcast.Snippet.ActualStartTime ?? DateTime.MinValue,
                       // EndTime = liveBroadcast.Snippet.ActualEndTime ?? DateTime.MinValue
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
                                     // ThumbnailPath = liveStream.ThumbnailPath, //TODO
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
                       ThumbnailPath = video.Snippet.Thumbnails.Maxres.Url,
                       Tags = (List<string>) (video.Snippet.Tags ?? new List<string>()),
                       TextLanguage = video.Snippet.DefaultLanguage,
                       AudioLanguage = video.Snippet.DefaultAudioLanguage,
                       StartTime = video.LiveStreamingDetails?.ScheduledStartTime ?? DateTime.MinValue,
                       EndTime = video.LiveStreamingDetails?.ScheduledEndTime ?? DateTime.MinValue
                   };
        }
    }
}