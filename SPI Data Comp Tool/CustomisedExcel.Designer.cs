namespace SPI_Data_Comp_Tool
{
    partial class CustomisedExcel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomisedExcel));
            this.btn_ReadExcel = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.btn_CreateExcel = new System.Windows.Forms.Button();
            this.cb_SelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btn_ReadExcel
            // 
            this.btn_ReadExcel.Location = new System.Drawing.Point(18, 12);
            this.btn_ReadExcel.Name = "btn_ReadExcel";
            this.btn_ReadExcel.Size = new System.Drawing.Size(140, 23);
            this.btn_ReadExcel.TabIndex = 0;
            this.btn_ReadExcel.Text = "Read Excel File";
            this.btn_ReadExcel.UseVisualStyleBackColor = true;
            this.btn_ReadExcel.Click += new System.EventHandler(this.btn_ReadExcel_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(18, 128);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(273, 555);
            this.checkedListBox1.TabIndex = 1;
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(164, 15);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(601, 107);
            this.lbl_Status.TabIndex = 2;
            this.lbl_Status.Text = "Status : Okay";
            // 
            // btn_CreateExcel
            // 
            this.btn_CreateExcel.Location = new System.Drawing.Point(18, 66);
            this.btn_CreateExcel.Name = "btn_CreateExcel";
            this.btn_CreateExcel.Size = new System.Drawing.Size(140, 23);
            this.btn_CreateExcel.TabIndex = 3;
            this.btn_CreateExcel.Text = "Create Customised Excel";
            this.btn_CreateExcel.UseVisualStyleBackColor = true;
            this.btn_CreateExcel.Click += new System.EventHandler(this.btn_CreateExcel_Click);
            // 
            // cb_SelectAll
            // 
            this.cb_SelectAll.AutoSize = true;
            this.cb_SelectAll.Location = new System.Drawing.Point(18, 43);
            this.cb_SelectAll.Name = "cb_SelectAll";
            this.cb_SelectAll.Size = new System.Drawing.Size(143, 17);
            this.cb_SelectAll.TabIndex = 4;
            this.cb_SelectAll.Text = "Select All \\ None Toggle";
            this.cb_SelectAll.UseVisualStyleBackColor = true;
            this.cb_SelectAll.CheckedChanged += new System.EventHandler(this.cb_SelectAll_CheckedChanged);
            // 
            // CustomisedExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 701);
            this.Controls.Add(this.cb_SelectAll);
            this.Controls.Add(this.btn_CreateExcel);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.btn_ReadExcel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustomisedExcel";
            this.Text = "CustomisedExcel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ReadExcel;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Button btn_CreateExcel;
        private System.Windows.Forms.CheckBox cb_SelectAll;
    }
}