// SPDX-License-Identifier: GPL-3.0-or-later
using Steward.Core;
using System.Text.Json;
var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Steward-tests-" + Guid.NewGuid());
Directory.CreateDirectory(root);
var failures = 0;
void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
void Test(string name, Action action) { try { action(); Console.WriteLine("PASS " + name); } catch (Exception e) { failures++; Console.WriteLine("FAIL " + name + ": " + e.Message); } }
try
{
    Directory.CreateDirectory(Path.Combine(root, "资料", "nested"));
    Directory.CreateDirectory(Path.Combine(root, "empty"));
    File.WriteAllBytes(Path.Combine(root, "a.bin"), new byte[1024]);
    File.WriteAllBytes(Path.Combine(root, "资料", "nested", "b.txt"), new byte[3072]);
    File.WriteAllText(Path.Combine(root, "=formula.csv"), "abc");
    var scanner = new DiskScanner();
    Test("nested Unicode totals and empty folders", () => {
        var r = scanner.Scan(root);
        Check(r.Root.Bytes == 4099, "Incorrect byte total");
        Check(r.Entries.Count(e => !e.IsDirectory) == 3, "Files missing");
        Check(r.Entries.Single(e => e.Name == "empty").Bytes == 0, "Empty directory incorrect");
        Check(r.Status == "Complete", "Unexpected partial scan");
    });
    Test("cancelled scan is not complete", () => {
        using var cts = new CancellationTokenSource(); cts.Cancel();
        Check(scanner.Scan(root, cts.Token).Status == "Cancelled", "Cancelled reported complete");
    });
    Test("entry cap bounds memory and reports partial", () => {
        var r = scanner.Scan(root, entryLimit: 2);
        Check(r.Entries.Count <= 2 && r.Status == "Limited", "Cap not enforced");
    });
    Test("missing root fails explicitly", () => {
        try { scanner.Scan(Path.Combine(root, "missing")); throw new Exception("Missing accepted"); }
        catch (DirectoryNotFoundException) { }
    });
    Test("limited scan does not label unvisited directories as read", () => {
        var fixture = Path.Combine(root, "coverage");
        Directory.CreateDirectory(Path.Combine(fixture, "one")); Directory.CreateDirectory(Path.Combine(fixture, "two"));
        File.WriteAllText(Path.Combine(fixture, "one", "content"), "pending");
        try { var r = scanner.Scan(fixture, entryLimit: 2);
        Check(r.Entries.Where(e => e.IsDirectory && e != r.Root).All(e => e.Status != "Read"), "Unvisited directory labelled Read"); }
        finally { Directory.Delete(fixture, true); }
    });
    Test("new inventory request supersedes older request", () => {
        var gate = new RequestGate(); var older = gate.Next(); var newer = gate.Next();
        Check(!gate.IsCurrent(older) && gate.IsCurrent(newer), "Stale response accepted");
    });
    Test("report JSON avoids object cycles", () => {
        using var json = JsonDocument.Parse(Reports.Json(scanner.Scan(root)));
        Check(json.RootElement.GetProperty("status").GetString() == "Complete", "Missing coverage");
        Check(json.RootElement.GetProperty("entries").GetArrayLength() >= 6, "Missing entries");
    });
    Test("CSV quotes filenames and neutralizes formula cells", () => {
        var csv = Reports.Csv(scanner.Scan(root));
        Check(csv.Contains("\"'=formula.csv\""), "Spreadsheet formula not neutralized");
        Check(csv.Contains("Complete"), "Missing coverage");
    });
    Test("directory link is never traversed", () => {
        var link = Path.Combine(root, "loop");
        var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe") {
            Arguments = $"/c mklink /J \"{link}\" \"{root}\"", CreateNoWindow = true, UseShellExecute = false,
            RedirectStandardOutput = true, RedirectStandardError = true });
        p!.WaitForExit(); Check(p.ExitCode == 0, "Junction fixture creation failed");
        try { var r = scanner.Scan(root); Check(r.Skipped == 1 && r.Status == "Partial", "Link not reported"); Check(r.Root.Bytes == 4099, "Link traversed"); }
        finally { Directory.Delete(link); }
    });
}
finally { Directory.Delete(root, true); }
Console.WriteLine($"Failures: {failures}");
return failures == 0 ? 0 : 1;
