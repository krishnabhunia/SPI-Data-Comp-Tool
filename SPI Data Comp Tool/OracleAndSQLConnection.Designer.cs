namespace SPI_Data_Comp_Tool
{
    partial class OracleAndSQLConnection
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.connection = new SPI_Data_Comp_Tool.ConnectionPanel();
            this.tb_Source_tableName = new System.Windows.Forms.TextBox();
            this.tb_destination_tableName = new System.Windows.Forms.TextBox();
            this.tb_querybox = new System.Windows.Forms.TextBox();
            this.cb_query = new System.Windows.Forms.CheckBox();
            this.btn_Connect_Reference_ID = new System.Windows.Forms.Button();
            this.btn_ResetQuery = new System.Windows.Forms.Button();
            this.lbl_Oracle_SQL_Connection = new System.Windows.Forms.Label();
            var root = new System.Windows.Forms.TableLayoutPanel();
            var tables = new System.Windows.Forms.TableLayoutPanel();
            var options = new System.Windows.Forms.FlowLayoutPanel();
            var sourceLabel = new System.Windows.Forms.Label();
            var targetLabel = new System.Windows.Forms.Label();
            var queryHint = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.connection.Name = "connection";
            this.connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.connection.TabIndex = 0;

            sourceLabel.Text = "Source table";
            sourceLabel.AutoSize = true;
            sourceLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            sourceLabel.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            targetLabel.Text = "Target table (SQL Server)";
            targetLabel.AutoSize = true;
            targetLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            targetLabel.Margin = new System.Windows.Forms.Padding(16, 3, 8, 3);
            this.tb_Source_tableName.Name = "tb_Source_tableName";
            this.tb_Source_tableName.AccessibleName = "Source table";
            this.tb_Source_tableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_Source_tableName.TabIndex = 0;
            this.tb_destination_tableName.Name = "tb_destination_tableName";
            this.tb_destination_tableName.AccessibleName = "Target table";
            this.tb_destination_tableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_destination_tableName.TabIndex = 1;
            tables.AutoSize = true;
            tables.Dock = System.Windows.Forms.DockStyle.Fill;
            tables.ColumnCount = 4;
            tables.RowCount = 1;
            tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            tables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tables.Controls.Add(sourceLabel, 0, 0);
            tables.Controls.Add(this.tb_Source_tableName, 1, 0);
            tables.Controls.Add(targetLabel, 2, 0);
            tables.Controls.Add(this.tb_destination_tableName, 3, 0);
            tables.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            tables.TabIndex = 1;

            this.cb_query.Name = "cb_query";
            this.cb_query.Text = "Use a query instead of table names";
            this.cb_query.AutoSize = true;
            this.cb_query.Margin = new System.Windows.Forms.Padding(0, 8, 16, 3);
            this.cb_query.TabIndex = 0;
            this.cb_query.CheckedChanged += new System.EventHandler(this.cb_query_CheckedChanged);
            this.btn_ResetQuery.Name = "btn_ResetQuery";
            this.btn_ResetQuery.Text = "Reset to specification query";
            this.btn_ResetQuery.AutoSize = true;
            this.btn_ResetQuery.MinimumSize = new System.Drawing.Size(210, 32);
            this.btn_ResetQuery.TabIndex = 1;
            this.btn_ResetQuery.Click += new System.EventHandler(this.btn_ResetQuery_Click);
            options.AutoSize = true;
            options.Dock = System.Windows.Forms.DockStyle.Fill;
            options.WrapContents = true;
            options.Margin = new System.Windows.Forms.Padding(0);
            options.Controls.Add(this.cb_query);
            options.Controls.Add(this.btn_ResetQuery);
            options.TabIndex = 2;

            queryHint.Text = "Advanced query mode runs the same SQL on Source and Target. Use trusted queries with restricted accounts.";
            queryHint.AutoSize = true;
            queryHint.Dock = System.Windows.Forms.DockStyle.Fill;
            queryHint.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.tb_querybox.Name = "tb_querybox";
            this.tb_querybox.AccessibleName = "Query for Source and Target";
            this.tb_querybox.Multiline = true;
            this.tb_querybox.AcceptsReturn = true;
            this.tb_querybox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_querybox.WordWrap = false;
            this.tb_querybox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_querybox.MinimumSize = new System.Drawing.Size(0, 180);
            this.tb_querybox.TabIndex = 3;
            this.tb_querybox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);

            this.btn_Connect_Reference_ID.Name = "btn_Connect_Reference_ID";
            this.btn_Connect_Reference_ID.Text = "Connect and load";
            this.btn_Connect_Reference_ID.AutoSize = true;
            this.btn_Connect_Reference_ID.MinimumSize = new System.Drawing.Size(150, 34);
            this.btn_Connect_Reference_ID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btn_Connect_Reference_ID.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btn_Connect_Reference_ID.TabIndex = 4;
            this.btn_Connect_Reference_ID.Click += new System.EventHandler(this.btn_Connect_Reference_ID_Click);
            this.lbl_Oracle_SQL_Connection.Name = "lbl_Oracle_SQL_Connection";
            this.lbl_Oracle_SQL_Connection.AutoSize = true;
            this.lbl_Oracle_SQL_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Oracle_SQL_Connection.Margin = new System.Windows.Forms.Padding(0);

            root.AutoSize = true;
            root.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            root.Dock = System.Windows.Forms.DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 7;
            root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            for (int row = 0; row < 7; row++) root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            root.Controls.Add(this.connection, 0, 0);
            root.Controls.Add(tables, 0, 1);
            root.Controls.Add(options, 0, 2);
            root.Controls.Add(queryHint, 0, 3);
            root.Controls.Add(this.tb_querybox, 0, 4);
            root.Controls.Add(this.btn_Connect_Reference_ID, 0, 5);
            root.Controls.Add(this.lbl_Oracle_SQL_Connection, 0, 6);
            root.Margin = new System.Windows.Forms.Padding(0);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MinimumSize = new System.Drawing.Size(760, 0);
            this.Size = new System.Drawing.Size(1040, 560);
            this.Controls.Add(root);
            this.Name = "OracleAndSQLConnection";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private ConnectionPanel connection;
        private System.Windows.Forms.TextBox tb_Source_tableName;
        private System.Windows.Forms.TextBox tb_destination_tableName;
        private System.Windows.Forms.TextBox tb_querybox;
        private System.Windows.Forms.CheckBox cb_query;
        private System.Windows.Forms.Button btn_Connect_Reference_ID;
        private System.Windows.Forms.Button btn_ResetQuery;
        private System.Windows.Forms.Label lbl_Oracle_SQL_Connection;
    }
}
