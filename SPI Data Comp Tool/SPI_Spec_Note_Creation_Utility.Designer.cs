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
            this.btn_GenerateExcel = new System.Windows.Forms.Button();
            this.gb_ViewExcelData = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.gb_ControlAndStatus = new System.Windows.Forms.GroupBox();
            this.btn_Spec_Create = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.excelFileReadCustomControl1 = new SPI_Data_Comp_Tool.ExcelFileReadCustomControl();
            this.gb_ViewExcelData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.gb_ControlAndStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_GenerateExcel
            // 
            this.btn_GenerateExcel.Location = new System.Drawing.Point(134, 19);
            this.btn_GenerateExcel.Name = "btn_GenerateExcel";
            this.btn_GenerateExcel.Size = new System.Drawing.Size(94, 23);
            this.btn_GenerateExcel.TabIndex = 1;
            this.btn_GenerateExcel.Text = "Export to Excel";
            this.btn_GenerateExcel.UseVisualStyleBackColor = true;
            this.btn_GenerateExcel.Click += new System.EventHandler(this.btn_GenerateExcel_Click);
            // 
            // gb_ViewExcelData
            // 
            this.gb_ViewExcelData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_ViewExcelData.Controls.Add(this.dataGridView1);
            this.gb_ViewExcelData.Location = new System.Drawing.Point(12, 113);
            this.gb_ViewExcelData.Name = "gb_ViewExcelData";
            this.gb_ViewExcelData.Size = new System.Drawing.Size(776, 252);
            this.gb_ViewExcelData.TabIndex = 3;
            this.gb_ViewExcelData.TabStop = false;
            this.gb_ViewExcelData.Text = "View Data";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 16);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(770, 233);
            this.dataGridView1.TabIndex = 0;
            // 
            // gb_ControlAndStatus
            // 
            this.gb_ControlAndStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_ControlAndStatus.Controls.Add(this.btn_Spec_Create);
            this.gb_ControlAndStatus.Controls.Add(this.lbl_Status);
            this.gb_ControlAndStatus.Controls.Add(this.btn_GenerateExcel);
            this.gb_ControlAndStatus.Location = new System.Drawing.Point(12, 371);
            this.gb_ControlAndStatus.Name = "gb_ControlAndStatus";
            this.gb_ControlAndStatus.Size = new System.Drawing.Size(776, 71);
            this.gb_ControlAndStatus.TabIndex = 4;
            this.gb_ControlAndStatus.TabStop = false;
            this.gb_ControlAndStatus.Text = "Control and Status";
            // 
            // btn_Spec_Create
            // 
            this.btn_Spec_Create.Location = new System.Drawing.Point(9, 19);
            this.btn_Spec_Create.Name = "btn_Spec_Create";
            this.btn_Spec_Create.Size = new System.Drawing.Size(119, 23);
            this.btn_Spec_Create.TabIndex = 3;
            this.btn_Spec_Create.Text = "Create and Show";
            this.btn_Spec_Create.UseVisualStyleBackColor = true;
            this.btn_Spec_Create.Click += new System.EventHandler(this.btn_Spec_Create_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(6, 49);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(71, 13);
            this.lbl_Status.TabIndex = 2;
            this.lbl_Status.Text = "Status : Okay";
            // 
            // excelFileReadCustomControl1
            // 
            this.excelFileReadCustomControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.excelFileReadCustomControl1.AutoScroll = true;
            this.excelFileReadCustomControl1.Location = new System.Drawing.Point(12, 12);
            this.excelFileReadCustomControl1.Name = "excelFileReadCustomControl1";
            this.excelFileReadCustomControl1.Size = new System.Drawing.Size(776, 102);
            this.excelFileReadCustomControl1.TabIndex = 0;
            // 
            // SPI_Spec_Note_Creation_Utility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.excelFileReadCustomControl1);
            this.Controls.Add(this.gb_ControlAndStatus);
            this.Controls.Add(this.gb_ViewExcelData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPI_Spec_Note_Creation_Utility";
            this.Text = "SPI_Spec_Note_Creation_Utility";
            this.gb_ViewExcelData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.gb_ControlAndStatus.ResumeLayout(false);
            this.gb_ControlAndStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ExcelFileReadCustomControl excelFileReadCustomControl1;
        private System.Windows.Forms.Button btn_GenerateExcel;
        private System.Windows.Forms.GroupBox gb_ViewExcelData;
        private System.Windows.Forms.GroupBox gb_ControlAndStatus;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btn_Spec_Create;
    }
}