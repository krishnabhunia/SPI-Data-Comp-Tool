namespace SPI_Data_Comp_Tool
{
    partial class Generate_IUD_Script_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Generate_IUD_Script_Form));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tb_TableName = new System.Windows.Forms.TextBox();
            this.rb_SheetName = new System.Windows.Forms.RadioButton();
            this.rb_FromFileName = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_SelectSheet = new System.Windows.Forms.ComboBox();
            this.btn_Script = new System.Windows.Forms.Button();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rb_Delete = new System.Windows.Forms.RadioButton();
            this.rb_Update = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.clb_Checked = new System.Windows.Forms.CheckedListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.llb_File = new System.Windows.Forms.LinkLabel();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.rb_Insert = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox9);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1212, 400);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Location = new System.Drawing.Point(994, 20);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(200, 360);
            this.groupBox6.TabIndex = 42;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Note : ";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(187, 142);
            this.label6.TabIndex = 0;
            this.label6.Text = "*From File Name : will select table name from file name before \"_Output\" text or " +
    "full filename if not found";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tb_TableName);
            this.groupBox5.Controls.Add(this.rb_SheetName);
            this.groupBox5.Controls.Add(this.rb_FromFileName);
            this.groupBox5.Location = new System.Drawing.Point(6, 157);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(279, 103);
            this.groupBox5.TabIndex = 41;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Use \"Table Name\" From : ";
            // 
            // tb_TableName
            // 
            this.tb_TableName.Location = new System.Drawing.Point(17, 70);
            this.tb_TableName.Name = "tb_TableName";
            this.tb_TableName.Size = new System.Drawing.Size(239, 20);
            this.tb_TableName.TabIndex = 2;
            // 
            // rb_SheetName
            // 
            this.rb_SheetName.AutoSize = true;
            this.rb_SheetName.Location = new System.Drawing.Point(17, 47);
            this.rb_SheetName.Name = "rb_SheetName";
            this.rb_SheetName.Size = new System.Drawing.Size(110, 17);
            this.rb_SheetName.TabIndex = 1;
            this.rb_SheetName.Text = "From Sheet Name";
            this.rb_SheetName.UseVisualStyleBackColor = true;
            this.rb_SheetName.CheckedChanged += new System.EventHandler(this.rb_SheetName_Insert_CheckedChanged);
            // 
            // rb_FromFileName
            // 
            this.rb_FromFileName.AutoSize = true;
            this.rb_FromFileName.Checked = true;
            this.rb_FromFileName.Location = new System.Drawing.Point(17, 24);
            this.rb_FromFileName.Name = "rb_FromFileName";
            this.rb_FromFileName.Size = new System.Drawing.Size(102, 17);
            this.rb_FromFileName.TabIndex = 0;
            this.rb_FromFileName.TabStop = true;
            this.rb_FromFileName.Text = "From File Name*";
            this.rb_FromFileName.UseVisualStyleBackColor = true;
            this.rb_FromFileName.CheckedChanged += new System.EventHandler(this.rb_FromFileName_Insert_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Select Sheet";
            // 
            // cb_SelectSheet
            // 
            this.cb_SelectSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SelectSheet.FormattingEnabled = true;
            this.cb_SelectSheet.Location = new System.Drawing.Point(6, 130);
            this.cb_SelectSheet.Name = "cb_SelectSheet";
            this.cb_SelectSheet.Size = new System.Drawing.Size(279, 21);
            this.cb_SelectSheet.TabIndex = 39;
            this.cb_SelectSheet.SelectedIndexChanged += new System.EventHandler(this.cb_SelectSheetInsert_SelectedIndexChanged);
            // 
            // btn_Script
            // 
            this.btn_Script.Location = new System.Drawing.Point(9, 266);
            this.btn_Script.Name = "btn_Script";
            this.btn_Script.Size = new System.Drawing.Size(276, 23);
            this.btn_Script.TabIndex = 38;
            this.btn_Script.Text = "Generate Insert Script";
            this.btn_Script.UseVisualStyleBackColor = true;
            this.btn_Script.Click += new System.EventHandler(this.btn_Script_Click);
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Location = new System.Drawing.Point(6, 78);
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.Size = new System.Drawing.Size(279, 23);
            this.btn_ListTableExcel.TabIndex = 36;
            this.btn_ListTableExcel.Text = "Browse Excel";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.rb_Insert);
            this.groupBox8.Controls.Add(this.rb_Delete);
            this.groupBox8.Controls.Add(this.rb_Update);
            this.groupBox8.Location = new System.Drawing.Point(6, 19);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(279, 53);
            this.groupBox8.TabIndex = 47;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Update / Delete";
            // 
            // rb_Delete
            // 
            this.rb_Delete.AutoSize = true;
            this.rb_Delete.Location = new System.Drawing.Point(139, 19);
            this.rb_Delete.Name = "rb_Delete";
            this.rb_Delete.Size = new System.Drawing.Size(56, 17);
            this.rb_Delete.TabIndex = 1;
            this.rb_Delete.Text = "Delete";
            this.rb_Delete.UseVisualStyleBackColor = true;
            this.rb_Delete.CheckedChanged += new System.EventHandler(this.rb_Delete_CheckedChanged);
            // 
            // rb_Update
            // 
            this.rb_Update.AutoSize = true;
            this.rb_Update.Location = new System.Drawing.Point(73, 19);
            this.rb_Update.Name = "rb_Update";
            this.rb_Update.Size = new System.Drawing.Size(60, 17);
            this.rb_Update.TabIndex = 0;
            this.rb_Update.Text = "Update";
            this.rb_Update.UseVisualStyleBackColor = true;
            this.rb_Update.CheckedChanged += new System.EventHandler(this.rb_Update_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(291, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(219, 50);
            this.label3.TabIndex = 41;
            this.label3.Text = "Select Column to set \'Where\' Clause in Update/Delete Script";
            this.label3.Visible = false;
            // 
            // clb_Checked
            // 
            this.clb_Checked.CheckOnClick = true;
            this.clb_Checked.FormattingEnabled = true;
            this.clb_Checked.Location = new System.Drawing.Point(291, 78);
            this.clb_Checked.Name = "clb_Checked";
            this.clb_Checked.Size = new System.Drawing.Size(219, 274);
            this.clb_Checked.TabIndex = 40;
            this.clb_Checked.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.llb_File);
            this.groupBox4.Location = new System.Drawing.Point(531, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(457, 361);
            this.groupBox4.TabIndex = 41;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output";
            // 
            // llb_File
            // 
            this.llb_File.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llb_File.Location = new System.Drawing.Point(10, 18);
            this.llb_File.Name = "llb_File";
            this.llb_File.Size = new System.Drawing.Size(441, 328);
            this.llb_File.TabIndex = 38;
            this.llb_File.TabStop = true;
            this.llb_File.Text = "llb_File";
            this.llb_File.Visible = false;
            this.llb_File.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_File_LinkClicked);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.progressBar1);
            this.groupBox10.Controls.Add(this.btn_Clear_Reset);
            this.groupBox10.Controls.Add(this.rtb_Log);
            this.groupBox10.Location = new System.Drawing.Point(12, 419);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(1212, 447);
            this.groupBox10.TabIndex = 28;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 23);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1106, 15);
            this.progressBar1.TabIndex = 1;
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Location = new System.Drawing.Point(1122, 19);
            this.btn_Clear_Reset.Name = "btn_Clear_Reset";
            this.btn_Clear_Reset.Size = new System.Drawing.Size(84, 23);
            this.btn_Clear_Reset.TabIndex = 36;
            this.btn_Clear_Reset.Text = "Clear/Reset";
            this.btn_Clear_Reset.UseVisualStyleBackColor = true;
            this.btn_Clear_Reset.Click += new System.EventHandler(this.btn_Clear_Reset_Click);
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.Color.White;
            this.rtb_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Log.Location = new System.Drawing.Point(10, 52);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(1196, 385);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // rb_Insert
            // 
            this.rb_Insert.AutoSize = true;
            this.rb_Insert.Checked = true;
            this.rb_Insert.Location = new System.Drawing.Point(16, 19);
            this.rb_Insert.Name = "rb_Insert";
            this.rb_Insert.Size = new System.Drawing.Size(51, 17);
            this.rb_Insert.TabIndex = 2;
            this.rb_Insert.TabStop = true;
            this.rb_Insert.Text = "Insert";
            this.rb_Insert.UseVisualStyleBackColor = true;
            this.rb_Insert.CheckedChanged += new System.EventHandler(this.rb_Insert_CheckedChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.btn_Script);
            this.groupBox9.Controls.Add(this.groupBox8);
            this.groupBox9.Controls.Add(this.groupBox5);
            this.groupBox9.Controls.Add(this.label3);
            this.groupBox9.Controls.Add(this.btn_ListTableExcel);
            this.groupBox9.Controls.Add(this.clb_Checked);
            this.groupBox9.Controls.Add(this.cb_SelectSheet);
            this.groupBox9.Controls.Add(this.label4);
            this.groupBox9.Location = new System.Drawing.Point(9, 19);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(516, 362);
            this.groupBox9.TabIndex = 48;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Input";
            // 
            // Generate_IUD_Script_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1250, 821);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Generate_IUD_Script_Form";
            this.Text = "Generate Insert/Update/Delete Script Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.LinkLabel llb_File;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox clb_Checked;
        private System.Windows.Forms.Button btn_Script;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_SelectSheet;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rb_SheetName;
        private System.Windows.Forms.RadioButton rb_FromFileName;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton rb_Delete;
        private System.Windows.Forms.RadioButton rb_Update;
        private System.Windows.Forms.TextBox tb_TableName;
        private System.Windows.Forms.RadioButton rb_Insert;
        private System.Windows.Forms.GroupBox groupBox9;
    }
}