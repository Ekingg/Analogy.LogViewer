﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Analogy.Interfaces;
using Analogy.Properties;
using Newtonsoft.Json;

namespace Analogy
{
    public class UserSettingsManager
    {
        private readonly string splitter = "*#*#*#";

        private static readonly Lazy<UserSettingsManager> _instance =
            new Lazy<UserSettingsManager>(() => new UserSettingsManager());

        public string ApplicationSkinName { get; set; }
        public static UserSettingsManager UserSettings { get; } = _instance.Value;
        public bool SaveExcludeTexts { get; set; }
        public string IncludeText { get; set; }
        public string ExcludedText { get; set; }
        public string ExcludedModules { get; set; }
        public string ExcludedSource { get; set; }
        public string IncludedModule { get; set; }
        public string IncludedSource { get; set; }
        public List<(Guid ID, string FileName)> RecentFiles { get; set; }
        public bool ShowHistoryOfClearedMessages { get; set; }
        public int RecentFilesCount { get; set; }
        public bool EnableUserStatistics { get; set; }
        public TimeSpan AnalogyRunningTime { get; set; }
        public uint AnalogyLaunches { get; set; }
        public uint AnalogyOpenedFiles { get; set; }
        public bool EnableFileCaching { get; set; }
        public string DisplayRunningTime => $"{AnalogyRunningTime:dd\\.hh\\:mm\\:ss} days";
        public bool LoadExtensionsOnStartup { get; set; }
        public List<Guid> StartupExtensions { get; set; }
        public bool StartupRibbonMinimized { get; set; }
        public bool StartupErrorLogLevel { get; set; }
        public bool PagingEnabled { get; set; }
        public int PagingSize { get; set; }
        public bool ShowChangeLogAtStartUp { get; set; }
        public float FontSize { get; set; }
        public bool SearchAlsoInSourceAndModule { get; set; }
        public string InitialSelectedDataProvider { get; set; } = "D3047F5D-CFEB-4A69-8F10-AE5F4D3F2D04";
        public bool IdleMode { get; set; }
        public int IdleTimeMinutes { get; set; }

        public List<string> EventLogs { get; set; }

        public List<Guid> AutoStartDataProviders { get; set; }
        public bool AutoScrollToLastMessage { get; set; }
        public LogParserSettings NLogSettings { get; set; }
        public UserSettingsManager()
        {
            Load();
        }

