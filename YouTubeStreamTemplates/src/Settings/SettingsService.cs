﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.Templates;

namespace YouTubeStreamTemplates.Settings
{
    public class SettingsService
    {
        private const string DefaultPath = @"res/Default.cfg";
        private const string Path = @"settings.cfg";
        private readonly Dictionary<Settings, string> _defaultSettings;
        public Dictionary<Settings, string> Settings { get; init; }

        #region Public Methods

        public async Task Init(TemplateService templateService)
        {
            if (string.IsNullOrWhiteSpace(Settings[YouTubeStreamTemplates.Settings.Settings.SavePath]))
                throw new InvalidPathException(Settings[YouTubeStreamTemplates.Settings.Settings.SavePath]);
            foreach (var path in Directory.GetFiles(Settings[YouTubeStreamTemplates.Settings.Settings.SavePath]))
                await templateService.LoadTemplate(path);
        }

        #endregion

        #region Initialisation

        public SettingsService()
        {
            Settings = new Dictionary<Settings, string>();
            _defaultSettings = new Dictionary<Settings, string>();

            if (!File.Exists(DefaultPath)) throw new CorruptInstallationException(DefaultPath);
            AddAllSettings(_defaultSettings, DefaultPath);

            if (!File.Exists(Path)) File.Copy(DefaultPath, Path);
            AddAllSettings(Settings, Path);

            if (!Directory.Exists(Settings[YouTubeStreamTemplates.Settings.Settings.SavePath]))
                Directory.CreateDirectory(Settings[YouTubeStreamTemplates.Settings.Settings.SavePath]);
        }

        private void AddAllSettings(IDictionary<Settings, string> settings, string path)
        {
            var lines = File.ReadLines(path).Where(l => l.Contains('=')).Select(l => l.Split('=')).ToArray();
            var settingNames = Enum.GetValues<Settings>();

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

        #endregion
    }
}