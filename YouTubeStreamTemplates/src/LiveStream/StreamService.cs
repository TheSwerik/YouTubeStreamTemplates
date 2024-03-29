﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using NLog;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.Helpers;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.LiveStream
{
    public class StreamService
    {
        #region Attributes

        #region Static

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static StreamService _instance = null!;
        private static SettingsService SettingsService => SettingsService.Instance;
        public static bool IsInitialized { get; private set; }

        public static StreamService Instance
        {
            get
            {
                if (_instance == null) throw new StreamServiceNotInitializedException();
                return _instance;
            }
        }

        #endregion

        private readonly YouTubeService _youTubeService;
        private static readonly CoolDownTimer CoolDownTimer = new();

        public Stream? CurrentLiveStream { get; private set; }

        /// <summary>
        ///     First string is the Category ID
        ///     Second string is the Category Name
        /// </summary>
        public Dictionary<string, string> Category { get; }

        public List<Playlist> Playlists { get; }

        #endregion

        #region Initialisation

        public static async Task Init(string? clientId = null, string? clientSecret = null)
        {
            if (CoolDownTimer.IsRunning) return;
            if (_instance != null) throw new AlreadyInitializedException(typeof(StreamService));
            CoolDownTimer.StartBlock();
            var ytService = await CreateDefaultYouTubeService(clientId, clientSecret);
            if (ytService == null) throw new CouldNotCreateServiceException();
            _instance = new StreamService(ytService);
            await _instance.InitCategories();
            await _instance.InitPlaylists();
#pragma warning disable 4014
            _instance.AutoUpdate();
#pragma warning restore 4014
            IsInitialized = true;
            CoolDownTimer.Reset();
        }

        #region YouTubeService

        private static async Task<YouTubeService> CreateDefaultYouTubeService(string? clientId, string? clientSecret)
        {
            return await CreateYouTubeService(clientId, clientSecret,
                                              YouTubeService.Scope.YoutubeReadonly,
                                              YouTubeService.Scope.YoutubeForceSsl);
        }

        private static async Task<YouTubeService> CreateYouTubeService(string? clientId, string? clientSecret,
                                                                       params string[] scopes)
        {
            return new YouTubeService(new BaseClientService.Initializer
                                      {
                                          HttpClientInitializer = await GetCredentials(clientId, clientSecret, scopes),
                                          ApplicationName = "YouTubeStreamTemplates"
                                      });
        }

        private static async Task<UserCredential> GetCredentials(string? clientId, string? clientSecret,
                                                                 IEnumerable<string> scopes)
        {
            ClientSecrets secrets;
            if (File.Exists("client_id.json"))
            {
                await using var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read);
                secrets = (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets;
            }
            else
            {
                if (clientId == null || clientSecret == null) throw new NoClientIdOrSecretException();
                secrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret };
            }

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                       secrets,
                       scopes,
                       "user",
                       CancellationToken.None,
                       new FileDataStore("YouTubeStreamTemplates")
                   );
        }

        #endregion

        private StreamService(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
            Category = new Dictionary<string, string>();
            Playlists = new List<Playlist>();
        }

        private async Task InitCategories()
        {
            var request = _youTubeService.VideoCategories.List("snippet");
            request.RegionCode = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            request.Hl = SettingsService.GetBool(Setting.ForceEnglish)
                             ? CultureInfo.GetCultureInfo("en_us").IetfLanguageTag
                             : CultureInfo.InstalledUICulture.IetfLanguageTag;
            var result = await request.ExecuteAsync();
            foreach (var videoCategory in result.Items.Where(v => v.Snippet.Assignable == true))
                Category.Add(videoCategory.Id, videoCategory.Snippet.Title);

            Logger.Debug("Found Categories: {0}", string.Join(", ", Category));
        }

        private async Task InitPlaylists()
        {
            var playlists = new Dictionary<string, string>();
            var page = "";
            while (true)
            {
                var request = _youTubeService.Playlists.List("snippet");
                request.Mine = true;
                request.PageToken = page;
                request.MaxResults = 50;
                request.Hl = SettingsService.GetBool(Setting.ForceEnglish)
                                 ? CultureInfo.GetCultureInfo("en_us").IetfLanguageTag
                                 : CultureInfo.InstalledUICulture.IetfLanguageTag;
                var result = await request.ExecuteAsync();
                foreach (var playlist in result.Items) playlists.Add(playlist.Id, playlist.Snippet.Title);
                if (result.NextPageToken == null) break;
                page = result.NextPageToken;
            }

            Logger.Debug("Found {0} Playlists. Creating Playlist-Objects...", playlists.Count);
            Playlists.Clear();
            foreach (var (id, title) in playlists)
            {
                var videos = new List<PlaylistItem>();
                page = "";
                while (true)
                {
                    var listRequest = _youTubeService.PlaylistItems.List("id,snippet");
                    listRequest.PageToken = page;
                    listRequest.MaxResults = 50;
                    listRequest.PlaylistId = id;
                    var listResult = await listRequest.ExecuteAsync();
                    videos.AddRange(listResult.Items);
                    if (listResult.NextPageToken == null) break;
                    page = listResult.NextPageToken;
                }

                Playlists.Add(new Playlist(id, title, videos.ToDistinctDictionary()));
            }

            Logger.Debug("Found Playlists: {0}", string.Join(", ", Playlists));
        }

        public void Dispose() { _youTubeService.Dispose(); }

        #endregion

        #region Methods

        #region Private Methods

        private async Task<LiveBroadcast> GetCurrentBroadcast()
        {
            var request = _youTubeService.LiveBroadcasts.List("id,snippet,contentDetails,status");
            request.MaxResults = 50; // should never be > 50, should realistically never be > 3
            request.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
            request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;

            var response = await request.ExecuteAsync();
            if (response.Items == null || response.Items.Count <= 0) throw new NoCurrentStreamException();
            if (response.Items.Count == 1) return response.Items[0];

            // Get the latest Stream if there is more than one:
            var streams = response.Items.ToList();
            streams.Sort(LiveBroadcastComparer.ByDateDesc);
            return streams[0];
        }

        private async Task SetThumbnail(string videoId, string filePath)
        {
            if (filePath.StartsWith("http")) filePath = await ImageHelper.GetImagePathAsync(filePath, false, videoId);
            await using var fileStream = File.OpenRead(filePath);

            if (fileStream.Length > Stream.MaxThumbnailSize)
                throw new ThumbnailTooLargeException(fileStream.Length);

            Logger.Debug($"Changing Thumbnail for {videoId} to {filePath}...");
            var request =
                _youTubeService.Thumbnails.Set(videoId, fileStream, ExtensionGetter.GetJsonExtension(filePath));
            var response = await request.UploadAsync();
            await fileStream.DisposeAsync();
            Logger.Debug($"Changed Thumbnail for {videoId} to {filePath}.");

            if (response.Exception != null)
                throw new YouTubeStreamTemplateException($"Error happened:\n{response.Exception.Message}");
        }

        private async Task AddVideoToPlaylist(string videoId, string playlistId)
        {
            Logger.Debug($"Adding {videoId} to PLaylist: {playlistId}...");
            var newPlaylistItem = new PlaylistItem
                                  {
                                      Snippet = new PlaylistItemSnippet
                                                {
                                                    PlaylistId = playlistId,
                                                    ResourceId = new ResourceId
                                                                 {
                                                                     Kind = "youtube#video",
                                                                     VideoId = videoId
                                                                 }
                                                }
                                  };
            await _youTubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
            Logger.Debug($"Added {videoId} to PLaylist: {playlistId}.");
        }

        private async Task RemoveVideoFromPlaylist(string playlistItemId, string playlistId)
        {
            Logger.Debug($"Removing {playlistItemId} from PLaylist: {playlistId}...");
            await _youTubeService.PlaylistItems.Delete(playlistItemId).ExecuteAsync();
            Logger.Debug($"Removed {playlistItemId} from PLaylist: {playlistId}.");
        }

        #endregion

        #region Public Methods

        public async Task<Stream> GetCurrentStream() { return (await GetCurrentBroadcast()).ToStream(); }

        public async Task<Stream> GetCurrentStreamAsVideo()
        {
            var liveStream = await GetCurrentBroadcast();
            var videoRequest = _youTubeService.Videos.List("snippet");
            videoRequest.Id = liveStream.Id;
            var videos = await videoRequest.ExecuteAsync();
            if (videos.Items == null || videos.Items.Count <= 0) throw new NoVideoFoundException(liveStream.Id);
            var result = videos.Items[0].ToStream();
            result.PlaylistIDs = Playlists.Where(p => p.Videos.ContainsKey(result.Id)).Select(p => p.Id).ToList();
            return result;
        }

        public async Task UpdateStream(Template template)
        {
            var liveStream = await GetCurrentStream();
            var video = template.ToVideo();
            video.Id = liveStream.Id;
            video.Status.SelfDeclaredMadeForKids = false; //TODO add setting
            video.Status.PrivacyStatus = "public"; //TODO add setting
            var request = _youTubeService.Videos.Update(video, "id,snippet,liveStreamingDetails,status");

            Logger.Debug("Updating Video:\t{0} -> {1}", template.Name, liveStream.Id);
            await request.ExecuteAsync();
            Logger.Debug("Updated Video:\t{0} -> {1}", template.Name, liveStream.Id);
        }

        public async Task CheckedUpdate()
        {
            if (CoolDownTimer.IsRunning)
            {
                Logger.Debug("Not Updating Video because of CoolDown.");
                return;
            }

            CoolDownTimer.Start(20000);
            if (CurrentLiveStream == null) return;
            var stream = CurrentLiveStream;
            var onlySaved = SettingsService.GetBool(Setting.OnlyUpdateSavedTemplates);
            var template = (onlySaved
                                ? TemplateService.Instance.GetCurrentTemplate
                                : TemplateService.Instance.GetEditedTemplate).Invoke();
            if (stream.HasDifference(template)) await UpdateStream(template);
            if (!template.Thumbnail.HasSameResult(await ImageHelper.GetStreamThumbnailBytesAsync(stream.Id)))
            {
                await SetThumbnail(stream.Id, template.Thumbnail.Source);
                template.Thumbnail.Result = await ImageHelper.GetStreamThumbnailBytesAsync(stream.Id);
            }

            if (template.PlaylistIDs.Count != stream.PlaylistIDs.Count ||
                template.PlaylistIDs.Any(p => !stream.PlaylistIDs.Contains(p)))
            {
                foreach (var playlistId in template.PlaylistIDs.Where(p => !stream.PlaylistIDs.Contains(p)))
                    await AddVideoToPlaylist(stream.Id, playlistId);
                foreach (var playlistId in stream.PlaylistIDs.Where(p => !template.PlaylistIDs.Contains(p)))
                    await RemoveVideoFromPlaylist(Playlists.Where(p => p.Id.Equals(playlistId))
                                                           .Select(p => p.Videos[stream.Id]).Single(),
                                                  playlistId);
                await InitPlaylists();
            }

            CoolDownTimer.Restart();
        }

        #region Looping

        public async IAsyncEnumerable<Stream?> CheckForStream(int delay = 1000)
        {
            var longDelay = delay * 20;
            while (true)
            {
                await Task.Delay(CurrentLiveStream == null ? delay : longDelay);
                try
                {
                    var stream = await GetCurrentStreamAsVideo();
                    if (CurrentLiveStream == null)
                        Logger.Debug("Stream Detected:\tid: {0} \tTitle: {1}", stream.Id, stream.Title);
                    CurrentLiveStream = stream;
                }
                catch (NoCurrentStreamException)
                {
                    Logger.Debug("Not currently streaming...");
                    CurrentLiveStream = null;
                }

                yield return CurrentLiveStream;
            }
        }

        private async Task AutoUpdate()
        {
            while (true)
            {
                Logger.Debug("Checking If AutoUpdate is true...");
                while (!SettingsService.GetBool(Setting.AutoUpdate)) await Task.Delay(300);
                Logger.Debug("Checking If you are live...");
                while (CurrentLiveStream == null) await Task.Delay(300);
                if (!SettingsService.GetBool(Setting.AutoUpdate)) continue;

                await CheckedUpdate();
                await Task.Delay(20000);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}