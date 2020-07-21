namespace SPI_Data_Comp_Tool
{
    partial class ExcelFileReadCustomControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Excel = new System.Windows.Forms.Button();
            this.lbl_ExcelFileName = new System.Windows.Forms.Label();
            this.gb_ImportExcelFile = new System.Windows.Forms.GroupBox();
            this.gb_ImportExcelFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Excel
            // 
            this.btn_Excel.Location = new System.Drawing.Point(12, 19);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.Size = new System.Drawing.Size(85, 23);
            this.btn_Excel.TabIndex = 0;
            this.btn_Excel.Text = "Import Excel";
            this.btn_Excel.UseVisualStyleBackColor = true;
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // lbl_ExcelFileName
            // 
            this.lbl_ExcelFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_ExcelFileName.Location = new System.Drawing.Point(103, 24);
            this.lbl_ExcelFileName.Name = "lbl_ExcelFileName";
            this.lbl_ExcelFileName.Size = new System.Drawing.Size(577, 68);
            this.lbl_ExcelFileName.TabIndex = 1;
            this.lbl_ExcelFileName.Text = "No File Selected";
            // 
            // gb_ImportExcelFile
            // 
            this.gb_ImportExcelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_ImportExcelFile.Controls.Add(this.btn_Excel);
            this.gb_ImportExcelFile.Controls.Add(this.lbl_ExcelFileName);
            this.gb_ImportExcelFile.Location = new System.Drawing.Point(3, 3);
            this.gb_ImportExcelFile.Name = "gb_ImportExcelFile";
            this.gb_ImportExcelFile.Size = new System.Drawing.Size(686, 96);
            this.gb_ImportExcelFile.TabIndex = 2;
            this.gb_ImportExcelFile.TabStop = false;
            this.gb_ImportExcelFile.Text = "Import Excel File";
            // 
            // ExcelFileReadCustomControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.gb_ImportExcelFile);
            this.Name = "ExcelFileReadCustomControl";
            this.Size = new System.Drawing.Size(692, 101);
            this.gb_ImportExcelFile.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Excel;
        private System.Windows.Forms.Label lbl_ExcelFileName;
        private System.Windows.Forms.GroupBox gb_ImportExcelFile;
    }
}
