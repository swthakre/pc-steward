// SPDX-License-Identifier: GPL-3.0-or-later
using Microsoft.Win32;
using Steward.Core;
using Steward.Windows;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Steward.App;
public partial class MainWindow : Window
{
    private readonly string[] sections = ["Overview", "Cleaner", "Disk Map", "Apps & services", "Security", "Health & updates", "Privacy", "Automation", "Recovery & history", "Advanced tools"];
    private readonly List<string> history = [];
    private readonly Stack<Entry> back = new();
    private ScanResult? scan;
    private Entry? current;
    private CancellationTokenSource? cancellation;
    private string active = "Overview";
    private string scanPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private TextBox? pathBox;
    private bool busy;
    private int pageGeneration;
    private readonly RequestGate inventoryRequests = new();
    private readonly HashSet<string> expandedFolders = new(StringComparer.OrdinalIgnoreCase);
    private bool focusTree;
    public MainWindow()
    {
        InitializeComponent();
        foreach (var section in sections) { var name = section; var b = Button(name, () => Show(name)); b.HorizontalContentAlignment = HorizontalAlignment.Left; b.Margin = new(0, 2, 0, 4); Navigation.Children.Add(b); }
        Closed += (_, _) => cancellation?.Cancel();
        Show("Overview");
    }
    private static Brush Brush(string color) => new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    private static TextBlock Text(string value, double size = 14, bool muted = false) => new() { Text = value, FontSize = size, Foreground = Brush(muted ? "#5B6085" : "#211C50"), TextWrapping = TextWrapping.Wrap, Margin = new(0, 0, 0, 8) };
    private static Button Button(string label, Action action, bool primary = false)
    {
        var b = new Button { Tag = label, Content = new TextBlock { Text = label, Foreground = Brush(primary ? "#FFFFFF" : "#211C50"), Margin = new(0) }, Background = Brush(primary ? "#6940D7" : "#EEE8FF"), Foreground = Brush(primary ? "#FFFFFF" : "#211C50") };
        System.Windows.Automation.AutomationProperties.SetName(b, label);
        b.Click += (_, _) => action(); return b;
    }
    private static Border Panel(UIElement child) => new() { Background = Brush("#FCFBFF"), CornerRadius = new(16), Padding = new(18), Margin = new(0, 0, 0, 16), Child = child };
    private static StackPanel Stack(params UIElement[] items) { var p = new StackPanel(); foreach (var item in items) p.Children.Add(item); return p; }
    private static WrapPanel Row(params UIElement[] items) { var p = new WrapPanel(); foreach (var item in items) p.Children.Add(item); return p; }
    private void Heading(string title, string subtitle) { Page.Children.Add(Text(title, 28)); Page.Children.Add(Text(subtitle, 14, true)); }
    private void Show(string section)
    {
        active = section; pageGeneration++; Page.Children.Clear();
        foreach (Button b in Navigation.Children) b.Background = Brush((string)b.Tag == section ? "#DFD5FF" : "#F1EEFF");
        switch (section)
        {
            case "Overview": Overview(); break;
            case "Disk Map": Disk(); break;
            case "Cleaner": Cleaner(); break;
            case "Apps & services": Apps(); break;
            case "Security": Security(); break;
            case "Health & updates": Health(); break;
            default: Roadmap(section); break;
        }
    }
    private void Overview()
    {
        Heading("A little care. A better PC.", "Your space, security, and maintenance — in one place. Real data, read-only development build.");
        var grid = new Grid(); grid.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) }); grid.ColumnDefinitions.Add(new() { Width = new GridLength(1.5, GridUnitType.Star) });
        var storage = Stack(Text("Storage overview", 18));
        try
        {
            var drive = new DriveInfo(System.IO.Path.GetPathRoot(Environment.SystemDirectory)!);
            var used = drive.TotalSize - drive.TotalFreeSpace;
            var ring = new Grid { Width = 175, Height = 175, Margin = new(0, 12, 0, 18) };
            ring.Children.Add(new Ellipse { Stroke = Brush("#E6E0F7"), StrokeThickness = 17 });
            var fraction = Math.Clamp((double)used / drive.TotalSize, 0, 1);
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open()) { var angle = Math.Max(.001, Math.Min(.9999, fraction)) * 2 * Math.PI; ctx.BeginFigure(new Point(87.5, 8.5), false, false); ctx.ArcTo(new Point(87.5 + 79 * Math.Sin(angle), 87.5 - 79 * Math.Cos(angle)), new Size(79, 79), 0, fraction > .5, SweepDirection.Clockwise, true, false); }
            ring.Children.Add(new System.Windows.Shapes.Path { Data = geometry, Stroke = Brush("#9C7CE5"), StrokeThickness = 17 });
            ring.Children.Add(new TextBlock { Text = $"{fraction:P0}\nused", FontSize = 24, TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            storage.Children.Add(ring); storage.Children.Add(Text($"{drive.Name}  {Sizes.Format(used)} used")); storage.Children.Add(Text($"{Sizes.Format(drive.TotalFreeSpace)} free of {Sizes.Format(drive.TotalSize)}", muted: true));
        }
        catch (IOException e) { storage.Children.Add(Text("Volume information unavailable: " + e.Message)); }
        var left = Panel(storage); left.Margin = new(0, 12, 14, 16); grid.Children.Add(left);
        var rightContent = Stack(Text("Where your space goes", 18), Text(scan == null ? "Choose a folder or drive to build your personal disk map. Nothing is scanned automatically." : $"Last scan: {scan.Status} · {scan.Entries.Count:N0} entries · {Sizes.Format(scan.Root.Bytes)} logical", muted: true), Button("Explore Disk Map", () => Show("Disk Map"), true));
        if (scan != null) rightContent.Children.Add(new Treemap(scan.Root.Children, OpenEntry) { Height = 190 });
        else { rightContent.Children.Add(Text("Folder → subfolder → file", 22)); rightContent.Children.Add(Text("Sizes, extensions, largest files, coverage and export. Reparse points and offline placeholders are skipped.", muted: true)); }
        var right = Panel(rightContent); right.Margin = new(0, 12, 0, 16); Grid.SetColumn(right, 1); grid.Children.Add(right); Page.Children.Add(grid);
        var cards = new Grid(); for (int i = 0; i < 3; i++) cards.ColumnDefinitions.Add(new());
        var descriptions = new[] { ("Cleaner", "Review aged temporary files. Deletion is not enabled in this build.", "Cleaner"), ("Security", "Query Microsoft Defender status. Malware scans and CVE matching are not yet implemented.", "Security"), ("Maintenance", "Inspect Windows and storage information without changing settings.", "Health & updates") };
        for (int i = 0; i < descriptions.Length; i++) { var d = descriptions[i]; var panel = Panel(Stack(Text(d.Item1, 19), Text(d.Item2, muted: true), Button("Open " + d.Item1, () => Show(d.Item3)))); panel.Margin = new(0, 0, i < 2 ? 12 : 0, 16); Grid.SetColumn(panel, i); cards.Children.Add(panel); }
        Page.Children.Add(cards);
        Page.Children.Add(Panel(Stack(Text("Development alpha", 17), Text("No telemetry, background schedules, cleanup, process termination or elevation. Reports stay local and contain full paths. Advanced features are clearly marked as pending.", muted: true))));
    }
    private void ScanControls()
    {
        pathBox = new TextBox { Text = scanPath, MinWidth = 300, Padding = new(9), Margin = new(0, 0, 10, 8), IsEnabled = !busy };
        System.Windows.Automation.AutomationProperties.SetName(pathBox, "Folder to scan");
        var choose = Button("Choose folder", () => { var picker = new OpenFolderDialog { Title = "Choose a folder for read-only analysis" }; if (picker.ShowDialog(this) == true) { scanPath = picker.FolderName; pathBox.Text = scanPath; } }); choose.IsEnabled = !busy;
        var start = Button("Scan folder", () => { scanPath = pathBox.Text; _ = ScanAsync(scanPath); }, true); start.IsEnabled = !busy;
        var cancel = Button("Cancel scan", () => cancellation?.Cancel()); cancel.IsEnabled = busy;
        Page.Children.Add(Row(pathBox, choose, start, cancel));
        var drives = new WrapPanel();
        foreach (var d in DriveInfo.GetDrives().Where(d => d.DriveType is DriveType.Fixed or DriveType.Removable))
        { var name = d.Name; var b = Button(name, () => { scanPath = name; pathBox.Text = name; }); b.IsEnabled = !busy; drives.Children.Add(b); }
        Page.Children.Add(drives);
    }
    private async Task ScanAsync(string path)
    {
        if (busy) return;
        busy = true; cancellation = new(); var token = cancellation.Token;
        Show(active); Status.Text = "Scanning metadata…";
        var progress = new Progress<ScanProgress>(p => Status.Text = $"Scanning · {p.Entries:N0} entries · {p.Path}");
        try
        {
            var result = await Task.Run(() => new DiskScanner().Scan(path, token, progress));
            scan = result; current = result.Root; back.Clear(); expandedFolders.Clear(); expandedFolders.Add(result.Root.Path); history.Add($"{DateTime.Now:g} · {result.Status} · {result.Entries.Count:N0} entries · {result.Duration.TotalSeconds:N1}s");
            Status.Text = $"{result.Status} · {result.Entries.Count:N0} entries · {result.Skipped:N0} skipped · {Sizes.Format(result.Root.Bytes)} logical";
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or ArgumentException or System.Security.SecurityException) { Status.Text = "Scan failed: " + e.Message; MessageBox.Show(this, e.Message, "Scan unavailable", MessageBoxButton.OK, MessageBoxImage.Information); }
        finally { busy = false; cancellation.Dispose(); cancellation = null; Show(active); }
    }
    private void Disk()
    {
        Heading("Disk Map", "Follow the space, from a drive down to individual files. Select a root, then scan."); ScanControls();
        Page.Children.Add(Text("Logical bytes only · Hard links counted per path · No file content read · 100,000-entry development limit", 12, true));
        if (scan == null || current == null) { Page.Children.Add(Panel(Text("No scan yet. Choose a folder above to begin.", 18))); return; }
        Page.Children.Add(Text($"Coverage: {scan.Status} · {scan.Skipped} skipped · {scan.Started.LocalDateTime:g}. This is a changing filesystem, not an atomic snapshot.", 12, true));
        var up = Button("↑ Up", () => { if (current.Parent != null) OpenEntry(current.Parent); }); up.IsEnabled = current.Parent != null;
        var prev = Button("← Back", () => { if (back.Count > 0) { current = back.Pop(); Show("Disk Map"); } }); prev.IsEnabled = back.Count > 0;
        Page.Children.Add(Row(prev, up, Button("Export JSON", () => Export(false)), Button("Export CSV", () => Export(true))));
        Page.Children.Add(Text(current.Path, 16)); Page.Children.Add(Text($"{Sizes.Format(current.Bytes)} · {current.Children.Count:N0} direct children · {current.Status}", 12, true));
        var grid = new Grid { Height = 320 }; grid.ColumnDefinitions.Add(new() { Width = new GridLength(230) }); grid.ColumnDefinitions.Add(new());
        var tree = new TreeView { Background = Brushes.Transparent, BorderThickness = new(0) }; tree.Items.Add(TreeNode(scan.Root));
        grid.Children.Add(tree);
        var table = FileTable(current.Children.OrderByDescending(e => e.Bytes));
        table.MouseDoubleClick += (_, _) => { if (table.SelectedItem is Entry e) OpenEntry(e); };
        table.KeyDown += (_, e) => { if (e.Key == Key.Enter && table.SelectedItem is Entry entry) { OpenEntry(entry); e.Handled = true; } };
        Grid.SetColumn(table, 1); grid.Children.Add(table); Page.Children.Add(Panel(grid));
        Page.Children.Add(Text("Folder treemap · select to open / inspect", 17));
        Page.Children.Add(Panel(new Treemap(current.Children, OpenEntry) { Height = 230 }));
        var types = scan.Entries.Where(e => !e.IsDirectory && IsWithin(e, current)).GroupBy(e => string.IsNullOrEmpty(e.Type) ? "No extension" : e.Type).Select(g => new InventoryRow(g.Key, Sizes.Format(g.Sum(e => e.Bytes)), $"{g.Count():N0} files")).OrderBy(r => r.Name).ToList();
        Page.Children.Add(Text("File types in this subtree", 17)); Page.Children.Add(Panel(InventoryTable(types, 160)));
    }
    private static bool IsWithin(Entry entry, Entry parent) { for (var p = entry.Parent; p != null; p = p.Parent) if (ReferenceEquals(p, parent)) return true; return false; }
    private TreeViewItem TreeNode(Entry e)
    {
        var item = new TreeViewItem { Header = string.IsNullOrEmpty(e.Name) ? e.Path : e.Name, Tag = e, ToolTip = e.Path };
        if (e.Children.Any(c => c.IsDirectory)) item.Items.Add("Loading…");
        item.Expanded += (_, args) => { if (args.OriginalSource != item) return; expandedFolders.Add(e.Path); if (item.Items.Count == 1 && item.Items[0] is string) { item.Items.Clear(); foreach (var c in e.Children.Where(c => c.IsDirectory).OrderBy(c => c.Name)) item.Items.Add(TreeNode(c)); } };
        item.Collapsed += (_, args) => { if (args.OriginalSource == item) expandedFolders.Remove(e.Path); };
        item.Selected += (_, args) => { if (args.OriginalSource == item) { args.Handled = true; if (current != e) { focusTree = true; Dispatcher.BeginInvoke(() => OpenEntry(e)); } } };
        item.IsExpanded = expandedFolders.Contains(e.Path);
        if (current == e) { item.IsSelected = true; if (focusTree) { focusTree = false; item.Loaded += (_, _) => item.Focus(); } }
        return item;
    }
    private void OpenEntry(Entry e)
    {
        if (e.IsDirectory) { if (current != null && current != e) back.Push(current); current = e; Show("Disk Map"); }
        else MessageBox.Show(this, $"{e.Path}\n\nLogical size: {e.Size}\nType: {e.Type}\nModified (UTC): {e.ModifiedUtc:u}\nCoverage: {e.Status}\n\nAllocated bytes are not measured. No file will be opened or deleted.", "File details", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    private static DataGrid FileTable(IEnumerable<Entry> entries)
    {
        var table = new DataGrid { ItemsSource = entries.ToList(), SelectionMode = DataGridSelectionMode.Single };
        table.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
        table.Columns.Add(new DataGridTextColumn { Header = "Logical size", Binding = new Binding("Size"), SortMemberPath = "Bytes", Width = 110 });
        table.Columns.Add(new DataGridTextColumn { Header = "Type", Binding = new Binding("Type"), Width = 80 });
        table.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("Status"), Width = 150 });
        return table;
    }
    private void Export(bool csv)
    {
        if (scan == null) return;
        var dialog = new SaveFileDialog { Title = "Export local paths and scan metadata — review before sharing", Filter = csv ? "CSV report|*.csv" : "JSON report|*.json", FileName = "steward-scan", AddExtension = true, OverwritePrompt = true };
        if (dialog.ShowDialog(this) != true) return;
        try { File.WriteAllText(dialog.FileName, csv ? Reports.Csv(scan) : Reports.Json(scan), new System.Text.UTF8Encoding(true)); Status.Text = "Report exported locally. It contains full paths; review before sharing."; }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException) { MessageBox.Show(this, e.Message, "Export failed"); }
    }
    private void Cleaner()
    {
        Heading("Cleaner preview", "Read-only discovery of files in your temporary directory. Nothing is deleted.");
        Page.Children.Add(Panel(Stack(Text("Aged temporary files", 18), Text("Scan your current user's TEMP folder, then review files last modified more than seven days ago. Age alone does not establish that a file is safe to remove. Cleanup remains unavailable pending identity, recovery and adversarial tests.", muted: true), Button(busy ? "Scan already running" : "Scan TEMP metadata", () => { if (!busy) _ = ScanAsync(System.IO.Path.GetTempPath()); }, true))));
        if (scan == null || !System.IO.Path.GetFullPath(scan.Root.Path).TrimEnd('\\').Equals(System.IO.Path.GetFullPath(System.IO.Path.GetTempPath()).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)) return;
        var candidates = scan.Entries.Where(e => !e.IsDirectory && e.Status == "Read" && e.ModifiedUtc < DateTime.UtcNow.AddDays(-7)).OrderByDescending(e => e.Bytes).ToList();
        Page.Children.Add(Text($"{candidates.Count:N0} aged files · {Sizes.Format(candidates.Sum(e => e.Bytes))} logical · Scan {scan.Status}", 18));
        var table = FileTable(candidates); table.Height = 330; Page.Children.Add(Panel(table));
        Page.Children.Add(Button("Inspect scan in Disk Map", () => Show("Disk Map")));
    }
    private static DataGrid InventoryTable(IEnumerable<InventoryRow> rows, double height = 380)
    {
        var table = new DataGrid { ItemsSource = rows, Height = height };
        table.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
        table.Columns.Add(new DataGridTextColumn { Header = "Details", Binding = new Binding("Detail"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
        table.Columns.Add(new DataGridTextColumn { Header = "State / source", Binding = new Binding("State"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
        table.MouseDoubleClick += (_, _) => { if (table.SelectedItem is InventoryRow r) MessageBox.Show(r.Name + "\n\n" + r.Detail + "\n\n" + r.State, "Inventory details"); };
        return table;
    }
    private async Task LoadInventory(Func<Task<InventoryResult>> load, StackPanel target)
    {
        var generation = pageGeneration; var request = inventoryRequests.Next(); target.Children.Clear(); target.Children.Add(Text("Reading local metadata…", muted: true));
        try { var result = await load(); if (generation != pageGeneration || !inventoryRequests.IsCurrent(request)) return; target.Children.Clear(); target.Children.Add(Text(result.Coverage, 13, true)); target.Children.Add(InventoryTable(result.Rows)); Status.Text = $"Read {result.Rows.Count:N0} inventory rows. No changes performed."; }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidOperationException) { if (generation != pageGeneration || !inventoryRequests.IsCurrent(request)) return; target.Children.Clear(); target.Children.Add(Text("Inventory unavailable: " + e.Message)); }
    }
    private void Apps()
    {
        Heading("Apps & services", "Read-only inventory. Idle or unfamiliar software is not automatically unnecessary or malicious.");
        var results = new StackPanel();
        Page.Children.Add(Row(Button("Installed apps", () => _ = LoadInventory(() => Task.Run(Inventory.Applications), results)), Button("Startup entries", () => _ = LoadInventory(() => Task.Run(Inventory.Startup), results)), Button("Processes", () => _ = LoadInventory(() => Task.Run(Inventory.Processes), results)), Button("Services", () => _ = LoadInventory(() => Inventory.QueryAsync("Services"), results)), Button("Scheduled tasks", () => _ = LoadInventory(() => Inventory.QueryAsync("Tasks"), results))));
        Page.Children.Add(Panel(results)); results.Children.Add(Text("Choose an inventory above. No processes or services will be stopped.", muted: true));
    }
    private void Security()
    {
        Heading("Security", "Separate protection status, malware scans, and software vulnerability evidence.");
        var result = new StackPanel();
        Page.Children.Add(Panel(Stack(Text("Microsoft Defender", 18), Text("Query the local provider status. This does not start a scan and does not claim the machine is free from malware.", muted: true), Button("Read Defender status", () => _ = LoadInventory(() => Inventory.QueryAsync("Security"), result), true), result)));
        Page.Children.Add(Panel(Stack(Text("Vulnerability report — not implemented", 18), Text("Planned: installed-software identity and version matching against CVE/NVD, OSV and GitHub advisories, with source freshness and confidence. KEV and EPSS will prioritize confirmed matches. No software inventory is uploaded by this build.", muted: true))));
    }
    private void Health()
    {
        Heading("Health & updates", "Read basic device information without changing Windows settings.");
        var result = Inventory.Hardware(); Page.Children.Add(Panel(Stack(Text(result.Coverage, 13, true), InventoryTable(result.Rows, 280))));
        Page.Children.Add(Panel(Text("Windows update checks, SMART reliability, repair tools and upgrade execution are pending. A capacity reading is not a drive-health assessment.", muted: true)));
    }
    private void Roadmap(string section)
    {
        Heading(section, "Planned module — no system action is available in this development build.");
        var description = section switch
        {
            "Privacy" => "Planned: per-browser profile review, explicit history/cookie selection and exclusions. This application does not read browser credentials or upload browsing data.",
            "Automation" => "Planned: user-enabled schedules, idle/battery gates, cancellation and local reports. No task or background service has been installed.",
            "Recovery & history" => "Current-session scan history only. Persistent journals, quarantine, restore and retention will be added with the cleanup engine. Read-only scans do not create recoverable changes.",
            _ => "Planned: verified duplicate analysis, supported Windows diagnostics and specialist tools. No registry optimizer, RAM purge or indiscriminate service killing."
        };
        Page.Children.Add(Panel(Text(description, 16)));
        if (section == "Recovery & history") foreach (var line in history.AsEnumerable().Reverse()) Page.Children.Add(Text(line, muted: true));
    }
}

/// <summary>Bounded visual overview. Full inventory remains in the virtualized table.</summary>
internal sealed class Treemap : Grid
{
    private readonly List<Entry> entries;
    private readonly Action<Entry> open;
    private static readonly string[] Palette = ["#C7B7F6", "#B8D9F4", "#DCC7EF", "#B6E2D4", "#F1D6AC", "#CDD3EF"];
    public Treemap(IEnumerable<Entry> source, Action<Entry> action)
    {
        entries = source.Where(e => e.Bytes > 0).OrderByDescending(e => e.Bytes).ToList(); open = action;
        SizeChanged += (_, _) => Draw();
    }
    private void Draw()
    {
        Children.Clear(); if (ActualWidth < 1 || ActualHeight < 1) return;
        if (entries.Count == 0) { Children.Add(new TextBlock { Text = "No measured bytes in this folder.", VerticalAlignment = VerticalAlignment.Center }); return; }
        var displayed = entries.Take(100).ToList();
        if (entries.Count > 100) displayed.Add(new Entry { Path = "Other entries — use table", Bytes = entries.Skip(100).Sum(e => e.Bytes), Status = "Aggregate" });
        var canvas = new Canvas(); Children.Add(canvas); Split(displayed, canvas, 0, 0, ActualWidth, ActualHeight);
    }
    private void Split(List<Entry> items, Canvas canvas, double x, double y, double w, double h)
    {
        if (items.Count == 1)
        {
            var e = items[0]; var type = e.Type; var color = e.IsDirectory ? 0 : type is ".mp4" or ".mkv" or ".mov" ? 1 : type is ".jpg" or ".png" ? 2 : type is ".zip" or ".7z" ? 4 : 5;
            var b = new Button { Width = Math.Max(0, w - 2), Height = Math.Max(0, h - 2), Margin = new(0), Padding = new(3), Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Palette[color])), ToolTip = $"{e.Path}\n{e.Size}", ClipToBounds = true, Content = w > 75 && h > 35 ? new TextBlock { Text = e.Name + "\n" + e.Size, TextTrimming = TextTrimming.CharacterEllipsis, FontSize = 12, TextWrapping = TextWrapping.NoWrap } : null };
            System.Windows.Automation.AutomationProperties.SetName(b, e.Name + " " + e.Size);
            b.Click += (_, _) => { if (e.Status != "Aggregate") open(e); else MessageBox.Show("Smaller items are grouped here. Use the full table to inspect every entry.", "Grouped entries"); };
            Canvas.SetLeft(b, x); Canvas.SetTop(b, y); canvas.Children.Add(b); return;
        }
        var total = items.Sum(e => e.Bytes); long sum = 0; int split = 0;
        while (split < items.Count - 1 && sum < total / 2d) sum += items[split++].Bytes;
        var ratio = (double)sum / total;
        if (w >= h) { Split(items.GetRange(0, split), canvas, x, y, w * ratio, h); Split(items.GetRange(split, items.Count - split), canvas, x + w * ratio, y, w * (1 - ratio), h); }
        else { Split(items.GetRange(0, split), canvas, x, y, w, h * ratio); Split(items.GetRange(split, items.Count - split), canvas, x, y + h * ratio, w, h * (1 - ratio)); }
    }
}
