namespace SPI_Data_Comp_Tool
{
    partial class SPI_Spec_Note_Creation_Utility
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPI_Spec_Note_Creation_Utility));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.btn_GenerateExcel = new System.Windows.Forms.Button();
            this.gb_ViewExcelData = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.gb_ControlAndStatus = new System.Windows.Forms.GroupBox();
            this.btn_Spec_Create = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.excelFileReadCustomControl1 = new SPI_Data_Comp_Tool.ExcelFileReadCustomControl();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.step_Strip = new SPI_Data_Comp_Tool.StepStrip();
            this.tlp_Controls = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_View = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tlp_Root.SuspendLayout();
            this.gb_ControlAndStatus.SuspendLayout();
            this.tlp_Controls.SuspendLayout();
            this.gb_ViewExcelData.SuspendLayout();
            this.tlp_View.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 1;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 4;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.step_Strip, 0, 0);
            this.tlp_Root.Controls.Add(this.excelFileReadCustomControl1, 0, 1);
            this.tlp_Root.Controls.Add(this.gb_ControlAndStatus, 0, 2);
            this.tlp_Root.Controls.Add(this.gb_ViewExcelData, 0, 3);
            this.tlp_Root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Root.AutoSize = false;
            this.tlp_Root.TabIndex = 0;
            // 
            // step_Strip
            // 
            this.step_Strip.Name = "step_Strip";
            this.step_Strip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.step_Strip.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.step_Strip.Size = new System.Drawing.Size(800, 66);
            this.step_Strip.TabIndex = 1;
            // 
            // excelFileReadCustomControl1
            // 
            this.excelFileReadCustomControl1.AutoScroll = true;
            this.excelFileReadCustomControl1.Name = "excelFileReadCustomControl1";
            this.excelFileReadCustomControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.excelFileReadCustomControl1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.excelFileReadCustomControl1.Size = new System.Drawing.Size(776, 102);
            this.excelFileReadCustomControl1.TabIndex = 2;
            // 
            // gb_ControlAndStatus
            // 
            this.gb_ControlAndStatus.Name = "gb_ControlAndStatus";
            this.gb_ControlAndStatus.TabStop = false;
            this.gb_ControlAndStatus.Text = "Notes";
            this.gb_ControlAndStatus.Controls.Add(this.tlp_Controls);
            this.gb_ControlAndStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_ControlAndStatus.AutoSize = true;
            this.gb_ControlAndStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_ControlAndStatus.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.gb_ControlAndStatus.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_ControlAndStatus.TabIndex = 3;
            // 
            // tlp_Controls
            // 
            this.tlp_Controls.Name = "tlp_Controls";
            this.tlp_Controls.ColumnCount = 3;
            this.tlp_Controls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Controls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Controls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Controls.RowCount = 1;
            this.tlp_Controls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Controls.Controls.Add(this.btn_Spec_Create, 0, 0);
            this.tlp_Controls.Controls.Add(this.btn_GenerateExcel, 1, 0);
            this.tlp_Controls.Controls.Add(this.lbl_Status, 2, 0);
            this.tlp_Controls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Controls.AutoSize = true;
            this.tlp_Controls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Controls.TabIndex = 4;
            // 
            // btn_Spec_Create
            // 
            this.btn_Spec_Create.Name = "btn_Spec_Create";
            this.btn_Spec_Create.UseVisualStyleBackColor = true;
            this.btn_Spec_Create.Click += new System.EventHandler(this.btn_Spec_Create_Click);
            this.btn_Spec_Create.Text = "Create and show";
            this.btn_Spec_Create.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btn_Spec_Create.Size = new System.Drawing.Size(150, 32);
            this.btn_Spec_Create.TabIndex = 5;
            // 
            // btn_GenerateExcel
            // 
            this.btn_GenerateExcel.Name = "btn_GenerateExcel";
            this.btn_GenerateExcel.UseVisualStyleBackColor = true;
            this.btn_GenerateExcel.Click += new System.EventHandler(this.btn_GenerateExcel_Click);
            this.btn_GenerateExcel.Text = "Export to Excel";
            this.btn_GenerateExcel.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btn_GenerateExcel.Size = new System.Drawing.Size(150, 32);
            this.btn_GenerateExcel.TabIndex = 6;
            // 
            // lbl_Status
            // 
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Text = "Status: Okay";
            this.lbl_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_Status.TabIndex = 7;
            // 
            // gb_ViewExcelData
            // 
            this.gb_ViewExcelData.Name = "gb_ViewExcelData";
            this.gb_ViewExcelData.TabStop = false;
            this.gb_ViewExcelData.Text = "View data";
            this.gb_ViewExcelData.Controls.Add(this.tlp_View);
            this.gb_ViewExcelData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_ViewExcelData.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.gb_ViewExcelData.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.gb_ViewExcelData.TabIndex = 8;
            // 
            // tlp_View
            // 
            this.tlp_View.Name = "tlp_View";
            this.tlp_View.ColumnCount = 1;
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_View.RowCount = 1;
            this.tlp_View.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_View.Controls.Add(this.dataGridView1, 0, 0);
            this.tlp_View.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_View.AutoSize = false;
            this.tlp_View.TabIndex = 9;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.dataGridView1.TabIndex = 10;
            // 
            // SPI_Spec_Note_Creation_Utility
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPI_Spec_Note_Creation_Utility";
            this.Text = "Spec note creation";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 640);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 540);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.gb_ControlAndStatus.ResumeLayout(false);
            this.gb_ControlAndStatus.PerformLayout();
            this.tlp_Controls.ResumeLayout(false);
            this.tlp_Controls.PerformLayout();
            this.gb_ViewExcelData.ResumeLayout(false);
            this.gb_ViewExcelData.PerformLayout();
            this.tlp_View.ResumeLayout(false);
            this.tlp_View.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private SPI_Data_Comp_Tool.StepStrip step_Strip;
        private System.Windows.Forms.TableLayoutPanel tlp_Controls;
        private System.Windows.Forms.TableLayoutPanel tlp_View;

        private ExcelFileReadCustomControl excelFileReadCustomControl1;
        private System.Windows.Forms.Button btn_GenerateExcel;
        private System.Windows.Forms.GroupBox gb_ViewExcelData;
        private System.Windows.Forms.GroupBox gb_ControlAndStatus;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btn_Spec_Create;
    }
}