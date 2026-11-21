namespace SPI_Data_Comp_Tool
{
    partial class PreMergingTool_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreMergingTool_Form));
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.tb_SchemaName = new System.Windows.Forms.TextBox();
            this.cb_Schema = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_ClearLogs = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_ListTableExcel = new System.Windows.Forms.Button();
            this.btn_Duplicate = new System.Windows.Forms.Button();
            this.lb_link = new System.Windows.Forms.Label();
            this.llb_File = new System.Windows.Forms.LinkLabel();
            this.cb_WithoutSpecialCharacters = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.step_Strip = new SPI_Data_Comp_Tool.StepStrip();
            this.tlp_Connection = new System.Windows.Forms.TableLayoutPanel();
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tlp_Schema = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Right = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Output = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Log = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.tlp_Connection.SuspendLayout();
            this.tlp_Schema.SuspendLayout();
            this.tlp_Right.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlp_Output.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlp_Log.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 440F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowCount = 3;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.Controls.Add(this.step_Strip, 0, 0);
            this.tlp_Root.SetColumnSpan(this.step_Strip, 2);
            this.tlp_Root.Controls.Add(this.groupBox12, 0, 1);
            this.tlp_Root.Controls.Add(this.tlp_Right, 1, 1);
            this.tlp_Root.Controls.Add(this.groupBox2, 0, 2);
            this.tlp_Root.SetColumnSpan(this.groupBox2, 2);
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
            // groupBox12
            // 
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Connection";
            this.groupBox12.Controls.Add(this.tlp_Connection);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.AutoSize = true;
            this.groupBox12.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox12.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox12.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox12.TabIndex = 2;
            // 
            // tlp_Connection
            // 
            this.tlp_Connection.Name = "tlp_Connection";
            this.tlp_Connection.ColumnCount = 1;
            this.tlp_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Connection.RowCount = 3;
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Connection.Controls.Add(this.connection, 0, 0);
            this.tlp_Connection.Controls.Add(this.tlp_Schema, 0, 1);
            this.tlp_Connection.Controls.Add(this.btn_Connect, 0, 2);
            this.tlp_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Connection.AutoSize = true;
            this.tlp_Connection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Connection.TabIndex = 3;
            // 
            // connection
            // 
            this.connection.Name = "connection";
            this.connection.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.connection.AutoSize = true;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.connection.TabIndex = 4;
            // 
            // tlp_Schema
            // 
            this.tlp_Schema.Name = "tlp_Schema";
            this.tlp_Schema.ColumnCount = 2;
            this.tlp_Schema.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Schema.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Schema.RowCount = 1;
            this.tlp_Schema.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Schema.Controls.Add(this.cb_Schema, 0, 0);
            this.tlp_Schema.Controls.Add(this.tb_SchemaName, 1, 0);
            this.tlp_Schema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Schema.AutoSize = true;
            this.tlp_Schema.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Schema.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.tlp_Schema.TabIndex = 5;
            // 
            // cb_Schema
            // 
            this.cb_Schema.Name = "cb_Schema";
            this.cb_Schema.UseVisualStyleBackColor = true;
            this.cb_Schema.CheckedChanged += new System.EventHandler(this.cb_Schema_CheckedChanged);
            this.cb_Schema.Text = "Schema name";
            this.cb_Schema.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_Schema.AutoSize = true;
            this.cb_Schema.Margin = new System.Windows.Forms.Padding(0, 3, 12, 3);
            this.cb_Schema.TabIndex = 6;
            // 
            // tb_SchemaName
            // 
            this.tb_SchemaName.Enabled = false;
            this.tb_SchemaName.Name = "tb_SchemaName";
            this.tb_SchemaName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.tb_SchemaName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tb_SchemaName.TabIndex = 7;
            // 
            // btn_Connect
            // 
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            this.btn_Connect.Text = "Test connection";
            this.btn_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btn_Connect.Size = new System.Drawing.Size(150, 30);
            this.btn_Connect.TabIndex = 8;
            // 
            // tlp_Right
            // 
            this.tlp_Right.Name = "tlp_Right";
            this.tlp_Right.ColumnCount = 1;
            this.tlp_Right.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Right.RowCount = 2;
            this.tlp_Right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Right.Controls.Add(this.tabControl1, 0, 0);
            this.tlp_Right.Controls.Add(this.groupBox3, 0, 1);
            this.tlp_Right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Right.AutoSize = false;
            this.tlp_Right.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_Right.TabIndex = 9;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.tabControl1.Size = new System.Drawing.Size(700, 180);
            this.tabControl1.TabIndex = 10;
            // 
            // groupBox3
            // 
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output";
            this.groupBox3.Controls.Add(this.tlp_Output);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox3.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox3.TabIndex = 11;
            // 
            // tlp_Output
            // 
            this.tlp_Output.Name = "tlp_Output";
            this.tlp_Output.ColumnCount = 1;
            this.tlp_Output.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Output.RowCount = 2;
            this.tlp_Output.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Output.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Output.Controls.Add(this.lb_link, 0, 0);
            this.tlp_Output.Controls.Add(this.llb_File, 0, 1);
            this.tlp_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Output.AutoSize = false;
            this.tlp_Output.TabIndex = 12;
            // 
            // lb_link
            // 
            this.lb_link.Name = "lb_link";
            this.lb_link.Visible = false;
            this.lb_link.Text = "Open the output file:";
            this.lb_link.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lb_link.AutoSize = true;
            this.lb_link.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lb_link.TabIndex = 13;
            // 
            // llb_File
            // 
            this.llb_File.Name = "llb_File";
            this.llb_File.TabStop = true;
            this.llb_File.Text = "No File";
            this.llb_File.Visible = false;
            this.llb_File.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_File_LinkClicked);
            this.llb_File.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llb_File.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.llb_File.TabIndex = 14;
            // 
            // groupBox2
            // 
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log";
            this.groupBox2.Controls.Add(this.tlp_Log);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox2.TabIndex = 15;
            // 
            // tlp_Log
            // 
            this.tlp_Log.Name = "tlp_Log";
            this.tlp_Log.ColumnCount = 2;
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowCount = 3;
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Log.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Log.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Log.Controls.Add(this.btn_ClearLogs, 1, 0);
            this.tlp_Log.Controls.Add(this.lbl_Progress, 0, 1);
            this.tlp_Log.SetColumnSpan(this.lbl_Progress, 2);
            this.tlp_Log.Controls.Add(this.rtb_Log, 0, 2);
            this.tlp_Log.SetColumnSpan(this.rtb_Log, 2);
            this.tlp_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Log.AutoSize = false;
            this.tlp_Log.TabIndex = 16;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 17;
            // 
            // btn_ClearLogs
            // 
            this.btn_ClearLogs.BackColor = System.Drawing.Color.White;
            this.btn_ClearLogs.Name = "btn_ClearLogs";
            this.btn_ClearLogs.UseVisualStyleBackColor = false;
            this.btn_ClearLogs.Click += new System.EventHandler(this.btn_ClearLogs_Click);
            this.btn_ClearLogs.Text = "Clear log";
            this.btn_ClearLogs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_ClearLogs.Size = new System.Drawing.Size(110, 30);
            this.btn_ClearLogs.TabIndex = 18;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Text = "Progress";
            this.lbl_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.lbl_Progress.TabIndex = 19;
            // 
            // rtb_Log
            // 
            this.rtb_Log.BackColor = System.Drawing.SystemColors.HighlightText;
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Text = "";
            this.rtb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Log.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.rtb_Log.TabIndex = 20;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ListTableExcel);
            this.groupBox1.Location = new System.Drawing.Point(16, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(574, 51);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pre Merging Excel File";
            // 
            // btn_ListTableExcel
            // 
            this.btn_ListTableExcel.Location = new System.Drawing.Point(12, 19);
            this.btn_ListTableExcel.Name = "btn_ListTableExcel";
            this.btn_ListTableExcel.Size = new System.Drawing.Size(164, 23);
            this.btn_ListTableExcel.TabIndex = 1;
            this.btn_ListTableExcel.Text = "Pre Merging Excel Process";
            this.btn_ListTableExcel.UseVisualStyleBackColor = true;
            this.btn_ListTableExcel.Click += new System.EventHandler(this.btn_ListTableExcel_Click);
            // 
            // btn_Duplicate
            // 
            this.btn_Duplicate.Location = new System.Drawing.Point(9, 78);
            this.btn_Duplicate.Name = "btn_Duplicate";
            this.btn_Duplicate.Size = new System.Drawing.Size(110, 23);
            this.btn_Duplicate.TabIndex = 4;
            this.btn_Duplicate.Text = "Check Duplicate";
            this.btn_Duplicate.UseVisualStyleBackColor = true;
            this.btn_Duplicate.Click += new System.EventHandler(this.btn_Duplicate_Click);
            // 
            // cb_WithoutSpecialCharacters
            // 
            this.cb_WithoutSpecialCharacters.AutoSize = true;
            this.cb_WithoutSpecialCharacters.Location = new System.Drawing.Point(9, 23);
            this.cb_WithoutSpecialCharacters.Name = "cb_WithoutSpecialCharacters";
            this.cb_WithoutSpecialCharacters.Size = new System.Drawing.Size(215, 17);
            this.cb_WithoutSpecialCharacters.TabIndex = 5;
            this.cb_WithoutSpecialCharacters.Text = "Check By Removing Special Characters";
            this.cb_WithoutSpecialCharacters.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cb_IgnoreCase);
            this.groupBox4.Controls.Add(this.cb_WithoutSpecialCharacters);
            this.groupBox4.Controls.Add(this.btn_Duplicate);
            this.groupBox4.Location = new System.Drawing.Point(12, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(242, 117);
            this.groupBox4.TabIndex = 37;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Check Duplicacy in Single Domain";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(860, 96);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Pre Merging";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(860, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Check Duplicacy";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Location = new System.Drawing.Point(9, 48);
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.Size = new System.Drawing.Size(83, 17);
            this.cb_IgnoreCase.TabIndex = 6;
            this.cb_IgnoreCase.Text = "Ignore Case";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            // 
            // PreMergingTool_Form
            // 
            this.AutoSize = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PreMergingTool_Form";
            this.Text = "Pre-merging tool";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 860);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1080, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.tlp_Connection.ResumeLayout(false);
            this.tlp_Connection.PerformLayout();
            this.tlp_Schema.ResumeLayout(false);
            this.tlp_Schema.PerformLayout();
            this.tlp_Right.ResumeLayout(false);
            this.tlp_Right.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlp_Output.ResumeLayout(false);
            this.tlp_Output.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tlp_Log.ResumeLayout(false);
            this.tlp_Log.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SPI_Data_Comp_Tool.StepStrip step_Strip;

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private System.Windows.Forms.TableLayoutPanel tlp_Connection;
        private SPI_Data_Comp_Tool.ConnectionPanel connection;
        private System.Windows.Forms.TableLayoutPanel tlp_Schema;
        private System.Windows.Forms.TableLayoutPanel tlp_Right;
        private System.Windows.Forms.TableLayoutPanel tlp_Output;
        private System.Windows.Forms.TableLayoutPanel tlp_Log;

        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_ClearLogs;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ListTableExcel;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.LinkLabel llb_File;
        private System.Windows.Forms.Label lb_link;
        private System.Windows.Forms.TextBox tb_SchemaName;
        private System.Windows.Forms.CheckBox cb_Schema;
        private System.Windows.Forms.Button btn_Duplicate;
        private System.Windows.Forms.CheckBox cb_WithoutSpecialCharacters;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
    }
}