        public void Load()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            ApplicationSkinName = Settings.Default.ApplicationSkinName;
            EnableUserStatistics = Settings.Default.EnableUserStatistics;
            AnalogyRunningTime = Settings.Default.AnalogyRunningTime;
            AnalogyLaunches = Settings.Default.AnalogyLaunchesCount;
            AnalogyOpenedFiles = Settings.Default.OpenFilesCount;
            ExcludedText = Settings.Default.ExcludedText;
            ExcludedSource = Settings.Default.ExcludedSource;
            ExcludedModules = Settings.Default.ExcludedModules;
            ShowHistoryOfClearedMessages = Settings.Default.ShowHistoryClearedMessages;
            SaveExcludeTexts = Settings.Default.SaveExcludeTexts;
            //SimpleMode = Properties.Settings.Default.SimpleMode;
            RecentFiles =
                Settings.Default.RecentFiles
                    .Split(new[] { "##" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(itm =>
                    {
                        var items = itm.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (Guid.TryParse(items[0], out Guid id))
                        {
                            return (id, items.Last());
                        }

                        return (Guid.NewGuid(), items.Last());
                    }).ToList();
            RecentFilesCount = Settings.Default.RecentFilesCount;
            EnableFileCaching = Settings.Default.EnableFileCaching;
            LoadExtensionsOnStartup = Settings.Default.LoadExtensionsOnStartup;
            StartupExtensions = Settings.Default.StartupExtensions
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList();
            StartupRibbonMinimized = Settings.Default.StartupRibbonMinimized;
            StartupErrorLogLevel = Settings.Default.StartupErrorLogLevel;
            PagingEnabled = Settings.Default.PagingEnabled;
            PagingSize = Settings.Default.PagingSize;
            ShowChangeLogAtStartUp = Settings.Default.ShowChangeLogAtStartUp;
            FontSize = Settings.Default.FontSize;
            IncludeText = Settings.Default.IncludeText;
            SearchAlsoInSourceAndModule = Settings.Default.SearchAlsoInSourceAndModule;
            IdleMode = Settings.Default.IdleMode;
            IdleTimeMinutes = Settings.Default.IdleTimeMinutes;
            EventLogs = Settings.Default.WindowsEventLogs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            AutoStartDataProviders = Settings.Default.AutoStartDataProviders
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList();
            AutoScrollToLastMessage = Settings.Default.AutoScrollToLastMessage;

            try
            {

                NLogSettings = string.IsNullOrEmpty(Settings.Default.NlogSettings) ?
                    new LogParserSettings() :
                    JsonConvert.DeserializeObject<LogParserSettings>(Settings.Default.NlogSettings);
            }
            catch
            {
                NLogSettings = new LogParserSettings();
            }


        }


        public void Save()
        {
            Settings.Default.ApplicationSkinName = ApplicationSkinName;
            Settings.Default.EnableUserStatistics = EnableUserStatistics;
            Settings.Default.AnalogyRunningTime = AnalogyRunningTime;
            Settings.Default.AnalogyLaunchesCount = AnalogyLaunches;
            Settings.Default.OpenFilesCount = AnalogyOpenedFiles;
            Settings.Default.ExcludedText = ExcludedText;
            Settings.Default.ExcludedSource = ExcludedSource;
            Settings.Default.ExcludedModules = ExcludedModules;
            Settings.Default.ShowHistoryClearedMessages = ShowHistoryOfClearedMessages;
            Settings.Default.SaveExcludeTexts = SaveExcludeTexts;
            Settings.Default.RecentFilesCount = RecentFilesCount;
            Settings.Default.RecentFiles = string.Join("##",
                RecentFiles.Take(RecentFilesCount).Select(i => $"{i.ID},{i.FileName}"));
            //Properties.Settings.Default.SimpleMode = SimpleMode;
            Settings.Default.EnableFileCaching = EnableFileCaching;
            Settings.Default.LoadExtensionsOnStartup = LoadExtensionsOnStartup;
            Settings.Default.StartupExtensions = string.Join(",", StartupExtensions);
            Settings.Default.StartupRibbonMinimized = StartupRibbonMinimized;
            Settings.Default.StartupErrorLogLevel = StartupErrorLogLevel;
            Settings.Default.PagingEnabled = PagingEnabled;
            Settings.Default.PagingSize = PagingSize;
            Settings.Default.ShowChangeLogAtStartUp = false;
            Settings.Default.FontSize = FontSize;
            Settings.Default.IncludeText = IncludeText;
            Settings.Default.SearchAlsoInSourceAndModule = SearchAlsoInSourceAndModule;
            Settings.Default.IdleMode = IdleMode;
            Settings.Default.IdleTimeMinutes = IdleTimeMinutes;
            Settings.Default.WindowsEventLogs = string.Join(",", EventLogs);
            Settings.Default.AutoStartDataProviders = string.Join(",", AutoStartDataProviders);
            Settings.Default.AutoScrollToLastMessage = AutoScrollToLastMessage;
            try
            {
                Settings.Default.NlogSettings = JsonConvert.SerializeObject(NLogSettings);
            }
            catch
            {
                Settings.Default.NlogSettings = string.Empty;
            }
            Settings.Default.Save();

        }

        public void AddIncludeEntry(string text)
        {
            if (!IncludeText.Contains(text))
                IncludeText += splitter + text;
        }

        public void AddToRecentFiles(Guid iD, string file)
        {
            AnalogyOpenedFiles += 1;
            if (!RecentFiles.Contains((iD, file)))
                RecentFiles.Insert(0, (iD, file));
        }

        public void ClearStatistics()
        {
            AnalogyRunningTime = TimeSpan.FromSeconds(0);
            AnalogyLaunches = 0;
            AnalogyOpenedFiles = 0;
        }

        public void UpdateRunningTime() => AnalogyRunningTime =
            AnalogyRunningTime.Add(DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime));

        public void IncreaseNumberOfLaunches() => AnalogyLaunches++;

        public List<string> ExcludedEntries =>
            ExcludedText.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries).Take(10).ToList();

        public List<string> IncludeEntries => IncludeText.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries)
            .Take(10).ToList();

    }

    public class LogParserSettings
    {
        public List<string> SupportedFilesExtensions { get; set; }
        public bool IsConfigured { get; set; }
        public string Splitter { get; set; }
        public string Layout { get; set; }
        public Dictionary<int, AnalogyLogMessagePropertyName> Maps { get; set; }


        public LogParserSettings()
        {
            IsConfigured = false;
            Layout = string.Empty;
            Splitter = string.Empty;
            Maps = new Dictionary<int, AnalogyLogMessagePropertyName>();
            SupportedFilesExtensions = new List<string>();
        }

        public void Configure(string layout, string splitter, List<string> supportedFilesExtension, Dictionary<int, AnalogyLogMessagePropertyName> maps)
        {
            Layout = layout;
            Splitter = splitter;
            SupportedFilesExtensions = supportedFilesExtension;
            Maps = maps ?? new Dictionary<int, AnalogyLogMessagePropertyName>();
            IsConfigured = true;
        }
        public void AddMap(int index, AnalogyLogMessagePropertyName name) => Maps.Add(index, name);

        public bool CanOpenFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            return SupportedFilesExtensions.Any(s =>s.EndsWith(Path.GetExtension(filename),StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
