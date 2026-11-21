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
            internal bool Shown;
            internal int Number;
        }

        private readonly List<Step> steps = new List<Step>();
        private readonly Dictionary<Control, Step> owner = new Dictionary<Control, Step>();
        private readonly Control host;
        private bool busy;
        private bool refreshing;
        private bool queued;

        internal StepFlow(Control host, StepStrip strip)
        {
            this.host = host;
            Strip = strip;
            if (strip != null) strip.Flow = this;
            host.HandleCreated += (sender, args) => Refresh();
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
            if (queued) return;
            if (!host.IsHandleCreated || host.IsDisposed)
            {
                Refresh();
                return;
            }
            queued = true;
            host.BeginInvoke((MethodInvoker)(() =>
            {
                queued = false;
                if (!host.IsDisposed) Refresh();
            }));
        }

        internal void Refresh()
        {
            if (refreshing) return;
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
                    bool done = open && Safe(step.Done, false);
                    step.State = !open ? StepState.Locked : done ? StepState.Done : StepState.Current;
                    foreach (Control c in step.Controls) Set(c, open && !blocked);
                    open = done;
                }
            }
            finally
            {
                refreshing = false;
            }
            if (Strip != null) Strip.Relayout();
        }

        /// <summary>The text under the strip: the next thing to do.</summary>
        internal string NextText()
        {
            Step current = Current;
            int shown = steps.Count(s => s.Shown);
            if (current == null) return "All steps done. Change any step to run it again.";
            return "Step " + current.Number + " of " + shown + ": " + Safe(current.Action, string.Empty) + ".";
        }

        /// <summary>The tooltip of a locked step: what has to be done first.</summary>
        internal string LockedText(Step step)
        {
            Step current = Current;
            string first = current == null ? string.Empty : Safe(current.Action, string.Empty);
            return "Locked. " + first + " first.";
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
            catch (Exception)
            {
                return false;
            }
        }

        private void Set(Control c, bool enabled)
        {
            if (c == null || c.IsDisposed) return;
            if (c.Enabled != enabled) c.Enabled = enabled;
        }

        /// <summary>Window code that enables a control of a locked step (or while busy) is undone at once.</summary>
        private void KeepLocked(Control c)
        {
            if (refreshing || !c.Enabled) return;
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
