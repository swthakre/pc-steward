// SPDX-License-Identifier: GPL-3.0-or-later
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
namespace Steward.Windows;
public sealed record InventoryRow(string Name, string Detail, string State);
public sealed record InventoryResult(IReadOnlyList<InventoryRow> Rows, string Coverage);
public static class Inventory
{
    public static InventoryResult Applications()
    {
        var rows = new List<InventoryRow>(); var errors = 0;
        foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            try
            {
                using var root = RegistryKey.OpenBaseKey(hive, view);
                using var key = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                if (key == null) continue;
                foreach (var name in key.GetSubKeyNames())
                {
                    try
                    {
                        using var app = key.OpenSubKey(name);
                        var title = app?.GetValue("DisplayName") as string;
                        if (!string.IsNullOrWhiteSpace(title)) rows.Add(new(title, $"{app?.GetValue("DisplayVersion")} · {app?.GetValue("Publisher")}", $"{hive} / {view}"));
                    }
                    catch (Exception e) when (e is System.Security.SecurityException or UnauthorizedAccessException or IOException) { errors++; }
                }
            }
            catch (Exception e) when (e is System.Security.SecurityException or UnauthorizedAccessException or IOException) { errors++; }
        }
        return new(rows.Distinct().OrderBy(r => r.Name).ToList(), $"Uninstall registry entries only; Store/portable apps may be absent. Publisher is unverified metadata. Read errors: {errors}.");
    }
    public static InventoryResult Startup()
    {
        var rows = new List<InventoryRow>();
        foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        {
            try
            {
                using var root = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
                using var key = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (key != null) foreach (var name in key.GetValueNames()) rows.Add(new(name, key.GetValue(name)?.ToString() ?? "", hive.ToString()));
            }
            catch (Exception e) when (e is System.Security.SecurityException or UnauthorizedAccessException or IOException) { rows.Add(new("Unavailable", e.Message, hive.ToString())); }
        }
        foreach (var special in new[] { Environment.SpecialFolder.Startup, Environment.SpecialFolder.CommonStartup })
        {
            try { var path = Environment.GetFolderPath(special); if (Directory.Exists(path)) foreach (var p in Directory.EnumerateFiles(path)) rows.Add(new(Path.GetFileName(p), p, special.ToString())); }
            catch (Exception e) when (e is UnauthorizedAccessException or IOException) { rows.Add(new("Unavailable", e.Message, special.ToString())); }
        }
        return new(rows, "Run keys and startup folders only. Presence does not mean enabled; no startup entries are changed.");
    }
    public static InventoryResult Processes()
    {
        var rows = new List<InventoryRow>(); var errors = 0;
        foreach (var process in Process.GetProcesses())
        using (process)
        {
            try { rows.Add(new(process.ProcessName, $"Working set {process.WorkingSet64 / 1048576d:N1} MiB · PID {process.Id}", "CPU total " + process.TotalProcessorTime)); }
            catch (Exception e) when (e is InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException) { errors++; }
        }
        return new(rows.OrderBy(r => r.Name).ToList(), $"Point-in-time snapshot, not CPU utilization or an unused/malware verdict. Unreadable/exited processes: {errors}.");
    }
    public static InventoryResult Hardware()
    {
        var rows = new List<InventoryRow> { new("Windows", Environment.OSVersion.ToString(), "Reported by OS"), new("Architecture", System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString(), "Host"), new("Logical processors", Environment.ProcessorCount.ToString(), "Available to process") };
        foreach (var drive in DriveInfo.GetDrives())
        {
            try { rows.Add(new(drive.Name, drive.IsReady ? $"{drive.DriveFormat} · {drive.TotalSize / 1073741824d:N1} GiB capacity · {drive.AvailableFreeSpace / 1073741824d:N1} GiB available" : "Not ready", drive.DriveType.ToString())); }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException) { rows.Add(new(drive.Name, e.Message, "Unavailable")); }
        }
        return new(rows, "Basic OS/volume inventory. SMART, drive reliability, allocated-file accounting and hardware sensors are not implemented.");
    }
    public static async Task<InventoryResult> QueryAsync(string kind, CancellationToken token = default)
    {
        // Closed command set: no paths, filenames, inventory values or user input are interpolated into scripts.
        var command = kind switch
        {
            "Services" => "Get-Service | Select-Object -First 2000 @{n='Name';e={$_.DisplayName}},@{n='Detail';e={$_.Name}},@{n='State';e={[string]$_.Status}}",
            "Tasks" => "Get-ScheduledTask | Select-Object -First 2000 @{n='Name';e={$_.TaskName}},@{n='Detail';e={$_.TaskPath}},@{n='State';e={[string]$_.State}}",
            "Security" => "Get-MpComputerStatus | Select-Object AMServiceEnabled,AntivirusEnabled,RealTimeProtectionEnabled,AntivirusSignatureLastUpdated,QuickScanEndTime,FullScanEndTime | Format-List | Out-String -Width 200",
            _ => throw new ArgumentException("Unsupported inventory query")
        };
        var script = "$ErrorActionPreference='Stop'; $ProgressPreference='SilentlyContinue'; " + (kind == "Security" ? command : "@(" + command + ") | ConvertTo-Json -Compress");
        var executable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"WindowsPowerShell\v1.0\powershell.exe");
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
        start.ArgumentList.Add("-NoProfile"); start.ArgumentList.Add("-NonInteractive"); start.ArgumentList.Add("-EncodedCommand"); start.ArgumentList.Add(Convert.ToBase64String(Encoding.Unicode.GetBytes(script)));
        using var process = new Process { StartInfo = start };
        try
        {
            process.Start();
            var output = process.StandardOutput.ReadToEndAsync(token); var error = process.StandardError.ReadToEndAsync(token);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token); timeout.CancelAfter(TimeSpan.FromSeconds(30));
            try { await process.WaitForExitAsync(timeout.Token); }
            catch (OperationCanceledException) { try { process.Kill(true); } catch (InvalidOperationException) { } return new([], "Query cancelled or timed out. Status unknown."); }
            var text = await output; var errors = await error;
            if (process.ExitCode != 0) return new([], "Unavailable: " + errors.Trim());
            if (kind == "Security") return new([new("Microsoft Defender", text.Trim(), "Provider status only")], "A status query is not a malware scan. Other antivirus providers may be active; vulnerability matching is not implemented.");
            var rows = JsonSerializer.Deserialize<List<InventoryRow>>(text) ?? [];
            return new(rows, "Accessible entries only, capped at 2,000. No changes performed.");
        }
        catch (Exception e) when (e is System.ComponentModel.Win32Exception or IOException or JsonException or OperationCanceledException) { return new([], "Unavailable: " + e.Message); }
    }
}
