using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Dolittle.Booting;
using Dolittle.Types;
using System.Linq;
using Dolittle.Configuration;
using Dolittle.Configuration.Files;

namespace RaaLabs.TimeSeries
{
    /// <summary>
    /// This class will monitor all files associated with configuration classes implementing ITriggerAppRestartOnChange,
    /// and halts the application if any of these files change.
    /// </summary>
    class ConfigurationFileChangedWatcher : ICanPerformBootProcedure
    {
        private readonly ITypeFinder _typeFinder;
        private readonly FileConfigurationObjectsProvider _fileProvider;
        private static readonly string[] _searchPaths = { ".dolittle", "config", "/config", "data", ".", ".." };
        private static readonly HashSet<string> _validConfigurationExtensions = new HashSet<string> { ".json" };
        private List<FileSystemWatcher> _watchers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFinder"></param>
        /// <param name="fileProvider"></param>
        public ConfigurationFileChangedWatcher(ITypeFinder typeFinder, FileConfigurationObjectsProvider fileProvider)
        {
            _typeFinder = typeFinder;
            _fileProvider = fileProvider;
        }

        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            var configurationClasses = _typeFinder.FindMultiple<ITriggerAppRestartOnChange>();
            var filenames = configurationClasses
                .Where(_ => _fileProvider.CanProvide(_))
                .Select(clazz => (clazz, attribute: clazz.GetCustomAttribute<NameAttribute>(true)))
                .Select(cc => (cc.clazz, cc.attribute.Name))
                .ToArray();

            var pwd = Directory.GetCurrentDirectory();

            var filesToWatch = filenames.Select(_ => (_.clazz, path: FindConfigurationFilePath(_.Name))).ToArray();
            var foldersToWatch = filesToWatch
                .GroupBy(_ => Path.GetDirectoryName(_.path));

            _watchers = foldersToWatch.Select(_ => SetupWatcherForDirectory(_)).ToList();
        }

        private FileSystemWatcher SetupWatcherForDirectory(IGrouping<string, (Type clazz, string filename)> filesToWatchInFolder)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            var folder = filesToWatchInFolder.Key;
            var filesToClass = filesToWatchInFolder
                .ToDictionary(_ => _.filename, _ => _.clazz);

            watcher.Path = folder;
            watcher.Changed += (sender, args) => {
                if (!filesToClass.ContainsKey(args.FullPath)) return;

                Environment.Exit(0);
            };

            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private string FindConfigurationFilePath(string filename)
        {
            var pwd = Directory.GetCurrentDirectory();
            var dirs = _searchPaths
                .Select(_ => Path.Combine(pwd, _))
                .Where(_ => Directory.Exists(_))
                .ToArray();

            var matchedFiles = dirs
                .SelectMany(_ => Directory.EnumerateFiles(Path.Combine(pwd, _)))
                .Where(_ => Path.GetFileNameWithoutExtension(_) == filename)
                .ToArray();

            var filesWithRightExtension = matchedFiles
                .Where(_ => _validConfigurationExtensions.Contains(Path.GetExtension(_)))
                .ToArray();

            return filesWithRightExtension.First();
        }
    }
}
