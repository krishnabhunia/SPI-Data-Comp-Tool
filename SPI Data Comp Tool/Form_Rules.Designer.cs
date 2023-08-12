namespace SPI_Data_Comp_Tool
{
    partial class Form_Rules
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
            this.btn_ImportRules = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_ImportRules
            // 
            this.btn_ImportRules.Location = new System.Drawing.Point(12, 42);
            this.btn_ImportRules.Name = "btn_ImportRules";
            this.btn_ImportRules.Size = new System.Drawing.Size(115, 23);
            this.btn_ImportRules.TabIndex = 0;
            this.btn_ImportRules.Text = "Import Rules";
            this.btn_ImportRules.UseVisualStyleBackColor = true;
            this.btn_ImportRules.Click += new System.EventHandler(this.btn_ImportRules_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(564, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Import Excel File to Incorporate rules based comparison in Data Comparison and Qu" +
    "ality Form";
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(12, 81);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(564, 120);
            this.lbl_Status.TabIndex = 2;
            this.lbl_Status.Text = "Status : Okay";
            // 
            // Form_Rules
            // 
            this.AcceptButton = this.btn_ImportRules;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 210);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_ImportRules);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Rules";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Import Rules";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_Rules_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ImportRules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Status;
    }
}