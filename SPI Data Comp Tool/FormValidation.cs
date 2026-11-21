using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>An expected input error, associated with the field the user can correct.</summary>
    internal sealed class ValidationFailure : ArgumentException
    {
        internal Control Target { get; }

        internal ValidationFailure(Control target, string message) : base(message)
        {
            Target = target;
        }
    }

    /// <summary>
    /// Shared action-boundary validation. Does not open databases, mutate inputs, create output
    /// files, suppress unexpected failures, or use modal dialogs. Every action must revalidate;
    /// enabled buttons and an earlier successful check are not proof that inputs are still valid.
    /// </summary>
    internal static class FormValidation
    {
        private static readonly ConditionalWeakTable<Control, Feedback> feedback =
            new ConditionalWeakTable<Control, Feedback>();

        internal static bool Check(Control owner, Action validation, Action<string> report = null)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (validation == null) throw new ArgumentNullException(nameof(validation));
            if (owner.IsDisposed || owner.Disposing) return false;
            Feedback state = feedback.GetValue(owner, control => new Feedback(control));
            state.Clear();
            try
            {
                validation();
                return true;
            }
            catch (ValidationFailure failure)
            {
                string message = Common_Data.RedactConnectionCredentials(failure.Message);
                state.Show(failure.Target ?? owner, message);
                report?.Invoke(message);
                return false;
            }
        }

        internal static string LastMessage(Control owner)
        {
            Feedback state;
            return owner != null && feedback.TryGetValue(owner, out state) ? state.Message : string.Empty;
        }

        internal static void Require(bool condition, Control field, string message)
        {
            if (!condition) Reject(field, message);
        }

        internal static void Reject(Control field, string message)
        {
            throw new ValidationFailure(field, message);
        }

        internal static string Required(Control field, string label)
        {
            string value = field == null ? null : field.Text;
            Require(!string.IsNullOrWhiteSpace(value), field, "Enter " + label + ".");
            return value;
        }

        internal static int Port(Control field, string label)
        {
            string value = Required(field, label).Trim();
            int port;
            Require(int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out port)
                && port >= 1 && port <= 65535, field, label + " must be a whole number from 1 to 65535.");
            return port;
        }

        internal static void Selection(ListControl field, string label)
        {
            Require(field != null && field.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(field.Text),
                field, "Select " + label + " from the loaded list.");
        }

        internal static void Table(DataTable table, Control field, string label, bool requireRows = true)
        {
            Require(table != null && table.Columns.Count > 0, field, "Load " + label + " with column headers first.");
            if (requireRows) Require(table.Rows.Count > 0, field, label + " contains no data rows.");
        }

        internal static void ExcelTable(DataTable table, Control field, string label)
        {
            Table(table, field, label, false);
            Require(table.Columns.Count <= 16384 && table.Rows.Count <= 1048575, field,
                label + " exceeds Excel's worksheet limits (16,384 columns or 1,048,575 data rows plus a header).");
        }

        /// <summary>Validate the first worksheet before accepting an imported workbook.</summary>
        internal static void ValidateWorkbook(DataSet candidate, Control field)
        {
            Require(candidate != null && candidate.Tables.Count > 0, field,
                "The workbook contains no readable worksheets.");
            Table(candidate.Tables[0], field, "First worksheet");
        }

        internal static void Workbook(DataSet data, Control field, string label)
        {
            Require(data != null && data.Tables.Count > 0, field, "Load " + label + " before exporting.");
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataTable table in data.Tables)
            {
                ExcelTable(table, field, label);
                WorksheetName(table.TableName, field);
                Require(names.Add(table.TableName), field, "Excel worksheet names must be unique, ignoring letter case.");
            }
        }

        internal static string WorksheetName(string name, Control field)
        {
            Require(!string.IsNullOrWhiteSpace(name) && name.Length <= 31 && !name.Any(char.IsControl)
                && name.IndexOfAny(new[] { '[', ']', ':', '*', '?', '/', '\\', '\0', '\r', '\n' }) < 0
                && !name.StartsWith("'", StringComparison.Ordinal) && !name.EndsWith("'", StringComparison.Ordinal),
                field, "A worksheet name must have 1–31 characters, no [ ] : * ? / \\, and no leading/trailing apostrophe.");
            return name;
        }

        internal static string WorksheetNameFromFile(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName ?? string.Empty);
            name = new string(name.Select(character => char.IsControl(character) ? '_' : character).ToArray());
            foreach (char character in new[] { '[', ']', ':', '*', '?', '/', '\\', '\0', '\r', '\n' })
                name = name.Replace(character, '_');
            name = name.Trim().Trim('\'');
            if (name.Length > 31) name = name.Substring(0, 31).TrimEnd('\'');
            return name.Length == 0 ? "Output" : name;
        }

        /// <summary>Publish an output only after its complete write succeeds, preserving an existing destination on failure.</summary>
        internal static void WriteAtomically(string path, Action<string> writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            string destination = OutputFile(path, null, "output file");
            bool replaceExisting = File.Exists(destination);
            string temporary = Path.Combine(Path.GetDirectoryName(destination), ".spi-" + Guid.NewGuid().ToString("N")
                + Path.GetExtension(destination));
            bool ownedTemporary = false;
            try
            {
                // Reserve our own unique file. Do not overwrite someone else's temporary output.
                using (new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { }
                ownedTemporary = true;
                writer(temporary);
                if (!File.Exists(temporary) || new FileInfo(temporary).Length == 0)
                    throw new IOException("The generated output is empty; the existing destination was not replaced.");
                if (replaceExisting) File.Replace(temporary, destination, null);
                else File.Move(temporary, destination);
            }
            finally
            {
                if (ownedTemporary && File.Exists(temporary))
                {
                    try { File.Delete(temporary); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
        }

        internal static void Columns(DataTable table, IEnumerable<string> columns, Control field, string label)
        {
            Table(table, field, label, false);
            var names = columns == null ? new string[0] : columns.ToArray();
            Require(names.Length > 0 && names.All(name => !string.IsNullOrWhiteSpace(name)), field,
                "Select at least one " + label + " column.");
            Require(names.Distinct(StringComparer.OrdinalIgnoreCase).Count() == names.Length, field,
                "Select each " + label + " column only once.");
            foreach (string name in names)
                Require(table.Columns.Cast<DataColumn>().Count(column =>
                    string.Equals(column.ColumnName, name, StringComparison.OrdinalIgnoreCase)) == 1, field,
                    "The selected " + label + " columns no longer match the loaded data. Select them again.");
        }

        internal static void IsBusy(BackgroundWorker worker, Control field)
        {
            Require(worker == null || !worker.IsBusy, field, "Wait for the current operation to finish before starting another.");
        }

        /// <summary>Dispose at the beginning of completion (or on a failed start), then set final action availability.</summary>
        internal static IDisposable LockInputs(Control owner, params Control[] keepEnabled)
        {
            return new InputLock(owner, keepEnabled ?? new Control[0]);
        }

        internal static void StartWorker(Control owner, BackgroundWorker worker, ref IDisposable inputLock,
            params Control[] keepEnabled)
        {
            if (worker == null) throw new ArgumentNullException(nameof(worker));
            IsBusy(worker, owner);
            Require(inputLock == null, owner, "Wait for the previous operation to finish updating the window.");
            inputLock = LockInputs(owner, keepEnabled);
            try { worker.RunWorkerAsync(); }
            catch
            {
                inputLock.Dispose();
                inputLock = null;
                throw;
            }
        }

        internal static string ReadableFile(string path, Control field, string label, params string[] extensions)
        {
            string fullPath = FilePath(path, field, label, extensions);
            try
            {
                Require(File.Exists(fullPath), field, "Select an existing " + label + " file.");
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    Require(stream.Length > 0, field, label + " is empty. Select a file containing data.");
            }
            catch (Exception ex) when (IsFileFailure(ex))
            {
                Reject(field, "Cannot read " + label + ". Check access permissions and close any application locking the file.");
            }
            return fullPath;
        }

        internal static string ExistingDirectory(string path, Control field, string label)
        {
            Require(!string.IsNullOrWhiteSpace(path), field, "Select " + label + ".");
            string fullPath = CanonicalPath(path, field, label);
            Require(Directory.Exists(fullPath), field, "Select an existing " + label + " folder.");
            return fullPath;
        }

        internal static string OutputFile(string path, Control field, string label, params string[] extensions)
        {
            string fullPath = FilePath(path, field, label, extensions);
            Require(!Directory.Exists(fullPath), field, label + " must name a file, not a folder.");
            Require(Directory.Exists(Path.GetDirectoryName(fullPath)), field,
                "Select an existing destination folder for " + label + ".");
            try
            {
                if (File.Exists(fullPath))
                {
                    Require((File.GetAttributes(fullPath) & FileAttributes.ReadOnly) == 0, field,
                        label + " is read-only. Select another destination.");
                    // Open, never truncate. Actual writers must still handle a later access failure.
                    using (new FileStream(fullPath, FileMode.Open, FileAccess.Write, FileShare.None)) { }
                }
            }
            catch (Exception ex) when (IsFileFailure(ex))
            {
                Reject(field, "Cannot write " + label + ". Check access permissions and close any application locking the file.");
            }
            return fullPath;
        }

        internal static void DifferentFiles(string output, string input, Control field)
        {
            if (string.IsNullOrWhiteSpace(input)) return;
            Require(!string.Equals(CanonicalPath(output, field, "output file"),
                CanonicalPath(input, field, "input file"), StringComparison.OrdinalIgnoreCase), field,
                "Choose a different output file. The input file must not be overwritten.");
        }

        internal static string FileName(string name, Control field, string label)
        {
            Require(!string.IsNullOrWhiteSpace(name) && name != "." && name != ".." &&
                name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !name.EndsWith(".", StringComparison.Ordinal)
                && !name.EndsWith(" ", StringComparison.Ordinal), field,
                label + " contains an invalid file name. Remove path separators, trailing spaces/dots and reserved characters.");
            string stem = name.Split('.')[0].TrimEnd(' ').ToUpperInvariant();
            bool reserved = stem == "CON" || stem == "PRN" || stem == "AUX" || stem == "NUL"
                || stem == "CONIN$" || stem == "CONOUT$"
                || (stem.Length == 4 && (stem.StartsWith("COM", StringComparison.Ordinal)
                    || stem.StartsWith("LPT", StringComparison.Ordinal))
                    && ((stem[3] >= '1' && stem[3] <= '9') || "¹²³".IndexOf(stem[3]) >= 0));
            Require(!reserved, field, label + " uses a reserved Windows device name. Choose another name.");
            return name;
        }

        private static string FilePath(string path, Control field, string label, string[] extensions)
        {
            Require(!string.IsNullOrWhiteSpace(path), field, "Select " + label + ".");
            string fullPath = CanonicalPath(path, field, label);
            FileName(Path.GetFileName(fullPath), field, label);
            Require(extensions == null || extensions.Length == 0 || extensions.Any(extension =>
                string.Equals(Path.GetExtension(fullPath), extension.StartsWith(".", StringComparison.Ordinal)
                    ? extension : "." + extension, StringComparison.OrdinalIgnoreCase)), field,
                "Select a supported " + label + " file (" + string.Join(", ", extensions ?? new string[0]) + ").");
            return fullPath;
        }

        private static string CanonicalPath(string path, Control field, string label)
        {
            try
            {
                // Trimming can redirect an output to a different existing file; reject instead.
                Require(!string.IsNullOrWhiteSpace(path) && path == path.Trim() && path.IndexOf('\0') < 0,
                    field, label + " has an invalid path or leading/trailing whitespace.");
                Require(!path.StartsWith(@"\\?\", StringComparison.Ordinal) && !path.StartsWith(@"\\.\", StringComparison.Ordinal),
                    field, "Select a regular file or folder path for " + label + ".");
                // Check before GetFullPath: .NET Framework can normalize trailing dots/spaces and
                // silently redirect a chosen file. Ordinary relative . and .. navigation is valid.
                string[] parts = path.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                int first = path.StartsWith(@"\\", StringComparison.Ordinal) || path.StartsWith("//", StringComparison.Ordinal) ? 2 : 0;
                for (int index = first; index < parts.Length; index++)
                {
                    string part = parts[index];
                    if (part == "." || part == ".." || (index == 0 && part.Length == 2 && char.IsLetter(part[0]) && part[1] == ':')) continue;
                    FileName(part, field, label);
                }
                return Path.GetFullPath(path);
            }
            catch (Exception ex) when (ex is ArgumentException && !(ex is ValidationFailure)
                || ex is NotSupportedException || ex is PathTooLongException || ex is SecurityException)
            {
                Reject(field, label + " has an invalid or unsupported path.");
                return null;
            }
        }

        private static bool IsFileFailure(Exception ex)
        {
            return ex is IOException || ex is UnauthorizedAccessException || ex is SecurityException;
        }

        private sealed class InputLock : IDisposable
        {
            private readonly List<KeyValuePair<Control, bool>> states = new List<KeyValuePair<Control, bool>>();
            private bool disposed;

            internal InputLock(Control owner, Control[] keepEnabled)
            {
                Capture(owner, keepEnabled);
                foreach (var state in states)
                    if (!state.Key.IsDisposed) state.Key.Enabled = false;
            }

            private void Capture(Control owner, Control[] keepEnabled)
            {
                // Enabled is inherited from parents. Do not turn an inherited disabled state into
                // a permanent local one when the ancestor is later enabled.
                if (owner == null || owner.IsDisposed || !owner.Enabled || keepEnabled.Contains(owner)) return;
                bool input = owner is ButtonBase || owner is ComboBox || owner is ListBox
                    || owner is ListView || owner is TreeView || owner is NumericUpDown || owner is DateTimePicker
                    || (owner is TextBoxBase && !((TextBoxBase)owner).ReadOnly)
                    || (owner is DataGridView && !((DataGridView)owner).ReadOnly);
                if (input)
                {
                    states.Add(new KeyValuePair<Control, bool>(owner, owner.Enabled));
                    return;
                }
                foreach (Control child in owner.Controls) Capture(child, keepEnabled);
            }

            public void Dispose()
            {
                if (disposed) return;
                disposed = true;
                foreach (var state in states)
                    if (!state.Key.IsDisposed && !state.Key.Disposing) state.Key.Enabled = state.Value;
                states.Clear();
            }
        }

        private sealed class Feedback : IDisposable
        {
            private readonly ErrorProvider errors;
            private Control target;
            private string description;
            internal string Message { get; private set; } = string.Empty;

            internal Feedback(Control owner)
            {
                errors = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };
                errors.ContainerControl = owner as ContainerControl ?? owner.FindForm();
                owner.Disposed += (sender, args) => Dispose();
            }

            internal void Clear()
            {
                Message = string.Empty;
                errors.Clear();
                if (target != null && !target.IsDisposed)
                {
                    target.TextChanged -= Corrected;
                    target.AccessibleDescription = description;
                }
                target = null;
                description = null;
            }

            internal void Show(Control field, string message)
            {
                Message = message;
                if (field == null || field.IsDisposed) return;
                target = field;
                description = field.AccessibleDescription;
                field.AccessibleDescription = message;
                field.TextChanged += Corrected;
                errors.SetError(field, message);
                // Reveal the affected tab first; validation must not trap keyboard navigation.
                var pages = new List<TabPage>();
                for (Control parent = field; parent != null; parent = parent.Parent)
                    if (parent is TabPage) pages.Add((TabPage)parent);
                for (int index = pages.Count - 1; index >= 0; index--)
                    if (pages[index].Parent is TabControl) ((TabControl)pages[index].Parent).SelectedTab = pages[index];
                if (field.CanFocus) field.Focus();
            }

            private void Corrected(object sender, EventArgs args) { Clear(); }
            public void Dispose() { Clear(); errors.Dispose(); }
        }
    }
}
