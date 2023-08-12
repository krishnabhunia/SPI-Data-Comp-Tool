using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class ComparisonProcessWindow : Form
    {
        public ComparisonProcessWindow()
        {
            InitializeComponent();

            try
            {
                if(!(bgW_Process.IsBusy))
                {
                    bgW_Process.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

        }

        private void bgW_Process_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i=1; i<= 100; i++)
                {
                    bgW_Process.ReportProgress(i);
                    Thread.Sleep(100);
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void bgW_Process_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                progressBar1.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {

            }
        }

        private void bgW_Process_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        public void DisplayMsg(string strMsg)
        {
            try
            {
                rtb_Logs.Text = strMsg;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        public void DisplayError(string strMsg)
        {
            try
            {
                rtb_Logs.Text = strMsg;
            }
            catch (Exception ex)
            {
                rtb_Logs.Text = ex.ToString();
            }
        }

        public void DisplayError(Exception ex)
        {
            try
            {
                rtb_Logs.Text = ex.ToString();
            }
            catch (Exception ex1)
            {
                rtb_Logs.Text = ex1.ToString();
            }
        }

    }
}
