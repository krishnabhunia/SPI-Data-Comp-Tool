using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
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

            if (Debugger.IsAttached)// && false)
            {
                //Application.Run(new LTTS_Data_Comparison()); // This is the main GUI of the utility

                //File Menu
                //Application.Run(new UserProfileForm());

                //SPI Analysis Menu
                //Application.Run(new SPI_Spec_Note_Creation_Utility());
                //Application.Run(new SPI_Analysis());
                //Application.Run(new SPIDeltaChangeTracker());
                //Application.Run(new SPIDeltaChangeTrackerOptimised());
                //Application.Run(new SPIDeltaChangeLegacyID());
                //Application.Run(new SPI_FK_Extraction());

                //Compare Menu
                Application.Run(new DB_Comparison_Form()); // this form is DB comparison
                //Application.Run(new Excel_Excel_Comparison());
                //Application.Run(new Compare_Multiples_List_Tables_Forms());

                //Tools Menu
                //Application.Run(new CustomisedExcel());
                //Application.Run(new PreMergingTool_Form());
                //Application.Run(new Pre_Merging_Duplicacy_Form());
                //Application.Run(new Pre_Merging_Duplicacy_Form(true)); //boolConfigurationMode = true
                //Application.Run(new Generate_IUD_Script_Form());
                //Application.Run(new Execute_Excel_Scripts_Form());

                //Export Menu
                //Application.Run(new SQL_DB_To_Excel());
                //Application.Run(new Table_Extraction_After_Delta_Form());
                //Application.Run(new MergeLogsExport());

                //Other Discarded or Aborted
                //Application.Run(new MultiReferenceDBComparison());
                //Application.Run(new AboutBox1());
                //Application.Run(new ComparisonProcessWindow());

                //other
                //Application.Run(new UserTest());
                //Application.Run(new Test());

            }
            else
            {
                Application.Run(new LTTS_Data_Comparison());
            }

            Logs.EnterLogs($"\n**************** Logs Ended at {Common_Data.AppendDateInOutputFileName()} ***************************************\n");
        }
    }
}
