using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Where a step of a window stands.</summary>
    internal enum StepState
    {
        Locked,
        Current,
        Done,
    }

    /// <summary>
    /// The steps of one window, in order. A step's controls stay disabled until every step before it
    /// is done; changing an earlier step locks the later ones again. Each window says when a step is
    /// done; this class keeps the controls and the <see cref="StepStrip"/> in line with that.
    /// The window still owns everything that is not a step control (Cancel, logs, links).
    /// </summary>
    internal sealed class StepFlow
    {
        internal sealed class Step
        {
            internal Func<string> Title;
            internal Func<string> Action;     // what to do in this step, e.g. "Choose the source workbook"
            internal Func<bool> Done;
            internal Func<bool> Applies;
            internal Control[] Controls;
            internal StepState State;
            internal string Reason;           // why an open step is not done yet, e.g. "Enter the Source password."
            internal bool Shown;
            internal int Number;
        }

        private readonly List<Step> steps = new List<Step>();
        private readonly Dictionary<Control, Step> owner = new Dictionary<Control, Step>();
        private readonly Control host;
        private readonly int ownerThreadId;
        private readonly object schedulingGate = new object();
        private bool busy;
        [ThreadStatic] private static string lastFailure;   // set by Passes while a step is checked
        private bool refreshing;
        private object pendingRefresh;

        internal StepFlow(Control host, StepStrip strip)
        {
            this.host = host;
            ownerThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Strip = strip;
            if (strip != null) strip.Flow = this;
            // A constructor can create the handle while it is still initializing fields or
            // applying a profile. Evaluate the completed state on the next UI event instead.
            host.HandleCreated += (sender, args) => Later();
            host.HandleDestroyed += (sender, args) =>
            {
                // Invalidate this handle's reservation atomically. A delayed poster/callback
                // can never clear a different reservation made for the replacement handle.
                lock (schedulingGate) pendingRefresh = null;
            };
        }

        internal StepStrip Strip { get; }

        internal IList<Step> Steps { get { return steps.AsReadOnly(); } }

        /// <summary>Adds a step that always applies.</summary>
        internal StepFlow Add(string title, string action, Func<bool> done, params Control[] controls)
        {
            return Add(() => title, () => action, done, null, controls);
        }

        /// <summary>Adds a step; <paramref name="applies"/> false hides it and lets the next step through (e.g. a mode it does not belong to).</summary>
        internal StepFlow Add(Func<string> title, Func<string> action, Func<bool> done, Func<bool> applies, params Control[] controls)
        {
            var step = new Step { Title = title, Action = action, Done = done, Applies = applies ?? (() => true), Controls = controls ?? new Control[0] };
            steps.Add(step);
            foreach (Control c in step.Controls)
            {
                if (c == null || owner.ContainsKey(c)) continue;
                owner[c] = step;
                Control watched = c;
                watched.EnabledChanged += (sender, args) => KeepLocked(watched);
                Watch(watched);
            }
            return this;
        }

        private readonly List<System.ComponentModel.BackgroundWorker> workers = new List<System.ComponentModel.BackgroundWorker>();

        /// <summary>
        /// The window's background workers: while one runs every step control stays disabled, and the
        /// steps are checked again when it finishes (after the window's own completion handler).
        /// </summary>
        internal StepFlow Workers(params System.ComponentModel.BackgroundWorker[] list)
        {
            foreach (var worker in list)
            {
                if (worker == null || workers.Contains(worker)) continue;
                workers.Add(worker);
                worker.RunWorkerCompleted += (sender, args) => Later();
            }
            return this;
        }

        private bool AnyWorkerBusy
        {
            get { return workers.Any(w => w.IsBusy); }
        }

        /// <summary>While true (loading, comparing) every step control is disabled.</summary>
        internal bool Busy
        {
            get { return busy || AnyWorkerBusy; }
            set
            {
                busy = value;
                Refresh();
            }
        }

        /// <summary>The first step that is open but not done, or null when every step is done.</summary>
        internal Step Current
        {
            get { return steps.FirstOrDefault(s => s.Shown && s.State == StepState.Current); }
        }

        internal StepState StateOf(int index)
        {
            return steps[index].State;
        }

        internal bool IsOpen(Control control)
        {
            Step step;
            return !owner.TryGetValue(control, out step) || step.State != StepState.Locked;
        }

        /// <summary>Re-checks the steps when the user changes one of these controls.</summary>
        internal void Watch(params Control[] controls)
        {
            foreach (Control c in controls)
            {
                if (c == null) continue;
                c.TextChanged += (sender, args) => Later();
                var check = c as CheckBox;
                if (check != null) check.CheckedChanged += (sender, args) => Later();
                var radio = c as RadioButton;
                if (radio != null) radio.CheckedChanged += (sender, args) => Later();
                var list = c as CheckedListBox;
                if (list != null) list.ItemCheck += (sender, args) => Later();   // raised before the item changes
                var combo = c as ComboBox;
                if (combo != null) combo.SelectedIndexChanged += (sender, args) => Later();
                var tabs = c as TabControl;
                if (tabs != null) tabs.SelectedIndexChanged += (sender, args) => Later();
                var button = c as ButtonBase;
                if (button != null && check == null && radio == null) button.Click += (sender, args) => Later();   // after the window's own handler
                var link = c as LinkLabel;
                if (link != null) link.LinkClicked += (sender, args) => Later();
            }
        }

        /// <summary>Refreshes once the current event has finished (a checked list changes its item after ItemCheck).</summary>
        internal void Later()
        {
            // ItemCheck and profile application may notify us before their new values have
            // been committed. Never evaluate them inline, including before the first Show.
            // HandleCreated will schedule the deferred work once there is a UI dispatcher.
            object reservation;
            lock (schedulingGate)
            {
                if (host.IsDisposed || host.Disposing || !host.IsHandleCreated || pendingRefresh != null) return;
                reservation = new object();
                pendingRefresh = reservation;
            }
            try
            {
                // Do not hold the scheduling lock while calling WinForms. The UI thread may
                // be destroying/recreating the handle while a worker posts this request.
                host.BeginInvoke((MethodInvoker)(() =>
                {
                    lock (schedulingGate)
                    {
                        if (!ReferenceEquals(pendingRefresh, reservation)) return;
                        pendingRefresh = null;
                    }
                    if (!host.IsDisposed && !host.Disposing) Refresh();
                }));
            }
            catch (InvalidOperationException)
            {
                // The handle can disappear during BeginInvoke and already be replaced by
                // the time this catch runs. Release only this request, never the new one.
                bool invalidated;
                lock (schedulingGate)
                {
                    invalidated = !ReferenceEquals(pendingRefresh, reservation);
                    if (!invalidated) pendingRefresh = null;
                }
                // An unchanged live handle does not explain the posting error. Keep that
                // unexpected failure visible; callback/Refresh errors are never caught here.
                if (!invalidated && !host.IsDisposed && !host.Disposing && host.IsHandleCreated) throw;
            }
        }

        internal void Refresh()
        {
            if (host.IsDisposed || host.Disposing) return;
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ownerThreadId)
            {
                Later();
                return;
            }
            if (refreshing)
            {
                Later();
                return;
            }
            refreshing = true;
            try
            {
                bool open = true;
                bool blocked = Busy;
                int number = 0;
                foreach (Step step in steps)
                {
                    step.Shown = Safe(step.Applies, true);
                    if (!step.Shown)
                    {
                        step.State = open ? StepState.Done : StepState.Locked;
                        foreach (Control c in step.Controls) Set(c, open && !blocked);
                        continue;
                    }
                    step.Number = ++number;
                    lastFailure = null;
                    bool done = open && Safe(step.Done, false);
                    step.Reason = open && !done ? lastFailure : null;
                    step.State = !open ? StepState.Locked : done ? StepState.Done : StepState.Current;
                    foreach (Control c in step.Controls) Set(c, open && !blocked);
                    open = done;
                }
            }
            finally
            {
                refreshing = false;
            }
            if (Strip != null && !Strip.IsDisposed && !Strip.Disposing) Strip.Relayout();
        }

        /// <summary>The text under the strip: the next thing to do.</summary>
        internal string NextText()
        {
            Step current = Current;
            int shown = steps.Count(s => s.Shown);
            if (current == null) return "All steps done. Change any step to run it again.";
            string text = "Step " + current.Number + " of " + shown + ": " + Safe(current.Action, string.Empty) + ".";
            // say what is still missing, e.g. a selected profile that has no saved password
            return string.IsNullOrWhiteSpace(current.Reason) ? text : text + " " + current.Reason.Trim();
        }

        /// <summary>The tooltip of a locked step: what has to be done first.</summary>
        internal string LockedText(Step step)
        {
            Step current = Current;
            string first = current == null ? string.Empty : Safe(current.Action, string.Empty);
            string reason = current == null || string.IsNullOrWhiteSpace(current.Reason) ? string.Empty : " (" + current.Reason.Trim().TrimEnd('.') + ")";
            return "Locked. " + first + " first" + reason + ".";
        }

        /// <summary>Puts the keyboard focus on the first usable control of an open step.</summary>
        internal void Focus(Step step)
        {
            if (step == null || step.State == StepState.Locked) return;
            Control target = step.Controls.FirstOrDefault(c => c != null && c.Visible && c.Enabled && c.CanSelect);
            if (target != null) target.Focus();
        }

        /// <summary>The ticked item names of a checked list.</summary>
        internal static List<string> CheckedNames(CheckedListBox list)
        {
            return list.CheckedItems.Cast<object>().Select(item => item.ToString()).ToList();
        }

        /// <summary>
        /// After a reload: ticks again what was ticked before, where the same name still exists
        /// (an earlier step changed, the later choices are kept where they still fit). Returns how many.
        /// </summary>
        internal static int Restore(CheckedListBox list, IEnumerable<string> names)
        {
            int restored = 0;
            foreach (string name in names)
            {
                int index = list.FindStringExact(name);
                if (index < 0) continue;
                list.SetItemChecked(index, true);
                restored++;
            }
            return restored;
        }

        internal static bool Passes(Action check)
        {
            try
            {
                check();
                return true;
            }
            catch (Exception ex)
            {
                lastFailure = ex.Message;   // shown under the strip while this step is the current one
                return false;
            }
        }

        private void Set(Control c, bool enabled)
        {
            if (c == null || c.IsDisposed || c.Disposing) return;
            if (c.Enabled != enabled) c.Enabled = enabled;
        }

        /// <summary>Window code that enables a control of a locked step (or while busy) is undone at once.</summary>
        private void KeepLocked(Control c)
        {
            if (refreshing || host.IsDisposed || host.Disposing || c.IsDisposed || c.Disposing || !c.Enabled) return;
            Step step;
            if (!owner.TryGetValue(c, out step)) return;
            if (Busy || step.State == StepState.Locked)
            {
                refreshing = true;
                try { c.Enabled = false; }
                finally { refreshing = false; }
            }
        }

        private static T Safe<T>(Func<T> read, T fallback)
        {
            try
            {
                return read == null ? fallback : read();
            }
            catch (Exception)
            {
                return fallback;
            }
        }
    }

    /// <summary>
    /// The row of steps at the top of a window: done steps get a tick, the current step its number in
    /// the accent colour, locked steps a padlock. Under the row: what to do next. Hovering a locked step
    /// says what to do first; clicking an open step moves the keyboard focus to it.
    /// </summary>
    internal sealed class StepStrip : Control
    {
        private const int Pad = 8;
        private const int RowHeight = 28;
        private const int Circle = 22;
        private const int Gap = 18;
        private readonly ToolTip tip = new ToolTip();
        private readonly List<KeyValuePair<Rectangle, StepFlow.Step>> chips = new List<KeyValuePair<Rectangle, StepFlow.Step>>();
        private StepFlow.Step hovered;
        private int laidOutWidth = -1;

        public StepStrip()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Height = Pad * 2 + RowHeight + 22;
            AccessibleRole = AccessibleRole.StatusBar;
            UiTheme.Changed += ThemeChanged;
            Disposed += (sender, args) => { UiTheme.Changed -= ThemeChanged; tip.Dispose(); };
        }

        internal StepFlow Flow { get; set; }

        /// <summary>
        /// For a window without a generated layout: puts its controls into a panel that fills the window
        /// (positions and anchors unchanged) and adds the strip above it. Docked children stay where they are.
        /// </summary>
        internal static StepStrip Attach(Form form)
        {
            var strip = new StepStrip { Name = "step_Strip", Dock = DockStyle.Top };
            var body = new Panel { Name = "pnl_StepBody", Dock = DockStyle.Fill, AutoScroll = form.AutoScroll };
            form.SuspendLayout();
            body.SuspendLayout();
            Size client = form.ClientSize;
            List<Control> moved = form.Controls.Cast<Control>().Where(c => c.Dock == DockStyle.None).ToList();
            body.Size = client;
            foreach (Control c in moved) body.Controls.Add(c);   // same order, so the same z-order
            form.AutoScroll = false;
            form.Controls.Add(strip);
            form.Controls.Add(body);
            body.BringToFront();                                 // the filling panel is laid out last
            form.ClientSize = new Size(client.Width, client.Height + strip.Height);
            body.ResumeLayout(false);
            form.ResumeLayout(true);
            return strip;
        }

        private void ThemeChanged(object sender, EventArgs e)
        {
            if (!IsDisposed) Invalidate();
        }

        /// <summary>Re-measures the chips (they wrap onto more rows in a narrow window) and repaints.</summary>
        internal void Relayout()
        {
            chips.Clear();
            int rows = 1;
            if (Flow != null)
            {
                int x = Pad, y = Pad;
                foreach (StepFlow.Step step in Flow.Steps)
                {
                    if (!step.Shown) continue;
                    using (Font bold = new Font(Font, FontStyle.Bold))
                    {
                        Size text = TextRenderer.MeasureText(Label(step), bold, Size.Empty, TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);
                        int w = Circle + 6 + text.Width;
                        if (x > Pad && x + w > Width - Pad)
                        {
                            x = Pad;
                            y += RowHeight;
                            rows++;
                        }
                        chips.Add(new KeyValuePair<Rectangle, StepFlow.Step>(new Rectangle(x, y, w, RowHeight), step));
                        x += w + Gap;
                    }
                }
                AccessibleDescription = Flow.NextText();
            }
            laidOutWidth = Width;
            int height = Pad * 2 + rows * RowHeight + 22;
            if (Height != height) Height = height;
            Invalidate();
        }

        private static string Label(StepFlow.Step step)
        {
            string title;
            try { title = step.Title(); }
            catch (Exception) { title = string.Empty; }
            return title;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width != laidOutWidth && Flow != null) Relayout();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (Flow != null) Relayout();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            StepFlow.Step over = At(e.Location);
            Cursor = over != null && over.State != StepState.Locked ? Cursors.Hand : Cursors.Default;
            if (over == hovered) return;
            hovered = over;
            tip.SetToolTip(this, over == null ? string.Empty
                : over.State == StepState.Locked ? Flow.LockedText(over)
                : over.State == StepState.Done ? "Done. Changing it locks the steps after it again."
                : "Current step.");
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hovered = null;
            Cursor = Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (Flow != null) Flow.Focus(At(e.Location));
        }

        private StepFlow.Step At(Point p)
        {
            foreach (var chip in chips)
                if (chip.Key.Contains(p)) return chip.Value;
            return null;
        }

        /// <summary>Fill, mark and title colours of one step in the given theme.</summary>
        internal static void ColorsOf(StepState state, UiPalette p, out Color fill, out Color mark, out Color title)
        {
            switch (state)
            {
                case StepState.Done:
                    fill = p.Success; mark = p.CardBack; title = p.TextColor;
                    break;
                case StepState.Current:
                    fill = p.Accent; mark = p.AccentText; title = p.TextColor;
                    break;
                default:
                    fill = p.DisabledBack; mark = p.DisabledText; title = p.MutedText;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UiPalette p = UiTheme.Current;
            Graphics g = e.Graphics;
            using (var back = new SolidBrush(p.CardBack)) g.FillRectangle(back, ClientRectangle);
            using (var line = new Pen(p.Border)) g.DrawLine(line, 0, Height - 1, Width, Height - 1);
            if (Flow == null) return;
            if (laidOutWidth != Width) Relayout();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Font bold = new Font(Font, FontStyle.Bold))
            {
                foreach (var chip in chips)
                {
                    StepFlow.Step step = chip.Value;
                    Rectangle r = chip.Key;
                    Color fill, mark, title;
                    ColorsOf(step.State, p, out fill, out mark, out title);
                    var dot = new Rectangle(r.X, r.Y + (RowHeight - Circle) / 2, Circle, Circle);
                    using (var brush = new SolidBrush(fill)) g.FillEllipse(brush, dot);
                    if (step.State == StepState.Locked)
                        using (var edge = new Pen(p.Border)) g.DrawEllipse(edge, dot);
                    if (step.State == StepState.Current)
                        TextRenderer.DrawText(g, step.Number.ToString(), bold, dot, mark,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    else
                        Glyphs.Draw(g, step.State == StepState.Done ? Glyph.Check : Glyph.Lock, mark, Rectangle.Inflate(dot, -4, -4));
                    var text = new Rectangle(dot.Right + 6, r.Y, r.Width - Circle - 6, RowHeight);
                    TextRenderer.DrawText(g, Label(step), step.State == StepState.Current ? bold : Font, text, title, p.CardBack,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);
                }
            }
            int top = chips.Count == 0 ? Pad : chips.Max(c => c.Key.Bottom);
            TextRenderer.DrawText(g, Flow.NextText(), Font, new Rectangle(Pad, top, Math.Max(10, Width - 2 * Pad), 20), p.TextColor, p.CardBack,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }
    }
}
