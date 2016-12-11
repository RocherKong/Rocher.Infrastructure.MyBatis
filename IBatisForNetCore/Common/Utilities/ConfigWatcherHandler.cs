namespace IBatisNet.Common.Utilities
{
    using IBatisNet.Common.Logging;
    using System;
    using System.Collections;
    using System.IO;
    using System.Threading;

    public sealed class ConfigWatcherHandler
    {
        private static ArrayList _filesToWatch = new ArrayList();
        private static ArrayList _filesWatcher = new ArrayList();
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Timer _timer;
        private const int TIMEOUT_MILLISECONDS = 500;

        public ConfigWatcherHandler(TimerCallback onWhatchedFileChange, StateConfig state)
        {
            for (int i = 0; i < _filesToWatch.Count; i++)
            {
                FileInfo configFile = (FileInfo) _filesToWatch[i];
                this.AttachWatcher(configFile);
                this._timer = new Timer(onWhatchedFileChange, state, -1, -1);
            }
        }

        public static void AddFileToWatch(FileInfo configFile)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Adding file [" + Path.GetFileName(configFile.FullName) + "] to list of watched files.");
            }
            _filesToWatch.Add(configFile);
        }

        private void AttachWatcher(FileInfo configFile)
        {
            FileSystemWatcher watcher = new FileSystemWatcher {
                Path = configFile.DirectoryName,
                Filter = configFile.Name,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            watcher.Changed += new FileSystemEventHandler(this.ConfigWatcherHandler_OnChanged);
            watcher.Created += new FileSystemEventHandler(this.ConfigWatcherHandler_OnChanged);
            watcher.Deleted += new FileSystemEventHandler(this.ConfigWatcherHandler_OnChanged);
            watcher.Renamed += new RenamedEventHandler(this.ConfigWatcherHandler_OnRenamed);
            watcher.EnableRaisingEvents = true;
            _filesWatcher.Add(watcher);
        }

        public static void ClearFilesMonitored()
        {
            _filesToWatch.Clear();
            for (int i = 0; i < _filesWatcher.Count; i++)
            {
                FileSystemWatcher watcher = (FileSystemWatcher) _filesWatcher[i];
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }

        private void ConfigWatcherHandler_OnChanged(object source, FileSystemEventArgs e)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug(string.Concat(new object[] { "ConfigWatcherHandler : ", e.ChangeType, " [", e.Name, "]" }));
            }
            this._timer.Change(500, -1);
        }

        private void ConfigWatcherHandler_OnRenamed(object source, RenamedEventArgs e)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug(string.Concat(new object[] { "ConfigWatcherHandler : ", e.ChangeType, " [", e.OldName, "/", e.Name, "]" }));
            }
            this._timer.Change(500, -1);
        }
    }
}

