namespace SPI_Data_Comp_Tool
{
    partial class MergeLogsExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeLogsExport));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_ReadTxtFile = new System.Windows.Forms.LinkLabel();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_ReadTxtFile);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1212, 115);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input Text File";
            // 
            // lbl_ReadTxtFile
            // 
            this.lbl_ReadTxtFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReadTxtFile.Location = new System.Drawing.Point(15, 24);
            this.lbl_ReadTxtFile.Name = "lbl_ReadTxtFile";
            this.lbl_ReadTxtFile.Size = new System.Drawing.Size(1190, 75);
            this.lbl_ReadTxtFile.TabIndex = 0;
            this.lbl_ReadTxtFile.TabStop = true;
            this.lbl_ReadTxtFile.Text = "No File Selected";
            this.lbl_ReadTxtFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_ReadTxtFile_LinkClicked);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.progressBar1);
            this.groupBox10.Controls.Add(this.rtb_Log);
            this.groupBox10.Controls.Add(this.btn_Clear_Reset);
            this.groupBox10.Location = new System.Drawing.Point(13, 134);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(1212, 500);
            this.groupBox10.TabIndex = 27;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 24);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1106, 15);
            this.progressBar1.TabIndex = 1;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Log.Location = new System.Drawing.Point(10, 52);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(1196, 437);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Location = new System.Drawing.Point(1121, 20);
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.Size = new System.Drawing.Size(84, 23);
            this.btn_Clear_Reset.TabIndex = 14;
            this.btn_Clear_Reset.Text = "Clear/Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            // 
            // MergeLogsExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 644);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MergeLogsExport";
            this.Text = "MergeLogsExport";
            this.groupBox1.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lbl_ReadTxtFile;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Button btn_Clear_Reset;
    }
}