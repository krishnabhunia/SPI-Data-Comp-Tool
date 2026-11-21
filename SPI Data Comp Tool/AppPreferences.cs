using System;
using System.Configuration;
using System.IO;
using System.Security;
using System.Threading;

namespace SPI_Data_Comp_Tool
{
    internal enum ExcelWriterMode { EPPlus, MicrosoftExcel }

    internal sealed class AppPreferenceValues
    {
        internal string RuleFilePath;
        internal ExcelWriterMode ExcelWriter;
        internal string Warning;
    }

    // Only the optional comparison preferences use this boundary. The updater continues to
    // require a successfully persisted snapshot before allowing installation.
    internal interface IAppPreferenceStore
    {
        string RuleFilePath { get; set; }
        bool EPPlusExcelSave { get; set; }
        bool MicrosoftExcelSave { get; set; }
        void Save();
    }

    internal static class AppPreferences
    {
        private sealed class SettingsStore : IAppPreferenceStore
        {
            public string RuleFilePath
            {
                get { return Properties.Settings.Default.RuleFilePath; }
                set { Properties.Settings.Default.RuleFilePath = value; }
            }
            public bool EPPlusExcelSave
            {
                get { return Properties.Settings.Default.EPPlus_Excel_Save; }
                set { Properties.Settings.Default.EPPlus_Excel_Save = value; }
            }
            public bool MicrosoftExcelSave
            {
                get { return Properties.Settings.Default.XL_Excel_Save; }
                set { Properties.Settings.Default.XL_Excel_Save = value; }
            }
            public void Save()
            {
                if (!SelfTest.IsRunning) Properties.Settings.Default.Save();
            }
        }

        private static IAppPreferenceStore store = new SettingsStore();
        private const string ReadWarning = "Saved application preferences could not be read. " +
            "Unavailable rules are left unselected. If the Excel output preference is unavailable, EPPlus is used. " +
            "Your settings files have not been reset or changed.";
        private const string SaveWarning = "This preference could not be saved. " +
            "Check access to your settings file before trying again. Your settings files have not been reset.";

        internal static string RuleFilePath
        {
            get { string warning; return ReadRules(store, out warning); }
        }

        internal static ExcelWriterMode ExcelWriter
        {
            get { string warning; return ReadWriter(store, out warning); }
        }

        internal static AppPreferenceValues Read()
        {
            IAppPreferenceStore current = store;
            string rulesWarning, writerWarning;
            string rules = ReadRules(current, out rulesWarning);
            ExcelWriterMode writer = ReadWriter(current, out writerWarning);
            return new AppPreferenceValues
            {
                RuleFilePath = rules,
                ExcelWriter = writer,
                Warning = string.IsNullOrEmpty(rulesWarning) ? writerWarning : rulesWarning,
            };
        }

        private static string ReadRules(IAppPreferenceStore current, out string warning)
        {
            warning = string.Empty;
            try { return current.RuleFilePath ?? string.Empty; }
            catch (Exception error) when (IsUnavailable(error))
            {
                warning = ReadWarning;
                return string.Empty;
            }
        }

        private static ExcelWriterMode ReadWriter(IAppPreferenceStore current, out string warning)
        {
            warning = string.Empty;
            try
            {
                bool epplus = current.EPPlusExcelSave;
                bool excel = current.MicrosoftExcelSave;
                if (epplus == excel)
                    warning = "The saved Excel output mode is inconsistent. EPPlus will be used for this session; " +
                        "your settings files have not been changed.";
                // A damaged pair must never select neither branch and report success without a file.
                return !epplus && excel ? ExcelWriterMode.MicrosoftExcel : ExcelWriterMode.EPPlus;
            }
            catch (Exception error) when (IsUnavailable(error))
            {
                warning = ReadWarning;
                return ExcelWriterMode.EPPlus;
            }
        }

        private static bool IsUnavailable(Exception error)
        {
            return error is ConfigurationErrorsException || error is IOException ||
                error is UnauthorizedAccessException || error is SecurityException ||
                error is SettingsPropertyWrongTypeException || error is SettingsPropertyNotFoundException ||
                error is FormatException || error is InvalidCastException ||
                (error is TypeInitializationException && error.InnerException != null && IsUnavailable(error.InnerException));
        }

        internal static bool TrySaveRulesPath(string path, out string warning)
        {
            IAppPreferenceStore current = store;
            string previous = null;
            bool havePrevious = false;
            try
            {
                previous = current.RuleFilePath;
                havePrevious = true;
                current.RuleFilePath = path;
                current.Save();
                warning = string.Empty;
                return true;
            }
            catch (Exception)
            {
                // Restoration is in memory only. A broken provider can also reject setters;
                // do not retry Save, reset the file, or let restoration hide the original failure.
                if (havePrevious) TryRestore(() => current.RuleFilePath = previous);
                warning = SaveWarning;
                return false;
            }
        }

        internal static bool TrySaveExcelWriter(ExcelWriterMode writer, out string warning)
        {
            if (!Enum.IsDefined(typeof(ExcelWriterMode), writer)) throw new ArgumentOutOfRangeException(nameof(writer));
            IAppPreferenceStore current = store;
            bool previousEpplus = true, previousExcel = false, havePrevious = false;
            try
            {
                previousEpplus = current.EPPlusExcelSave;
                previousExcel = current.MicrosoftExcelSave;
                havePrevious = true;
                current.EPPlusExcelSave = writer == ExcelWriterMode.EPPlus;
                current.MicrosoftExcelSave = writer == ExcelWriterMode.MicrosoftExcel;
                current.Save();
                warning = string.Empty;
                return true;
            }
            catch (Exception)
            {
                if (havePrevious)
                {
                    TryRestore(() => current.EPPlusExcelSave = previousEpplus);
                    TryRestore(() => current.MicrosoftExcelSave = previousExcel);
                }
                warning = SaveWarning;
                return false;
            }
        }

        private static void TryRestore(Action restore)
        {
            try { restore(); }
            catch (Exception) { /* Best effort only; never writes or repairs the settings file. */ }
        }

        internal static IDisposable UseForChecks(IAppPreferenceStore replacement)
        {
            if (replacement == null) throw new ArgumentNullException(nameof(replacement));
            return new StoreScope(replacement);
        }

        private sealed class StoreScope : IDisposable
        {
            private readonly IAppPreferenceStore previous;
            private bool disposed;
            internal StoreScope(IAppPreferenceStore replacement) { previous = Interlocked.Exchange(ref store, replacement); }
            public void Dispose()
            {
                if (disposed) return;
                disposed = true;
                Interlocked.Exchange(ref store, previous);
            }
        }
    }
}
