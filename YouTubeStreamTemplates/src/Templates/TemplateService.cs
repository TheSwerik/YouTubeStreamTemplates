using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using YouTubeStreamTemplates.Exceptions;
using YouTubeStreamTemplates.Settings;

namespace YouTubeStreamTemplates.Templates
{
    public class TemplateService
    {
        private const string LineSeparator = "■\\n■";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private List<string> _templatePaths;

        public TemplateService()
        {
            _templatePaths = new List<string>();
            Templates = new List<Template>();
        }

        public List<Template> Templates { get; }

        #region Helper Methods

        private async Task<Template> GetTemplateFromPath(string path)
        {
            var id = Path.GetFileNameWithoutExtension(path);
            var lines = await File.ReadAllLinesAsync(path);

            return Templates.FirstOrDefault(t => t.Id.Equals(id))
                   ?? new Template(lines.GetValue("Name"))
                      {
                          Id = id,
                          Name = lines.GetValue("Name"),
                          Title = lines.GetValue("Title"),
                          Category = lines.GetValue("Category"),
                          Description = lines.GetValue("Description").Replace(LineSeparator, "\n"),
                          StartTime = DateTime.Parse(lines.GetValue("StartTime")),
                          EndTime = DateTime.Parse(lines.GetValue("EndTime")),
                          TextLanguage = lines.GetValue("TextLanguage"),
                          AudioLanguage = lines.GetValue("AudioLanguage"),
                          Tags = lines.GetValue("Tags").Split(",").ToList(),
                          ThumbnailsPath = lines.GetValue("ThumbnailsPath")
                      };
        }

        #endregion

        #region Public Methods

        // public async Task<List<string>> GetTemplatePaths()
        // {
        //     var openFileDialog = new OpenFileDialog();
        //     openFileDialog.Filter = "Template files (*.tlpt)|*.tlpt|All files (*.*)|*.*";
        //     if (openFileDialog.ShowDialog()) return openFileDialog.FileName;
        //     throw new Exception();
        // }
        public async Task LoadAllTemplates(string folderPath)
        {
            foreach (var filePath in Directory.EnumerateFiles(folderPath)) await LoadTemplate(filePath);
            Logger.Debug(Templates.Count);
        }

        public async Task<Template> LoadTemplate(string path)
        {
            path = path.Trim();
            if (string.IsNullOrWhiteSpace(path)) throw new EmptyPathException();
            if (!File.Exists(path)) throw new InvalidPathException(path);

            var id = Path.GetFileNameWithoutExtension(path);
            var template = Templates.FirstOrDefault(t => t.Id.Equals(id));
            if (template != null) Templates.Remove(template);

            template = await GetTemplateFromPath(path);
            Templates.Add(template);
            return template;
        }

        public async Task SaveTemplate(Template template)
        {
            var path = SettingsService.Instance.Settings[Settings.Settings.SavePath] + $"/{template.Id}.tlpt";
            await using var file = File.CreateText(path);

            await file.WriteLineAsync($"Name: {template.Name}");
            await file.WriteLineAsync($"Title: {template.Title}");
            await file.WriteLineAsync($"Category: {template.Category}");
            await file.WriteLineAsync($"Description: {template.Description.Replace("\n", LineSeparator)}");
            await file.WriteLineAsync($"StartTime: {template.StartTime}");
            await file.WriteLineAsync($"EndTime: {template.EndTime}");
            await file.WriteLineAsync($"TextLanguage: {template.TextLanguage}");
            await file.WriteLineAsync($"AudioLanguage: {template.AudioLanguage}");
            await file.WriteLineAsync($"Tags: {string.Join(',', template.Tags)}");
            await file.WriteLineAsync($"ThumbnailsPath: {template.ThumbnailsPath}");
        }

        #endregion
    }
}