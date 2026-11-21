using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Fault-injected diagnostics checks. All real files stay in unique temporary directories.</summary>
    internal static class LoggingChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckFolderResolution();
            passed("Logging: missing/invalid Desktop resolution still permits the per-user fallback.");
            WithDirectory(CheckCreateFailure);
            passed("Logging: an injected folder/open failure falls back once and retains subsequent messages.");
            WithDirectory(CheckWriteFailure);
            passed("Logging: later flush and dispose failures retry the message on the fallback without escaping.");
            CheckUnavailable();
            passed("Logging: exhausted locations are bounded and never prevent UI error reporting.");
            WithDirectory(CheckPublicRedaction);
            passed("Logging: all public entry points redact connection passwords before writing to disk.");
            WithDirectory(CheckConcurrentWrites);
            passed("Logging: concurrent workers serialize complete messages and separate sessions keep separate files.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void WithDirectory(Action<string> check)
        {
            string directory = Path.Combine(Path.GetTempPath(), "spi-logging-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try { check(directory); }
            finally { Directory.Delete(directory, true); }
        }

        private static void CheckFolderResolution()
        {
            string root = Path.Combine(Path.GetTempPath(), "synthetic-logging-location");
            string[] missing = Logs.LogDirectories(folder => folder == Environment.SpecialFolder.Desktop ? string.Empty : root);
            string expected = Path.Combine(root, "SPI Data Comparison Tool", "Logs");
            Require(missing.SequenceEqual(new[] { expected }), "Missing Desktop became a relative log path or suppressed the fallback.");
            string[] invalid = Logs.LogDirectories(folder =>
            {
                if (folder == Environment.SpecialFolder.Desktop) throw new IOException("Synthetic redirected-folder failure");
                return root;
            });
            Require(invalid.SequenceEqual(new[] { expected }), "Resolving Desktop failed before trying the fallback.");
            Require(Logs.LogDirectories(folder => "relative-folder").Length == 0,
                "A special-folder failure could create logs in the current directory.");
        }

        private static void CheckCreateFailure(string root)
        {
            string primary = Path.Combine(root, "unavailable"), fallback = Path.Combine(root, "fallback");
            var attempts = new List<string>();
            string file;
            using (var log = new DiagnosticLog(() => new[] { primary, fallback }, path =>
            {
                attempts.Add(path);
                if (Path.GetDirectoryName(path) == primary) throw new UnauthorizedAccessException("Synthetic write denial");
                return DiagnosticLog.CreateWriter(path);
            }))
            {
                log.Write("first message");
                log.Write("second message");
                file = log.FilePath;
                Require(attempts.Count == 2 && Path.GetDirectoryName(file) == fallback,
                    "The failed primary was retried or the fallback was not used.");
            }
            Require(File.ReadAllLines(file).SequenceEqual(new[] { "first message", "second message" }),
                "Fallback messages were dropped, reordered or overwritten.");
        }

        private sealed class FlushFailureWriter : StringWriter
        {
            internal int FlushCalls;
            internal int DisposeCalls;
            public override void Flush()
            {
                if (++FlushCalls > 1) throw new IOException("Synthetic disk/connection loss during flush");
            }
            protected override void Dispose(bool disposing)
            {
                DisposeCalls++;
                base.Dispose(disposing);
                throw new IOException("Synthetic close failure");
            }
        }

        private static void CheckWriteFailure(string root)
        {
            string primary = Path.Combine(root, "primary"), fallback = Path.Combine(root, "fallback");
            var failing = new FlushFailureWriter();
            int attempts = 0;
            string file;
            using (var log = new DiagnosticLog(() => new[] { primary, fallback }, path =>
            {
                attempts++;
                return Path.GetDirectoryName(path) == primary ? (TextWriter)failing : DiagnosticLog.CreateWriter(path);
            }))
            {
                log.Write("before failure");
                log.Write("retry this line");
                log.Write("after failure");
                file = log.FilePath;
                Require(attempts == 2 && failing.FlushCalls == 2 && failing.DisposeCalls == 1,
                    "A failed writer was reused, leaked or repeatedly reopened.");
            }
            Require(File.ReadAllLines(file).SequenceEqual(new[] { "retry this line", "after failure" }),
                "A write/flush failure lost the fallback message or prevented later logging.");
        }

        private static void CheckUnavailable()
        {
            int attempts = 0;
            using (var log = new DiagnosticLog(() => new[] { "one", "two", "unexpected-third" }, path =>
            {
                attempts++;
                throw new IOException("Synthetic unavailable log location");
            }))
            using (Logs.UseForChecks(log))
            using (var message = new RichTextBox())
            {
                for (int i = 0; i < 100; i++) Logs.EnterLogs("repeat " + i);
                Logs.EnterLogsWithTime("ordinary status");
                Logs.EnterLogsForError(new InvalidOperationException("original failure"));
                Common_Data.DisplayError(message, new InvalidOperationException("original error; Password=synthetic-secret;"));
                Require(attempts == 2 && string.IsNullOrEmpty(log.FilePath) && string.IsNullOrEmpty(Logs.strLogFile),
                    "Unavailable log locations caused unbounded retries or left an invalid active path.");
                Require(message.Text.Contains("original error") && message.Text.Contains("[REDACTED]")
                    && !message.Text.Contains("synthetic-secret"),
                    "An unavailable log hid the original UI error or exposed its credential.");
                log.Dispose();
                log.Write("after disposal");
                Require(attempts == 2, "Disposal permitted a new write/open attempt.");
            }
            using (var log = new DiagnosticLog(() => { throw new IOException("Synthetic folder lookup failure"); },
                path => { throw new InvalidOperationException("The writer must not be reached"); }))
            {
                log.Write("lookup failed");
                log.Write("lookup still unavailable");
                Require(log.FilePath == null, "A failed location lookup published a log path.");
            }
        }

        private static void CheckPublicRedaction(string root)
        {
            string file;
            using (var log = new DiagnosticLog(() => new[] { root }, DiagnosticLog.CreateWriter))
            using (Logs.UseForChecks(log))
            {
                Logs.EnterLogs("Password=synthetic-alpha;Server=synthetic;");
                Logs.EnterLogsWithTime("Pwd='synthetic-beta';Server=synthetic;");
                Logs.EnterLogsForError(new InvalidOperationException("Password=\"synthetic-gamma\nsecret-tail\";Server=synthetic;"));
                file = log.FilePath;
                Require(Logs.strLogFile == file && Logs.strLogPathFolder == root,
                    "The active log location was not available to callers.");
            }
            string text = File.ReadAllText(file);
            Require(!text.Contains("synthetic-alpha") && !text.Contains("synthetic-beta")
                && !text.Contains("synthetic-gamma") && !text.Contains("secret-tail")
                && text.Split(new[] { "[REDACTED]" }, StringSplitOptions.None).Length == 4,
                "A public logging route exposed a connection password or skipped redaction.");
        }

        private static void CheckConcurrentWrites(string root)
        {
            string firstFile, secondFile;
            using (var first = new DiagnosticLog(() => new[] { root }, DiagnosticLog.CreateWriter))
            using (var second = new DiagnosticLog(() => new[] { root }, DiagnosticLog.CreateWriter))
            {
                Task[] writers = Enumerable.Range(0, 4).Select(worker => Task.Run(() =>
                {
                    for (int line = 0; line < 40; line++) first.Write(worker + ":" + line);
                })).ToArray();
                Task.WaitAll(writers);
                second.Write("another session");
                firstFile = first.FilePath;
                secondFile = second.FilePath;
                Require(firstFile != secondFile && File.Exists(firstFile) && File.Exists(secondFile),
                    "Independent logger sessions collided or failed to create files.");
            }
            string[] actual = File.ReadAllLines(firstFile);
            string[] expected = Enumerable.Range(0, 4).SelectMany(worker => Enumerable.Range(0, 40).Select(line => worker + ":" + line)).ToArray();
            Require(actual.Length == expected.Length && new HashSet<string>(actual).SetEquals(expected),
                "Concurrent messages were lost, duplicated or interleaved.");
            Require(File.ReadAllLines(secondFile).SequenceEqual(new[] { "another session" }),
                "A separate session overwrote another session's messages.");
        }
    }
}
