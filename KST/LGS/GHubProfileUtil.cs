using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using KST.Config;
using Newtonsoft.Json.Linq;

namespace KST.LGS {
    /// <summary>
    /// Responsible for adding profiles to Logitech Gaming Software
    /// </summary>
    internal class GHubProfileUtil {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GHubProfileUtil));

        /**
         * Parse the G-hub JSON format and add the exe path to all profiles
         */
        private static bool AddExeToJson(string json, string exe, out string newJson) {
            int numChanges = 0;
            var exeArray = new string[] { exe };

            JToken token = JToken.Parse(json);
            var applications = token.SelectToken("applications").SelectTokens("applications");
            foreach (var app in applications.Children()) {
                var userPath = app.SelectToken("userPaths");

                if (userPath == null) {
                    // "userPaths" does not exist
                    Logger.Warn($"Mapping missing for entry {app.SelectToken("name")}, auto adding..");
                    app.Last.AddAfterSelf(new JProperty("userPaths", exeArray));
                    numChanges++;
                }
                else {
                    // "userPaths" exists, but does not contain this exe
                    if (userPath.Children().All(m => !m.ToString().Equals(exe, StringComparison.InvariantCultureIgnoreCase))) {
                        Logger.Warn($"Mapping missing for entry {app.SelectToken("name")}, auto adding..");
                        JArray arr = userPath as JArray;
                        arr.Add(exe);
                        numChanges++;
                    }
                }
            }

            newJson = token.ToString();
            return numChanges > 0;
        }
        private static bool AllowsLedApi(string json) {
            JToken token = JToken.Parse(json);
            var integrationManagerSettings= token.SelectToken("integration_manager_settings");
            if (integrationManagerSettings == null) {
                return false;
            }

            var allowLedSdk = integrationManagerSettings.SelectToken("allowLedSdk");
            return allowLedSdk.Value<bool>();
        }

        private static string GetGHubJson() {
            using (var con = new SqliteConnection($"Data Source={LogitechPaths.GHubConfig}")) {
                con.Open();
                using (var cmd = new SqliteCommand("SELECT file FROM data", con)) {
                    using (var reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            return reader.GetString(0);
                        }
                    }
                }
            }

            return string.Empty;
        }

        private static bool SetGHubJson(string json) {
            using (var con = new SqliteConnection($"Data Source={LogitechPaths.GHubConfig}")) {
                con.Open();
                using (var cmd = new SqliteCommand("UPDATE data SET file = @json", con)) {
                    cmd.Parameters.AddWithValue("@json", json);
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
        }

        public static void Install() {

            if (!File.Exists(LogitechPaths.GHubConfig)) {
                Logger.Info("GHub profile not found, skipping G-hub configuration");
                return;
            }

            try {
                Logger.Info("Verifying G-Hub configuration");
                string assemblyPath = Assembly.GetEntryAssembly().Location.Replace("\\\\", "\\").ToUpperInvariant();

                var ghubJson = GetGHubJson();
                if (AllowsLedApi(ghubJson)) {
                    Logger.Info("G-Hub is configured to allow third party programs to use the LED API");
                } else {
                    Logger.Warn("G-Hub is not configured to allow third party programs to use the LED API, SetBacklightColor() will not function properly.");
                }

                if (!AddExeToJson(ghubJson, assemblyPath, out var json)) {
                    Logger.Info("No changes required for G-Hub");
                    return;
                }

                File.Copy(LogitechPaths.GHubConfig, LogitechPaths.GHubConfig + ".bak-" + DateTimeOffset.UtcNow.Ticks);

                try {
                    Process p = Process.GetProcessesByName("lghub").FirstOrDefault();
                    Process p2 = Process.GetProcessesByName("lghub_agent").FirstOrDefault();
                    if (p != null) {
                        // G-Hub is running.. kill it before updating, it'll write to DB before starting
                        Logger.Info("Restarting G-Hub to load configuration changes..");

                        var exe = p.MainModule.FileName;
                        p2?.Kill();
                        p.Kill();

                        if (SetGHubJson(json)) {
                            Logger.Info("Update G-Hub configuration");
                        } else {
                            Logger.Warn("Error storing new G-hub configuration");
                        }

                        Process.Start(exe, "--background");
                        
                    }
                    else {
                        // G-Hub is not running.. odd?
                        if (SetGHubJson(json)) {
                            Logger.Info("Update G-Hub configuration");
                        } else {
                            Logger.Warn("Error storing new G-hub configuration");
                        }
                    }
                } catch (Exception ex) {
                    Logger.Warn("Error restarting G-Hub. A manual restart is required");
                    Logger.Warn(ex.Message, ex);
                }

            }
            catch (Exception ex) {
                Logger.Error("Error verifying/adding G-hub profiles");
                Logger.Error(ex.Message, ex);
            }
        }
    }
}