using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // The copied helper must never open the normal app or touch normal startup preferences.
            if (UpdateApplyWorker.TryRun(args)) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Registering 
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            #endregion

            // CI-only checks of a finished build (see SelfTest.cs); does nothing in normal use
            if (SelfTest.TryRun(args)) return;

            try { UpdatePreferences.Initialize(); }
            catch (Exception ex)
            {
                MessageBox.Show("Update preferences could not be loaded safely. Existing files were not overwritten. " +
                    "You can continue using SPI, but update installation is disabled for this session." +
                    Environment.NewLine + Environment.NewLine + Common_Data.RedactConnectionCredentials(ex.Message),
                    "Update preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Logs.EnterLogs($"\n=========== Logs Entry Started at {Common_Data.AppendDateInOutputFileName()} ==================================\n");

            try { Application.Run(new MainForm()); }
            finally
            {
                try { UpdatePreferences.SaveCurrent(); }
                catch (Exception ex)
                {
                    MessageBox.Show("Update preferences could not be saved. The previous saved copy has been retained." +
                        Environment.NewLine + Environment.NewLine + Common_Data.RedactConnectionCredentials(ex.Message),
                        "Update preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            Logs.EnterLogs($"\n**************** Logs Ended at {Common_Data.AppendDateInOutputFileName()} ***************************************\n");
        }
    }
}
