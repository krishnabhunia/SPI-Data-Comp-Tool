using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Test-only mode used by CI to take a picture of every window, so layout changes can be checked
    /// without opening the app by hand. It never connects to a database or opens a file.
    ///   "SPI Data Comp Tool.exe" --list-forms OUT_FILE         writes every form type to OUT_FILE, one per line
    ///   "SPI Data Comp Tool.exe" --screenshot TYPE OUT_FOLDER  saves TYPE.png (designed size) and
    ///                                                          TYPE_large.png (stretched to 1920 x 1040)
    ///   "SPI Data Comp Tool.exe" --check-assemblies OUT_FILE   loads every library the app uses (embedded in the
    ///                                                          .exe by Costura) and writes ok / FAIL per library;
    ///                                                          exit code 1 if any fails
    /// </summary>
    internal static class FormScreenshots
    {
        public static bool TryRun(string[] args)
        {
            if (args == null || args.Length == 0) return false;
            if (args[0] == "--list-forms" && args.Length >= 2)
            {
                File.WriteAllLines(args[1], FormTypes().Select(t => t.FullName));
                return true;
            }
            if (args[0] == "--check-assemblies" && args.Length >= 2)
            {
                Environment.ExitCode = CheckAssemblies(args[1]);
                return true;
            }
            if (args[0] == "--screenshot" && args.Length >= 3)
            {
                Environment.ExitCode = Capture(args[1], args[2]);
                return true;
            }
            return false;
        }

        /// <summary>Loads the app's references and theirs, all the way down, the same way the app would.</summary>
        private static int CheckAssemblies(string outFile)
        {
            var lines = new System.Collections.Generic.List<string>();
            var seen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new System.Collections.Generic.Queue<AssemblyName>(Assembly.GetExecutingAssembly().GetReferencedAssemblies());
            int failed = 0;
            while (queue.Count > 0)
            {
                AssemblyName name = queue.Dequeue();
                if (!seen.Add(name.Name)) continue;
                try
                {
                    Assembly loaded = Assembly.Load(name);
                    bool inGac = loaded.GlobalAssemblyCache;
                    lines.Add($"ok   {name.Name} {loaded.GetName().Version}{(inGac ? " (framework)" : string.IsNullOrEmpty(loaded.Location) ? " (embedded)" : " (file: " + loaded.Location + ")")}");
                    if (!inGac)
                    {
                        foreach (AssemblyName child in loaded.GetReferencedAssemblies()) queue.Enqueue(child);
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    lines.Add($"FAIL {name.FullName}: {ex.Message}");
                }
            }
            File.WriteAllLines(outFile, lines);
            return failed == 0 ? 0 : 1;
        }

        private static Type[] FormTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Form)) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private static int Capture(string typeName, string outFolder)
        {
            var type = FormTypes().FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
            if (type == null) return 2;
            Directory.CreateDirectory(outFolder);
            using (var form = (Form)Activator.CreateInstance(type))
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(0, 0);
                form.ShowInTaskbar = false;
                form.WindowState = FormWindowState.Normal;
                form.Show();
                Settle();
                Save(form, Path.Combine(outFolder, type.Name + ".png"));

                // what a maximised window on a 1080p screen would show
                form.Size = new Size(1920, 1040);
                Settle();
                Save(form, Path.Combine(outFolder, type.Name + "_large.png"));
                form.Close();
            }
            return 0;
        }

        private static void Settle()
        {
            for (int i = 0; i < 5; i++) { Application.DoEvents(); System.Threading.Thread.Sleep(80); }
        }

        private static void Save(Form form, string path)
        {
            using (var bmp = new Bitmap(Math.Max(1, form.Width), Math.Max(1, form.Height)))
            {
                form.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save(path, ImageFormat.Png);
            }
        }
    }
}
