using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using NLog;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.Settings;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.LiveStreaming
{
    public class LiveStreamService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly YouTubeService _youTubeService;

        #region Initialisation

        private static bool _isInitializing;

        public static async Task<LiveStreamService> Init()
        {
            if (_isInitializing) throw new AlreadyInitializingException(typeof(LiveStreamService));
            _isInitializing = true;
            var ytService =
                await CreateYouTubeService(YouTubeService.Scope.YoutubeReadonly, YouTubeService.Scope.YoutubeForceSsl);
            if (ytService == null) throw new CouldNotCreateServiceException();
            var liveStreamService = new LiveStreamService(ytService);
            await liveStreamService.InitCategories();
            _isInitializing = false;
            return liveStreamService;
        }

        private LiveStreamService(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
            Category = new Dictionary<string, string>();
        }

        ~LiveStreamService() { _youTubeService.Dispose(); }

        private static async Task<UserCredential> GetCredentials(IEnumerable<string> scopes)
        {
            await using var stream = new FileStream(@"..\..\..\..\client_id.json", FileMode.Open, FileAccess.Read);
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                       GoogleClientSecrets.Load(stream).Secrets,
                       scopes,
                       "user",
                       CancellationToken.None,
                       new FileDataStore("YouTube.Test2"));
        }

        private static async Task<YouTubeService> CreateYouTubeService(params string[] scopes)
        {
            return new(new BaseClientService.Initializer
                       {
                           HttpClientInitializer = await GetCredentials(scopes),
                           ApplicationName = "YouTube Sample",
                           ApiKey = await File.ReadAllTextAsync(@"..\..\..\..\apikey.txt")
                       });
        }

        private async Task InitCategories()
        {
            var request = _youTubeService.VideoCategories.List("snippet");
            request.RegionCode = "DE";
            request.Hl = bool.Parse(SettingsService.Instance.Settings[Settings.Settings.ForceEnglish])
                             ? "en_US"
                             : "de_DE";
            var result = await request.ExecuteAsync();
            foreach (var videoCategory in result.Items.Where(v => v.Snippet.Assignable == true))
                Category.Add(videoCategory.Id, videoCategory.Snippet.Title);

            Logger.Debug(string.Join(", ", Category));
        }

        #endregion

        #region Public Methods

        public async Task<LiveStream> GetCurrentStream()
        {
            var request = _youTubeService.LiveBroadcasts.List("id,snippet,contentDetails,status");
            request.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
            // request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;
            request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Upcoming;

            var response = await request.ExecuteAsync();
            if (response.Items == null || response.Items.Count <= 0) throw new NoCurrentStreamException();
            if (response.Items.Count == 1) return response.Items[0].ToLiveStream();

            // Get the latest Stream if there is more than one:
            var streams = response.Items.Select(s => s.ToLiveStream()).ToList();
            streams.Sort(LiveStreamComparer.ByDateDesc);
            Logger.Debug("all streams:\n{0}",
                         string.Join("\n", streams.Select(s => s.Id + " " + s.Title + " " + s.StartTime)));
            return streams[0];
        }

        public async Task UpdateStream(Template template)
        {
            var liveStream = await GetCurrentStream();
            var video = template.ToVideo();
            video.Id = liveStream.Id;
            var request = _youTubeService.Videos.Update(video, "snippet");

            var response = await request.ExecuteAsync();
            if (response == null) return;
            Logger.Debug(response.Snippet.Title + " - " + response.Snippet.Title);
        }

        /// <summary>
        ///     First string is the Category ID
        ///     Second string is the Category Name
        /// </summary>
        public Dictionary<string, string> Category { get; }

        #endregion
    }
}