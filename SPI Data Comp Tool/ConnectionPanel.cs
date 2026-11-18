using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// One Connection panel for every window that talks to a Source and / or Target database:
    /// the saved-profile picker, the Source and Target halves and the Copy buttons. A window drops
    /// it in, sets <see cref="Mode"/>, and keeps its own extras (table pickers, Load buttons)
    /// next to it. Replaces the block that was copied into each of those windows.
    /// </summary>
    public class ConnectionPanel : UserControl
    {
        private readonly TableLayoutPanel root;
        private readonly FlowLayoutPanel copyButtons;
        private readonly FlowLayoutPanel profileRow;
        private ConnectionMode mode = ConnectionMode.SourceAndTarget;

        public ComboBox ProfileList { get; }
        /// <summary>Space to the right of the profile picker for a window's own options.</summary>
        public FlowLayoutPanel ProfileExtras { get; }
        public ConnectionSide Source { get; }
        public ConnectionSide Target { get; }
        public Button CopyToTarget { get; }
        public Button CopyToSource { get; }

        /// <summary>Raised after a saved profile has been put into the fields.</summary>
        public event EventHandler<DBConnectionDetails> ProfileApplied;
        /// <summary>Raised when something in the panel fails, so the window can show it its own way.</summary>
        public event EventHandler<Exception> Failed;

        public ConnectionPanel()
        {
            BackColor = Color.Transparent;
            Margin = new Padding(0);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            ProfileList = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Margin = new Padding(0, 3, 12, 3) };
            ProfileExtras = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false, BackColor = Color.Transparent, Margin = new Padding(0) };
            profileRow = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 6) };
            profileRow.Controls.Add(new Label { Text = "User profile", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 12, 3) });
            profileRow.Controls.Add(ProfileList);
            profileRow.Controls.Add(ProfileExtras);

            Source = new ConnectionSide("Source");
            Target = new ConnectionSide("Target");
            CopyToTarget = new Button { Text = "Copy →", Size = new Size(84, 30), Margin = new Padding(0, 0, 0, 8) };
            CopyToSource = new Button { Text = "← Copy", Size = new Size(84, 30), Margin = new Padding(0) };
            copyButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent,
                Margin = new Padding(8, 0, 8, 0),
            };
            copyButtons.Controls.Add(CopyToTarget);
            copyButtons.Controls.Add(CopyToSource);

            root = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 3, RowCount = 2, BackColor = Color.Transparent, Margin = new Padding(0) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(profileRow, 0, 0);
            root.SetColumnSpan(profileRow, 3);
            root.Controls.Add(Source.Group, 0, 1);
            root.Controls.Add(copyButtons, 1, 1);
            root.Controls.Add(Target.Group, 2, 1);
            Controls.Add(root);

            CopyToTarget.Click += (s, e) => Run(() => Target.CopyFrom(Source));
            CopyToSource.Click += (s, e) => Run(() => Source.CopyFrom(Target));
            ProfileList.SelectedIndexChanged += (s, e) => Run(ApplySelectedProfile);

            Source.ApplyPlatform();
            Target.ApplyPlatform();
        }

        /// <summary>Source and Target, or Source only (the Target half and the Copy buttons hide).</summary>
        public ConnectionMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                bool both = value == ConnectionMode.SourceAndTarget;
                Target.Group.Visible = both;
                copyButtons.Visible = both;
                root.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, both ? 50F : 100F);
                root.ColumnStyles[2] = both ? new ColumnStyle(SizeType.Percent, 50F) : new ColumnStyle(SizeType.Absolute, 0F);
            }
        }

        /// <summary>In Source-only mode: fill the one half from the profile's Source or its Target.</summary>
        public ProfileHalf ProfileHalf { get; set; } = ProfileHalf.Source;

        /// <summary>Shows or hides the profile picker row.</summary>
        public bool ShowProfile
        {
            get { return profileRow.Visible; }
            set { profileRow.Visible = value; }
        }

        /// <summary>Only SQL Server can be chosen (windows whose code talks to SQL Server only).</summary>
        public bool SqlServerOnly
        {
            get { return Source.SqlServerOnly; }
            set { Source.SqlServerOnly = value; Target.SqlServerOnly = value; }
        }

        /// <summary>
        /// Lets the user type a new profile name into the picker (the User profiles window saves
        /// under that name). Off by default: other windows only pick a saved profile.
        /// </summary>
        public bool AllowNewProfileName
        {
            get { return ProfileList.DropDownStyle == ComboBoxStyle.DropDown; }
            set { ProfileList.DropDownStyle = value ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList; }
        }

        /// <summary>Puts the selected saved profile into both halves.</summary>
        public void ApplySelectedProfile()
        {
            DBConnectionDetails d = new DBConnectionDetails(ProfileList.SelectedIndex);
            if (mode == ConnectionMode.SourceOnly && ProfileHalf == ProfileHalf.Target)
            {
                Source.Apply(d.bool_Rb_TRG_Oracle, d.bool_Rb_TRG_SQL, d.bool_TRG_SQL_WA, d.bool_TRG_Oracle_DefaultPort,
                             d.int_TRG_Oracle_DefaultPort, d.str_TRG_DataSource, d.str_TRG_DBName, d.str_TRG_UserName, d.str_TRG_Password);
                ProfileApplied?.Invoke(this, d);
                return;
            }
            Source.Apply(d.bool_Rb_SRC_Oracle, d.bool_Rb_SRC_SQL, d.bool_SRC_SQL_WA, d.bool_SRC_Oracle_DefaultPort,
                         d.int_SRC_Oracle_DefaultPort, d.str_SRC_DataSource, d.str_SRC_DBName, d.str_SRC_UserName, d.str_SRC_Password);
            if (mode == ConnectionMode.SourceAndTarget)
            {
                Target.Apply(d.bool_Rb_TRG_Oracle, d.bool_Rb_TRG_SQL, d.bool_TRG_SQL_WA, d.bool_TRG_Oracle_DefaultPort,
                             d.int_TRG_Oracle_DefaultPort, d.str_TRG_DataSource, d.str_TRG_DBName, d.str_TRG_UserName, d.str_TRG_Password);
            }
            ProfileApplied?.Invoke(this, d);
        }

        private void Run(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (Failed != null)
                {
                    Failed(this, ex);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
