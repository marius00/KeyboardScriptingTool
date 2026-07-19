using System;
using System.IO;
using log4net;
using KST.Config;

namespace KST.Led {
    /// <summary>
    /// Selects and initializes the appropriate <see cref="ILedProvider"/>.
    ///
    /// Selection order for "auto":
    ///   1. If the OpenRGB SDK server is reachable, use it (preferred, works without Logitech software).
    ///   2. Otherwise, if Logitech Gaming Software or G-Hub is installed, use the Logitech SDK.
    ///   3. Otherwise, fall back to the Logitech provider (no-op if the SDK is unavailable).
    ///
    /// The selection can be forced through the "LedBackend" setting ("openrgb" | "logitech" | "auto").
    /// </summary>
    internal static class LedProviderFactory {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LedProviderFactory));

        public static ILedProvider Create(string backend) {
            backend = (backend ?? "auto").Trim().ToLowerInvariant();

            switch (backend) {
                case "openrgb":
                    Logger.Info("LED backend forced to OpenRGB via settings");
                    return new OpenRgbLedProvider();

                case "logitech":
                    Logger.Info("LED backend forced to Logitech via settings");
                    return new LogitechLedProvider();

                default:
                    return CreateAuto();
            }
        }

        private static ILedProvider CreateAuto() {
            var openRgb = new OpenRgbLedProvider();
            if (openRgb.Start()) {
                Logger.Info("Using OpenRGB LED backend");
                return new StartedProvider(openRgb);
            }

            openRgb.Dispose();

            if (IsLogitechSoftwareInstalled()) {
                Logger.Info("OpenRGB not available; using Logitech LED backend");
            }
            else {
                Logger.Warn("Neither OpenRGB nor Logitech software detected; LED colors will be unavailable.");
            }

            return new LogitechLedProvider();
        }

        private static bool IsLogitechSoftwareInstalled() {
            return File.Exists(LogitechPaths.DefaultProfile) || File.Exists(LogitechPaths.GHubConfig);
        }

        /// <summary>
        /// Wraps a provider that has already been successfully started (during auto-detection)
        /// so that a second <see cref="ILedProvider.Start"/> call is a no-op instead of reconnecting.
        /// </summary>
        private sealed class StartedProvider : ILedProvider {
            private readonly ILedProvider _inner;

            public StartedProvider(ILedProvider inner) {
                _inner = inner;
            }

            public bool Start() => true;
            public void SetColor(string key, int r, int g, int b) => _inner.SetColor(key, r, g, b);
            public void SaveState() => _inner.SaveState();
            public void RestoreState() => _inner.RestoreState();
            public void Flush() => _inner.Flush();
            public void Dispose() => _inner.Dispose();
        }
    }
}
