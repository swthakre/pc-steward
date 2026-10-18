// SPDX-License-Identifier: GPL-3.0-or-later
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Steward.Core;
using Steward.App;
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var output = args.Length > 0 ? args[0] : "artifacts/ui-check"; Directory.CreateDirectory(output);
        var fixture = Path.Combine(Path.GetTempPath(), "Steward-ui-" + Guid.NewGuid());
        Directory.CreateDirectory(Path.Combine(fixture, "Videos", "Projects"));
        File.WriteAllBytes(Path.Combine(fixture, "Videos", "Projects", "clip.mp4"), new byte[16384]);
        try
        {
            var app = new Steward.App.App(); app.InitializeComponent();
            var window = new MainWindow();
            var view = (FrameworkElement)window.Content;
            void Pump() => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            void Render(string name)
            {
                view.Measure(new Size(1220, 850)); view.Arrange(new Rect(0, 0, 1220, 850)); view.UpdateLayout(); Pump();
                var image = new RenderTargetBitmap(1220, 850, 96, 96, PixelFormats.Pbgra32); image.Render(view);
                var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(image));
                using var stream = File.Create(Path.Combine(output, name + ".png")); encoder.Save(stream);
            }
            void Call(string name, params object[] values) => typeof(MainWindow).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(window, values);
            void Set(string name, object value) => typeof(MainWindow).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(window, value);
            Render("overview");
            var scan = new DiskScanner().Scan(fixture); Set("scan", scan); Set("current", scan.Root);
            Call("Show", "Disk Map"); Render("disk-map");
            var tree = Descendants<TreeView>(view).Single(); var root = (TreeViewItem)tree.Items[0]; root.IsExpanded = true; Pump();
            var videos = (TreeViewItem)root.Items[0]; videos.IsExpanded = true; videos.IsSelected = true; Pump();
            view.UpdateLayout(); Pump();
            var rebuilt = Descendants<TreeView>(view).Single(); var rebuiltRoot = (TreeViewItem)rebuilt.Items[0];
            if (!rebuiltRoot.IsExpanded || !((TreeViewItem)rebuiltRoot.Items[0]).IsExpanded) throw new Exception("Expanded tree state lost");
            var location = (Entry)typeof(MainWindow).GetField("current", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
            if (location.Name != "Videos") throw new Exception("Tree selection did not navigate");
            Console.WriteLine("PASS native WPF render, folder tree navigation and expansion preservation");
            foreach (var section in new[] { "Cleaner", "Apps & services", "Security", "Health & updates", "Privacy", "Automation", "Recovery & history", "Advanced tools" }) { Call("Show", section); Render(section.Replace(" ", "-")); }
            Console.WriteLine("PASS all ten native views construct and render");
            app.Shutdown(); return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
        finally { Directory.Delete(fixture, true); }
    }
    private static IEnumerable<T> Descendants<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        { var child = VisualTreeHelper.GetChild(parent, i); if (child is T found) yield return found; foreach (var nested in Descendants<T>(child)) yield return nested; }
    }
}
