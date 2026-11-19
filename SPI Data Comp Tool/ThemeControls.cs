using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>The small line drawings used on the main window.</summary>
    internal enum Glyph
    {
        Database,
        Sheet,
        Delta,
        Export,
    }

    internal static class Glyphs
    {
        /// <summary>Draws a glyph in one colour inside the square area.</summary>
        public static void Draw(Graphics g, Glyph glyph, Color color, Rectangle area)
        {
            GraphicsState saved = g.Save();
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(area.X, area.Y);
                g.ScaleTransform(area.Width / 24f, area.Height / 24f);   // drawn on a 24 x 24 grid
                using (var pen = new Pen(color, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                {
                    switch (glyph)
                    {
                        case Glyph.Database:
                            g.DrawEllipse(pen, 5f, 3f, 14f, 6f);
                            g.DrawLine(pen, 5f, 6f, 5f, 18f);
                            g.DrawLine(pen, 19f, 6f, 19f, 18f);
                            g.DrawArc(pen, 5f, 9f, 14f, 6f, 0f, 180f);
                            g.DrawArc(pen, 5f, 15f, 14f, 6f, 0f, 180f);
                            break;
                        case Glyph.Sheet:
                            g.DrawRectangle(pen, 4f, 3f, 16f, 18f);
                            g.DrawLine(pen, 4f, 9f, 20f, 9f);
                            g.DrawLine(pen, 4f, 15f, 20f, 15f);
                            g.DrawLine(pen, 10f, 3f, 10f, 21f);
                            break;
                        case Glyph.Delta:
                            g.DrawPolygon(pen, new[] { new PointF(12f, 4f), new PointF(21f, 20f), new PointF(3f, 20f) });
                            break;
                        default:
                            g.DrawLine(pen, 12f, 15f, 12f, 4f);
                            g.DrawLines(pen, new[] { new PointF(8f, 8f), new PointF(12f, 4f), new PointF(16f, 8f) });
                            g.DrawLines(pen, new[] { new PointF(4f, 14f), new PointF(4f, 20f), new PointF(20f, 20f), new PointF(20f, 14f) });
                            break;
                    }
                }
            }
            finally
            {
                g.Restore(saved);
            }
        }
    }

    /// <summary>A glyph on its own, drawn in the control's text colour (the header band of the main window).</summary>
    internal sealed class GlyphIcon : Control
    {
        public GlyphIcon()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            BackColor = Color.Transparent;
            TabStop = false;
        }

        public Glyph Glyph { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            int side = Math.Min(Width, Height);
            Glyphs.Draw(e.Graphics, Glyph, ForeColor, new Rectangle((Width - side) / 2, (Height - side) / 2, side, side));
        }
    }

    /// <summary>
    /// A large button on the main window that opens one tool: a colour strip, a glyph, the tool's
    /// name and one line saying what it does. The colours come from the current theme; it is still
    /// a button, so the keyboard, Enter / Space and screen readers work as on any other button.
    /// </summary>
    internal sealed class TileButton : Button
    {
        private bool hot;
        private bool down;

        public TileButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
            MinimumSize = new Size(180, 84);
        }

        /// <summary>Which of the theme's four tile colours this tile uses (0 to 3).</summary>
        public int Tone { get; set; }

        public Glyph Glyph { get; set; }

        private string hint = string.Empty;

        /// <summary>One line under the name; also read out as the button's description.</summary>
        public string Hint
        {
            get { return hint; }
            set
            {
                hint = value ?? string.Empty;
                AccessibleDescription = hint;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e) { hot = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hot = false; down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { down = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { down = false; Invalidate(); base.OnMouseUp(mevent); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            UiPalette p = UiTheme.Current;
            Graphics g = pevent.Graphics;
            Rectangle all = ClientRectangle;
            Color tone = Enabled ? p.Tile(Tone) : p.DisabledText;
            Color back = !Enabled ? p.DisabledBack : down ? p.PressedBack : hot ? p.HoverBack : p.CardBack;
            using (var fill = new SolidBrush(back)) g.FillRectangle(fill, all);
            using (var strip = new SolidBrush(tone)) g.FillRectangle(strip, 0, 0, all.Width, 6);
            using (var border = new Pen(Focused ? tone : p.Border)) g.DrawRectangle(border, 0, 0, all.Width - 1, all.Height - 1);

            Glyphs.Draw(g, Glyph, tone, new Rectangle(14, 18, 28, 28));
            using (var bold = new Font(Font.FontFamily, Font.SizeInPoints + 1.5f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, Text, bold, new Rectangle(52, 18, Math.Max(10, all.Width - 62), 28),
                    Enabled ? p.TextColor : p.DisabledText, back,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            }
            TextRenderer.DrawText(g, hint, Font, new Rectangle(12, 52, Math.Max(10, all.Width - 24), Math.Max(10, all.Height - 58)),
                Enabled ? p.MutedText : p.DisabledText, back,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            if (Focused && ShowFocusCues)
                ControlPaint.DrawFocusRectangle(g, new Rectangle(3, 9, all.Width - 6, all.Height - 12), p.TextColor, back);
        }
    }

    /// <summary>A strip of a theme's main colours, shown next to the theme picker in Settings.</summary>
    internal sealed class ThemeSwatch : Control
    {
        public ThemeSwatch()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UiPalette p = UiTheme.Current;
            Color[] colors = { p.Head, p.Source, p.Target, p.Tile2, p.Tile3, p.Tile4, p.CardBack, p.WindowBack };
            int each = Math.Max(1, Width / colors.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                using (var fill = new SolidBrush(colors[i]))
                    e.Graphics.FillRectangle(fill, i * each, 0, i == colors.Length - 1 ? Width - i * each : each, Height);
            }
            using (var border = new Pen(p.FieldBorder)) e.Graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
        }
    }
}
