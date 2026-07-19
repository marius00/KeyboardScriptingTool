using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using KST.Config;
using KST.InputProviders;
using KST.InputProviders.Args;
using KST.Led;
using KST.LGS;
using KST.LuaIntegration;
using KST.Misc;
using KST.Settings;
using KST.UI;
using log4net;
using log4net.Config;

namespace KST {
    internal class Program {
        private static volatile bool _isRunning = true;
        private static volatile bool _isLuaThreadRunning = false;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));



        [STAThread]
        static void Main(string[] args) {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4net.config"));
            SQLitePCL.Batteries.Init();

            Guid guid = new Guid("{6E86E5D8-A735-4486-822B-2AAC10557CEB}");
            using (SingleInstance singleInstance = new SingleInstance(guid)) {
                if (singleInstance.IsFirstInstance) {
                    Run();
                }
                else {
                    
                    MessageBox.Show("KST is already running\nIt is most likely minimized to tray.", "Already running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process p = Process.GetProcessesByName("KST").FirstOrDefault();
                    if (p == null) return;
                }
            }
        }

        private static void Run() {

            Logger.Info("Starting KST..");
            Initialize();

            List<IDisposable> disposables = new List<IDisposable>();

            InterceptKeys nativeKeyboardHook = new InterceptKeys();
            disposables.Add(nativeKeyboardHook);

            LogitechLedProvider ledProvider = new LogitechLedProvider();
            disposables.Add(ledProvider);

            SettingsService settingsService = new SettingsService(ledProvider);
            disposables.Add(settingsService);

            LogitechInputProvider logitechInputProvider = new LogitechInputProvider();
            disposables.Add(logitechInputProvider);

            MouseInputProvider mouseInputProvider = new MouseInputProvider();
            disposables.Add(mouseInputProvider);
            if (!SettingsReader.Load(AppPaths.SettingsFile).NoAnonymousUsageStats) {
                disposables.Add(new UsageStatisticsReporter());
            }

            try {
                nativeKeyboardHook.Start();
                ledProvider.Start();
                logitechInputProvider.Start();
                mouseInputProvider.Start();
                IdleSleeper idleSleeper = new IdleSleeper();


                new Thread(() => {
                    _isLuaThreadRunning = true;

                    ConcurrentQueue<InputEventArg> eventQueue = new ConcurrentQueue<InputEventArg>();

                    void HandleInputEvent(object _, InputEventArg arg) {
                        var process = Win32.GetForegroundProcessName();
                        if (settingsService.IsProcessRelevant(process)) {
                            eventQueue.Enqueue(arg);
                        }
                        else {
                            Logger.Debug($"Ignoring key event, process: {process}");
                        }
                    }

                    logitechInputProvider.OnInput += HandleInputEvent;
                    InterceptKeys.OnInput += HandleInputEvent;
                    mouseInputProvider.OnInput += HandleInputEvent;


                    var lastProcessRelevant = false;
                    var lastProcess = "";
                    while (_isRunning) {
                        Thread.Sleep(1);
                        var processName = Win32.GetForegroundProcessName();
                        if (settingsService.IsProcessRelevant(processName)) {
                            if (!lastProcessRelevant) {
                                // Notify current process that it just gained focus
                                settingsService.OnEvent(processName, LuaEventType.Focus, "true", 0);

                            }

                            while (eventQueue.TryDequeue(out var arg)) {
                                var type = arg.Type == InputEventType.Down ? LuaEventType.KeyDown : LuaEventType.KeyUp;
                                settingsService.OnEvent(processName, type, arg.Key, arg.Modifiers);
                            }

                            settingsService.OnEvent(processName, LuaEventType.Tick, null, 0);
                            lastProcessRelevant = true;
                        }
                        else {
                            if (lastProcessRelevant) {
                                // Notify the last relevant process that it lost focus
                                settingsService.OnEvent(lastProcess, LuaEventType.Focus, "false", 0);
                                LogitechGSDK.LogiLedRestoreLighting();
                            }
                            lastProcessRelevant = false;
                        }

                        lastProcess = processName;
                        idleSleeper.SetStatus(lastProcessRelevant);
                    }

                    _isLuaThreadRunning = false;
                }).Start();



                ApplicationConfiguration.Initialize();
                Application.Run(new MainContainer());

                _isRunning = false;
            }
            finally {
                while (_isLuaThreadRunning) {
                    /* Waiting for lua thread to terminate before cleaning up dependencies */
                }

                foreach (var item in disposables) {
                    item.Dispose();
                }
            }

            Logger.Info("KST terminated");
        }

        /// <summary>
        /// Copy the example files to appdata if no settings exists yet
        /// </summary>
        private static void CopyInitialFiles() {
            string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
            
            Logger.Info($"Copying missing files, if any.. to {AppPaths.SettingsFolder}");

            foreach (string filename in Directory.GetFiles(appResFolder, "*.*", SearchOption.TopDirectoryOnly)) {
                if (Path.GetExtension(filename) == ".json" || Path.GetExtension(filename) == ".lua") {
                    var target = filename.Replace(appResFolder, AppPaths.SettingsFolder);
                    if (!File.Exists(target)) {
                        File.Copy(filename, filename.Replace(appResFolder, AppPaths.SettingsFolder), false);
                        Logger.Info($"Copying {filename}..");
                    }
                }
            }

            // These are native to KST, not meant to be edited by the user, so overwrite them at will.
            // The scripts generated by the wizard depend on them being up to date.
            foreach (string library in new[] { "core.lua", "keyspam.lua", "mousespam.lua", "holdbutton.lua", "autorun.lua" }) {
                string source = Path.Combine(appResFolder, library);
                if (File.Exists(source)) {
                    File.Copy(source, Path.Combine(AppPaths.SettingsFolder, library), true);
                }
            }

            LgsProfileUtil.Install();
            GHubProfileUtil.Install();

        }

        private static void Initialize() {
            Logger.Info("Source code available at https://github.com/marius00/KeyboardScriptingTool");

            CopyInitialFiles();
            InitializeUuid();

            var settings = SettingsReader.Load(AppPaths.SettingsFile);
            if (!settings.NoAnonymousUsageStats) {
                UsageStatisticsReporter.Uuid = settings.Uuid;
                UsageStatisticsReporter.UrlStats = "https://webstats.evilsoft.net/report/kst";
                UsageStatisticsReporter.ReportUsageAsync();
            }
        }

        private static void InitializeUuid() {
            var settings = SettingsReader.Load(AppPaths.SettingsFile);

            if (string.IsNullOrEmpty(settings.Uuid)) { 
                settings.Uuid = Guid.NewGuid().ToString();
                SettingsReader.Persist(AppPaths.SettingsFile, settings);
            }
        }
    }
}