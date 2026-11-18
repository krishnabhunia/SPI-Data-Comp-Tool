using System;
using System.Drawing;
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
