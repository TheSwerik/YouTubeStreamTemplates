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
        private static SettingsService? _instance;
        private readonly Dictionary<Setting, string> _defaultSettings;

        private readonly string _path =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\YouTubeStreamTemplates\settings.cfg";

        public static SettingsService Instance => _instance ??= new SettingsService();
        public Dictionary<Setting, string> Settings { get; }

        #region Initialisation

        private SettingsService()
        {
            Settings = new Dictionary<Setting, string>();
            _defaultSettings = new Dictionary<Setting, string>
                               {
                                   {
                                       Setting.SavePath,
                                       $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/YouTubeStreamTemplates/Templates"
                                   },
                                   { Setting.ForceEnglish, "false" },
                                   { Setting.CurrentTemplate, "" },
                                   { Setting.OnlyUpdateSavedTemplates, "false" },
                                   { Setting.AutoUpdate, "false" }
                               };

            Directory.CreateDirectory(_defaultSettings[Setting.SavePath]);
            if (!File.Exists(_path))
            {
                var lines = new List<string>(5);
                lines.AddRange(_defaultSettings.Select(pair => pair.Key + " = " + pair.Value));
                File.WriteAllText(_path, string.Join("\n", lines));
            }

            AddAllSettings(Settings, _path);

            Directory.CreateDirectory(Settings[Setting.SavePath]);
            ImageHelper.CreateDirectories();
        }

        private void AddAllSettings(IDictionary<Setting, string> settings, string path)
        {
            var lines = File.ReadLines(path).Where(l => l.Contains('=')).Select(l => l.Split('=')).ToArray();
            var settingNames = Enum.GetValues<Setting>();

            foreach (var setting in settingNames)
            {
                var value = lines.SingleOrDefault(line => line[0].Trim().Equals(setting.ToString()));
                settings.Add(setting, value is { Length: 2 } ? value[1].Trim() : _defaultSettings[setting].Trim());
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

        private async Task Save()
        {
            var lines = Enum.GetValues<Setting>()
                            .Select(setting => $"{setting} = {Settings[setting]}")
                            .ToList();
            await File.WriteAllLinesAsync(_path, lines);
        }

        public async Task UpdateSetting(Setting setting, string value)
        {
            Settings[setting] = value;
            await Save();
        }

        public bool GetBool(Setting setting) { return bool.Parse(Settings[setting]); }

        #endregion
    }
}