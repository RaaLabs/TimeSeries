using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Dolittle.Booting;
using Dolittle.Types;
using System.Linq;
using System.Threading;
using Dolittle.Configuration;
using Dolittle.Configuration.Files;
using Dolittle.Logging;

namespace RaaLabs.TimeSeries.Modules
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
        private readonly ILogger _logger;
        //private List<PhysicalFileProvider> _watchers;
        private Thread _watcherThread;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFinder"></param>
        /// <param name="logger"></param>
        /// <param name="fileProvider"></param>
        public ConfigurationFileChangedWatcher(ITypeFinder typeFinder, ILogger logger, FileConfigurationObjectsProvider fileProvider)
        {
            _typeFinder = typeFinder;
            _logger = logger;
            _fileProvider = fileProvider;
        }

        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            var configurationClasses = _typeFinder.FindMultiple<RaaLabs.TimeSeries.Modules.ITriggerAppRestartOnChange>();
            var filenames = configurationClasses
                .Where(_ => _fileProvider.CanProvide(_))
                .Select(clazz => (clazz, attribute: clazz.GetCustomAttribute<NameAttribute>(true)))
                .Select(cc => (cc.clazz, cc.attribute.Name))
                .ToArray();

            var pwd = Directory.GetCurrentDirectory();

            var filesToWatch = filenames.Select(_ => (_.clazz, path: FindConfigurationFilePath(_.Name))).ToArray();

            // Neither FileSystemWatcher nor PhysicalFileProvider have worked platform-independently at watching files asynchronously,
            // not even with DOTNET_USE_POLLING_FILE_WATCHER=1. Because of this, we will watch all configuration files manually instead.
            _watcherThread = new Thread(_ =>
            {
                var filesChangedAt = filesToWatch.ToDictionary(file => file.path, file => File.GetLastWriteTimeUtc(file.path));

                while (true)
                {
                    if (filesChangedAt.Any(file => File.GetLastWriteTimeUtc(file.Key) != file.Value))
                    {
                        _logger.Information($"Configuration changed, restarting application...");
                        Environment.Exit(0);
                    }
                    Thread.Sleep(5_000);
                }
            });
            _watcherThread.Start();
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
