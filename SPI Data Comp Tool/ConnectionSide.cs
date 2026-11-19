using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Which halves of the Connection panel a window shows.</summary>
    public enum ConnectionMode
    {
        SourceAndTarget,
        SourceOnly,
    }

    /// <summary>Which half of a saved profile fills a Source-only panel.</summary>
    public enum ProfileHalf
    {
        Source,
        Target,
    }

    /// <summary>
    /// The Source or the Target half of the Connection panel: database type, login options,
    /// port, server, database, user and password, and the behaviour that used to be copied
    /// into every window (switching Oracle / SQL Server, Windows login, default port).
    /// </summary>
    public sealed class ConnectionSide
    {
        public const string OracleDefaultPort = "1521";

        private string title;

        public GroupBox Group { get; }
        public RadioButton Oracle { get; }
        public RadioButton Sql { get; }
        public CheckBox WindowsLogin { get; }
        public CheckBox DefaultPort { get; }
        public TextBox Port { get; }
        public TextBox DataSource { get; }
        public TextBox Database { get; }
        public TextBox User { get; }
        public TextBox Password { get; }

        internal ConnectionSide(string title)
        {
            this.title = title;
            Group = new GroupBox { Text = title, Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 6, 8, 8), Margin = new Padding(0) };
            // The radio text is passed on as the database type ("Oracle" / "SQL"), so it must stay as is.
            Oracle = new RadioButton { Text = "Oracle", AutoSize = true, Checked = true, Margin = new Padding(0, 3, 16, 3) };
            Sql = new RadioButton { Text = "SQL", AutoSize = true, Margin = new Padding(0, 3, 0, 3) };
            WindowsLogin = new CheckBox { Text = "Windows authentication", AutoSize = true, Enabled = false, Margin = new Padding(0, 3, 0, 3) };
            DefaultPort = new CheckBox { Text = "Default port", AutoSize = true, Checked = true, Margin = new Padding(0, 3, 0, 3) };
            Port = new TextBox { Text = OracleDefaultPort, ReadOnly = true, Width = 90, MinimumSize = new Size(90, 0), Anchor = AnchorStyles.Left, Margin = new Padding(0, 3, 16, 3) };
            DataSource = Field();
            Database = Field();
            User = Field();
            Password = Field();
            Password.UseSystemPasswordChar = true;
            NameFields();

            TableLayoutPanel grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            FlowLayoutPanel platform = Flow(Oracle, Sql);
            FlowLayoutPanel port = Flow(Port, DefaultPort);
            AddRow(grid, Caption("Platform"), platform);
            AddRow(grid, Caption("Login"), WindowsLogin);
            AddRow(grid, Caption("Port"), port);
            AddRow(grid, Caption("Data source"), DataSource);
            AddRow(grid, Caption("Database"), Database);
            AddRow(grid, Caption("Username"), User);
            AddRow(grid, Caption("Password"), Password);
            Group.Controls.Add(grid);

            Oracle.CheckedChanged += (s, e) => { if (Oracle.Checked) ApplyPlatform(); };
            Sql.CheckedChanged += (s, e) => { if (Sql.Checked) ApplyPlatform(); };
            WindowsLogin.CheckedChanged += (s, e) => ApplyWindowsLogin();
            DefaultPort.CheckedChanged += (s, e) => ApplyDefaultPort();
            Port.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; };
        }

        /// <summary>The half's name in its frame, e.g. "Source"; the database type is added after it.</summary>
        public string Title
        {
            get { return title; }
            set { title = value; NameFields(); ApplyPlatform(); }
        }

        private bool sqlServerOnly;

        /// <summary>For windows that only work with SQL Server: Oracle can't be chosen.</summary>
        public bool SqlServerOnly
        {
            get { return sqlServerOnly; }
            set
            {
                sqlServerOnly = value;
                Oracle.Enabled = !value;
                if (value)
                {
                    Sql.Checked = true;
                }
            }
        }

        /// <summary>The selected database type as the connection code expects it: "Oracle" or "SQL".</summary>
        public string DatabaseType
        {
            get { return Oracle.Checked ? Oracle.Text : Sql.Text; }
        }

        /// <summary>Checks the entered values before a connection is attempted; never opens a database.</summary>
        internal void ValidateInput(bool requireCredentials = true)
        {
            ValidateInput(title, requireCredentials);
        }

        internal void ValidateInput(string label, bool requireCredentials = true)
        {
            FormValidation.Require(!sqlServerOnly || Sql.Checked, Sql, label + " supports SQL Server only.");
            ValidateFields(label, Oracle, Sql, WindowsLogin, DataSource, Database, User, Password, Port, requireCredentials);
        }

        /// <summary>Also used by the remaining legacy layouts with separate connection controls.</summary>
        internal static void ValidateFields(string label, RadioButton oracle, RadioButton sql, CheckBox windowsLogin,
            Control dataSource, Control database, Control user, Control password, Control port, bool requireCredentials = true)
        {
            FormValidation.Require(oracle.Checked != sql.Checked, oracle, "Select one database platform for " + label + ".");
            FormValidation.Require(!oracle.Checked || !windowsLogin.Checked, windowsLogin,
                label + " Windows authentication is available only for SQL Server.");
            foreach (Control field in windowsLogin.Checked ? new[] { dataSource, database } : new[] { dataSource, database, user })
                FormValidation.Require(!ContainsControlCharacters(field.Text), field,
                    label + " connection fields cannot contain line breaks or control characters.");
            if (oracle.Checked)
            {
                FormValidation.Port(port, label + " Oracle port");
                // This application builds a TCP descriptor, not a free-form TNS descriptor.
                FormValidation.Require(dataSource.Text.IndexOfAny(new[] { '(', ')', '=' }) < 0, dataSource,
                    label + " must contain an Oracle host name or address, not a connection descriptor.");
                FormValidation.Require(database.Text.IndexOfAny(new[] { '(', ')', '=' }) < 0, database,
                    label + " must contain an Oracle service name, not a connection descriptor.");
            }
            if (!requireCredentials) return;
            FormValidation.Required(dataSource, label + " data source");
            FormValidation.Required(database, label + " database or service");
            if (!windowsLogin.Checked)
            {
                FormValidation.Required(user, label + " username");
                // A password's leading/trailing spaces are significant. Do not trim or echo it.
                FormValidation.Require(password.Text.Length > 0, password, "Enter the " + label + " password.");
            }
        }

        internal static bool ContainsControlCharacters(string value)
        {
            if (value == null) return false;
            foreach (char character in value)
                if (char.IsControl(character)) return true;
            return false;
        }

        /// <summary>Oracle: port options on, no Windows login. SQL Server: Windows login possible, no port.</summary>
        public void ApplyPlatform()
        {
            if (Oracle.Checked)
            {
                Group.Text = title + " · Oracle";
                WindowsLogin.Enabled = false;
                WindowsLogin.Checked = false;
                DefaultPort.Enabled = true;
                DefaultPort.Checked = true;
                Port.Enabled = true;
                Port.ReadOnly = true;
            }
            else if (Sql.Checked)
            {
                Group.Text = title + " · SQL Server";
                WindowsLogin.Enabled = true;
                WindowsLogin.Checked = false;
                DefaultPort.Enabled = false;
                DefaultPort.Checked = false;
                Port.Enabled = false;
                Port.ReadOnly = true;
            }
        }

        private void ApplyWindowsLogin()
        {
            User.Enabled = !WindowsLogin.Checked;
            Password.Enabled = !WindowsLogin.Checked;
        }

        private void ApplyDefaultPort()
        {
            if (DefaultPort.Checked)
            {
                Port.Text = OracleDefaultPort;
                Port.Enabled = true;
                Port.ReadOnly = true;
            }
            else
            {
                Port.ResetText();
                Port.Enabled = true;
                Port.ReadOnly = false;
            }
        }

        /// <summary>Takes every setting from the other half (the Copy buttons).</summary>
        public void CopyFrom(ConnectionSide other)
        {
            Oracle.Checked = other.Oracle.Checked;
            Sql.Checked = other.Sql.Checked;
            WindowsLogin.Checked = other.WindowsLogin.Checked;
            DefaultPort.Checked = other.DefaultPort.Checked;
            Port.Text = other.Port.Text;
            DataSource.Text = other.DataSource.Text;
            Database.Text = other.Database.Text;
            User.Text = other.User.Text;
            Password.Text = other.Password.Text;
        }

        /// <summary>Fills this half from a saved profile.</summary>
        internal void Apply(bool oracle, bool sql, bool windowsLogin, bool defaultPort, int port,
                            string dataSource, string database, string user, string password)
        {
            if (sqlServerOnly)
            {
                oracle = false;
                sql = true;
            }
            Oracle.Checked = oracle;
            Sql.Checked = sql;
            WindowsLogin.Checked = windowsLogin;
            DefaultPort.Checked = defaultPort;
            Port.Text = oracle ? (port == 0 ? OracleDefaultPort : port.ToString()) : string.Empty;
            DataSource.Text = dataSource;
            Database.Text = database;
            User.Text = user;
            Password.Text = password;
        }

        /// <summary>
        /// Names the text boxes after their half and caption ("Source data source"), so the
        /// "… is not validated" messages and screen readers say which box is meant.
        /// </summary>
        private void NameFields()
        {
            NameField(Port, "port");
            NameField(DataSource, "data source");
            NameField(Database, "database");
            NameField(User, "username");
            NameField(Password, "password");
        }

        private void NameField(TextBox box, string caption)
        {
            box.Name = title + " " + caption;
            box.AccessibleName = box.Name;
        }

        private static TextBox Field()
        {
            return new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 3, 0, 3) };
        }

        private static Label Caption(string text)
        {
            return new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 3, 16, 3) };
        }

        private static FlowLayoutPanel Flow(params Control[] items)
        {
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
            };
            flow.Controls.AddRange(items);
            return flow;
        }

        private static void AddRow(TableLayoutPanel grid, Control caption, Control field)
        {
            int row = grid.RowCount;
            grid.RowCount = row + 1;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(caption, 0, row);
            grid.Controls.Add(field, 1, row);
        }
    }
}
