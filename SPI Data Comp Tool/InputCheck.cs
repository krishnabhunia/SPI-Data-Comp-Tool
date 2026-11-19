using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Checks a window's inputs before an action starts, the same way in every window:
    ///   if (!InputCheck.For(this).Connection(connection.Source).Required(tb_Table, "Table name").Report()) return;
    /// A wrong input gets a mark beside its field (hovering shows the reason), every reason is listed in the
    /// window's log, the first wrong field gets the cursor - and the action does not start.
    /// </summary>
    internal sealed class InputCheck
    {
        private static readonly ConditionalWeakTable<Form, ErrorProvider> Providers = new ConditionalWeakTable<Form, ErrorProvider>();
        private static readonly ConditionalWeakTable<Form, ToolTip> Tips = new ConditionalWeakTable<Form, ToolTip>();
        private static readonly Regex NamePattern = new Regex(@"^[A-Za-z_#@\[][A-Za-z0-9_#@$.\[\]]*$");

        private readonly Form form;
        private readonly List<KeyValuePair<Control, string>> problems = new List<KeyValuePair<Control, string>>();

        private InputCheck(Form form)
        {
            this.form = form;
        }

        /// <summary>Starts a check for the window that holds <paramref name="control"/> (or is it).</summary>
        public static InputCheck For(Control control)
        {
            Form form = control as Form ?? (control == null ? null : control.FindForm());
            return new InputCheck(form);
        }

        public bool Passed { get { return problems.Count == 0; } }

        /// <summary>The reasons found so far, in the order the checks ran.</summary>
        public IList<string> Messages { get { return problems.Select(p => p.Value).ToList(); } }

        /// <summary>The fields that failed, first one first.</summary>
        public IList<Control> Fields { get { return problems.Select(p => p.Key).Distinct().ToList(); } }

        // ---------- the checks ----------

        /// <summary>A free check: when <paramref name="ok"/> is false, <paramref name="message"/> is shown at the field.</summary>
        public InputCheck That(bool ok, Control field, string message)
        {
            if (!ok) problems.Add(new KeyValuePair<Control, string>(field, message));
            return this;
        }

        /// <summary>The field must not be empty (spaces alone count as empty). Disabled or hidden fields are skipped.</summary>
        public InputCheck Required(Control field, string name)
        {
            if (!InUse(field)) return this;
            return That(!IsEmpty(field), field, name + " is empty.");
        }

        /// <summary>A number from <paramref name="min"/> to <paramref name="max"/>. An empty field is reported as empty.</summary>
        public InputCheck Number(Control field, string name, int min, int max)
        {
            if (!InUse(field)) return this;
            if (IsEmpty(field)) return That(false, field, name + " is empty.");
            int value;
            bool ok = int.TryParse(field.Text.Trim(), out value) && value >= min && value <= max;
            return That(ok, field, name + " must be a number from " + min + " to " + max + " (it is " + field.Text.Trim() + ").");
        }

        public InputCheck Port(Control field, string name)
        {
            return Number(field, name, 1, 65535);
        }

        /// <summary>The field must name a file that exists; <paramref name="extensions"/> (".xlsx", ...) limits the type.</summary>
        public InputCheck FileExists(Control field, string name, params string[] extensions)
        {
            if (!InUse(field)) return this;
            return FilePath(field, field.Text, name, extensions);
        }

        /// <summary>Like <see cref="FileExists"/> for a path the window keeps in a variable; the mark goes on <paramref name="field"/>.</summary>
        public InputCheck FilePath(Control field, string path, string name, params string[] extensions)
        {
            path = (path ?? string.Empty).Trim().Trim('"');
            if (path.Length == 0) return That(false, field, name + " is not chosen.");
            if (!SafeExists(path, File.Exists)) return That(false, field, name + " was not found: " + path);
            if (extensions != null && extensions.Length > 0 && !extensions.Any(e => path.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                return That(false, field, name + " must be a " + string.Join(" or ", extensions) + " file.");
            return this;
        }

        public InputCheck FolderExists(Control field, string name)
        {
            if (!InUse(field)) return this;
            return FolderPath(field, field.Text, name);
        }

        public InputCheck FolderPath(Control field, string path, string name)
        {
            path = (path ?? string.Empty).Trim().Trim('"');
            if (path.Length == 0) return That(false, field, name + " is not chosen.");
            return That(SafeExists(path, Directory.Exists), field, name + " was not found: " + path);
        }

        /// <summary>A table, view, schema or column name: letters, digits, _ # @ $ and dots, not starting with a digit.</summary>
        public InputCheck ValidName(Control field, string name)
        {
            if (!InUse(field)) return this;
            if (IsEmpty(field)) return That(false, field, name + " is empty.");
            return That(NamePattern.IsMatch(field.Text.Trim()), field, name + " has characters a name cannot have: " + field.Text.Trim());
        }

        /// <summary>A file name (not a path) Windows accepts.</summary>
        public InputCheck ValidFileName(Control field, string name)
        {
            if (!InUse(field)) return this;
            if (IsEmpty(field)) return That(false, field, name + " is empty.");
            string text = field.Text.Trim();
            char[] bad = text.Where(c => Path.GetInvalidFileNameChars().Contains(c)).Distinct().ToArray();
            return That(bad.Length == 0, field, name + " cannot contain " + string.Join(" ", bad.Select(c => c.ToString()).ToArray()));
        }

        /// <summary>A list, drop-down or grid must have something chosen (ticked, selected, or - for a grid - rows).</summary>
        public InputCheck AnyChosen(Control field, string name)
        {
            if (!InUse(field)) return this;
            bool ok;
            CheckedListBox ticks = field as CheckedListBox;
            ListBox list = field as ListBox;
            ComboBox combo = field as ComboBox;
            DataGridView grid = field as DataGridView;
            if (ticks != null) ok = ticks.CheckedItems.Count > 0;
            else if (list != null) ok = list.SelectedItems.Count > 0;
            else if (combo != null) ok = combo.SelectedIndex >= 0 || combo.Text.Trim().Length > 0;
            else if (grid != null) ok = grid.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow);
            else ok = !IsEmpty(field);
            return That(ok, field, "Nothing is chosen in " + name + ".");
        }

        /// <summary>
        /// One half of a connection, with the fields its platform needs. Oracle: port, data source, database, user
        /// name, password. SQL Server: data source and database, plus user name and password unless Windows
        /// authentication is ticked.
        /// </summary>
        public InputCheck Connection(string side, bool oracle, bool windowsLogin, Control port, Control dataSource,
                                     Control database, Control user, Control password)
        {
            if (oracle && port != null) Port(port, side + " port");
            Required(dataSource, side + " data source");
            Required(database, side + " database");
            if (oracle || !windowsLogin)
            {
                Required(user, side + " username");
                if (password != null && InUse(password) && IsEmpty(password))
                    That(false, password, side + " password is empty." + (oracle ? string.Empty : " Fill it in, or tick Windows authentication."));
            }
            return this;
        }

        public InputCheck Connection(ConnectionSide side)
        {
            return Connection(side.Title, side.Oracle.Checked, side.WindowsLogin.Checked, side.Port, side.DataSource,
                              side.Database, side.User, side.Password);
        }

        // ---------- showing the result ----------

        /// <summary>
        /// Shows the result and returns true when every check passed. On problems: a mark beside each wrong field,
        /// the reasons in the window's log (or beside the first field when the window has no log), the count in
        /// <paramref name="status"/> when given, and the cursor in the first wrong field. Earlier marks are cleared.
        /// </summary>
        public bool Report(Control status = null)
        {
            if (form == null || form.IsDisposed) return Passed;
            ErrorProvider provider = Providers.GetValue(form, CreateProvider);
            provider.Clear();
            if (Passed) return true;

            foreach (Control field in Fields)
            {
                if (field == null) continue;
                string reasons = string.Join(Environment.NewLine, problems.Where(p => ReferenceEquals(p.Key, field)).Select(p => p.Value).ToArray());
                // inside the field's right edge, so no window layout has to change (left of a drop-down's arrow)
                provider.SetIconAlignment(field, ErrorIconAlignment.MiddleRight);
                provider.SetIconPadding(field, field is ComboBox ? -40 : -20);
                provider.SetError(field, reasons);
            }

            string summary = Summary();
            RichTextBox log = FindLog();
            if (log != null)
            {
                foreach (string message in Messages) Common_Data.DisplayMessage(log, message, true);
                Common_Data.DisplayMessage(log, summary, true);
            }
            if (status != null) FormHelpers.ShowText(status, summary, Color.FromArgb(180, 35, 24));

            Control first = Fields.FirstOrDefault(f => f != null && f.CanFocus);
            if (first != null)
            {
                first.Focus();
                if (log == null && status == null)
                {
                    // no log and no status line in this window: say the reason right at the field
                    ToolTip tip = Tips.GetValue(form, f => new ToolTip { IsBalloon = false });
                    tip.Show(problems[0].Value + (problems.Count > 1 ? "  (+" + (problems.Count - 1) + " more)" : string.Empty), first, 0, first.Height + 2, 6000);
                }
            }
            return false;
        }

        /// <summary>"3 inputs need fixing. Nothing was started."</summary>
        public string Summary()
        {
            if (Passed) return string.Empty;
            return (problems.Count == 1 ? "1 input needs" : problems.Count + " inputs need") + " fixing. Nothing was started.";
        }

        /// <summary>Removes the marks of an earlier check from the window.</summary>
        public static void Clear(Control control)
        {
            Form form = control as Form ?? (control == null ? null : control.FindForm());
            ErrorProvider provider;
            if (form != null && Providers.TryGetValue(form, out provider)) provider.Clear();
        }

        /// <summary>The reason shown at <paramref name="field"/>'s mark, or an empty text when the field has no mark.</summary>
        public static string ReasonAt(Control field)
        {
            Form form = field == null ? null : field.FindForm();
            ErrorProvider provider;
            return form != null && Providers.TryGetValue(form, out provider) ? provider.GetError(field) : string.Empty;
        }

        private static ErrorProvider CreateProvider(Form form)
        {
            var provider = new ErrorProvider { ContainerControl = form, BlinkStyle = ErrorBlinkStyle.NeverBlink };
            form.Disposed += (s, e) => provider.Dispose();
            return provider;
        }

        private RichTextBox FindLog()
        {
            Control[] named = form.Controls.Find("rtb_Log", true);
            if (named.Length > 0 && named[0] is RichTextBox && named[0].Visible) return (RichTextBox)named[0];
            return null;
        }

        private static bool InUse(Control field)
        {
            return field != null && field.Enabled;
        }

        private static bool IsEmpty(Control field)
        {
            return field == null || string.IsNullOrWhiteSpace(field.Text);
        }

        private static bool SafeExists(string path, Func<string, bool> exists)
        {
            try
            {
                return exists(path);
            }
            catch (Exception)
            {
                return false;   // a path Windows cannot even read is not there
            }
        }
    }
}
