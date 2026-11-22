using System;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Retains the existing workflow and its close guards while it is hosted as a workspace page.
    /// Preparation raises guards without closing anything; completion happens only after every page agrees.
    /// </summary>
    public abstract class WorkspacePageForm : Form
    {
        private bool completingWorkspaceClose;
        private bool closedNotificationRaised;

        private WorkspaceForm hostWorkspace;

        internal WorkspaceForm HostWorkspace
        {
            get { return hostWorkspace; }
            set
            {
                hostWorkspace = value;
                OnHostWorkspaceChanged();
            }
        }

        /// <summary>A page that hosts other pages (see <see cref="CompareModePage"/>) passes its workspace on.</summary>
        internal virtual void OnHostWorkspaceChanged() { }

        /// <summary>True while the workspace finishes an accepted close; guards must not run again.</summary>
        protected bool CompletingWorkspaceClose => completingWorkspaceClose;

        internal bool PrepareWorkspaceClose(CloseReason reason)
        {
            if (IsDisposed || closedNotificationRaised) return true;
            var args = new FormClosingEventArgs(reason, false);
            OnFormClosing(args);
            return !args.Cancel;
        }

        internal void CompleteWorkspaceClose()
        {
            if (IsDisposed) return;
            // Application.Exit may have already notified this child before the workspace itself.
            if (closedNotificationRaised) { Dispose(); return; }
            completingWorkspaceClose = true;
            try { Close(); }
            finally { completingWorkspaceClose = false; }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!completingWorkspaceClose) base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (closedNotificationRaised) return;
            closedNotificationRaised = true;
            base.OnFormClosed(e);
        }

        internal bool ProcessWorkspaceDialogKey(Keys keyData)
        {
            // Preserve the active page's AcceptButton/CancelButton. The host never assigns a
            // global Enter action, especially when the Updates page deliberately has none.
            return ProcessDialogKey(keyData);
        }
    }
}
