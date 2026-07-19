using System.Text;
using HidSharp;

// KstHidProbe — discovery tool for Logitech G-key HID mapping.
//
// Goal: find which HID interface emits input reports when you press G1..G9
// (with G-Hub / LGS NOT running), and how those bytes encode each key.
//
// Usage:
//   KstHidProbe            -> list all Logitech (VID 0x046D) HID interfaces
//   KstHidProbe listen     -> open every candidate interface and stream raw
//                             input reports. Press G1..G9 and watch the bytes.
//   KstHidProbe listen all -> same, but also opens the plain keyboard/consumer
//                             interfaces (usage page 0x01 / 0x0C) in case the
//                             G-keys arrive there.

const int LogitechVid = 0x046D;

var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "list";
var openEverything = args.Length > 1 && args[1].ToLowerInvariant() == "all";

// G910 Orion Spectrum activation: switch G-keys out of onboard/F-key mode into
// software/macro mode so they emit raw 11 FF 08 .. reports. (report ID 0x11)
byte[] g910Activate = { 0x11, 0xFF, 0x08, 0x2E, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
const int G910Pid = 0xC335;

if (mode == "activate") {
    ActivateAndListen();
    return;
}

var devices = DeviceList.Local.GetHidDevices(LogitechVid).ToList();
if (devices.Count == 0) {
    Console.WriteLine("No Logitech (VID 0x046D) HID devices found.");
    Console.WriteLine("Is the keyboard plugged in? Try running as Administrator.");
    return;
}

Console.WriteLine($"Found {devices.Count} Logitech HID interface(s):\n");

var candidates = new List<HidDevice>();
foreach (var dev in devices) {
    int usagePage = -1, usage = -1;
    try {
        var report = dev.GetReportDescriptor();
        var di = report.DeviceItems.FirstOrDefault();
        if (di != null && di.Usages.GetAllValues().Any()) {
            var u = di.Usages.GetAllValues().First();
            usagePage = (int)(u >> 16);
            usage = (int)(u & 0xFFFF);
        }
    }
    catch { /* some interfaces refuse descriptor reads without elevation */ }

    Console.WriteLine($"  PID=0x{dev.ProductID:X4}  usagePage=0x{usagePage:X4} usage=0x{usage:X4}  maxIn={dev.GetMaxInputReportLength()}");
    Console.WriteLine($"    product : {SafeName(() => dev.GetProductName())}");
    Console.WriteLine($"    path    : {dev.DevicePath}");
    Console.WriteLine();

    // Vendor-defined pages (0xFF00..0xFFFF) are where G-keys almost always live.
    bool isVendor = usagePage >= 0xFF00;
    bool isInputCapable = dev.GetMaxInputReportLength() > 0;
    if (isInputCapable && (isVendor || openEverything || usagePage == -1))
        candidates.Add(dev);
}

if (mode != "listen") {
    Console.WriteLine("Run `KstHidProbe listen` to stream reports from the vendor interfaces");
    Console.WriteLine("(add `all` to also open the standard keyboard interfaces).");
    return;
}

if (candidates.Count == 0) {
    Console.WriteLine("No candidate input interfaces to listen on. Try `listen all`.");
    return;
}

Console.WriteLine($"Listening on {candidates.Count} interface(s). Press G1..G9 now.");
Console.WriteLine("Each line: PID + the raw input report bytes. Ctrl+C to stop.\n");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var tasks = candidates.Select(dev => Task.Run(() => Listen(dev, cts.Token))).ToArray();
Task.WaitAll(tasks);

void ActivateAndListen() {
    var g910 = DeviceList.Local.GetHidDevices(LogitechVid, G910Pid).ToList();
    if (g910.Count == 0) { Console.WriteLine("G910 (PID 0xC335) not found."); return; }

    // The 20-byte vendor interface (FF43/0602, maxIn=20) is both where we send the
    // activation report and where G-key reports arrive.
    var target = g910.FirstOrDefault(d => d.GetMaxInputReportLength() == 20
                                          && d.GetMaxOutputReportLength() >= 20);
    if (target == null) {
        Console.WriteLine("Could not find the 20-byte vendor interface on the G910.");
        return;
    }
    Console.WriteLine($"Using interface: {target.DevicePath}");
    Console.WriteLine($"  maxIn={target.GetMaxInputReportLength()} maxOut={target.GetMaxOutputReportLength()}");

    HidStream stream;
    try { stream = target.Open(); }
    catch (Exception ex) { Console.WriteLine($"Open failed: {ex.Message}"); return; }

    try {
        Console.WriteLine("Sending activation packet: 11 FF 08 2E 01 ...");
        try {
            stream.Write(g910Activate);
            Console.WriteLine("  Write() OK");
        }
        catch (Exception ex) {
            Console.WriteLine($"  Write() failed ({ex.Message}); trying SetFeature...");
            try { stream.SetFeature(g910Activate); Console.WriteLine("  SetFeature() OK"); }
            catch (Exception ex2) { Console.WriteLine($"  SetFeature() failed: {ex2.Message}"); }
        }

        Console.WriteLine("\nNow press G1..G9 (and M1/M2/M3, MR). Ctrl+C to stop.\n");
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
        var buf = new byte[target.GetMaxInputReportLength()];
        stream.ReadTimeout = 500;
        while (!cts.IsCancellationRequested) {
            int n;
            try { n = stream.Read(buf, 0, buf.Length); }
            catch (TimeoutException) { continue; }
            if (n <= 0) continue;
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++) sb.Append(buf[i].ToString("X2")).Append(' ');
            Console.WriteLine($"  [{n,2}]  {sb}{Decode(buf, n)}");
        }
    }
    finally { stream.Dispose(); }
}

