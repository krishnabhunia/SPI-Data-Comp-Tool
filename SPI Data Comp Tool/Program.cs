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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Registering 
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            #endregion

            // CI-only checks of a finished build (see SelfTest.cs); does nothing in normal use
            if (SelfTest.TryRun(args)) return;

            Logs.EnterLogs($"\n=========== Logs Entry Started at {Common_Data.AppendDateInOutputFileName()} ==================================\n");

            Application.Run(new MainForm());

            Logs.EnterLogs($"\n**************** Logs Ended at {Common_Data.AppendDateInOutputFileName()} ***************************************\n");
        }
    }
}
