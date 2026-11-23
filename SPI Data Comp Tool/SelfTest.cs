using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Test-only modes used by CI to check a finished build without opening the app by hand.
    /// All modes except --check-sql-integration stay offline. CI uses synthetic inputs, temporary test files and output logs;
    /// these command-line modes do nothing in normal use.
    ///   "SPI Data Comp Tool.exe" --list-forms OUT_FILE         writes every window type to OUT_FILE, one per line
    ///   "SPI Data Comp Tool.exe" --open-form TYPE OUT_FILE     creates, shows and closes the window TYPE and adds
    ///                                                          "ok TYPE" or "FAIL TYPE: reason" to OUT_FILE;
    ///                                                          exit code 1 if the window could not be opened
    ///   "SPI Data Comp Tool.exe" --check-assemblies OUT_FILE   loads every library the app uses (embedded in the
    ///                                                          .exe by Costura) and writes ok / FAIL per library;
    ///                                                          exit code 1 if any fails
    ///   "SPI Data Comp Tool.exe" --check-ui OUT_FILE           runs synthetic UI/profile regressions without databases
    ///   "SPI Data Comp Tool.exe" --check-sql-integration OUT_FILE runs real comparisons only on the dedicated synthetic LocalDB instance
    ///   "SPI Data Comp Tool.exe" --measure-comparison OUT_FILE records synthetic comparison timing/memory and verifies row counts
    ///   "SPI Data Comp Tool.exe" --check-startup OUT_FILE      opens every task with synthetic saved-profile states
    ///   "SPI Data Comp Tool.exe" --check-excel-audit OUT_FILE  verifies the separate Excel Sheet Audit using temporary workbooks
    ///   "SPI Data Comp Tool.exe" --check-workspace-exit OUT_FILE tests successful application exit in an isolated process
    ///   "SPI Data Comp Tool.exe" --check-groups OUT_FILE       checks grouped-tool routes and safe workspace closure
    ///                                                          (tests/ToolGroupChecks.cs)
    ///   "SPI Data Comp Tool.exe" --check-steps OUT_FILE        checks the controlled steps (a later step stays locked until
    ///                                                           the one before is done) in the step-by-step windows
    ///   "SPI Data Comp Tool.exe" --check-themes OUT_FILE       builds every window in every colour theme and checks
    ///                                                          that its text stays readable (tests/ThemeChecks.cs)
    ///   "SPI Data Comp Tool.exe" --check-package OUT_FILE packaged|unpackaged  checks the Microsoft Store package
    ///                                                          identity and that a Store copy never self-updates
    /// </summary>
    internal static class SelfTest
    {
        /// <summary>True during explicit checks: normal profiles/preferences are untouched; only the SQL mode opens its dedicated disposable databases.</summary>
        public static bool IsRunning { get; private set; }

        public static bool TryRun(string[] args)
        {
            if (args == null || args.Length < 2) return false;
            string[] commands = { "--check-excel-audit", "--check-updates", "--check-inputs", "--list-forms",
                "--check-assemblies", "--check-ui", "--check-sql-integration", "--measure-comparison", "--check-startup", "--check-workspace-exit",
                "--check-groups", "--check-steps", "--check-themes", "--check-package", "--open-form" };
            if (!commands.Contains(args[0], StringComparer.Ordinal) ||
                (args[0] == "--open-form" && args.Length < 3)) return false;

            IsRunning = true;
            // Checks use their own temporary fixtures and must not read a real user's remembered files.
            // These overrides are in memory only; do not Save or Reset the user's settings here.
            Properties.Settings.Default.ProfilePathUser = string.Empty;
            Properties.Settings.Default.RuleFilePath = string.Empty;
            // An event-handler exception must fail CI rather than leave a modal error dialog waiting.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            if (args[0] == "--check-excel-audit" && args.Length >= 2)
            {
                Environment.ExitCode = ExcelAuditChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--check-updates" && args.Length >= 2)
            {
                Environment.ExitCode = UpdateChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--check-inputs" && args.Length >= 2)
            {
                Environment.ExitCode = InputCheckChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--list-forms" && args.Length >= 2)
            {
                File.WriteAllLines(args[1], FormTypes().Select(t => t.FullName));
                return true;
            }
            if (args[0] == "--check-package" && args.Length >= 3)
            {
                Environment.ExitCode = CheckPackage(args[1], args[2]);
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
            if (args[0] == "--check-sql-integration" && args.Length >= 2)
            {
                Environment.ExitCode = DatabaseIntegrationChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--measure-comparison" && args.Length >= 2)
            {
                Environment.ExitCode = ComparisonPerformanceChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--check-startup" && args.Length >= 2)
            {
                Environment.ExitCode = StartupChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--check-workspace-exit" && args.Length >= 2)
            {
                Environment.ExitCode = WorkspaceUiChecks.RunExitProbe(args[1]);
                return true;
            }
            if (args[0] == "--check-groups" && args.Length >= 2)
            {
                Environment.ExitCode = ToolGroupChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--check-steps" && args.Length >= 2)
            {
                Environment.ExitCode = StepChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--check-themes" && args.Length >= 2)
            {
                Environment.ExitCode = ThemeChecks.Run(args[1]);
                return true;
            }
            if (args[0] == "--open-form" && args.Length >= 3)
            {
                Environment.ExitCode = OpenForm(args[1], args[2]);
                return true;
            }
            IsRunning = false;
            return false;
        }

        /// <summary>Checks the Microsoft Store package identity ("packaged" or "unpackaged" expected) and,
        /// for a Store copy, that the app's own updater refuses to replace the read-only package files.</summary>
        private static int CheckPackage(string outFile, string expected)
        {
            bool packaged = PackagedApp.IsPackaged;
            string reason;
            bool canInstall = new UpdateInstaller().CanInstall(out reason);
            var lines = new System.Collections.Generic.List<string>
            {
                "packaged=" + packaged + (packaged ? " (" + PackagedApp.FullName + ")" : string.Empty),
                "updater can install=" + canInstall + (string.IsNullOrEmpty(reason) ? string.Empty : " (" + reason + ")"),
            };
            string storage = UpdatePreferences.ResolveStorageDirectory();
            string folderKeyed = UpdatePreferences.GetStorageDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            lines.Add("preferences folder=" + storage);
            // Every Store version shares one preferences folder; other copies keep one per install folder.
            bool storageOk = packaged
                ? storage == UpdatePreferences.GetPackageStorageDirectory(PackagedApp.FamilyName) && storage != folderKeyed
                : storage == folderKeyed;
            bool familyOk = PackagedApp.FamilyNameOf("App.Name_1.2.3.0_x64__abc123") == "App.Name_abc123" &&
                PackagedApp.FamilyNameOf("bad") == null;
            bool ok = expected == (packaged ? "packaged" : "unpackaged") && storageOk && familyOk &&
                (!packaged || (!canInstall && reason == PackagedApp.StoreUpdateReason));
            lines.Add(ok ? "ok   package check" : "FAIL package check: expected " + expected);
            File.WriteAllLines(outFile, lines);
            return ok ? 0 : 1;
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
