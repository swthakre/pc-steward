// SPDX-License-Identifier: GPL-3.0-or-later
using Steward.Core;
if (args.Length is < 2 or > 3 || args[0] != "scan") { Console.WriteLine("PC Steward read-only CLI\nUsage: Steward.Cli scan <folder> [report.json|report.csv]\nReports contain full local paths. No files are deleted."); return 2; }
using var cancel = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancel.Cancel(); };
try
{
    var result = new DiskScanner().Scan(args[1], cancel.Token);
    Console.WriteLine($"{result.Status}: {result.Entries.Count:N0} entries, {Sizes.Format(result.Root.Bytes)} logical, {result.Skipped} skipped");
    if (args.Length == 3)
    {
        var extension = Path.GetExtension(args[2]);
        if (extension is not ".json" and not ".csv") throw new ArgumentException("Report must end in .json or .csv");
        using var output = new StreamWriter(new FileStream(args[2], FileMode.CreateNew, FileAccess.Write));
        output.Write(extension == ".csv" ? Reports.Csv(result) : Reports.Json(result));
    }
    return result.Status == "Complete" ? 0 : 3;
}
catch (Exception e) when (e is IOException or UnauthorizedAccessException or ArgumentException) { Console.Error.WriteLine(e.Message); return 1; }
