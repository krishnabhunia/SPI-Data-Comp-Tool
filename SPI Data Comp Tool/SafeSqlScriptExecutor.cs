using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// A deliberately restrictive gate for spreadsheet-provided SQL, not a general SQL parser
    /// or a replacement for database-side least-privilege permissions.
    /// </summary>
    internal static class SqlScriptSafety
    {
        private static readonly Regex IdentifierPart = new Regex(
            @"\A[A-Za-z_][A-Za-z0-9_#$]*\z", RegexOptions.CultureInvariant);

        private sealed class Word
        {
            internal string Text;
            internal int Depth;
        }

        internal static string RequireSafeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("A table or column identifier is missing.");

            var parts = identifier.Trim().Split('.');
            if (parts.Length > 3 || parts.Any(part => !IdentifierPart.IsMatch(part)))
                throw new ArgumentException("Only simple, unquoted table and column names (optionally schema-qualified) are supported.");

            return string.Join(".", parts);
        }

        internal static string SqlLiteral(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            var text = Convert.ToString(value).Trim();
            return text.Length == 0 ? "NULL" : "N'" + text.Replace("'", "''") + "'";
        }

        internal static string SqlWhereEquals(string column, object value)
        {
            var safeColumn = RequireSafeIdentifier(column);
            var literal = SqlLiteral(value);
            return literal == "NULL" ? safeColumn + " IS NULL" : safeColumn + " = " + literal;
        }

        internal static void ValidateWrite(string sql)
        {
            var words = Scan(sql);
            if (words.Count < 2)
                throw new ArgumentException("Only single INSERT, UPDATE or DELETE statements are permitted.");

            var verb = words[0].Text;
            if (verb == "INSERT")
            {
                if (words[1].Text != "INTO")
                    throw new ArgumentException("INSERT must use INSERT INTO.");
            }
            else if (verb == "UPDATE" || verb == "DELETE")
            {
                if (verb == "DELETE" && words[1].Text != "FROM")
                    throw new ArgumentException("DELETE must use DELETE FROM.");
                if (!words.Any(word => word.Text == "WHERE" && word.Depth == 0))
                    throw new ArgumentException("UPDATE and DELETE require a top-level WHERE clause.");
            }
            else
            {
                throw new ArgumentException("Only single INSERT, UPDATE or DELETE statements are permitted.");
            }

            RejectKeywords(words, "EXEC", "EXECUTE", "CALL", "MERGE", "DROP", "TRUNCATE",
                "CREATE", "ALTER", "GRANT", "REVOKE", "DENY", "COMMIT", "ROLLBACK",
                "BEGIN", "DECLARE", "WAITFOR", "BULK", "OPENROWSET", "OPENQUERY",
                "OPENDATASOURCE", "XP_CMDSHELL", "SP_EXECUTESQL", "DBMS_SQL", "UTL_FILE");
        }

        internal static void ValidateSelect(string sql)
        {
            var words = Scan(sql);
            if (words.Count == 0 || words[0].Text != "SELECT")
                throw new ArgumentException("Only a single SELECT statement is permitted for exports.");

            RejectKeywords(words, "INTO", "EXEC", "EXECUTE", "CALL", "OPENROWSET",
                "OPENQUERY", "OPENDATASOURCE", "NEXTVAL", "XP_CMDSHELL",
                "SP_EXECUTESQL", "DBMS_SQL", "UTL_FILE");
            for (var i = 0; i < words.Count - 1; i++)
            {
                if (words[i].Text == "FOR" && words[i + 1].Text == "UPDATE")
                    throw new ArgumentException("SELECT FOR UPDATE is not permitted.");
                if (i < words.Count - 2 &&
                    words[i].Text == "NEXT" && words[i + 1].Text == "VALUE" && words[i + 2].Text == "FOR")
                    throw new ArgumentException("Sequence-advancing SELECT statements are not permitted.");
            }
        }

        private static void RejectKeywords(IEnumerable<Word> words, params string[] rejected)
        {
            var disallowed = new HashSet<string>(rejected, StringComparer.Ordinal);
            if (words.Any(word => disallowed.Contains(word.Text)))
                throw new ArgumentException("The script contains a disallowed SQL operation.");
        }

        private static List<Word> Scan(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("The SQL script is empty.");

            var words = new List<Word>();
            var depth = 0;
            var semicolon = false;
            for (var i = 0; i < sql.Length;)
            {
                var character = sql[i];
                if (char.IsWhiteSpace(character)) { i++; continue; }
                if (semicolon)
                    throw new ArgumentException("Multiple statements are not permitted.");
                if (character == '-' && i + 1 < sql.Length && sql[i + 1] == '-' ||
                    character == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
                    throw new ArgumentException("SQL comments are not supported in spreadsheet scripts.");
                if (character == '\'')
                {
                    i++;
                    var closed = false;
                    while (i < sql.Length)
                    {
                        if (sql[i++] != '\'') continue;
                        if (i < sql.Length && sql[i] == '\'') { i++; continue; }
                        closed = true;
                        break;
                    }
                    if (!closed) throw new ArgumentException("Unterminated SQL string literal.");
                    continue;
                }
                if (character == '"' || character == '[')
                {
                    var closing = character == '[' ? ']' : '"';
                    i++;
                    var closed = false;
                    while (i < sql.Length)
                    {
                        if (sql[i++] != closing) continue;
                        if (i < sql.Length && sql[i] == closing) { i++; continue; }
                        closed = true;
                        break;
                    }
                    if (!closed) throw new ArgumentException("Unterminated quoted SQL identifier.");
                    continue;
                }
                if (character == ';') { semicolon = true; i++; continue; }
                if (character == '(') { depth++; i++; continue; }
                if (character == ')')
                {
                    if (--depth < 0) throw new ArgumentException("Unbalanced SQL parentheses.");
                    i++;
                    continue;
                }
                if (char.IsLetter(character) || character == '_')
                {
                    var start = i++;
                    while (i < sql.Length && (char.IsLetterOrDigit(sql[i]) || sql[i] == '_' || sql[i] == '$' || sql[i] == '#'))
                        i++;
                    words.Add(new Word { Text = sql.Substring(start, i - start).ToUpperInvariant(), Depth = depth });
                    continue;
                }
                i++;
            }

            if (depth != 0) throw new ArgumentException("Unbalanced SQL parentheses.");
            return words;
        }
    }

    internal sealed class SqlScriptExecutionResult
    {
        internal int CompletedScripts { get; set; }
        internal long RowsAffected { get; set; }
        internal bool AuditCompleted { get; set; }
    }

    internal static class SqlScriptAudit
    {
        internal static void Write(string runId, string phase, string target, int scriptCount, long rows)
        {
            // No connection strings, SQL text, login names, exception details or passwords.
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SPIDataComparisonTool", "Audit", "sql-script-runs.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string fingerprint;
            using (var hash = SHA256.Create())
                fingerprint = BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(target))).Replace("-", "");
            File.AppendAllText(path, string.Join(",", DateTime.UtcNow.ToString("o"), runId,
                phase, fingerprint, scriptCount.ToString(), rows.ToString()) + Environment.NewLine);
        }

        internal static bool TryWrite(string runId, string phase, string target, int scriptCount, long rows)
        {
            try { Write(runId, phase, target, scriptCount, rows); return true; }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }
    }

    internal static class SafeSqlScriptExecutor
    {
        internal static SqlScriptExecutionResult Execute(
            string dbType, string connectionString, string target, IList<string> scripts,
            string runId, IProgress<int> progress, CancellationToken cancellationToken)
        {
            if (scripts == null || scripts.Count == 0)
                throw new ArgumentException("No SQL statements were selected.");
            foreach (var sql in scripts)
                SqlScriptSafety.ValidateWrite(sql); // Validate the entire immutable batch before opening a connection.

            cancellationToken.ThrowIfCancellationRequested();
            // Fail closed if the local audit destination cannot be written before any data changes.
            SqlScriptAudit.Write(runId, "START", target, scripts.Count, 0);

            var result = new SqlScriptExecutionResult();
            using (DbConnection connection = CreateConnection(dbType, connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        foreach (var script in scripts)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = script;
                                command.CommandTimeout = 60;
                                using (cancellationToken.Register(() => { try { command.Cancel(); } catch (Exception) { } }))
                                {
                                    var affected = command.ExecuteNonQuery();
                                    result.RowsAffected += Math.Max(0, affected);
                                }
                            }
                            result.CompletedScripts++;
                            if (progress != null)
                                progress.Report((int)((long)result.CompletedScripts * 100 / scripts.Count));
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        transaction.Commit();
                    }
                    catch
                    {
                        try { transaction.Rollback(); }
                        catch (Exception) { /* Connection loss may prevent rollback confirmation. */ }
                        SqlScriptAudit.TryWrite(runId, cancellationToken.IsCancellationRequested ? "CANCELLED" : "ROLLBACK_ATTEMPTED",
                            target, result.CompletedScripts, result.RowsAffected);
                        throw;
                    }
                }
            }

            result.AuditCompleted = SqlScriptAudit.TryWrite(runId, "COMMITTED", target,
                result.CompletedScripts, result.RowsAffected);
            return result;
        }

        private static DbConnection CreateConnection(string dbType, string connectionString)
        {
            if (string.Equals(dbType, "sql", StringComparison.OrdinalIgnoreCase))
                return new SqlConnection(connectionString);
            if (string.Equals(dbType, "oracle", StringComparison.OrdinalIgnoreCase))
                return new OracleConnection(connectionString);
            throw new ArgumentException("Unsupported database type.");
        }
    }

    internal sealed class SqlScriptPreviewForm : Form
    {
        internal SqlScriptPreviewForm(string target, IList<string> scripts)
        {
            Text = "Review SQL batch — no changes made yet";
            ClientSize = new Size(900, 655);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = true;
            MinimumSize = new Size(700, 500);

            var heading = new Label {
                Text = "TARGET: " + target + Environment.NewLine +
                    "Statements: " + scripts.Count + " (INSERT / UPDATE / DELETE only). " +
                    "Preview does not contact the database.",
                Location = new Point(18, 15), Size = new Size(860, 55),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            var content = new StringBuilder();
            var count = Math.Min(scripts.Count, 50);
            for (var i = 0; i < count; i++)
            {
                content.AppendLine("========== Statement " + (i + 1) + " ==========");
                content.AppendLine(scripts[i].Length > 4000
                    ? scripts[i].Substring(0, 4000) + " … [truncated]"
                    : scripts[i]);
                content.AppendLine();
            }
            if (scripts.Count > count)
                content.AppendLine((scripts.Count - count) + " further statements not shown. Review the complete source Excel file.");

            var preview = new TextBox {
                ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Both,
                WordWrap = false, Font = new Font("Consolas", 9),
                Text = content.ToString(), Location = new Point(18, 80),
                Size = new Size(864, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            var instruction = new Label {
                Text = "For database changes, type EXECUTE:",
                Location = new Point(18, 548), Size = new Size(225, 23),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            var confirmation = new TextBox {
                Location = new Point(244, 544), Width = 130,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            var previewOnly = new Button {
                Text = "Preview only / Cancel", DialogResult = DialogResult.Cancel,
                Location = new Point(533, 588), Size = new Size(170, 30),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            var execute = new Button {
                Text = "Execute batch", DialogResult = DialogResult.OK,
                Enabled = false, Location = new Point(715, 588),
                Size = new Size(167, 30), Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            confirmation.TextChanged += (sender, args) =>
                execute.Enabled = string.Equals(confirmation.Text, "EXECUTE", StringComparison.Ordinal);
            Controls.AddRange(new Control[] { heading, preview, instruction, confirmation, previewOnly, execute });
            CancelButton = previewOnly;
            AcceptButton = previewOnly; // Enter defaults to cancelling; execution requires an explicit click.
        }
    }

    internal sealed class SqlScriptProgressForm : Form
    {
        private readonly Func<IProgress<int>, CancellationToken, Task<SqlScriptExecutionResult>> work;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private readonly ProgressBar progressBar = new ProgressBar();
        private readonly Label progressLabel = new Label();
        private readonly Button cancelButton = new Button();
        private bool running = true;

        internal SqlScriptExecutionResult Result { get; private set; }
        internal Exception Error { get; private set; }

        internal SqlScriptProgressForm(Func<IProgress<int>, CancellationToken, Task<SqlScriptExecutionResult>> executeWork)
        {
            work = executeWork;
            Text = "SQL batch in progress — transaction not yet committed";
            ClientSize = new Size(510, 145);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ControlBox = true;
            MinimizeBox = false;
            MaximizeBox = false;
            progressLabel.Text = "Starting… use Cancel to request rollback.";
            progressLabel.Location = new Point(16, 15);
            progressLabel.Size = new Size(480, 26);
            progressBar.Location = new Point(16, 52);
            progressBar.Size = new Size(478, 23);
            cancelButton.Text = "Cancel and rollback";
            cancelButton.Location = new Point(340, 92);
            cancelButton.Size = new Size(154, 29);
            cancelButton.Click += (sender, args) => RequestCancellation();
            Controls.AddRange(new Control[] { progressLabel, progressBar, cancelButton });
            FormClosing += (sender, args) => {
                if (!running) return;
                RequestCancellation();
                args.Cancel = true; // Remain open until background execution confirms its outcome.
            };
            Shown += async (sender, args) => {
                var progress = new Progress<int>(percent => {
                    progressBar.Value = Math.Max(0, Math.Min(100, percent));
                    progressLabel.Text = progressBar.Value + "% processed (not committed until complete)";
                });
                var outcome = DialogResult.Abort;
                try
                {
                    Result = await work(progress, cancellation.Token);
                    outcome = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    Error = ex;
                    outcome = ex is OperationCanceledException ? DialogResult.Cancel : DialogResult.Abort;
                }
                finally
                {
                    running = false;
                    DialogResult = outcome;
                    Close();
                }
            };
        }

        private void RequestCancellation()
        {
            if (cancellation.IsCancellationRequested) return;
            cancellation.Cancel();
            cancelButton.Enabled = false;
            progressLabel.Text = "Cancellation requested; rolling back if possible…";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) cancellation.Dispose();
            base.Dispose(disposing);
        }
    }
}
