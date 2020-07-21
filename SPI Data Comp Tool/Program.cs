using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

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
            if(Debugger.IsAttached)
            {
                //Application.Run(new SPI_Spec_Note_Creation_Utility());
                //Application.Run(new SPI_Analysis());
                //Application.Run(new Excel_Excel_Comparison());
                Application.Run(new Form1()); // this form is DB comparison
                //Application.Run(new LTTS_Data_Comparison()); // This is the main GUI of the utility
            }
            else
            {
                Application.Run(new LTTS_Data_Comparison());
            }
        }
    }
}
