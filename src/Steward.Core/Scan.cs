// SPDX-License-Identifier: GPL-3.0-or-later
namespace Steward.Core;
public sealed class Entry
{
    public required string Path { get; init; }
    public string Name => System.IO.Path.GetFileName(Path.TrimEnd(System.IO.Path.DirectorySeparatorChar));
    public bool IsDirectory { get; init; }
    public long Bytes { get; set; }
    public string Status { get; set; } = "Read";
    public DateTime ModifiedUtc { get; init; }
    public Entry? Parent { get; init; }
    public List<Entry> Children { get; } = [];
    public string Type => IsDirectory ? "Folder" : System.IO.Path.GetExtension(Path).ToLowerInvariant();
    public string Size => Sizes.Format(Bytes);
}
public sealed record ScanResult(Entry Root, IReadOnlyList<Entry> Entries, string Status, int Skipped, DateTimeOffset Started, TimeSpan Duration);
public sealed record ScanProgress(int Entries, string Path);
public static class Sizes
{
    public static string Format(long bytes) => bytes >= 1073741824 ? $"{bytes / 1073741824d:N2} GiB" : bytes >= 1048576 ? $"{bytes / 1048576d:N1} MiB" : bytes >= 1024 ? $"{bytes / 1024d:N1} KiB" : $"{bytes} B";
}
public sealed class DiskScanner
{
    public ScanResult Scan(string path, CancellationToken token = default, IProgress<ScanProgress>? progress = null, int entryLimit = 100_000)
    {
        if (entryLimit < 1 || entryLimit > 1_000_000) throw new ArgumentOutOfRangeException(nameof(entryLimit));
        path = System.IO.Path.GetFullPath(path);
        if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path);
        // Reject a selected root beneath a junction too, not just links encountered below it.
        for (var ancestor = new DirectoryInfo(path); ancestor != null; ancestor = ancestor.Parent)
            if (Unsafe(ancestor.Attributes)) throw new IOException("The selected root passes through a reparse point or offline placeholder. Choose a local physical directory.");
        var started = DateTimeOffset.UtcNow;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var root = new Entry { Path = path, IsDirectory = true, Status = "Pending" };
        var entries = new List<Entry> { root };
        var stack = new Stack<Entry>(); stack.Push(root);
        var skipped = 0; var status = "Complete";
        var options = new EnumerationOptions { RecurseSubdirectories = false, IgnoreInaccessible = false, AttributesToSkip = 0, ReturnSpecialDirectories = false };
        while (stack.Count > 0)
        {
            if (token.IsCancellationRequested) { status = "Cancelled"; break; }
            var parent = stack.Pop();
            parent.Status = "Enumerating";
            try
            {
                if (Unsafe(File.GetAttributes(parent.Path))) { parent.Status = "Skipped link/offline"; skipped++; continue; }
                foreach (var info in new DirectoryInfo(parent.Path).EnumerateFileSystemInfos("*", options))
                {
                    if (token.IsCancellationRequested) { status = "Cancelled"; break; }
                    if (entries.Count >= entryLimit) { status = "Limited"; break; }
                    try
                    {
                        var attr = info.Attributes;
                        var directory = (attr & FileAttributes.Directory) != 0;
                        var skip = Unsafe(attr);
                        var entry = new Entry { Path = info.FullName, IsDirectory = directory, Parent = parent,
                            ModifiedUtc = info.LastWriteTimeUtc, Bytes = !directory && !skip ? ((FileInfo)info).Length : 0,
                            Status = skip ? "Skipped link/offline" : directory ? "Pending" : "Read" };
                        entries.Add(entry); parent.Children.Add(entry);
                        if (skip) skipped++; else if (directory) stack.Push(entry);
                    }
                    catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Security.SecurityException)
                    {
                        skipped++;
                        var entry = new Entry { Path = info.FullName, Parent = parent, Status = "Unreadable: " + e.Message };
                        entries.Add(entry); parent.Children.Add(entry);
                    }
                    if (entries.Count % 256 == 0) progress?.Report(new(entries.Count, parent.Path));
                }
                parent.Status = status is "Cancelled" or "Limited" ? "Partial enumeration" : "Complete";
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Security.SecurityException)
            { parent.Status = "Unreadable: " + e.Message; skipped++; }
            if (status is "Cancelled" or "Limited") break;
        }
        // Parents are inserted before children; reverse accumulation avoids recursive stack overflow.
        for (var i = entries.Count - 1; i > 0; i--)
        {
            var entry = entries[i]; var parent = entry.Parent!; parent.Bytes += entry.Bytes;
            if (entry.Status is not "Read" and not "Complete" && parent.Status == "Complete") parent.Status = "Partial subtree";
        }
        if (status == "Complete" && skipped > 0) status = "Partial";
        root.Status = status;
        progress?.Report(new(entries.Count, path));
        return new(root, entries, status, skipped, started, watch.Elapsed);
    }
    private static bool Unsafe(FileAttributes attributes) => ((int)attributes & ((int)FileAttributes.ReparsePoint | (int)FileAttributes.Offline | 0x40000 | 0x400000)) != 0;
}
public static class Reports
{
    public static string Csv(ScanResult result)
    {
        static string Quote(string value) { if (value.Length > 0 && "=+-@\t\r\n".Contains(value[0])) value = "'" + value; return "\"" + value.Replace("\"", "\"\"") + "\""; }
        var text = new System.Text.StringBuilder("Name,Path,Type,LogicalBytes,EntryStatus,ScanStatus\r\n");
        foreach (var e in result.Entries) text.AppendLine(string.Join(",", Quote(e.Name), Quote(e.Path), Quote(e.Type), e.Bytes.ToString(System.Globalization.CultureInfo.InvariantCulture), Quote(e.Status), Quote(result.Status)));
        return text.ToString();
    }
    public static string Json(ScanResult result) => System.Text.Json.JsonSerializer.Serialize(new {
        schemaVersion = 1, root = result.Root.Path, status = result.Status, skipped = result.Skipped,
        started = result.Started, durationSeconds = result.Duration.TotalSeconds,
        accounting = "Logical bytes; hard links counted per path; allocated space not measured; reparse/offline items skipped. Not an atomic snapshot.",
        entries = result.Entries.Select(e => new { path = e.Path, parent = e.Parent?.Path, directory = e.IsDirectory, bytes = e.Bytes, type = e.Type, status = e.Status, modifiedUtc = e.ModifiedUtc })
    }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
}
