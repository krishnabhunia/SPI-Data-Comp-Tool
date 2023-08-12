namespace SPI_Data_Comp_Tool
{
    partial class SPI_Analysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPI_Analysis));
            this.btn_Show = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.oracleAndSQLConnection1 = new SPI_Data_Comp_Tool.OracleAndSQLConnection();
            this.SuspendLayout();
            // 
            // btn_Show
            // 
            this.btn_Show.Location = new System.Drawing.Point(780, 420);
            this.btn_Show.Name = "btn_Show";
            this.btn_Show.Size = new System.Drawing.Size(126, 23);
            this.btn_Show.TabIndex = 3;
            this.btn_Show.Text = "Open ";
            this.btn_Show.UseVisualStyleBackColor = true;
            this.btn_Show.Click += new System.EventHandler(this.btn_Show_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(777, 450);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(71, 13);
            this.lbl_Status.TabIndex = 4;
            this.lbl_Status.Text = "Status : Okay";
            // 
            // oracleAndSQLConnection1
            // 
            this.oracleAndSQLConnection1.AutoSize = true;
            this.oracleAndSQLConnection1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.oracleAndSQLConnection1.Location = new System.Drawing.Point(6, 7);
            this.oracleAndSQLConnection1.Name = "oracleAndSQLConnection1";
            this.oracleAndSQLConnection1.Size = new System.Drawing.Size(1276, 409);
            this.oracleAndSQLConnection1.TabIndex = 2;
            this.oracleAndSQLConnection1.Load += new System.EventHandler(this.oracleAndSQLConnection1_Load);
            // 
            // SPI_Analysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1284, 646);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.btn_Show);
            this.Controls.Add(this.oracleAndSQLConnection1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SPI_Analysis";
            this.Text = "SPI Spec Form Gap Analysis";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private OracleAndSQLConnection oracleAndSQLConnection1;
        private System.Windows.Forms.Button btn_Show;
        private System.Windows.Forms.Label lbl_Status;
    }
}