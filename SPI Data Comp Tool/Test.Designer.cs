namespace SPI_Data_Comp_Tool
{
    partial class Test
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
            this.btn_Test = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_Clear_Reset = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.userControlDataBase1 = new SPI_Data_Comp_Tool.UserControlDataBase();
            this.btn_Intersect = new System.Windows.Forms.Button();
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Test
            // 
            this.btn_Test.Location = new System.Drawing.Point(13, 12);
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(75, 23);
            this.btn_Test.TabIndex = 0;
            this.btn_Test.Text = "Click";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.progressBar1);
            this.groupBox10.Controls.Add(this.btn_Clear_Reset);
            this.groupBox10.Controls.Add(this.rtb_Log);
            this.groupBox10.Location = new System.Drawing.Point(13, 60);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(835, 655);
            this.groupBox10.TabIndex = 34;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Status";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 23);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(726, 15);
            this.progressBar1.TabIndex = 1;
            // 
            // btn_Clear_Reset
            // 
            this.btn_Clear_Reset.Location = new System.Drawing.Point(742, 19);
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
            this.rtb_Log.Size = new System.Drawing.Size(816, 596);
            this.rtb_Log.TabIndex = 0;
            this.rtb_Log.Text = "";
            // 
            // userControlDataBase1
            // 
            this.userControlDataBase1.Location = new System.Drawing.Point(854, 12);
            this.userControlDataBase1.Name = "userControlDataBase1";
            this.userControlDataBase1.Size = new System.Drawing.Size(798, 435);
            this.userControlDataBase1.TabIndex = 35;
            // 
            // btn_Intersect
            // 
            this.btn_Intersect.Location = new System.Drawing.Point(870, 453);
            this.btn_Intersect.Name = "btn_Intersect";
            this.btn_Intersect.Size = new System.Drawing.Size(75, 23);
            this.btn_Intersect.TabIndex = 36;
            this.btn_Intersect.Text = "Intersect";
            this.btn_Intersect.UseVisualStyleBackColor = true;
            this.btn_Intersect.Click += new System.EventHandler(this.btn_Intersect_Click);
            // 
            // Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1731, 720);
            this.Controls.Add(this.btn_Intersect);
            this.Controls.Add(this.userControlDataBase1);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.btn_Test);
            this.Name = "Test";
            this.Text = "Test";
            this.groupBox10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_Clear_Reset;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private UserControlDataBase userControlDataBase1;
        private System.Windows.Forms.Button btn_Intersect;
    }
}