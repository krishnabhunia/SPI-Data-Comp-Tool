using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>One active window per workflow, without discarding its in-progress state.</summary>
    internal sealed class WindowNavigator
    {
        private readonly Dictionary<string, Form> windows = new Dictionary<string, Form>();

        public T Open<T>(IWin32Window owner, Func<T> create, string mode = "", bool newWindow = false) where T : Form
        {
            string key = typeof(T).FullName + ":" + mode;
            Form existing;
            if (!newWindow && windows.TryGetValue(key, out existing) && !existing.IsDisposed)
            {
                if (existing.WindowState == FormWindowState.Minimized)
                    existing.WindowState = FormWindowState.Normal;
                existing.BringToFront();
                existing.Activate();
                return (T)existing;
            }

            T window = create();
            if (!newWindow)
            {
                windows[key] = window;
                window.FormClosed += (sender, args) =>
                {
                    Form current;
                    if (windows.TryGetValue(key, out current) && ReferenceEquals(current, window))
                        windows.Remove(key);
                };
            }
            try
            {
                window.Show(owner);
                return window;
            }
            catch
            {
                if (!newWindow) windows.Remove(key);
                window.Dispose();
                throw;
            }
        }
    }
}
