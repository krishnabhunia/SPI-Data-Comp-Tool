using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    public partial class SPI_Analysis : Form
    {
        public SPI_Analysis()
        {
            InitializeComponent();

            if(Debugger.IsAttached)
            {
                oracleAndSQLConnection1.LoadDebuggingInformation();
            }
        }

        private void btn_Show_Click(object sender, EventArgs e)
        {
            try
            {
                SPI_Analysis_View sPI_Analysis_View = new SPI_Analysis_View(oracleAndSQLConnection1.FetchDataSet());
                sPI_Analysis_View.Show();
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }
    }
}
