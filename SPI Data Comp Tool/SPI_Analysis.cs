using System;
using System.Data;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Connection setup and the resulting analysis share one workspace.</summary>
    public partial class SPI_Analysis : Form
    {
        private SPI_Analysis_View results;

        public SPI_Analysis()
        {
            InitializeComponent();
            UiTheme.Apply(this, btn_Show);
            oracleAndSQLConnection1.DataLoadStateChanged += ConnectionDataLoadStateChanged;
        }

        private void ConnectionDataLoadStateChanged(object sender, EventArgs e)
        {
            btn_Show.Enabled = oracleAndSQLConnection1.HasLoadedData;
            lbl_Status.Text = btn_Show.Enabled
                ? "Data loaded. Select Show results to review it."
                : "Load source and target data before showing results.";
        }

        private void btn_Show_Click(object sender, EventArgs e)
        {
            try
            {
                if (!oracleAndSQLConnection1.HasLoadedData)
                {
                    lbl_Status.Text = "Load source and target data before showing results.";
                    return;
                }
                if (results != null && MessageBox.Show(this,
                    "Replace the current results? Export any arranged results you want to keep first.",
                    "Replace analysis results", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }
                DisplayResults(oracleAndSQLConnection1.FetchDataSet());
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message;
            }
        }

        /// <summary>Build first, then replace: an invalid reload must not destroy the current results.</summary>
        internal void DisplayResults(DataSet dataSet)
        {
            var replacement = new SPI_Analysis_View(dataSet) { Dock = DockStyle.Fill };
            var previousControls = new Control[resultsPage.Controls.Count];
            resultsPage.Controls.CopyTo(previousControls, 0);
            results = replacement;
            resultsPage.Controls.Clear();
            resultsPage.Controls.Add(replacement);
            foreach (Control previous in previousControls) previous.Dispose();
            workspaceTabs.SelectedTab = resultsPage;
            lbl_Status.Text = "Results loaded. Return to Connection setup at any time.";
        }
    }
}
