using System;
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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Registering 
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            #endregion

            Logs.EnterLogs($"\n=========== Logs Entry Started at {Common_Data.AppendDateInOutputFileName()} ==================================\n");

            // Both Release and Debug builds begin with authentication.
            // Attaching a debugger must not open a privileged form directly.
            Application.Run(new MainForm());

            Logs.EnterLogs($"\n**************** Logs Ended at {Common_Data.AppendDateInOutputFileName()} ***************************************\n");
        }
    }
}
