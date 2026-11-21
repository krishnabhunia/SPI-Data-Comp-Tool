namespace SPI_Data_Comp_Tool
{
    partial class Excel_Excel_Comparison
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Excel_Excel_Comparison));
            this.Load += new System.EventHandler(this.Excel_Excel_Comparison_Load);
            this.components = new System.ComponentModel.Container();
            this.lblError = new System.Windows.Forms.Label();
            this.lbl_FirstExcelFileName = new System.Windows.Forms.Label();
            this.lbl_SecondExcelFileName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_MultiReference = new System.Windows.Forms.CheckBox();
            this.btn_SelectLoad = new System.Windows.Forms.Button();
            this.lbl_LoadStatus = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lbl_Timer = new System.Windows.Forms.Label();
            this.btn_Compare = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_Primary_Key = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_select_first_clb = new System.Windows.Forms.CheckBox();
            this.lb_IgnoreTable2 = new System.Windows.Forms.ListBox();
            this.lb_IgnoreTable1 = new System.Windows.Forms.ListBox();
            this.clb_secondExcel = new System.Windows.Forms.CheckedListBox();
            this.clb_firstExcel = new System.Windows.Forms.CheckedListBox();
            this.clb_reference_ID = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbl_Parameter = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cb_IncludeDiff_Columns = new System.Windows.Forms.CheckBox();
            this.cb_IgnoreCase = new System.Windows.Forms.CheckBox();
            this.cb_IncludeRule = new System.Windows.Forms.CheckBox();
            this.btn_RuleForm = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Validate = new System.Windows.Forms.Button();
            this.llbl_filePath = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lbl_Percentage = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.backgroundWorkerInstrument = new System.ComponentModel.BackgroundWorker();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lbl_InstrumentText = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_TestSystemMemoryOutException = new System.Windows.Forms.Button();
            this.tlp_Root = new System.Windows.Forms.TableLayoutPanel();
            this.step_Strip = new SPI_Data_Comp_Tool.StepStrip();
            this.tlp_Files = new System.Windows.Forms.TableLayoutPanel();
            this.btn_FirstExcelSheet = new System.Windows.Forms.Button();
            this.btn_SecondExcelSheet = new System.Windows.Forms.Button();
            this.tlp_View = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_RefKey = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_SrcCols = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_TrgCols = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_IgnSrc = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_IgnTrg = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Right = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Compare = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Status = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Progress = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Instrument = new System.Windows.Forms.TableLayoutPanel();
            this.flp_Instrument = new System.Windows.Forms.FlowLayoutPanel();
            this.tlp_Root.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlp_Files.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlp_View.SuspendLayout();
            this.tlp_RefKey.SuspendLayout();
            this.tlp_SrcCols.SuspendLayout();
            this.tlp_TrgCols.SuspendLayout();
            this.tlp_IgnSrc.SuspendLayout();
            this.tlp_IgnTrg.SuspendLayout();
            this.tlp_Right.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tlp_Compare.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlp_Status.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tlp_Progress.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tlp_Instrument.SuspendLayout();
            this.flp_Instrument.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Root
            // 
            this.tlp_Root.Name = "tlp_Root";
            this.tlp_Root.ColumnCount = 2;
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tlp_Root.RowCount = 5;
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Root.Controls.Add(this.step_Strip, 0, 0);
            this.tlp_Root.SetColumnSpan(this.step_Strip, 2);
            this.tlp_Root.Controls.Add(this.groupBox1, 0, 1);
            this.tlp_Root.Controls.Add(this.groupBox2, 0, 2);
            this.tlp_Root.Controls.Add(this.tlp_Right, 1, 1);
            this.tlp_Root.SetRowSpan(this.tlp_Right, 2);
            this.tlp_Root.Controls.Add(this.groupBox5, 0, 3);
            this.tlp_Root.SetColumnSpan(this.groupBox5, 2);
            this.tlp_Root.Controls.Add(this.groupBox7, 0, 4);
            this.tlp_Root.SetColumnSpan(this.groupBox7, 2);
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
            // groupBox1
            // 
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Workbooks";
            this.groupBox1.Controls.Add(this.tlp_Files);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox1.TabIndex = 2;
            // 
            // tlp_Files
            // 
            this.tlp_Files.Name = "tlp_Files";
            this.tlp_Files.ColumnCount = 2;
            this.tlp_Files.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_Files.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Files.RowCount = 4;
            this.tlp_Files.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Files.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Files.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Files.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Files.Controls.Add(this.btn_FirstExcelSheet, 0, 0);
            this.tlp_Files.Controls.Add(this.lbl_FirstExcelFileName, 1, 0);
            this.tlp_Files.Controls.Add(this.btn_SecondExcelSheet, 0, 1);
            this.tlp_Files.Controls.Add(this.lbl_SecondExcelFileName, 1, 1);
            this.tlp_Files.Controls.Add(this.cb_MultiReference, 0, 2);
            this.tlp_Files.SetColumnSpan(this.cb_MultiReference, 2);
            this.tlp_Files.Controls.Add(this.btn_SelectLoad, 0, 3);
            this.tlp_Files.Controls.Add(this.lbl_LoadStatus, 1, 3);
            this.tlp_Files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Files.AutoSize = true;
            this.tlp_Files.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Files.TabIndex = 3;
            // 
            // btn_FirstExcelSheet
            // 
            this.btn_FirstExcelSheet.Name = "btn_FirstExcelSheet";
            this.btn_FirstExcelSheet.Text = "Choose source…";
            this.btn_FirstExcelSheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_FirstExcelSheet.Margin = new System.Windows.Forms.Padding(0, 2, 10, 2);
            this.btn_FirstExcelSheet.Size = new System.Drawing.Size(140, 30);
            this.btn_FirstExcelSheet.TabIndex = 4;
            // 
            // lbl_FirstExcelFileName
            // 
            this.lbl_FirstExcelFileName.Name = "lbl_FirstExcelFileName";
            this.lbl_FirstExcelFileName.Text = "No file selected";
            this.lbl_FirstExcelFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lbl_FirstExcelFileName.AutoSize = false;
            this.lbl_FirstExcelFileName.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_FirstExcelFileName.Size = new System.Drawing.Size(400, 30);
            this.lbl_FirstExcelFileName.TabIndex = 5;
            // 
            // btn_SecondExcelSheet
            // 
            this.btn_SecondExcelSheet.Name = "btn_SecondExcelSheet";
            this.btn_SecondExcelSheet.Text = "Choose target…";
            this.btn_SecondExcelSheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SecondExcelSheet.Margin = new System.Windows.Forms.Padding(0, 2, 10, 2);
            this.btn_SecondExcelSheet.Size = new System.Drawing.Size(140, 30);
            this.btn_SecondExcelSheet.TabIndex = 6;
            // 
            // lbl_SecondExcelFileName
            // 
            this.lbl_SecondExcelFileName.Name = "lbl_SecondExcelFileName";
            this.lbl_SecondExcelFileName.Text = "No file selected";
            this.lbl_SecondExcelFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lbl_SecondExcelFileName.AutoSize = false;
            this.lbl_SecondExcelFileName.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_SecondExcelFileName.Size = new System.Drawing.Size(400, 30);
            this.lbl_SecondExcelFileName.TabIndex = 7;
            // 
            // cb_MultiReference
            // 
            this.cb_MultiReference.Name = "cb_MultiReference";
            this.cb_MultiReference.UseVisualStyleBackColor = true;
            this.cb_MultiReference.Text = "Different column names / RefID comparison";
            this.cb_MultiReference.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_MultiReference.AutoSize = true;
            this.cb_MultiReference.Margin = new System.Windows.Forms.Padding(0, 6, 0, 2);
            this.cb_MultiReference.TabIndex = 8;
            // 
            // btn_SelectLoad
            // 
            this.btn_SelectLoad.Name = "btn_SelectLoad";
            this.btn_SelectLoad.UseVisualStyleBackColor = true;
            this.btn_SelectLoad.Click += new System.EventHandler(this.btn_SelectLoad_Click);
            this.btn_SelectLoad.Text = "Load both files";
            this.btn_SelectLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SelectLoad.Margin = new System.Windows.Forms.Padding(0, 2, 10, 2);
            this.btn_SelectLoad.Size = new System.Drawing.Size(140, 32);
            this.btn_SelectLoad.TabIndex = 9;
            // 
            // lbl_LoadStatus
            // 
            this.lbl_LoadStatus.Name = "lbl_LoadStatus";
            this.lbl_LoadStatus.Text = "Choose both workbooks, then load them.";
            this.lbl_LoadStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lbl_LoadStatus.AutoSize = false;
            this.lbl_LoadStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_LoadStatus.Size = new System.Drawing.Size(400, 32);
            this.lbl_LoadStatus.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Keys and columns";
            this.groupBox2.Controls.Add(this.tlp_View);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox2.TabIndex = 11;
            // 
            // tlp_View
            // 
            this.tlp_View.Name = "tlp_View";
            this.tlp_View.ColumnCount = 5;
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tlp_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tlp_View.RowCount = 1;
            this.tlp_View.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_View.Controls.Add(this.tlp_RefKey, 0, 0);
            this.tlp_View.Controls.Add(this.tlp_SrcCols, 1, 0);
            this.tlp_View.Controls.Add(this.tlp_TrgCols, 2, 0);
            this.tlp_View.Controls.Add(this.tlp_IgnSrc, 3, 0);
            this.tlp_View.Controls.Add(this.tlp_IgnTrg, 4, 0);
            this.tlp_View.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_View.AutoSize = false;
            this.tlp_View.TabIndex = 12;
            // 
            // tlp_RefKey
            // 
            this.tlp_RefKey.Name = "tlp_RefKey";
            this.tlp_RefKey.ColumnCount = 1;
            this.tlp_RefKey.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_RefKey.RowCount = 3;
            this.tlp_RefKey.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_RefKey.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_RefKey.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_RefKey.Controls.Add(this.label3, 0, 0);
            this.tlp_RefKey.Controls.Add(this.clb_reference_ID, 0, 1);
            this.tlp_RefKey.Controls.Add(this.cb_Primary_Key, 0, 2);
            this.tlp_RefKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_RefKey.AutoSize = false;
            this.tlp_RefKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tlp_RefKey.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.Name = "label3";
            this.label3.Text = "Reference key";
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label3.AutoSize = true;
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label3.TabIndex = 14;
            // 
            // clb_reference_ID
            // 
            this.clb_reference_ID.CheckOnClick = true;
            this.clb_reference_ID.FormattingEnabled = true;
            this.clb_reference_ID.HorizontalScrollbar = true;
            this.clb_reference_ID.Name = "clb_reference_ID";
            this.clb_reference_ID.SelectedIndexChanged += new System.EventHandler(this.clb_reference_ID_SelectedIndexChanged);
            this.clb_reference_ID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clb_reference_ID.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.clb_reference_ID.TabIndex = 15;
            // 
            // cb_Primary_Key
            // 
            this.cb_Primary_Key.Checked = true;
            this.cb_Primary_Key.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Primary_Key.Name = "cb_Primary_Key";
            this.cb_Primary_Key.UseVisualStyleBackColor = true;
            this.cb_Primary_Key.Text = "Reference key is the primary key";
            this.cb_Primary_Key.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_Primary_Key.AutoSize = true;
            this.cb_Primary_Key.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.cb_Primary_Key.TabIndex = 16;
            // 
            // tlp_SrcCols
            // 
            this.tlp_SrcCols.Name = "tlp_SrcCols";
            this.tlp_SrcCols.ColumnCount = 1;
            this.tlp_SrcCols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SrcCols.RowCount = 3;
            this.tlp_SrcCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SrcCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SrcCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_SrcCols.Controls.Add(this.label4, 0, 0);
            this.tlp_SrcCols.Controls.Add(this.clb_firstExcel, 0, 1);
            this.tlp_SrcCols.Controls.Add(this.cb_select_first_clb, 0, 2);
            this.tlp_SrcCols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SrcCols.AutoSize = false;
            this.tlp_SrcCols.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tlp_SrcCols.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.Name = "label4";
            this.label4.Text = "Source columns";
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label4.AutoSize = true;
            this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label4.TabIndex = 18;
            // 
            // clb_firstExcel
            // 
            this.clb_firstExcel.CheckOnClick = true;
            this.clb_firstExcel.FormattingEnabled = true;
            this.clb_firstExcel.HorizontalScrollbar = true;
            this.clb_firstExcel.Name = "clb_firstExcel";
            this.clb_firstExcel.SelectedIndexChanged += new System.EventHandler(this.clb_firstExcel_SelectedIndexChanged);
            this.clb_firstExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clb_firstExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.clb_firstExcel.TabIndex = 19;
            // 
            // cb_select_first_clb
            // 
            this.cb_select_first_clb.Name = "cb_select_first_clb";
            this.cb_select_first_clb.UseVisualStyleBackColor = true;
            this.cb_select_first_clb.CheckedChanged += new System.EventHandler(this.cb_select_first_clb_CheckedChanged);
            this.cb_select_first_clb.Text = "Select all / none";
            this.cb_select_first_clb.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_select_first_clb.AutoSize = true;
            this.cb_select_first_clb.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.cb_select_first_clb.TabIndex = 20;
            // 
            // tlp_TrgCols
            // 
            this.tlp_TrgCols.Name = "tlp_TrgCols";
            this.tlp_TrgCols.ColumnCount = 1;
            this.tlp_TrgCols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TrgCols.RowCount = 2;
            this.tlp_TrgCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_TrgCols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TrgCols.Controls.Add(this.label5, 0, 0);
            this.tlp_TrgCols.Controls.Add(this.clb_secondExcel, 0, 1);
            this.tlp_TrgCols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_TrgCols.AutoSize = false;
            this.tlp_TrgCols.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tlp_TrgCols.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.Name = "label5";
            this.label5.Text = "Target columns";
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label5.AutoSize = true;
            this.label5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label5.TabIndex = 22;
            // 
            // clb_secondExcel
            // 
            this.clb_secondExcel.CheckOnClick = true;
            this.clb_secondExcel.FormattingEnabled = true;
            this.clb_secondExcel.HorizontalScrollbar = true;
            this.clb_secondExcel.Name = "clb_secondExcel";
            this.clb_secondExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clb_secondExcel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.clb_secondExcel.TabIndex = 23;
            // 
            // tlp_IgnSrc
            // 
            this.tlp_IgnSrc.Name = "tlp_IgnSrc";
            this.tlp_IgnSrc.ColumnCount = 1;
            this.tlp_IgnSrc.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnSrc.RowCount = 2;
            this.tlp_IgnSrc.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_IgnSrc.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnSrc.Controls.Add(this.label6, 0, 0);
            this.tlp_IgnSrc.Controls.Add(this.lb_IgnoreTable1, 0, 1);
            this.tlp_IgnSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_IgnSrc.AutoSize = false;
            this.tlp_IgnSrc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tlp_IgnSrc.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.Name = "label6";
            this.label6.Text = "Ignored in Source";
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label6.AutoSize = true;
            this.label6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label6.TabIndex = 25;
            // 
            // lb_IgnoreTable1
            // 
            this.lb_IgnoreTable1.FormattingEnabled = true;
            this.lb_IgnoreTable1.HorizontalScrollbar = true;
            this.lb_IgnoreTable1.ItemHeight = 16;
            this.lb_IgnoreTable1.Name = "lb_IgnoreTable1";
            this.lb_IgnoreTable1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoreTable1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_IgnoreTable1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lb_IgnoreTable1.TabIndex = 26;
            // 
            // tlp_IgnTrg
            // 
            this.tlp_IgnTrg.Name = "tlp_IgnTrg";
            this.tlp_IgnTrg.ColumnCount = 1;
            this.tlp_IgnTrg.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnTrg.RowCount = 2;
            this.tlp_IgnTrg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_IgnTrg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_IgnTrg.Controls.Add(this.label7, 0, 0);
            this.tlp_IgnTrg.Controls.Add(this.lb_IgnoreTable2, 0, 1);
            this.tlp_IgnTrg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_IgnTrg.AutoSize = false;
            this.tlp_IgnTrg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tlp_IgnTrg.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.Name = "label7";
            this.label7.Text = "Ignored in Target";
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label7.AutoSize = true;
            this.label7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label7.TabIndex = 28;
            // 
            // lb_IgnoreTable2
            // 
            this.lb_IgnoreTable2.FormattingEnabled = true;
            this.lb_IgnoreTable2.HorizontalScrollbar = true;
            this.lb_IgnoreTable2.ItemHeight = 16;
            this.lb_IgnoreTable2.Name = "lb_IgnoreTable2";
            this.lb_IgnoreTable2.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_IgnoreTable2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_IgnoreTable2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lb_IgnoreTable2.TabIndex = 29;
            // 
            // tlp_Right
            // 
            this.tlp_Right.Name = "tlp_Right";
            this.tlp_Right.ColumnCount = 1;
            this.tlp_Right.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Right.RowCount = 2;
            this.tlp_Right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Right.Controls.Add(this.groupBox4, 0, 0);
            this.tlp_Right.Controls.Add(this.groupBox3, 0, 1);
            this.tlp_Right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Right.AutoSize = false;
            this.tlp_Right.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tlp_Right.TabIndex = 30;
            // 
            // groupBox4
            // 
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Compare";
            this.groupBox4.Controls.Add(this.tlp_Compare);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.AutoSize = true;
            this.groupBox4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.groupBox4.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox4.TabIndex = 31;
            // 
            // tlp_Compare
            // 
            this.tlp_Compare.Name = "tlp_Compare";
            this.tlp_Compare.ColumnCount = 2;
            this.tlp_Compare.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Compare.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Compare.RowCount = 8;
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Compare.Controls.Add(this.cb_IncludeDiff_Columns, 0, 0);
            this.tlp_Compare.SetColumnSpan(this.cb_IncludeDiff_Columns, 2);
            this.tlp_Compare.Controls.Add(this.cb_IgnoreCase, 0, 1);
            this.tlp_Compare.SetColumnSpan(this.cb_IgnoreCase, 2);
            this.tlp_Compare.Controls.Add(this.cb_IncludeRule, 0, 2);
            this.tlp_Compare.Controls.Add(this.btn_RuleForm, 1, 2);
            this.tlp_Compare.Controls.Add(this.btn_Validate, 0, 3);
            this.tlp_Compare.SetColumnSpan(this.btn_Validate, 2);
            this.tlp_Compare.Controls.Add(this.btn_Compare, 0, 4);
            this.tlp_Compare.SetColumnSpan(this.btn_Compare, 2);
            this.tlp_Compare.Controls.Add(this.btn_Cancel, 0, 5);
            this.tlp_Compare.SetColumnSpan(this.btn_Cancel, 2);
            this.tlp_Compare.Controls.Add(this.label1, 0, 6);
            this.tlp_Compare.SetColumnSpan(this.label1, 2);
            this.tlp_Compare.Controls.Add(this.llbl_filePath, 0, 7);
            this.tlp_Compare.SetColumnSpan(this.llbl_filePath, 2);
            this.tlp_Compare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Compare.AutoSize = true;
            this.tlp_Compare.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Compare.TabIndex = 32;
            // 
            // cb_IncludeDiff_Columns
            // 
            this.cb_IncludeDiff_Columns.Checked = true;
            this.cb_IncludeDiff_Columns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_IncludeDiff_Columns.Name = "cb_IncludeDiff_Columns";
            this.cb_IncludeDiff_Columns.UseVisualStyleBackColor = true;
            this.cb_IncludeDiff_Columns.Text = "Only columns that differ";
            this.cb_IncludeDiff_Columns.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IncludeDiff_Columns.AutoSize = true;
            this.cb_IncludeDiff_Columns.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IncludeDiff_Columns.TabIndex = 33;
            // 
            // cb_IgnoreCase
            // 
            this.cb_IgnoreCase.Name = "cb_IgnoreCase";
            this.cb_IgnoreCase.UseVisualStyleBackColor = true;
            this.cb_IgnoreCase.Text = "Ignore case";
            this.cb_IgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IgnoreCase.AutoSize = true;
            this.cb_IgnoreCase.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IgnoreCase.TabIndex = 34;
            // 
            // cb_IncludeRule
            // 
            this.cb_IncludeRule.Name = "cb_IncludeRule";
            this.cb_IncludeRule.UseVisualStyleBackColor = true;
            this.cb_IncludeRule.CheckedChanged += new System.EventHandler(this.cb_IncludeRule_CheckedChanged);
            this.cb_IncludeRule.Text = "Include rules";
            this.cb_IncludeRule.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.cb_IncludeRule.AutoSize = true;
            this.cb_IncludeRule.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cb_IncludeRule.TabIndex = 35;
            // 
            // btn_RuleForm
            // 
            this.btn_RuleForm.Name = "btn_RuleForm";
            this.btn_RuleForm.UseVisualStyleBackColor = true;
            this.btn_RuleForm.Click += new System.EventHandler(this.btn_RuleForm_Click);
            this.btn_RuleForm.Text = "Open rules…";
            this.btn_RuleForm.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.btn_RuleForm.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btn_RuleForm.Size = new System.Drawing.Size(110, 28);
            this.btn_RuleForm.TabIndex = 36;
            // 
            // btn_Validate
            // 
            this.btn_Validate.Enabled = false;
            this.btn_Validate.Name = "btn_Validate";
            this.btn_Validate.UseVisualStyleBackColor = true;
            this.btn_Validate.Click += new System.EventHandler(this.btn_Validate_Click);
            this.btn_Validate.Text = "Validate";
            this.btn_Validate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Validate.Margin = new System.Windows.Forms.Padding(0, 10, 0, 4);
            this.btn_Validate.Size = new System.Drawing.Size(120, 32);
            this.btn_Validate.TabIndex = 37;
            // 
            // btn_Compare
            // 
            this.btn_Compare.Enabled = false;
            this.btn_Compare.Name = "btn_Compare";
            this.btn_Compare.UseVisualStyleBackColor = true;
            this.btn_Compare.Click += new System.EventHandler(this.btn_Compare_Click);
            this.btn_Compare.Text = "Compare and generate Excel";
            this.btn_Compare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Compare.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btn_Compare.Size = new System.Drawing.Size(120, 36);
            this.btn_Compare.TabIndex = 38;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btn_Cancel.Size = new System.Drawing.Size(100, 30);
            this.btn_Cancel.TabIndex = 39;
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Visible = false;
            this.label1.Text = "Report";
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.label1.TabIndex = 40;
            // 
            // llbl_filePath
            // 
            this.llbl_filePath.Name = "llbl_filePath";
            this.llbl_filePath.TabStop = true;
            this.llbl_filePath.Visible = false;
            this.llbl_filePath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.llbl_filePath.MouseHover += new System.EventHandler(this.llbl_filePath_MouseHover);
            this.llbl_filePath.Text = "";
            this.llbl_filePath.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.llbl_filePath.AutoSize = false;
            this.llbl_filePath.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.llbl_filePath.Size = new System.Drawing.Size(100, 40);
            this.llbl_filePath.TabIndex = 41;
            // 
            // groupBox3
            // 
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status";
            this.groupBox3.Controls.Add(this.tlp_Status);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.groupBox3.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox3.TabIndex = 42;
            // 
            // tlp_Status
            // 
            this.tlp_Status.Name = "tlp_Status";
            this.tlp_Status.ColumnCount = 2;
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.RowCount = 4;
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Status.Controls.Add(this.label8, 0, 0);
            this.tlp_Status.Controls.Add(this.lbl_Status, 1, 0);
            this.tlp_Status.Controls.Add(this.lbl_Parameter, 0, 1);
            this.tlp_Status.SetColumnSpan(this.lbl_Parameter, 2);
            this.tlp_Status.Controls.Add(this.lbl_Timer, 0, 2);
            this.tlp_Status.SetColumnSpan(this.lbl_Timer, 2);
            this.tlp_Status.Controls.Add(this.lblError, 0, 3);
            this.tlp_Status.SetColumnSpan(this.lblError, 2);
            this.tlp_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Status.AutoSize = false;
            this.tlp_Status.TabIndex = 43;
            // 
            // label8
            // 
            this.label8.Name = "label8";
            this.label8.Text = "Status:";
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.label8.AutoSize = true;
            this.label8.Margin = new System.Windows.Forms.Padding(0, 0, 6, 4);
            this.label8.TabIndex = 44;
            // 
            // lbl_Status
            // 
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Text = "Okay";
            this.lbl_Status.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lbl_Status.AutoSize = false;
            this.lbl_Status.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lbl_Status.Size = new System.Drawing.Size(100, 60);
            this.lbl_Status.TabIndex = 45;
            // 
            // lbl_Parameter
            // 
            this.lbl_Parameter.Name = "lbl_Parameter";
            this.lbl_Parameter.Text = "";
            this.lbl_Parameter.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Parameter.AutoSize = true;
            this.lbl_Parameter.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lbl_Parameter.TabIndex = 46;
            // 
            // lbl_Timer
            // 
            this.lbl_Timer.Name = "lbl_Timer";
            this.lbl_Timer.Text = "";
            this.lbl_Timer.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Timer.AutoSize = true;
            this.lbl_Timer.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lbl_Timer.TabIndex = 47;
            // 
            // lblError
            // 
            this.lblError.Name = "lblError";
            this.lblError.Text = "Error Status : Okay";
            this.lblError.Visible = false;
            this.lblError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblError.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblError.TabIndex = 48;
            // 
            // groupBox5
            // 
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Progress";
            this.groupBox5.Controls.Add(this.tlp_Progress);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.AutoSize = true;
            this.groupBox5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox5.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox5.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox5.TabIndex = 49;
            // 
            // tlp_Progress
            // 
            this.tlp_Progress.Name = "tlp_Progress";
            this.tlp_Progress.ColumnCount = 2;
            this.tlp_Progress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Progress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Progress.RowCount = 1;
            this.tlp_Progress.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Progress.Controls.Add(this.progressBar1, 0, 0);
            this.tlp_Progress.Controls.Add(this.lbl_Percentage, 1, 0);
            this.tlp_Progress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Progress.AutoSize = true;
            this.tlp_Progress.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Progress.TabIndex = 50;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 4, 16, 4);
            this.progressBar1.Size = new System.Drawing.Size(300, 14);
            this.progressBar1.TabIndex = 51;
            // 
            // lbl_Percentage
            // 
            this.lbl_Percentage.Name = "lbl_Percentage";
            this.lbl_Percentage.Text = "0 %";
            this.lbl_Percentage.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left));
            this.lbl_Percentage.AutoSize = true;
            this.lbl_Percentage.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lbl_Percentage.MinimumSize = new System.Drawing.Size(120, 0);
            this.lbl_Percentage.TabIndex = 52;
            // 
            // groupBox7
            // 
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Instrument analysis";
            this.groupBox7.Controls.Add(this.tlp_Instrument);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.AutoSize = true;
            this.groupBox7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox7.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBox7.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.groupBox7.TabIndex = 53;
            // 
            // tlp_Instrument
            // 
            this.tlp_Instrument.Name = "tlp_Instrument";
            this.tlp_Instrument.ColumnCount = 2;
            this.tlp_Instrument.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Instrument.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Instrument.RowCount = 1;
            this.tlp_Instrument.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlp_Instrument.Controls.Add(this.lbl_InstrumentText, 0, 0);
            this.tlp_Instrument.Controls.Add(this.flp_Instrument, 1, 0);
            this.tlp_Instrument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Instrument.AutoSize = true;
            this.tlp_Instrument.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_Instrument.TabIndex = 54;
            // 
            // lbl_InstrumentText
            // 
            this.lbl_InstrumentText.Name = "lbl_InstrumentText";
            this.lbl_InstrumentText.Text = "lbl_InstrumentText";
            this.lbl_InstrumentText.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lbl_InstrumentText.AutoSize = false;
            this.lbl_InstrumentText.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lbl_InstrumentText.Size = new System.Drawing.Size(400, 40);
            this.lbl_InstrumentText.TabIndex = 55;
            // 
            // flp_Instrument
            // 
            this.flp_Instrument.Name = "flp_Instrument";
            this.flp_Instrument.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flp_Instrument.WrapContents = false;
            this.flp_Instrument.Controls.Add(this.btnLoad);
            this.flp_Instrument.Controls.Add(this.button1);
            this.flp_Instrument.Controls.Add(this.btn_TestSystemMemoryOutException);
            this.flp_Instrument.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Right));
            this.flp_Instrument.AutoSize = true;
            this.flp_Instrument.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_Instrument.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.flp_Instrument.TabIndex = 56;
            // 
            // btnLoad
            // 
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Text = "Generate Instrumentation Analysis Excel Input File";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click_1);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnLoad.Size = new System.Drawing.Size(300, 30);
            this.btnLoad.TabIndex = 57;
            // 
            // button1
            // 
            this.button1.Name = "button1";
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 58;
            // 
            // btn_TestSystemMemoryOutException
            // 
            this.btn_TestSystemMemoryOutException.Name = "btn_TestSystemMemoryOutException";
            this.btn_TestSystemMemoryOutException.Text = "Test System Memory Out Exception";
            this.btn_TestSystemMemoryOutException.UseVisualStyleBackColor = true;
            this.btn_TestSystemMemoryOutException.Visible = false;
            this.btn_TestSystemMemoryOutException.Click += new System.EventHandler(this.btn_TestSystemMemoryOutException_Click);
            this.btn_TestSystemMemoryOutException.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btn_TestSystemMemoryOutException.Size = new System.Drawing.Size(220, 30);
            this.btn_TestSystemMemoryOutException.TabIndex = 59;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorkerInstrument
            // 
            this.backgroundWorkerInstrument.WorkerReportsProgress = true;
            this.backgroundWorkerInstrument.WorkerSupportsCancellation = true;
            this.backgroundWorkerInstrument.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerInstrument_DoWork);
            this.backgroundWorkerInstrument.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerInstrument_ProgressChanged);
            this.backgroundWorkerInstrument.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerInstrument_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 999;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // Excel_Excel_Comparison
            // 
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Excel_Excel_Comparison";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = "Compare Excel files";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(980, 660);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.Controls.Add(this.tlp_Root);
            this.tlp_Root.ResumeLayout(false);
            this.tlp_Root.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlp_Files.ResumeLayout(false);
            this.tlp_Files.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tlp_View.ResumeLayout(false);
            this.tlp_View.PerformLayout();
            this.tlp_RefKey.ResumeLayout(false);
            this.tlp_RefKey.PerformLayout();
            this.tlp_SrcCols.ResumeLayout(false);
            this.tlp_SrcCols.PerformLayout();
            this.tlp_TrgCols.ResumeLayout(false);
            this.tlp_TrgCols.PerformLayout();
            this.tlp_IgnSrc.ResumeLayout(false);
            this.tlp_IgnSrc.PerformLayout();
            this.tlp_IgnTrg.ResumeLayout(false);
            this.tlp_IgnTrg.PerformLayout();
            this.tlp_Right.ResumeLayout(false);
            this.tlp_Right.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tlp_Compare.ResumeLayout(false);
            this.tlp_Compare.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlp_Status.ResumeLayout(false);
            this.tlp_Status.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tlp_Progress.ResumeLayout(false);
            this.tlp_Progress.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tlp_Instrument.ResumeLayout(false);
            this.tlp_Instrument.PerformLayout();
            this.flp_Instrument.ResumeLayout(false);
            this.flp_Instrument.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Root;
        private SPI_Data_Comp_Tool.StepStrip step_Strip;
        private System.Windows.Forms.TableLayoutPanel tlp_Files;
        private System.Windows.Forms.Button btn_FirstExcelSheet;
        private System.Windows.Forms.Button btn_SecondExcelSheet;
        private System.Windows.Forms.TableLayoutPanel tlp_View;
        private System.Windows.Forms.TableLayoutPanel tlp_RefKey;
        private System.Windows.Forms.TableLayoutPanel tlp_SrcCols;
        private System.Windows.Forms.TableLayoutPanel tlp_TrgCols;
        private System.Windows.Forms.TableLayoutPanel tlp_IgnSrc;
        private System.Windows.Forms.TableLayoutPanel tlp_IgnTrg;
        private System.Windows.Forms.TableLayoutPanel tlp_Right;
        private System.Windows.Forms.TableLayoutPanel tlp_Compare;
        private System.Windows.Forms.TableLayoutPanel tlp_Status;
        private System.Windows.Forms.TableLayoutPanel tlp_Progress;
        private System.Windows.Forms.TableLayoutPanel tlp_Instrument;
        private System.Windows.Forms.FlowLayoutPanel flp_Instrument;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lbl_FirstExcelFileName;
        private System.Windows.Forms.Label lbl_SecondExcelFileName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox clb_secondExcel;
        private System.Windows.Forms.CheckedListBox clb_firstExcel;
        private System.Windows.Forms.CheckedListBox clb_reference_ID;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbl_LoadStatus;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.ListBox lb_IgnoreTable1;
        private System.Windows.Forms.ListBox lb_IgnoreTable2;
        private System.Windows.Forms.LinkLabel llbl_filePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_select_first_clb;
        private System.Windows.Forms.Button btn_SelectLoad;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Validate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Compare;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lbl_Timer;
        private System.Windows.Forms.Label lbl_Percentage;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Label lbl_Parameter;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cb_IncludeRule;
        private System.Windows.Forms.Button btn_RuleForm;
        private System.Windows.Forms.CheckBox cb_IgnoreCase;
        private System.Windows.Forms.CheckBox cb_IncludeDiff_Columns;
        private System.Windows.Forms.CheckBox cb_MultiReference;
        private System.ComponentModel.BackgroundWorker backgroundWorkerInstrument;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label lbl_InstrumentText;
        private System.Windows.Forms.CheckBox cb_Primary_Key;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_TestSystemMemoryOutException;
    }
}
