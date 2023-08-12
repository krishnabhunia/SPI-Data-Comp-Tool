namespace SPI_Data_Comp_Tool
{
    partial class User_Settings
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
            this.gb_SaveExcel = new System.Windows.Forms.GroupBox();
            this.rb_XLSave = new System.Windows.Forms.RadioButton();
            this.rb_EPPlus = new System.Windows.Forms.RadioButton();
            this.gb_Logs = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.gb_SaveExcel.SuspendLayout();
            this.gb_Logs.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_SaveExcel
            // 
            this.gb_SaveExcel.Controls.Add(this.rb_XLSave);
            this.gb_SaveExcel.Controls.Add(this.rb_EPPlus);
            this.gb_SaveExcel.Location = new System.Drawing.Point(13, 13);
            this.gb_SaveExcel.Name = "gb_SaveExcel";
            this.gb_SaveExcel.Size = new System.Drawing.Size(263, 82);
            this.gb_SaveExcel.TabIndex = 0;
            this.gb_SaveExcel.TabStop = false;
            this.gb_SaveExcel.Text = "Save Excel";
            // 
            // rb_XLSave
            // 
            this.rb_XLSave.AutoSize = true;
            this.rb_XLSave.Location = new System.Drawing.Point(17, 48);
            this.rb_XLSave.Name = "rb_XLSave";
            this.rb_XLSave.Size = new System.Drawing.Size(212, 17);
            this.rb_XLSave.TabIndex = 1;
            this.rb_XLSave.Text = "Setting 2 Excel to be saved in XL Mode";
            this.rb_XLSave.UseVisualStyleBackColor = true;
            this.rb_XLSave.CheckedChanged += new System.EventHandler(this.rb_XLSave_CheckedChanged);
            // 
            // rb_EPPlus
            // 
            this.rb_EPPlus.AutoSize = true;
            this.rb_EPPlus.Checked = true;
            this.rb_EPPlus.Location = new System.Drawing.Point(17, 25);
            this.rb_EPPlus.Name = "rb_EPPlus";
            this.rb_EPPlus.Size = new System.Drawing.Size(233, 17);
            this.rb_EPPlus.TabIndex = 0;
            this.rb_EPPlus.TabStop = true;
            this.rb_EPPlus.Text = "Setting 1 Excel to be saved in EPPlus Mode";
            this.rb_EPPlus.UseVisualStyleBackColor = true;
            this.rb_EPPlus.CheckedChanged += new System.EventHandler(this.rb_EPPlus_CheckedChanged);
            // 
            // gb_Logs
            // 
            this.gb_Logs.Controls.Add(this.btn_ClearLogs);
            this.gb_Logs.Controls.Add(this.rtb_Log);
            this.gb_Logs.Controls.Add(this.lbl_Progress);
            this.gb_Logs.Location = new System.Drawing.Point(12, 101);
            this.gb_Logs.Name = "gb_Logs";
            this.gb_Logs.Size = new System.Drawing.Size(764, 251);
            this.gb_Logs.TabIndex = 34;
            this.gb_Logs.TabStop = false;
            this.gb_Logs.Text = "Logs";
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Location = new System.Drawing.Point(17, 221);
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.Size = new System.Drawing.Size(75, 23);
            this.btn_ClearLogs.TabIndex = 2;
            this.btn_ClearLogs.Text = "Clear Logs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Log.Location = new System.Drawing.Point(17, 23);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(735, 187);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Progress.Location = new System.Drawing.Point(14, 40);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(63, 16);
            this.lbl_Progress.TabIndex = 0;
            this.lbl_Progress.Text = "Progress";
            // 
            // User_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 357);
            this.Controls.Add(this.gb_Logs);
            this.Controls.Add(this.gb_SaveExcel);
            this.Name = "User_Settings";
            this.Text = "User Settings";
            this.gb_SaveExcel.ResumeLayout(false);
            this.gb_SaveExcel.PerformLayout();
            this.gb_Logs.ResumeLayout(false);
            this.gb_Logs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_SaveExcel;
        private System.Windows.Forms.RadioButton rb_XLSave;
        private System.Windows.Forms.RadioButton rb_EPPlus;
        private System.Windows.Forms.GroupBox gb_Logs;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label lbl_Progress;
    }
}