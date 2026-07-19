using System;

namespace KST.Led {
    /// <summary>
    /// Abstraction over a per-key RGB backend (Logitech SDK, OpenRGB, ..).
    /// Colors are always given as percentages [0, 100], regardless of backend.
    /// </summary>
    internal interface ILedProvider : IDisposable {
        /// <summary>
        /// Initializes the backend. Returns true if the backend is ready to receive colors.
        /// </summary>
        bool Start();

        /// <summary>
        /// Sets the color for a single key. r/g/b are percentages in the range [0, 100].
        /// </summary>
        void SetColor(string key, int r, int g, int b);

        /// <summary>
        /// Snapshots the current lighting so it can be restored later (e.g. on alt-tab).
        /// </summary>
        void SaveState();

        /// <summary>
        /// Restores lighting to the last snapshot taken by <see cref="SaveState"/>.
        /// </summary>
        void RestoreState();

        /// <summary>
        /// Pushes any batched color changes to the device. No-op for backends that write immediately.
        /// </summary>
        void Flush();
    }
}
