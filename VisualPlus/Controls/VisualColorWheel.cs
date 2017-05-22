namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    internal interface IColor
    {
        #region Properties

        /// <summary>Gets or sets the component color.</summary>
        /// <value>The component color.</value>
        Color Color { get; set; }

        #endregion

        #region Events

        /// <summary>Occurs when the <see cref="Color" /> property is changed.</summary>
        event EventHandler ColorChanged;

        #endregion
    }

    [Serializable]
    public struct HslColorManager
    {
        public static readonly HslColorManager Empty;

        private int alpha;
        private double hue;
        private bool isEmpty;
        private double lightness;
        private double saturation;

        static HslColorManager()
        {
            Empty = new HslColorManager
                {
                    IsEmpty = true
                };
        }

        public HslColorManager(double hue, double saturation, double lightness)
            : this(255, hue, saturation, lightness)
        {
        }

        public HslColorManager(int alpha, double hue, double saturation, double lightness)
        {
            this.hue = Math.Min(359, hue);
            this.saturation = Math.Min(1, saturation);
            this.lightness = Math.Min(1, lightness);
            this.alpha = alpha;
            isEmpty = false;
        }

        public HslColorManager(Color color)
        {
            alpha = color.A;
            hue = color.GetHue();
            saturation = color.GetSaturation();
            lightness = color.GetBrightness();
            isEmpty = false;
        }

        public static bool operator ==(HslColorManager a, HslColorManager b)
        {
            return a.H == b.H && a.L == b.L && a.S == b.S && a.A == b.A;
        }

        public static implicit operator HslColorManager(Color color)
        {
            return new HslColorManager(color);
        }

        public static implicit operator Color(HslColorManager colorManager)
        {
            return colorManager.ToRgbColor();
        }

        public static bool operator !=(HslColorManager a, HslColorManager b)
        {
            return !(a == b);
        }

        public int A
        {
            get
            {
                return alpha;
            }

            set
            {
                alpha = Math.Min(0, Math.Max(255, value));
            }
        }

        public double H
        {
            get
            {
                return hue;
            }

            set
            {
                hue = value;

                if (hue > 359)
                {
                    hue = 0;
                }

                if (hue < 0)
                {
                    hue = 359;
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }

            internal set
            {
                isEmpty = value;
            }
        }

        public double L
        {
            get
            {
                return lightness;
            }

            set
            {
                lightness = Math.Min(1, Math.Max(0, value));
            }
        }

        public double S
        {
            get
            {
                return saturation;
            }

            set
            {
                saturation = Math.Min(1, Math.Max(0, value));
            }
        }

        public override bool Equals(object obj)
        {
            bool result;

            if (obj is HslColorManager)
            {
                HslColorManager colorManager = (HslColorManager)obj;
                result = this == colorManager;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Color ToRgbColor()
        {
            return ToRgbColor(A);
        }

        public Color ToRgbColor(int alphaRGB)
        {
            double q;
            if (L < 0.5)
            {
                q = L * (1 + S);
            }
            else
            {
                q = L + S - L * S;
            }

            double p = 2 * L - q;
            double hk = H / 360;

            // R, G, B colors
            double[] tc =
                {
                    hk + 1d / 3d,
                    hk,
                    hk - 1d / 3d
                };
            double[] colors =
                {
                    0.0,
                    0.0,
                    0.0
                };

            for (var color = 0; color < colors.Length; color++)
            {
                if (tc[color] < 0)
                {
                    tc[color] += 1;
                }

                if (tc[color] > 1)
                {
                    tc[color] -= 1;
                }

                if (tc[color] < 1d / 6d)
                {
                    colors[color] = p + (q - p) * 6 * tc[color];
                }
                else if (tc[color] >= 1d / 6d && tc[color] < 1d / 2d)
                {
                    colors[color] = q;
                }
                else if (tc[color] >= 1d / 2d && tc[color] < 2d / 3d)
                {
                    colors[color] = p + (q - p) * 6 * (2d / 3d - tc[color]);
                }
                else
                {
                    colors[color] = p;
                }

                colors[color] *= 255;
            }

            return Color.FromArgb(alphaRGB, (int)colors[0], (int)colors[1], (int)colors[2]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().
                Name);
            builder.Append(" [");
            builder.Append("H=");
            builder.Append(H);
            builder.Append(", S=");
            builder.Append(S);
            builder.Append(", L=");
            builder.Append(L);
            builder.Append("]");

            return builder.ToString();
        }
    }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("ColorChanged")]
    [DefaultProperty("Color")]
    [Description("The Visual Color Wheel")]
    public sealed class VisualColorWheel : Control, IColor
    {
        #region Variables

        [Category(Localize.Category.Appearance)]
        [DefaultValue(typeof(Color), "Black")]
        [Description(Localize.Description.ComponentColor)]
        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                if (Color != value)
                {
                    color = value;

                    OnColorChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Property Changed")]
        public event EventHandler ColorChanged
        {
            add
            {
                Events.AddHandler(EventColorChanged, value);
            }

            remove
            {
                Events.RemoveHandler(EventColorChanged, value);
            }
        }

        #endregion

        #region Variables

        private static readonly object EventColorChanged = new object();
        private static readonly object EventColorStepChanged = new object();
        private static readonly object EventHslColorChanged = new object();
        private static readonly object EventLargeChangeChanged = new object();
        private static readonly object EventSelectionSizeChanged = new object();
        private static readonly object EventSmallChangeChanged = new object();

        private readonly Color buttonColor = Settings.DefaultValue.Style.ButtonNormalColor;

        private Border border = new Border();

        // private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        // private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        // private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        // private int borderSize = Settings.DefaultValue.BorderThickness;
        // private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Brush brush;
        private PointF centerPoint;
        private Color color;
        private HslColorManager colorManagement;
        private int colorStep;
        private ControlState controlState = ControlState.Normal;
        private bool dragWheel;
        private bool drawFocusRectangle;
        private int largeChange;
        private Border pickerBorder = new Border();
        private float radius;
        private int selectionSize;
        private int smallChange;
        private int updates;

        #endregion

        #region Constructors

        public VisualColorWheel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick |
                ControlStyles.SupportsTransparentBackColor, true);

            Color = Color.Black;
            ColorStep = 4;
            SelectionSize = 10;
            SmallChange = 1;
            LargeChange = 5;

            // SelectionGlyph = CreateSelectionGlyph();
            MinimumSize = new Size(130, 130);
            Size = new Size(130, 130);

            pickerBorder.HoverVisible = false;
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
                Invalidate();
            }
        }

        /// <summary>The color step.</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Value must be between 1 and 359</exception>
        [Category(Localize.Category.Appearance)]
        [DefaultValue(4)]
        [Description("Gets or sets the increment for rendering the color wheel.")]
        public int ColorStep
        {
            get
            {
                return colorStep;
            }

            set
            {
                if (value < 1 || value > 359)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, @"Value must be between 1 and 359");
                }

                if (ColorStep != value)
                {
                    colorStep = value;

                    OnColorStepChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool DrawFocusRectangle
        {
            get
            {
                return drawFocusRectangle;
            }

            set
            {
                drawFocusRectangle = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [DefaultValue(typeof(HslColorManager), "0, 0, 0")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HslColorManager HslColorManager
        {
            get
            {
                return colorManagement;
            }

            set
            {
                if (HslColorManager != value)
                {
                    colorManagement = value;

                    OnHslColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value to be added to or subtracted from the <see cref="Color" /> property when the wheel
        ///     selection is moved a large distance.
        /// </summary>
        /// <value>A numeric value. The default value is 5.</value>
        [Category(Localize.Category.Behavior)]
        [DefaultValue(5)]
        public int LargeChange
        {
            get
            {
                return largeChange;
            }

            set
            {
                if (LargeChange != value)
                {
                    largeChange = value;

                    OnLargeChangeChanged(EventArgs.Empty);
                }
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Picker
        {
            get
            {
                return pickerBorder;
            }

            set
            {
                pickerBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [DefaultValue(10)]
        [Description("Gets or sets the size of the selection handle.")]
        public int SelectionSize
        {
            get
            {
                return selectionSize;
            }

            set
            {
                if (SelectionSize != value)
                {
                    selectionSize = value;

                    OnSelectionSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value to be added to or subtracted from the <see cref="Color" /> property when the wheel
        ///     selection is moved a small distance.
        /// </summary>
        /// <value>A numeric value. The default value is 1.</value>
        [Category(Localize.Category.Behavior)]
        [DefaultValue(1)]
        public int SmallChange
        {
            get
            {
                return smallChange;
            }

            set
            {
                if (SmallChange != value)
                {
                    smallChange = value;

                    OnSmallChangeChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        private bool AllowPainting
        {
            get
            {
                return updates == 0;
            }
        }

        private Color[] Colors { get; set; }

        private bool LockUpdates { get; set; }

        private PointF[] Points { get; set; }

        private Image SelectionGlyph { get; set; }

        #endregion

        #region Events

        /// <summary>Disables any redrawing of the image box</summary>
        public void BeginUpdate()
        {
            updates++;
        }

        [Category("Property Changed")]
        public event EventHandler ColorStepChanged
        {
            add
            {
                Events.AddHandler(EventColorStepChanged, value);
            }

            remove
            {
                Events.RemoveHandler(EventColorStepChanged, value);
            }
        }

        /// <summary>Enables the redrawing of the image box</summary>
        public void EndUpdate()
        {
            if (updates > 0)
            {
                updates--;
            }

            if (AllowPainting)
            {
                Invalidate();
            }
        }

        [Category("Property Changed")]
        public event EventHandler HslColorChanged
        {
            add
            {
                Events.AddHandler(EventHslColorChanged, value);
            }

            remove
            {
                Events.RemoveHandler(EventHslColorChanged, value);
            }
        }

        [Category("Property Changed")]
        public event EventHandler LargeChangeChanged
        {
            add
            {
                Events.AddHandler(EventLargeChangeChanged, value);
            }

            remove
            {
                Events.RemoveHandler(EventLargeChangeChanged, value);
            }
        }

        [Category("Property Changed")]
        public event EventHandler SelectionSizeChanged
        {
            add
            {
                Events.AddHandler(EventSelectionSizeChanged, value);
            }

            remove
            {
                Events.RemoveHandler(EventSelectionSizeChanged, value);
            }
        }

        [Category("Property Changed")]
        public event EventHandler SmallChangeChanged
        {
            add
            {
                Events.AddHandler(EventSmallChangeChanged, value);
            }

            remove
            {
                Events.RemoveHandler(EventSmallChangeChanged, value);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (brush != null)
                {
                    brush.Dispose();
                }

                if (SelectionGlyph != null)
                {
                    SelectionGlyph.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            bool result;

            if ((keyData & Keys.Left) == Keys.Left || (keyData & Keys.Up) == Keys.Up || (keyData & Keys.Down) == Keys.Down ||
                (keyData & Keys.Right) == Keys.Right || (keyData & Keys.PageUp) == Keys.PageUp || (keyData & Keys.PageDown) == Keys.PageDown)
            {
                result = true;
            }
            else
            {
                result = base.IsInputKey(keyData);
            }

            return result;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            HslColorManager colorManager = HslColorManager;
            double hue = colorManager.H;

            int step = e.Shift ? LargeChange : SmallChange;

            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Up:
                    {
                        hue += step;
                        break;
                    }

                case Keys.Left:
                case Keys.Down:
                    {
                        hue -= step;
                        break;
                    }

                case Keys.PageUp:
                    {
                        hue += LargeChange;
                        break;
                    }

                case Keys.PageDown:
                    {
                        hue -= LargeChange;
                        break;
                    }
            }

            if (hue >= 360)
            {
                hue = 0;
            }

            if (hue < 0)
            {
                hue = 359;
            }

            if (hue != colorManager.H)
            {
                colorManager.H = hue;

                // As the Color and HslColorManager properties update each other, need to temporarily disable this and manually set both
                // otherwise the wheel "sticks" due to imprecise conversion from RGB to HSL
                LockUpdates = true;
                Color = colorManager.ToRgbColor();
                HslColorManager = colorManager;
                LockUpdates = false;

                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused && TabStop)
            {
                Focus();
            }

            if (e.Button == MouseButtons.Left && IsPointInWheel(e.Location))
            {
                dragWheel = true;
                SetColor(e.Location);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left && dragWheel)
            {
                SetColor(e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragWheel = false;
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            RefreshWheel();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (AllowPainting)
            {
                OnPaintBackground(e);

                // If the parent is using a transparent colorManager, it's likely to be something like a TabPage in a tab control
                // so we'll draw the parent background instead, to avoid having an ugly solid colorManager
                if (BackgroundImage == null && Parent != null && (BackColor == Parent.BackColor || Parent.BackColor.A != 255))
                {
                    ButtonRenderer.DrawParentBackground(e.Graphics, DisplayRectangle, this);
                }

                if (brush != null)
                {
                    e.Graphics.FillPie(brush, ClientRectangle, 0, 360);
                }

                // Smooth out the edge of the wheel.
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(BackColor, 2))
                {
                    e.Graphics.DrawEllipse(pen, new RectangleF(centerPoint.X - radius, centerPoint.Y - radius, radius * 2, radius * 2));
                }

                Point pointLocation = new Point(Convert.ToInt32(centerPoint.X - radius), Convert.ToInt32(centerPoint.Y - radius));
                Size newSize = new Size(Convert.ToInt32(radius * 2), Convert.ToInt32(radius * 2));

                GraphicsPath controlGraphicsPath = new GraphicsPath();
                controlGraphicsPath.AddEllipse(new RectangleF(pointLocation, newSize));

                // Setup border
                if (border.Visible)
                {
                    GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
                }

                // Draws the button
                if (!Color.IsEmpty)
                {
                    DrawColorPicker(e, HslColorManager, drawFocusRectangle);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            RefreshWheel();
        }

        /// <summary>Calculates wheel attributes.</summary>
        private void CalculateWheel()
        {
            var points = new List<PointF>();
            var colors = new List<Color>();

            // Only define the points if the control is above a minimum size, otherwise if it's too small, you get an "out of memory" exceptions when creating the brush.
            if (ClientSize.Width > 16 && ClientSize.Height > 16)
            {
                int w = ClientSize.Width;
                int h = ClientSize.Height;

                centerPoint = new PointF(w / 2.0F, h / 2.0F);
                radius = GetRadius(centerPoint);

                for (double angle = 0; angle < 360; angle += ColorStep)
                {
                    double angleR = angle * (Math.PI / 180);
                    PointF location = GetColorLocation(angleR, radius);

                    points.Add(location);
                    colors.Add(new HslColorManager(angle, 1, 0.5).ToRgbColor());
                }
            }

            Points = points.ToArray();
            Colors = colors.ToArray();
        }

        /// <summary>Creates the gradient brush used to paint the wheel.</summary>
        /// <returns>The <see cref="Brush" />.</returns>
        private Brush CreateGradientBrush()
        {
            Brush result;

            if (Points.Length != 0 && Points.Length == Colors.Length)
            {
                result = new PathGradientBrush(Points, WrapMode.Clamp)
                    {
                        CenterPoint = centerPoint,
                        CenterColor = Color.White,
                        SurroundColors = Colors
                    };
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>Creates the selection glyph.</summary>
        /// <returns>The <see cref="Image" />.</returns>
        private Image CreateSelectionGlyph()
        {
            int halfSize = SelectionSize / 2;
            Image image = new Bitmap(SelectionSize + 1, SelectionSize + 1, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(image))
            {
                var diamondOuter = new[]
                    {
                        new Point(halfSize, 0),
                        new Point(SelectionSize, halfSize),
                        new Point(halfSize, SelectionSize),
                        new Point(0, halfSize)
                    };

                g.FillPolygon(SystemBrushes.Control, diamondOuter);
                g.DrawPolygon(SystemPens.ControlDark, diamondOuter);

                using (Pen pen = new Pen(Color.FromArgb(128, SystemColors.ControlDark)))
                {
                    g.DrawLine(pen, halfSize, 1, SelectionSize - 1, halfSize);
                    g.DrawLine(pen, halfSize, 2, SelectionSize - 2, halfSize);
                    g.DrawLine(pen, halfSize, SelectionSize - 1, SelectionSize - 2, halfSize + 1);
                    g.DrawLine(pen, halfSize, SelectionSize - 2, SelectionSize - 3, halfSize + 1);
                }

                using (Pen pen = new Pen(Color.FromArgb(196, SystemColors.ControlLightLight)))
                {
                    g.DrawLine(pen, halfSize, SelectionSize - 1, 1, halfSize);
                }

                g.DrawLine(SystemPens.ControlLightLight, 1, halfSize, halfSize, 1);
            }

            return image;
        }

        private void DrawColorPicker(PaintEventArgs e, HslColorManager colorManager, bool includeFocus)
        {
            PointF location = GetColorLocation(colorManager);

            if (!float.IsNaN(location.X) && !float.IsNaN(location.Y))
            {
                int x = (int)location.X - SelectionSize / 2;
                int y = (int)location.Y - SelectionSize / 2;

                // Create the button path
                GraphicsPath buttonGraphicsPath = GDI.DrawRoundedRectangle(new Rectangle(x, y, SelectionSize, SelectionSize), 10);

                // Draw button
                e.Graphics.FillPath(new SolidBrush(buttonColor), buttonGraphicsPath);

                // Draw border
                if (pickerBorder.Visible)
                {
                    GDI.DrawBorderType(e.Graphics, controlState, buttonGraphicsPath, pickerBorder.Thickness, pickerBorder.Color, pickerBorder.HoverColor, pickerBorder.HoverVisible);
                }

                if (Focused && includeFocus)
                {
                    ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(x - 1, y - 1, SelectionSize + 2, SelectionSize + 2));
                }
            }
        }

        /// <summary>Gets the point within the wheel representing the source colorManager.</summary>
        /// <param name="colorManager">The colorManager.</param>
        private PointF GetColorLocation(HslColorManager colorManager)
        {
            double angle = colorManager.H * Math.PI / 180;
            double locationRadius = radius * colorManager.S;

            return GetColorLocation(angle, locationRadius);
        }

        private PointF GetColorLocation(double angleR, double locationRadius)
        {
            double x = Padding.Left + centerPoint.X + Math.Cos(angleR) * locationRadius;
            double y = Padding.Top + centerPoint.Y - Math.Sin(angleR) * locationRadius;

            return new PointF((float)x, (float)y);
        }

        private float GetRadius(PointF centerRadiusPoint)
        {
            return Math.Min(centerRadiusPoint.X, centerRadiusPoint.Y) - (Math.Max(Padding.Horizontal, Padding.Vertical) + SelectionSize / 2);
        }

        /// <summary>Determines whether the specified point is within the bounds of the colorManager wheel.</summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the specified point is within the bounds of the colorManager wheel; otherwise, <c>false</c>.</returns>
        private bool IsPointInWheel(Point point)
        {
            PointF normalized = new PointF(point.X - centerPoint.X, point.Y - centerPoint.Y);
            return normalized.X * normalized.X + normalized.Y * normalized.Y <= radius * radius;
        }

        /// <summary>Raises the <see cref="ColorChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnColorChanged(EventArgs e)
        {
            if (!LockUpdates)
            {
                HslColorManager = new HslColorManager(Color);
            }

            Refresh();
            EventHandler handler = (EventHandler)Events[EventColorChanged];
            handler?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="ColorStepChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnColorStepChanged(EventArgs e)
        {
            RefreshWheel();
            EventHandler handler = (EventHandler)Events[EventColorStepChanged];
            handler?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="HslColorChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnHslColorChanged(EventArgs e)
        {
            if (!LockUpdates)
            {
                Color = HslColorManager.ToRgbColor();
            }

            Invalidate();
            EventHandler handler = (EventHandler)Events[EventHslColorChanged];
            handler?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="LargeChangeChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnLargeChangeChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[EventLargeChangeChanged];
            handler?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="SelectionSizeChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnSelectionSizeChanged(EventArgs e)
        {
            SelectionGlyph?.Dispose();
            SelectionGlyph = CreateSelectionGlyph();
            RefreshWheel();
            EventHandler handler = (EventHandler)Events[EventSelectionSizeChanged];
            handler?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="SmallChangeChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnSmallChangeChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[EventSmallChangeChanged];
            handler?.Invoke(this, e);
        }

        /// <summary>Refreshes the wheel attributes and then repaints the control</summary>
        private void RefreshWheel()
        {
            if (brush != null)
            {
                brush.Dispose();
            }

            CalculateWheel();
            brush = CreateGradientBrush();
            Invalidate();
        }

        private void SetColor(Point point)
        {
            double dx = Math.Abs(point.X - centerPoint.X - Padding.Left);
            double dy = Math.Abs(point.Y - centerPoint.Y - Padding.Top);
            double angle = Math.Atan(dy / dx) / Math.PI * 180;
            double distance = Math.Pow(Math.Pow(dx, 2) + Math.Pow(dy, 2), 0.5);
            double saturation = distance / radius;

            if (distance < 6)
            {
                saturation = 0; // snap to center
            }

            if (point.X < centerPoint.X)
            {
                angle = 180 - angle;
            }

            if (point.Y > centerPoint.Y)
            {
                angle = 360 - angle;
            }

            LockUpdates = true;
            HslColorManager = new HslColorManager(angle, saturation, 0.5);
            Color = HslColorManager.ToRgbColor();
            LockUpdates = false;
        }

        #endregion
    }
}