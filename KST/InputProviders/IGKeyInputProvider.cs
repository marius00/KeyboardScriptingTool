using System;
using KST.InputProviders.Args;

namespace KST.InputProviders {
    /// <summary>
    /// Common shape for the G-key input providers (SDK-based and native HID),
    /// so <see cref="Program"/> can choose one at runtime without caring which.
    /// </summary>
    internal interface IGKeyInputProvider : IDisposable {
        event InputEventHandler OnInput;
        void Start();
    }
}
