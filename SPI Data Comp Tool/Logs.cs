using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Best-effort diagnostics. A log failure must never prevent an operation or its error message.</summary>
    public static class Logs
    {
        private static readonly object sync = new object();
        private static DiagnosticLog sink = new DiagnosticLog(DefaultDirectories, DiagnosticLog.CreateWriter);

        // Retained for callers that display the current log location. No unused data-capture file is created.
        public static string strLogPathFolder = string.Empty, strLogFile = string.Empty,
            counter = string.Empty, strDataCaptureFile = string.Empty;

        private static string[] DefaultDirectories()
        {
            // Build checks use injected temporary sinks; other checks must not create real user logs.
            if (SelfTest.IsRunning) return new string[0];
            return LogDirectories(Environment.GetFolderPath);
        }

        internal static string[] LogDirectories(Func<Environment.SpecialFolder, string> getFolder)
        {
            var directories = new List<string>();
            // Preserve the familiar location when available, then try a private per-user fallback.
            AddDirectory(directories, getFolder, Environment.SpecialFolder.Desktop, "LogFolder");
            AddDirectory(directories, getFolder, Environment.SpecialFolder.LocalApplicationData,
                "SPI Data Comparison Tool", "Logs");
            return directories.ToArray();
        }

        private static void AddDirectory(List<string> directories, Func<Environment.SpecialFolder, string> getFolder,
            Environment.SpecialFolder folder, params string[] parts)
        {
            try
            {
                string path = getFolder(folder);
                // An unavailable special folder must not turn into a relative/current-drive path.
                if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path)) return;
                foreach (string part in parts) path = Path.Combine(path, part);
                directories.Add(path);
            }
            catch (Exception)
            {
                // Resolving one folder must not prevent the next candidate from being tried.
            }
        }

        public static void EnterLogs(string strLog)
        {
            try { Write(Common_Data.RedactConnectionCredentials(strLog)); }
            catch (Exception) { /* Diagnostics, including credential filtering, are best effort. */ }
        }

        public static void EnterLogsWithTime(string strLog)
        {
            try
            {
                strLog = DateTime.Now.ToString("T", CultureInfo.CurrentCulture) + " : " + strLog;
                Write(Common_Data.RedactConnectionCredentials(strLog));
            }
            catch (Exception) { /* Never replace the original operation/error with a logging failure. */ }
        }

        public static void EnterLogsForError(Exception exc)
        {
            try
            {
                string strLog = "++++++++++++++" + Environment.NewLine + DateTime.Now.ToString("T", CultureInfo.CurrentCulture)
                    + " : " + (exc == null ? "Unknown error" : exc.ToString()) + Environment.NewLine + "++++++++++++";
                Write(Common_Data.RedactConnectionCredentials(strLog));
            }
            catch (Exception) { /* Do not retry with an unfiltered exception or show another dialog. */ }
        }

        private static void Write(string message)
        {
            lock (sync)
            {
                sink.Write(message);
                strLogFile = sink.FilePath ?? string.Empty;
                strLogPathFolder = strLogFile.Length == 0 ? string.Empty : Path.GetDirectoryName(strLogFile);
            }
        }

        /// <summary>Isolates public logging/error routes under an explicitly supplied synthetic sink.</summary>
        internal static IDisposable UseForChecks(DiagnosticLog replacement)
        {
            if (replacement == null) throw new ArgumentNullException(nameof(replacement));
            lock (sync)
            {
                var restore = new RestoreSink(sink, strLogPathFolder, strLogFile);
                sink = replacement;
                strLogPathFolder = strLogFile = string.Empty;
                return restore;
            }
        }

        private sealed class RestoreSink : IDisposable
        {
            private DiagnosticLog previous;
            private readonly string directory, file;
            internal RestoreSink(DiagnosticLog previous, string directory, string file)
            {
                this.previous = previous;
                this.directory = directory;
                this.file = file;
            }
            public void Dispose()
            {
                lock (sync)
                {
                    if (previous == null) return;
                    sink.Dispose();
                    sink = previous;
                    previous = null;
                    strLogPathFolder = directory;
                    strLogFile = file;
                }
            }
        }
    }

    /// <summary>
    /// One serialized writer per session. Each of at most two locations is attempted once;
    /// exhausted locations disable logging until restart. Failed lines are retried only on the next location.
    /// </summary>
    internal sealed class DiagnosticLog : IDisposable
    {
        private readonly object sync = new object();
        private readonly Func<string[]> getDirectories;
        private readonly Func<string, TextWriter> createWriter;
        private string[] directories;
        private string fileName;
        private int nextDirectory;
        private TextWriter writer;
        private string filePath;
        private bool disposed;

        internal DiagnosticLog(Func<string[]> getDirectories, Func<string, TextWriter> createWriter)
        {
            this.getDirectories = getDirectories ?? throw new ArgumentNullException(nameof(getDirectories));
            this.createWriter = createWriter ?? throw new ArgumentNullException(nameof(createWriter));
        }

        internal string FilePath { get { lock (sync) return filePath; } }

        internal static TextWriter CreateWriter(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            // A unique filename and CreateNew prevent concurrent app instances from sharing/truncating a file.
            return new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read));
        }

        internal void Write(string message)
        {
            lock (sync)
            {
                if (disposed) return;
                if (directories == null)
                {
                    directories = new string[0];
                    try
                    {
                        directories = (getDirectories() ?? new string[0]).Where(path => !string.IsNullOrWhiteSpace(path))
                            .Distinct(StringComparer.OrdinalIgnoreCase).Take(2).ToArray();
                        fileName = "Log_" + DateTime.Today.ToString("dd_MM_yyyy", CultureInfo.InvariantCulture)
                            + "_" + Guid.NewGuid().ToString("N") + ".log";
                    }
                    catch (Exception) { directories = new string[0]; }
                }
                while (writer != null || nextDirectory < directories.Length)
                {
                    try
                    {
                        if (writer == null)
                        {
                            string directory = directories[nextDirectory++];
                            string candidate = Path.Combine(directory, fileName);
                            writer = createWriter(candidate);
                            if (writer == null) throw new IOException("The diagnostic writer is unavailable.");
                            filePath = candidate;
                        }
                        writer.WriteLine(message);
                        writer.Flush();
                        return;
                    }
                    catch (Exception)
                    {
                        CloseWriter();
                        filePath = null;
                        // No recursive logging, modal UI, raw exception output, or retry of this location.
                    }
                }
            }
        }

        private void CloseWriter()
        {
            TextWriter previous = writer;
            writer = null;
            if (previous == null) return;
            try { previous.Dispose(); }
            catch (Exception) { /* Flush/close failures must not escape either. */ }
        }

        public void Dispose()
        {
            lock (sync)
            {
                if (disposed) return;
                disposed = true;
                CloseWriter();
            }
        }
    }
}
