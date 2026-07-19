# OpenRGB LED backend — notes

Implementation lives in this folder:
- `ILedProvider.cs` — backend abstraction (Logitech SDK / OpenRGB).
- `OpenRgbLedProvider.cs` — OpenRGB SDK client (connects to `127.0.0.1:6742`).
- `OpenRgbKeyMapping.cs` — KST key name → OpenRGB LED name (`"Key: X"`) table.
- `LedProviderFactory.cs` — picks the backend (OpenRGB preferred if reachable; else Logitech).

Colors are percentages `[0,100]` everywhere in KST; the OpenRGB provider scales to bytes `[0,255]`.

## How it behaves (recap)
- Direct mode is entered **lazily** on first `SetColor`, so the user's normal effect keeps
  running until a script actually paints a key.
- On alt-tab away / exit, `RestoreState()` switches the device back to the mode it was in at
  startup (`_originalModeIndex`) via `UpdateMode`, so the effect resumes.
- On startup, `RescanDevices()` sends a raw OpenRGB rescan packet (id 140) before connecting,
  because OpenRGB.NET 3.1.1 doesn't wrap that command. This works around the G910/G915 getting
  into a "detected but won't render" state that a GUI rescan fixes. See below.

## Known OpenRGB quirks observed on the G910 Orion Spectrum
- OpenRGB sometimes has the keyboard detected but **won't render** until "Rescan Devices" is
  clicked in the GUI. KST pushes frames successfully (no socket error) but nothing lights up.
  The startup rescan is the mitigation. If it still happens, the manual drill is:
  OpenRGB GUI → Rescan Devices → restart KST.
- `Original mode index 0` — on the G910, mode 0 appears to be "Direct". Restoring to it is
  therefore not a colorful effect. Not a problem, just don't be surprised.

---

# TODO / checklist for adding the Logitech G915 (planned ~Sep–Oct 2026)

The G915 should work through the exact same OpenRGB path — it's another per-key Logitech
keyboard. The likely deltas are **LED naming** and **device selection**. Work through this:

1. **Dump the G915's actual LED names.** The mapping is name-based and per-keyboard. Temporarily
   log every `keyboard.Leds[i].Name` in `OpenRgbLedProvider.Start()` (or `ResolveKeyIndices`),
   run KST, and read `log.txt`. Compare against `OpenRgbKeyMapping._ledNames`.
   - Check the "mapped N keys" line: if N is low, names don't match and need fixing.

2. **Fix `OpenRgbKeyMapping` for G915 differences.** Expect:
   - **Macro keys:** G915 has **G1–G5** only (current map has G1–G9; extras just won't resolve,
     which is fine, but confirm G1–G5 names match — may be `"Key: G1"` or similar).
   - **Media keys / volume roller:** G915 exposes media controls the G910 didn't. Add mappings
     only if scripts need them.
   - **TKL variant** (G915 TKL) has **no numpad** — those keys simply won't resolve; harmless.
   - **Logo / brand LED** naming may differ (`"Key: Logo"` guess may be wrong).
   - Consider whether the same table can serve both boards, or whether to key the mapping by
     device name. A single superset table is fine as long as names don't *collide*.

3. **Device selection when both keyboards are present.** `Start()` currently grabs the **first**
   `DeviceType.Keyboard`. With a G910 *and* G915 connected, that's ambiguous. Options:
   - Add a `"LedDeviceName"` setting (substring match against `Device.Name`) alongside the
     existing `"LedBackend"` setting in `SettingsRootNode`.
   - Or prefer an exact/substring name match, falling back to first keyboard.

4. **Confirm Direct mode.** Verify `SetCustomMode` finds a "Direct" (or "Custom") mode on the
   G915; if it throws, check the mode list OpenRGB reports and adjust.

5. **Wireless (Lightspeed) considerations.** The G915 is wireless. Detection/reconnect may be
   flakier than the wired G910 — the startup rescan matters more here. If the device drops and
   reconnects at runtime, KST won't recover (it connects once in `Start()`); a runtime
   reconnect/re-enumerate is a possible future enhancement if this proves annoying.

6. **Re-test the lifecycle** on the G915: colors show → alt-tab restores the effect → alt-tab
   back re-enters Direct → exit restores the effect.

## Before shipping (applies to both boards)
- `SetColor` and `Flush` currently log at **INFO** (kept for live debugging). Drop them back to
  **Debug** before release so `log.txt` isn't spammed on every key/frame. Keep the
  "Entered OpenRGB Direct mode" / rescan / init lines at INFO.
- The provider assumes little-endian (x64) for the raw rescan packet — fine for this app.
