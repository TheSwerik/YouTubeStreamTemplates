using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

namespace YouTubeStreamTemplates.LiveStreaming
{
    public class LiveStreamService
    {
        #region Attributes

        #region Static

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static LiveStreamService _instance = null!;
        private static SettingsService SettingsService => SettingsService.Instance;
        public static bool IsInitialized { get; private set; }

        public static LiveStreamService Instance
        {
            get
            {
                if (_instance == null) throw new Exception("INSTANCE IS NULL");
                return _instance;
            }
        }

        #endregion

        private readonly YouTubeService _youTubeService;
        private static CoolDownTimer _coolDownTimer = null!;

        public LiveStream? CurrentLiveStream { get; private set; }

        /// <summary>
        ///     First string is the Category ID
        ///     Second string is the Category Name
        /// </summary>
        public Dictionary<string, string> Category { get; }

        #endregion

        #region Initialisation

        public static async Task Init()
        {
            _coolDownTimer ??= new CoolDownTimer();
            if (_coolDownTimer.IsRunning) return;
            if (_instance != null) throw new AlreadyInitializedException(typeof(LiveStreamService));
            _coolDownTimer.StartBlock();
            var ytService = await CreateDefaultYouTubeService();
            if (ytService == null) throw new CouldNotCreateServiceException();
            _instance = new LiveStreamService(ytService);
            await _instance.InitCategories();
            _instance.AutoUpdate();
            IsInitialized = true;
            _coolDownTimer.Reset();
        }

        #region YouTubeService

        private static async Task<YouTubeService> CreateDefaultYouTubeService()
        {
            return await CreateYouTubeService(YouTubeService.Scope.YoutubeReadonly,
                                              YouTubeService.Scope.YoutubeForceSsl);
        }

        private static async Task<YouTubeService> CreateYouTubeService(params string[] scopes)
        {
            return new(new BaseClientService.Initializer
                       {
                           HttpClientInitializer = await GetCredentials(scopes),
                           ApplicationName = "YouTubeStreamTemplates",
                           ApiKey = await File.ReadAllTextAsync(@"..\..\..\..\apikey.txt")
                       });
        }

        private static async Task<UserCredential> GetCredentials(IEnumerable<string> scopes)
        {
            await using var stream = new FileStream(@"..\..\..\..\client_id.json", FileMode.Open, FileAccess.Read);
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                       GoogleClientSecrets.Load(stream).Secrets,
                       scopes,
                       "user",
                       CancellationToken.None,
                       new FileDataStore("YouTubeStreamTemplates.Dev")); //TODO
        }

        #endregion

        private LiveStreamService(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
            Category = new Dictionary<string, string>();
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

        public void Dispose() { _youTubeService.Dispose(); }

        #endregion

        #region Methods

        #region Private Methods

        private async Task<LiveBroadcast> GetCurrentBroadcast()
        {
            var request = _youTubeService.LiveBroadcasts.List("id,snippet,contentDetails,status");
            request.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
            // TODO Change back to Active:
            // request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;
            request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Upcoming;

            var response = await request.ExecuteAsync();
            if (response.Items == null || response.Items.Count <= 0) throw new NoCurrentStreamException();
            if (response.Items.Count == 1) return response.Items[0];

            // Get the latest Stream if there is more than one:
            var streams = response.Items.ToList();
            // TODO Change back to Actual (not Planned):
            // streams.Sort(LiveBroadcastComparer.ByDateDesc);
            streams.Sort(LiveBroadcastComparer.ByDateDescPlanned);
            return streams[0];
        }

        private async Task SetThumbnail(string videoId, string filePath)
        {
            Stream fileStream;
            if (filePath.StartsWith("http"))
            {
                using var webClient = new WebClient();
                fileStream = webClient.OpenRead(filePath);
            }
            else
            {
                fileStream = File.OpenRead(filePath);
            }

            if (fileStream.Length > LiveStream.MaxThumbnailSize)
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

        #endregion

        #region Public Methods

        public async Task<LiveStream> GetCurrentStream() { return (await GetCurrentBroadcast()).ToLiveStream(); }

        public async Task<LiveStream> GetCurrentStreamAsVideo()
        {
            var liveStream = await GetCurrentBroadcast();
            var videoRequest = _youTubeService.Videos.List("snippet");
            videoRequest.Id = liveStream.Id;
            var videos = await videoRequest.ExecuteAsync();
            if (videos.Items == null || videos.Items.Count <= 0) throw new NoVideoFoundException(liveStream.Id);
            return videos.Items[0].ToLiveStream();
        }

        public async Task UpdateStream(Template template)
        {
            var liveStream = await GetCurrentStream();
            var video = template.ToVideo();
            video.Id = liveStream.Id;
            var request = _youTubeService.Videos.Update(video, "id,snippet,liveStreamingDetails");

            Logger.Debug("Updating Video:\t{0} -> {1}", template.Name, liveStream.Id);
            await request.ExecuteAsync();
            Logger.Debug("Updated Video:\t{0} -> {1}", template.Name, liveStream.Id);
        }

        public async Task CheckedUpdate()
        {
            if (_coolDownTimer.IsRunning)
            {
                Logger.Debug("Not Updating Video because of CoolDown.");
                return;
            }

            _coolDownTimer.StartBlock();
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

            _coolDownTimer.ReStart();
        }

        #region Looping

        public async IAsyncEnumerable<LiveStream?> CheckForStream(int delay = 1000)
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
                while (!SettingsService.GetBool(Setting.AutoUpdate) || CurrentLiveStream == null) await Task.Delay(300);
                Logger.Debug("Checking If Should Update...");
                await CheckedUpdate();
                await Task.Delay(20000);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}