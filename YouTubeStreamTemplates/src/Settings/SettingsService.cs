using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.Helpers;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.Settings
{
    public class SettingsService
    {
        private const string DefaultPath = @"res/Default.cfg";
        private const string Path = @"settings.cfg";
        private static SettingsService? _instance;
        private readonly Dictionary<Setting, string> _defaultSettings;
        public static SettingsService Instance => _instance ??= new SettingsService();
        public Dictionary<Setting, string> Settings { get; }

        #region Initialisation

        private SettingsService()
        {
            Settings = new Dictionary<Setting, string>();
            _defaultSettings = new Dictionary<Setting, string>();

            if (!File.Exists(DefaultPath)) throw new CorruptInstallationException(DefaultPath);
            AddAllSettings(_defaultSettings, DefaultPath);

            if (!File.Exists(Path)) File.Copy(DefaultPath, Path);
            AddAllSettings(Settings, Path);

            Directory.CreateDirectory(Settings[Setting.SavePath]);
            ImageHelper.CreateDirectories();
        }

        private void AddAllSettings(IDictionary<Setting, string> settings, string path)
        {
            var lines = File.ReadLines(path).Where(l => l.Contains('=')).Select(l => l.Split('=')).ToArray();
            var settingNames = Enum.GetValues<Setting>();

            if (path.Equals(DefaultPath) &&
                (lines.Length != settingNames.Length ||
                 settingNames.Any(n => lines.All(l => !l[0].Trim().Equals(n.ToString())))))
                throw new CorruptInstallationException(DefaultPath);

            foreach (var setting in settingNames)
            {
                var value = lines.SingleOrDefault(line => line[0].Trim().Equals(setting.ToString()));
                if (value != null && value.Length == 2) settings.Add(setting, value[1].Trim());
                else settings.Add(setting, _defaultSettings[setting].Trim());
            }
        }

        public static async Task Init()
        {
            if (string.IsNullOrWhiteSpace(Instance.Settings[Setting.SavePath]))
                throw new InvalidPathException(Instance.Settings[Setting.SavePath]);
            await TemplateService.Instance.LoadAllTemplates(Instance.Settings[Setting.SavePath]);
        }

        #endregion

        #region Public Methods

        public async Task Save()
        {
            var lines = Enum.GetValues<Setting>()
                            .Select(setting => $"{setting} = {Settings[setting]}")
                            .ToList();
            await File.WriteAllLinesAsync(Path, lines);
        }

        public async Task UpdateSetting(Setting setting, string value)
        {
            Settings[setting] = value;
            await Instance.Save();
        }

        public bool GetBool(Setting setting) { return bool.Parse(Settings[setting]); }

        #endregion
    }
}