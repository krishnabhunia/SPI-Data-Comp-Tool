using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Test-only modes used by CI to check a finished build without opening the app by hand.
    /// They never connect to a database or open a file, and do nothing in normal use.
    ///   "SPI Data Comp Tool.exe" --list-forms OUT_FILE         writes every window type to OUT_FILE, one per line
    ///   "SPI Data Comp Tool.exe" --open-form TYPE OUT_FILE     creates, shows and closes the window TYPE and adds
    ///                                                          "ok TYPE" or "FAIL TYPE: reason" to OUT_FILE;
    ///                                                          exit code 1 if the window could not be opened
    ///   "SPI Data Comp Tool.exe" --check-assemblies OUT_FILE   loads every library the app uses (embedded in the
    ///                                                          .exe by Costura) and writes ok / FAIL per library;
    ///                                                          exit code 1 if any fails
    ///   "SPI Data Comp Tool.exe" --check-ui OUT_FILE           runs synthetic UI/profile regressions without databases
    /// </summary>
    internal static class SelfTest
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
            if (args[0] == "--check-ui" && args.Length >= 2)
            {
                Environment.ExitCode = UiWorkflowChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--open-form" && args.Length >= 3)
            {
                Environment.ExitCode = OpenForm(args[1], args[2]);
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

        /// <summary>Creates, shows and closes one window, the way the menu would open it.</summary>
        private static int OpenForm(string typeName, string outFile)
        {
            var type = FormTypes().FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
            if (type == null)
            {
                File.AppendAllText(outFile, "FAIL " + typeName + ": no such window" + Environment.NewLine);
                return 1;
            }
            try
            {
                using (var form = (Form)Activator.CreateInstance(type))
                {
                    form.ShowInTaskbar = false;
                    form.Show();
                    for (int i = 0; i < 5; i++) { Application.DoEvents(); System.Threading.Thread.Sleep(80); }
                    form.Close();
                }
                File.AppendAllText(outFile, "ok " + type.Name + Environment.NewLine);
                return 0;
            }
            catch (Exception ex)
            {
                Exception cause = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                File.AppendAllText(outFile, "FAIL " + type.Name + ": " + cause.GetType().Name + " - " + cause.Message.Replace("\r", " ").Replace("\n", " ") + Environment.NewLine);
                return 1;
            }
        }
    }
}
