using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Only synthetic credential-shaped strings; never opens a connection or writes a log file.</summary>
    internal static class CredentialRedactionChecks
    {
        public static void Run(Action<string> passed)
        {
            Require(Common_Data.RedactConnectionCredentials(null) == null, "Null logging input changed.");
            Expect(string.Empty, string.Empty, "empty message");
            Expect("Completed: 12 rows.", "Completed: 12 rows.", "ordinary status");
            Expect("PasswordHint=example;MyPassword=example;PwdCount=2", "PasswordHint=example;MyPassword=example;PwdCount=2", "non-credential property names");
            Expect("Server=demo;Password=synthetic-one;Database=sample", "Server=demo;Password=[REDACTED];Database=sample", "unquoted password");
            Expect("pWd \t= \t'synthetic;one''two';Database=sample", "pWd \t= \t[REDACTED];Database=sample", "single quotes and doubled quote");
            Expect("Password=\"synthetic;one\"\"two\";Database=sample", "Password=[REDACTED];Database=sample", "double quotes and doubled quote");
            Expect("Password=\"synthetic\r\none\"\"two\";Database=sample", "Password=[REDACTED];Database=sample", "quoted multiline password");
            Expect("Password=synthetic\r\none;Database=sample", "Password=[REDACTED];Database=sample", "unquoted multiline password");
            Expect("Password='synthetic\none", "Password=[REDACTED]", "unterminated single quote");
            Expect("Password=\"synthetic\none", "Password=[REDACTED]", "unterminated double quote");
            Expect("Pwd='synthetic;unterminated;tail", "Pwd=[REDACTED]", "unterminated quoted semicolons");
            Expect("Password=\"synthetic;unterminated;tail", "Password=[REDACTED]", "unterminated double-quoted semicolons");
            Expect("Pwd='synthetic'outside;Server=demo", "Pwd=[REDACTED];Server=demo", "malformed quoted suffix");
            Expect("Pwd=one;Password=two;Server=demo", "Pwd=[REDACTED];Password=[REDACTED];Server=demo", "multiple assignments");
            Expect("Pwd='';Password=\"\";Server=demo", "Pwd=[REDACTED];Password=[REDACTED];Server=demo", "empty quoted values");
            Expect("Password=[REDACTED];Server=demo", "Password=[REDACTED];Server=demo", "idempotence");
            Expect("Password=\"synthetic;Pwd=nested\";Server=demo", "Password=[REDACTED];Server=demo", "assignment inside quoted value");
            passed("Credential masking covers case/spacing, quoted and doubled-quote values, multiline/truncated values and multiple assignments without changing ordinary messages.");

            var longValue = new StringBuilder("Password=\"");
            for (int i = 0; i < 20000; i++) longValue.Append("x\"\"");
            longValue.Append("\";Server=demo");
            Expect(longValue.ToString(), "Password=[REDACTED];Server=demo", "long escaped value");
            string prefix = new string('x', 100000) + ": ";
            Expect(prefix + "Pwd=" + new string('y', 100000) + ";Server=demo", prefix + "Pwd=[REDACTED];Server=demo", "long unquoted value");
            passed("Long synthetic credential messages are masked without exposing a trailing value or losing following connection properties.");

            using (var label = new Label())
            {
                label.ForeColor = Color.DarkBlue;
                FormHelpers.ShowText(label, "Rows compared: 12");
                Require(label.Text == "Rows compared: 12" && label.ForeColor == Color.DarkBlue, "Ordinary status display changed.");
                FormHelpers.ShowText(label, "Pwd=synthetic-label;Server=demo", Color.DarkGreen);
                Require(label.Text == "Pwd=[REDACTED];Server=demo" && label.ForeColor == Color.DarkGreen, "Status text was not masked or its requested color changed.");
                FormHelpers.ShowError(label, new InvalidOperationException("Password=\"synthetic-error\"\"tail\";Server=demo"));
                Require(!label.Text.Contains("synthetic-error") && !label.Text.Contains("tail") && label.Text.Contains("Password=[REDACTED];Server=demo"),
                    "Exception rendering exposed a quoted credential tail.");
                FormHelpers.ShowError(label, null);
                Require(label.Text == string.Empty && label.ForeColor == Color.DarkGreen, "Clearing an error changed status-color semantics.");
            }
            using (var fallback = new ThrowOnceControl())
            {
                fallback.ThrowOnNextText = true;
                FormHelpers.ShowError(fallback, new InvalidOperationException("Synthetic display error"));
                Require(fallback.Text == "Pwd=[REDACTED];Server=demo", "Error-display fallback exposed its own credential-shaped message.");
            }
            passed("Shared status and error helpers mask credentials, including fallback errors, while retaining color and clear-message behaviour.");

            CheckFormDiagnosticRoutes();
            passed("Actual comparison/export form error handlers, worker status callbacks and audit logs redact synthetic credentials; Excel comparison errors stay visible without a debugger.");
        }

        private static void CheckFormDiagnosticRoutes()
        {
            const string message = "Synthetic operation failed; Password=\"synthetic-secret;tail\";Server=demo";
            var error = new InvalidOperationException(message);
            string previousPath = Properties.Settings.Default.ProfilePathUser;
            var previousProfiles = Profile.ProfileList;
            try
            {
                // Constructors must not read a user's profiles. SelfTest also disables the remembered-path fallback.
                Properties.Settings.Default.ProfilePathUser = string.Empty;
                using (var log = new DiagnosticLog(() => new string[0], DiagnosticLog.CreateWriter))
                using (Logs.UseForChecks(log))
                {
                    using (var form = new DB_Comparison_Form())
                    {
                        Invoke(form, "DisplayError", new[] { typeof(Exception) }, error);
                        CheckMasked(Field<Control>(form, "lblExpressionStatus"), "database comparison error");
                        Invoke(form, "backgroundWorker1_RunWorkerCompleted",
                            new[] { typeof(object), typeof(RunWorkerCompletedEventArgs) },
                            null, new RunWorkerCompletedEventArgs(null, error, false));
                        CheckMasked(Field<Control>(form, "lblStatus"), "database worker completion");
                    }
                    using (var form = new SQL_DB_To_Excel())
                    {
                        Invoke(form, "DisplayError", new[] { typeof(Exception) }, error);
                        CheckMasked(Field<Control>(form, "lbl_Status"), "database export error");
                        Invoke(form, "DisplayMessage", new[] { typeof(string) }, message);
                        CheckMasked(Field<Control>(form, "lbl_Status"), "database export status");
                    }
                    using (var form = new CustomisedExcel())
                    {
                        Invoke(form, "DisplayError", new[] { typeof(Exception) }, error);
                        CheckMasked(Field<Control>(form, "lbl_Status"), "customised workbook error");
                        Invoke(form, "DisplayMessage", new[] { typeof(string) }, message);
                        CheckMasked(Field<Control>(form, "lbl_Status"), "customised workbook status");
                    }
                    using (var form = new Excel_Excel_Comparison())
                    {
                        Invoke(form, "DisplayError", new[] { typeof(Exception) }, error);
                        CheckMasked(Field<Control>(form, "lblError"), "Excel comparison error without a debugger");
                        Invoke(form, "backgroundWorker1_ProgressChanged",
                            new[] { typeof(object), typeof(ProgressChangedEventArgs) },
                            null, new ProgressChangedEventArgs(105, message));
                        CheckMasked(Field<Control>(form, "lbl_Status"), "Excel worker status");
                    }
                    using (var form = new ViewForMultiReference())
                    {
                        Label label = Field<Label>(form, "lbl_Status");
                        Invoke(form, "DisplayError", new[] { typeof(Label), typeof(Exception), typeof(string) },
                            label, error, null);
                        CheckMasked(label, "multi-reference error");
                        Invoke(form, "DisplayMessage", new[] { typeof(Label), typeof(string) }, label, message);
                        CheckMasked(label, "multi-reference status");
                    }
                    using (var form = new ExcelSheetAuditForm())
                    {
                        Invoke(form, "SetStatus", new[] { typeof(string), typeof(bool) }, message, true);
                        CheckMasked(Field<Control>(form, "phase"), "Excel audit status");
                        Invoke(form, "AppendLog", new[] { typeof(string) }, message);
                        CheckMasked(Field<Control>(form, "log"), "Excel audit log");
                    }
                    using (var form = new WorkbookIdentityForm())
                    {
                        Invoke(form, "SetStatus", new[] { typeof(string), typeof(bool), typeof(bool) }, message, true, true);
                        CheckMasked(Field<Control>(form, "status"), "workbook identity status");
                        Invoke(form, "AppendLog", new[] { typeof(string) }, message);
                        CheckMasked(Field<Control>(form, "log"), "workbook identity log");
                    }
                }
            }
            finally
            {
                Properties.Settings.Default.ProfilePathUser = previousPath;
                Profile.ProfileList = previousProfiles;
            }
        }

        private static void CheckMasked(Control control, string scenario)
        {
            Require(control.Text.Contains("Synthetic operation failed") && control.Text.Contains("[REDACTED]")
                && !control.Text.Contains("synthetic-secret") && !control.Text.Contains("tail"),
                "A diagnostic was missing or contained a synthetic credential: " + scenario);
        }

        private static T Field<T>(Form form, string name) where T : class
        {
            FieldInfo field = form.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(field != null, "Missing form diagnostic control: " + name);
            return (T)field.GetValue(form);
        }

        private static void Invoke(Form form, string name, Type[] signature, params object[] arguments)
        {
            MethodInfo method = form.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic,
                null, signature, null);
            Require(method != null, "Missing form diagnostic handler: " + name);
            method.Invoke(form, arguments);
        }

        private static void Expect(string input, string expected, string scenario)
        {
            Require(Common_Data.RedactConnectionCredentials(input) == expected, "Credential masking failed: " + scenario);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class ThrowOnceControl : Control
        {
            internal bool ThrowOnNextText { get; set; }
            public override string Text
            {
                get { return base.Text; }
                set
                {
                    if (ThrowOnNextText)
                    {
                        ThrowOnNextText = false;
                        throw new InvalidOperationException("Pwd=synthetic-fallback;Server=demo");
                    }
                    base.Text = value;
                }
            }
        }
    }
}