static string Decode(byte[] b, int n) {
    if (n >= 5 && b[0] == 0x11 && b[1] == 0xFF) {
        if (b[2] == 0x08) {
            var keys = new List<string>();
            for (int i = 0; i < 8; i++) if ((b[4] & (1 << i)) != 0) keys.Add($"G{i + 1}");
            if (n >= 6 && (b[5] & 0x01) != 0) keys.Add("G9");
            return keys.Count == 0 ? "  <- G release" : "  <- " + string.Join("+", keys);
        }
        if (b[2] == 0x09) {
            if (b[4] == 0) return "  <- M release";
            return "  <- " + (b[4] == 1 ? "M1" : b[4] == 2 ? "M2" : b[4] == 4 ? "M3" : $"M?{b[4]:X2}");
        }
        if (b[2] == 0x0A) return b[4] == 1 ? "  <- MR" : "  <- MR release";
    }
    return "";
}

static void Listen(HidDevice dev, CancellationToken ct) {
    HidStream stream;
    try {
        var cfg = new OpenConfiguration();
        // Non-exclusive so we don't fight the OS keyboard stack.
        cfg.SetOption(OpenOption.Interruptible, true);
        stream = dev.Open(cfg);
    }
    catch (Exception ex) {
        Console.WriteLine($"  [PID 0x{dev.ProductID:X4}] could not open: {ex.Message}");
        return;
    }

    var buf = new byte[dev.GetMaxInputReportLength()];
    stream.ReadTimeout = 500;
    try {
        while (!ct.IsCancellationRequested) {
            int n;
            try { n = stream.Read(buf, 0, buf.Length); }
            catch (TimeoutException) { continue; }
            if (n <= 0) continue;

            var sb = new StringBuilder();
            for (int i = 0; i < n; i++) sb.Append(buf[i].ToString("X2")).Append(' ');
            Console.WriteLine($"  PID 0x{dev.ProductID:X4} [{n,2}]  {sb}");
        }
    }
    catch (Exception ex) when (!ct.IsCancellationRequested) {
        Console.WriteLine($"  [PID 0x{dev.ProductID:X4}] read error: {ex.Message}");
    }
    finally {
        stream.Dispose();
    }
}

static string SafeName(Func<string> f) {
    try { return f(); } catch { return "<unavailable>"; }
}
