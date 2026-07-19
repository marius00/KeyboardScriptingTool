using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using KST.Config;
using Newtonsoft.Json.Linq;

namespace KST.LGS {
    /// <summary>
    /// Responsible for adding profiles to Logitech Gaming Software
    /// </summary>
    internal class LgsProfileUtil {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LgsProfileUtil));

        public static void RestartLgs() {
            try {
                Process p = Process.GetProcessesByName("LCore").FirstOrDefault();
                if (p == null) return;
                Logger.Info("Restarting Logitech Gaming Software to load configuration changes..");

                var exe = p.MainModule.FileName;
                p.Kill();
                Process.Start(exe, "/minimized");
            }
            catch (Exception ex) {
                Logger.Warn("Error restarting Logitech Gaming Software. A manual restart is required");
                Logger.Warn(ex.Message, ex);
            }
        }

        public static void Install() {
            if (!File.Exists(LogitechPaths.DefaultProfile)) {
                Logger.Warn("Could not find Logitech Gaming Software default profile installed..");
                return;
            }

            try {
                var assemblyPath = Assembly.GetEntryAssembly().Location.Replace("\\\\", "\\").ToUpperInvariant();
                var xml = File.ReadAllText(LogitechPaths.DefaultProfile);
                

                if (!xml.Contains(assemblyPath)) {
                    Logger.Info("Installing KST into the Logitech Gaming Software default profile");
                    xml = StripExistingTargetEntry(xml);
                    xml = xml.Replace("</description>", "</description>\n    " + $"<target path=\"{assemblyPath}\"/>");

                    // Backup existing config and write in the software location
                    File.Copy(LogitechPaths.DefaultProfile, Path.Combine(AppPaths.CoreFolder, LogitechPaths.DefaultProfileFilename + "-bak" + DateTimeOffset.UtcNow.Ticks));
                    File.WriteAllText(LogitechPaths.DefaultProfile, xml);
                    RestartLgs();
                }
            }
            catch (IOException ex) {
                Logger.Warn("Error installing into logitech default profile");
                Logger.Warn(ex.Message, ex);
            }
        }

        /// <summary>
        /// Primarily useful when developing with LGS installed, as you'll switch between the debug and installed version.
        /// LGS will only use one of the entries, so having multiple entries gives an inconsistent experience.
        /// </summary>
        /// <returns></returns>
        private static string StripExistingTargetEntry(string xml) {
            return Regex.Replace(xml, @"\<target.*\/>", "");
        }

    }
}