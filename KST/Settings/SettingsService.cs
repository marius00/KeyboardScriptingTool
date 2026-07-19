using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using KST.Config;
using KST.Led;
using KST.LuaIntegration;

namespace KST.Settings {
    /// <summary>
    /// Responsible for emitting events with up-to-date script info
    /// Will read settings.json upon start, and continue to monitor the settings file and lua files for changes.
    /// </summary>
    internal class SettingsService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SettingsService));
        private readonly SettingsFileMonitor _fileMonitor = new SettingsFileMonitor();
        private readonly Dictionary<string, LuaEngine> _luaScriptFromFilename = new Dictionary<string, LuaEngine>();
        private readonly Dictionary<string, LuaEngine> _luaScriptFromProcess = new Dictionary<string, LuaEngine>();
        private readonly ILedProvider _ledProvider;

        public SettingsService(ILedProvider ledProvider) {
            this._ledProvider = ledProvider;
            _fileMonitor.Start();
            _fileMonitor.OnModified += _fileMonitor_OnModified;

            ParseSettingsJson(AppPaths.SettingsFile);
        }

        /// <summary>
        /// Parse the settings.json and create LUA engine entries for each process
        /// </summary>
        /// <param name="filename"></param>
        private void ParseSettingsJson(string filename) {
            var settings = SettingsReader.Load(filename);

            // Add any new entries
            foreach (var entry in settings.Entries) {
                
                if (string.IsNullOrEmpty(entry.Path) || !File.Exists(Path.Combine(AppPaths.SettingsFolder, entry.Path))) {
                    Logger.Warn($"Could not load script \"{entry.Path}\", file does not exist.");
                    continue;
                }

                if (!_luaScriptFromProcess.ContainsKey(entry.Process)) {
                    try {
                        
                        string script = File.ReadAllText(Path.Combine(AppPaths.SettingsFolder, entry.Path));

                        // May have multiple references to a single script
                        if (!_luaScriptFromFilename.ContainsKey(entry.Path.ToLower())) {
                            var engine = new LuaEngine(_ledProvider, script, entry.Path);
                            _luaScriptFromFilename[entry.Path.ToLower()] = engine;
                        }
                        _luaScriptFromProcess[entry.Process] = _luaScriptFromFilename[entry.Path.ToLower()];
                        Logger.Debug($"Configured \"{entry.Path}\" for \"{entry.Process}\"");
                    } catch (IOException ex) {
                        Logger.Warn(ex.Message, ex);
                        Logger.Warn($"Unable to read lua file {entry.Path}, script for {entry.Process} not loaded.");
                    } catch (UnauthorizedAccessException ex) {
                        Logger.Warn(ex.Message, ex);
                        Logger.Warn($"Unable to read lua file {entry.Path}, script for {entry.Process} not loaded.");
                    }
                }
            }

            // Remove any entries that has been removed from settings.json
            var processes = _luaScriptFromProcess.Keys.ToArray(); // Immuteable list
            foreach (var process in processes) {
                if (!settings.Entries.Any(m => m.Process == process)) {
                    _luaScriptFromProcess.Remove(process);
                    Logger.Debug($"Removed lua script for {process}");
                    // TODO: Remove it from _luaScriptFromFilename (Make sure it's not referenced by another process first)
                    // TODO: Dispose of the LUA engine
                }
            }
        }

        /// <summary>
        /// Listens for file changes and re-parses LUA and the settings.json when needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileMonitor_OnModified(object sender, System.IO.FileSystemEventArgs e) {
            Thread.Sleep(1500); // Very cheap hack


            // Timer will create threading issues, just use a mutex.. performance is irrelevant for this.
            /*
            var timer = new System.Windows.Forms.Timer();
            timer.Tick += TickFunction;
            timer.Interval = 1000;
            timer.Start();*/

            // TODO: File is most likely write protected at this point, queue an action to process in ~1s instead
            if (e.Name == AppPaths.SettingsFileName) {
                ParseSettingsJson(e.FullPath);
            }
            else if (e.Name.EndsWith(".lua", StringComparison.CurrentCultureIgnoreCase)) {
                if (_luaScriptFromFilename.ContainsKey(e.Name.ToLower())) {
                    try {
                        string script = File.ReadAllText(e.FullPath);
                        _luaScriptFromFilename[e.Name.ToLower()].QueueScriptChange(script);

                    }
                    catch (IOException ex) {
                        Logger.Warn($"Error updating internal LUA for {e.Name}, {ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if there are any scripts configured for the given process name
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public bool IsProcessRelevant(string processName) {
            return _luaScriptFromProcess.Keys.Any(key => key.Equals(processName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Responsible for dispatching keyboard and tick events to specified given process
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="eventType"></param>
        /// <param name="arg"></param>
        /// <param name="modifiers"></param>
        public void OnEvent(string processName, LuaEventType eventType, string arg, ushort modifiers) {
            if (!_luaScriptFromProcess.ContainsKey(processName)) {
                Logger.Error($"Attempting to dispatch event to {processName}, which does not exist.");
                return;
            }

            _luaScriptFromProcess[processName].OnEvent(eventType, arg, modifiers);
        }

        public void Dispose() {
            _fileMonitor.Dispose();

            foreach (var script in _luaScriptFromFilename.Values) {
                script.Dispose();
            }

            _luaScriptFromFilename.Clear();
            _luaScriptFromProcess.Clear();
        }
    }
}