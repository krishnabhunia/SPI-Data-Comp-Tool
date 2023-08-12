namespace SPI_Data_Comp_Tool
{
    partial class SPI_Analysis_View
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPI_Analysis_View));
            this.cb_Source = new System.Windows.Forms.ComboBox();
            this.cb_Target = new System.Windows.Forms.ComboBox();
            this.gb_Source = new System.Windows.Forms.GroupBox();
            this.dgv_Source = new System.Windows.Forms.DataGridView();
            this.gb_Target = new System.Windows.Forms.GroupBox();
            this.dgv_Target = new System.Windows.Forms.DataGridView();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.btn_Arrange_target = new System.Windows.Forms.Button();
            this.lbl_Error = new System.Windows.Forms.Label();
            this.btn_SaveInExcel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_Remarks = new System.Windows.Forms.DataGridView();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.gb_Source.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Source)).BeginInit();
            this.gb_Target.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Target)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Remarks)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_Source
            // 
            this.cb_Source.FormattingEnabled = true;
            this.cb_Source.Location = new System.Drawing.Point(228, 19);
            this.cb_Source.Name = "cb_Source";
            this.cb_Source.Size = new System.Drawing.Size(121, 21);
            this.cb_Source.TabIndex = 0;
            this.cb_Source.SelectedIndexChanged += new System.EventHandler(this.cb_Source_SelectedIndexChanged);
            this.cb_Source.TextChanged += new System.EventHandler(this.cb_Source_TextChanged);
            // 
            // cb_Target
            // 
            this.cb_Target.FormattingEnabled = true;
            this.cb_Target.Location = new System.Drawing.Point(239, 19);
            this.cb_Target.Name = "cb_Target";
            this.cb_Target.Size = new System.Drawing.Size(121, 21);
            this.cb_Target.TabIndex = 1;
            this.cb_Target.SelectedIndexChanged += new System.EventHandler(this.cb_Target_SelectedIndexChanged);
            this.cb_Target.TextChanged += new System.EventHandler(this.cb_Target_TextChanged);
            // 
            // gb_Source
            // 
            this.gb_Source.Controls.Add(this.dgv_Source);
            this.gb_Source.Controls.Add(this.cb_Source);
            this.gb_Source.Location = new System.Drawing.Point(12, 12);
            this.gb_Source.Name = "gb_Source";
            this.gb_Source.Size = new System.Drawing.Size(567, 585);
            this.gb_Source.TabIndex = 2;
            this.gb_Source.TabStop = false;
            this.gb_Source.Text = "Source Data";
            // 
            // dgv_Source
            // 
            this.dgv_Source.AllowUserToAddRows = false;
            this.dgv_Source.AllowUserToDeleteRows = false;
            this.dgv_Source.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Source.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Source.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Source.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_Source.Location = new System.Drawing.Point(8, 59);
            this.dgv_Source.Name = "dgv_Source";
            this.dgv_Source.ReadOnly = true;
            this.dgv_Source.Size = new System.Drawing.Size(550, 512);
            this.dgv_Source.TabIndex = 1;
            this.dgv_Source.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Source_RowEnter);
            this.dgv_Source.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_Source_Scroll);
            // 
            // gb_Target
            // 
            this.gb_Target.Controls.Add(this.dgv_Target);
            this.gb_Target.Controls.Add(this.cb_Target);
            this.gb_Target.Location = new System.Drawing.Point(587, 12);
            this.gb_Target.Name = "gb_Target";
            this.gb_Target.Size = new System.Drawing.Size(569, 585);
            this.gb_Target.TabIndex = 0;
            this.gb_Target.TabStop = false;
            this.gb_Target.Text = "Target Data";
            // 
            // dgv_Target
            // 
            this.dgv_Target.AllowUserToAddRows = false;
            this.dgv_Target.AllowUserToDeleteRows = false;
            this.dgv_Target.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Target.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_Target.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Target.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgv_Target.Location = new System.Drawing.Point(10, 59);
            this.dgv_Target.Name = "dgv_Target";
            this.dgv_Target.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Target.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_Target.Size = new System.Drawing.Size(550, 512);
            this.dgv_Target.TabIndex = 2;
            this.dgv_Target.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Target_RowEnter);
            this.dgv_Target.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_Target_Scroll);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(6, 26);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(212, 16);
            this.lbl_Status.TabIndex = 3;
            this.lbl_Status.Text = "Status : Intital Loaded Successfully";
            // 
            // btn_Arrange_target
            // 
            this.btn_Arrange_target.Location = new System.Drawing.Point(585, 19);
            this.btn_Arrange_target.Name = "btn_Arrange_target";
            this.btn_Arrange_target.Size = new System.Drawing.Size(75, 23);
            this.btn_Arrange_target.TabIndex = 4;
            this.btn_Arrange_target.Text = "Arrange";
            this.btn_Arrange_target.UseVisualStyleBackColor = true;
            this.btn_Arrange_target.Click += new System.EventHandler(this.btn_Arrange_target_Click);
            // 
            // lbl_Error
            // 
            this.lbl_Error.AutoSize = true;
            this.lbl_Error.Location = new System.Drawing.Point(6, 77);
            this.lbl_Error.Name = "lbl_Error";
            this.lbl_Error.Size = new System.Drawing.Size(32, 13);
            this.lbl_Error.TabIndex = 5;
            this.lbl_Error.Text = "Okay";
            // 
            // btn_SaveInExcel
            // 
            this.btn_SaveInExcel.Location = new System.Drawing.Point(666, 19);
            this.btn_SaveInExcel.Name = "btn_SaveInExcel";
            this.btn_SaveInExcel.Size = new System.Drawing.Size(112, 23);
            this.btn_SaveInExcel.TabIndex = 6;
            this.btn_SaveInExcel.Text = "Save In Excel";
            this.btn_SaveInExcel.UseVisualStyleBackColor = true;
            this.btn_SaveInExcel.Click += new System.EventHandler(this.btn_SaveInExcel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Arrange_target);
            this.groupBox1.Controls.Add(this.lbl_Error);
            this.groupBox1.Controls.Add(this.btn_SaveInExcel);
            this.groupBox1.Controls.Add(this.lbl_Status);
            this.groupBox1.Location = new System.Drawing.Point(12, 600);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1346, 107);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status and Controls";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_Remarks);
            this.groupBox2.Location = new System.Drawing.Point(1162, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(196, 585);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remarks After Arrangment";
            // 
            // dgv_Remarks
            // 
            this.dgv_Remarks.AllowUserToAddRows = false;
            this.dgv_Remarks.AllowUserToDeleteRows = false;
            this.dgv_Remarks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_Remarks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Remarks.Location = new System.Drawing.Point(6, 59);
            this.dgv_Remarks.Name = "dgv_Remarks";
            this.dgv_Remarks.ReadOnly = true;
            this.dgv_Remarks.Size = new System.Drawing.Size(184, 512);
            this.dgv_Remarks.TabIndex = 0;
            this.dgv_Remarks.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Remarks_RowEnter);
            this.dgv_Remarks.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_Remarks_Scroll);
            // 
            // SPI_Analysis_View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb_Target);
            this.Controls.Add(this.gb_Source);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPI_Analysis_View";
            this.Text = "SPI Spec Form Gap Analysis";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SPI_Analysis_View_Load);
            this.gb_Source.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Source)).EndInit();
            this.gb_Target.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Target)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Remarks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_Source;
        private System.Windows.Forms.ComboBox cb_Target;
        private System.Windows.Forms.GroupBox gb_Source;
        private System.Windows.Forms.GroupBox gb_Target;
        private System.Windows.Forms.DataGridView dgv_Source;
        private System.Windows.Forms.DataGridView dgv_Target;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Button btn_Arrange_target;
        private System.Windows.Forms.Label lbl_Error;
        private System.Windows.Forms.Button btn_SaveInExcel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_Remarks;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}