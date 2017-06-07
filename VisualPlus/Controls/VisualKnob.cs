namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    public delegate void ValueChangedEventHandler(object Sender);

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Control))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual Knob")]
    public sealed class VisualKnob : UserControl
    {
        #region Variables

        private readonly Color[] knobColor =
            {
                Color.LightGray,
                Color.White
            };

        private readonly Color[] knobTopColor =
            {
                Color.White,
                Color.LightGray
            };

        private readonly Color[] scaleColor =
            {
                Color.LightGray,
                Color.White
            };

        private int _value;
        private int buttonDivisions = 30;
        private Container components = null;
        private ControlState controlState = ControlState.Normal;
        private float deltaAngle;
        private bool drawDivInside;
        private float drawRatio;
        private float endAngle = 405;
        private bool focused;
        private Point[] gradientPoints;
        private Gradient knob = new Gradient();
        private Border knobBorder = new Border();
        private int knobDistance = 35;
        private Font knobFont;
        private Point knobPoint;
        private Rectangle knobRectangle;
        private Size knobSize = new Size(90, 90);
        private Size knobTickSize = new Size(86, 86);
        private Gradient knobTop = new Gradient();
        private Border knobTopBorder = new Border();
        private Size knobTopSize = new Size(75, 75);
        private int largeChange = 5;
        private Size lineSize = new Size(1, 1);
        private int maximum = 100;
        private int minimum;
        private int mouseWheelBarPartitions = 10;
        private Graphics offGraphics;
        private Image offScreenImage;
        private Color pointerColor = Settings.DefaultValue.Progress.Progress.Colors[0];
        private PointerStyle pointerStyle = PointerStyle.Circle;
        private bool rotating;
        private Gradient scale = new Gradient();
        private int scaleDivisions = 11;
        private int scaleSubDivisions = 4;
        private bool showLargeScale = true;
        private bool showSmallScale = true;
        private int smallChange = 1;
        private float startAngle = 135;
        private Color tickColor = Color.DimGray;
        private bool valueVisible = true;

        #endregion

        #region Constructors

        public VisualKnob()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            knobFont = Font;
            ForeColor = Color.DimGray;
            knobTopBorder.HoverVisible = false;

            float[] gradientPosition = { 0, 1 };

            knob.Angle = 180;
            knob.Colors = knobColor;
            knob.Positions = gradientPosition;

            knobTop.Colors = knobTopColor;
            knobTop.Positions = gradientPosition;

            scale.Colors = scaleColor;
            scale.Positions = gradientPosition;

            InitializeComponent();

            // "start angle" and "end angle" possible values:

            // 90 = bottom (minimum value for "start angle")
            // 180 = left
            // 270 = top
            // 360 = right
            // 450 = bottom again (maximum value for "end angle")

            // So the couple (90, 450) will give an entire circle and the couple (180, 360) will give half a circle.
            deltaAngle = endAngle - startAngle;
            ConfigureDimensions();
        }

        public event ValueChangedEventHandler ValueChanged;

        public enum PointerStyle
        {
            /// <summary>The circle.</summary>
            Circle,

            /// <summary>The line.</summary>
            Line
        }

        #endregion

        #region Properties

        // [Description("Set the number of intervals between minimum and maximum")]
        // [Category(Localize.Category.Behavior)]
        // public int ButtonDivisions
        // {
        // get
        // {
        // return buttonDivisions;
        // }

        // set
        // {
        // buttonDivisions = value;
        // Invalidate();
        // }
        // }
        [Category(Localize.Category.Behavior)]
        [Description("Draw graduation strings inside or outside the knob circle")]
        [DefaultValue(false)]
        public bool DrawDivInside
        {
            get
            {
                return drawDivInside;
            }

            set
            {
                drawDivInside = value;
                Invalidate();
            }
        }

        [Description("Set the end angle to display graduations (max 450)")]
        [Category(Localize.Category.Behavior)]
        [DefaultValue(405)]
        public float EndAngle
        {
            get
            {
                return endAngle;
            }

            set
            {
                if ((value <= 450) && (value > startAngle))
                {
                    endAngle = value;
                    deltaAngle = endAngle - startAngle;
                    Invalidate();
                }
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Knob
        {
            get
            {
                return knob;
            }

            set
            {
                knob = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border KnobBorder
        {
            get
            {
                return knobBorder;
            }

            set
            {
                knobBorder = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentSize)]
        [Category(Localize.Category.Layout)]
        public int KnobDistance
        {
            get
            {
                return knobDistance;
            }

            set
            {
                knobDistance = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentSize)]
        [Category(Localize.Category.Layout)]
        public Size KnobSize
        {
            get
            {
                return knobSize;
            }

            set
            {
                knobSize = value;
                Invalidate();
            }
        }

        [Description("Set the style of the knob pointer: a circle or a line")]
        [Category(Localize.Category.Appearance)]
        public PointerStyle KnobStyle
        {
            get
            {
                return pointerStyle;
            }

            set
            {
                pointerStyle = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentSize)]
        [Category(Localize.Category.Layout)]
        public Size KnobTickSize
        {
            get
            {
                return knobTickSize;
            }

            set
            {
                knobTickSize = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient KnobTop
        {
            get
            {
                return knobTop;
            }

            set
            {
                knobTop = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border KnobTopBorder
        {
            get
            {
                return knobTopBorder;
            }

            set
            {
                knobTopBorder = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentSize)]
        [Category(Localize.Category.Layout)]
        public Size KnobTopSize
        {
            get
            {
                return knobTopSize;
            }

            set
            {
                knobTopSize = value;
                Invalidate();
            }
        }

        [Description("set the value for the large changes")]
        [Category(Localize.Category.Behavior)]
        public int LargeChange
        {
            get
            {
                return largeChange;
            }

            set
            {
                largeChange = value;
                Invalidate();
            }
        }

        [Description("set the maximum value for the knob control")]
        [Category(Localize.Category.Behavior)]
        public int Maximum
        {
            get
            {
                return maximum;
            }

            set
            {
                if (value > minimum)
                {
                    maximum = value;

                    if ((scaleSubDivisions > 0) && (scaleDivisions > 0) && ((maximum - minimum) / (scaleSubDivisions * scaleDivisions) <= 0))
                    {
                        showSmallScale = false;
                    }

                    ConfigureDimensions();
                    Invalidate();
                }
            }
        }

        [Description("set the minimum value for the knob control")]
        [Category(Localize.Category.Behavior)]
        public int Minimum
        {
            get
            {
                return minimum;
            }

            set
            {
                minimum = value;
                Invalidate();
            }
        }

        [Description("Set to how many parts is bar divided when using mouse wheel")]
        [Category(Localize.Category.Behavior)]
        [DefaultValue(10)]
        public int MouseWheelBarPartitions
        {
            get
            {
                return mouseWheelBarPartitions;
            }

            set
            {
                if (value > 0)
                {
                    mouseWheelBarPartitions = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MouseWheelBarPartitions has to be greather than zero");
                }
            }
        }

        [Description(Localize.Description.ComponentColor)]
        [Category(Localize.Category.Appearance)]
        public Color PointerColor
        {
            get
            {
                return pointerColor;
            }

            set
            {
                pointerColor = value;
                Invalidate();
            }
        }

        [Description("Set the number of intervals between minimum and maximum")]
        [Category(Localize.Category.Behavior)]
        public int ScaleDivisions
        {
            get
            {
                return scaleDivisions;
            }

            set
            {
                scaleDivisions = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient ScaleGradient
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
                Invalidate();
            }
        }

        [Description("Set the number of subdivisions between main divisions of graduation.")]
        [Category(Localize.Category.Behavior)]
        public int ScaleSubDivisions
        {
            get
            {
                return scaleSubDivisions;
            }

            set
            {
                if ((value > 0) && (scaleDivisions > 0) && ((maximum - minimum) / (value * scaleDivisions) > 0))
                {
                    scaleSubDivisions = value;
                    Invalidate();
                }
            }
        }

        [Description("Show or hide graduations")]
        [Category(Localize.Category.Behavior)]
        public bool ShowLargeScale
        {
            get
            {
                return showLargeScale;
            }

            set
            {
                showLargeScale = value;

                // need to redraw
                ConfigureDimensions();

                Invalidate();
            }
        }

        [Description("Show or hide subdivisions of graduations")]
        [Category(Localize.Category.Behavior)]
        public bool ShowSmallScale
        {
            get
            {
                return showSmallScale;
            }

            set
            {
                if (value)
                {
                    if ((scaleDivisions > 0) && (scaleSubDivisions > 0) && ((maximum - minimum) / (scaleSubDivisions * scaleDivisions) > 0))
                    {
                        showSmallScale = value;
                        Invalidate();
                    }
                }
                else
                {
                    showSmallScale = value;

                    // need to redraw 
                    Invalidate();
                }
            }
        }

        [Description("set the minimum value for the small changes")]
        [Category(Localize.Category.Behavior)]
        public int SmallChange
        {
            get
            {
                return smallChange;
            }

            set
            {
                smallChange = value;
                Invalidate();
            }
        }

        [Description("Set the start angle to display graduations (min 90)")]
        [Category(Localize.Category.Behavior)]
        [DefaultValue(135)]
        public float StartAngle
        {
            get
            {
                return startAngle;
            }

            set
            {
                if ((value >= 90) && (value < endAngle))
                {
                    startAngle = value;
                    deltaAngle = endAngle - StartAngle;
                    Invalidate();
                }
            }
        }

        [Description(Localize.Description.ComponentColor)]
        [Category(Localize.Category.Behavior)]
        public Color TickColor
        {
            get
            {
                return tickColor;
            }

            set
            {
                tickColor = value;
                Invalidate();
            }
        }

        [Description("set the current value of the knob control")]
        [Category(Localize.Category.Behavior)]
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;

                // need to redraw 
                Invalidate();

                // call delegate  
                OnValueChanged(this);
            }
        }

        [Description("Displays the value text")]
        [Category(Localize.Category.Behavior)]
        public bool ValueVisible
        {
            get
            {
                return valueVisible;
            }

            set
            {
                valueVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override bool IsInputKey(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                    return true;
            }
            return base.IsInputKey(key);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (focused)
            {
                // --------------------------------------------------------
                // Handles knob rotation with up,down,left and right keys 
                // --------------------------------------------------------
                if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Right))
                {
                    if (_value < maximum)
                    {
                        Value = _value + 1;
                    }

                    Refresh();
                }
                else if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Left))
                {
                    if (_value > minimum)
                    {
                        Value = _value - 1;
                    }

                    Refresh();
                }
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            // unselect the control (remove dotted border)
            focused = false;
            rotating = false;
            Invalidate();

            base.OnLeave(new EventArgs());
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (GDI.IsMouseInBounds(e.Location, knobRectangle))
            {
                if (focused)
                {
                    // was already selected
                    // Start Rotation of knob only if it was selected before        
                    rotating = true;
                }
                else
                {
                    // Was not selected before => select it
                    Focus();
                    focused = true;
                    rotating = false; // disallow rotation, must click again

                    // draw dotted border to show that it is selected
                    Invalidate();
                }
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
            // --------------------------------------
            // Following Handles Knob Rotating     
            // --------------------------------------
            if ((e.Button == MouseButtons.Left) && rotating)
            {
                Cursor = Cursors.Hand;
                Point p = new Point(e.X, e.Y);
                int posVal = GetValueFromPosition(p);
                Value = posVal;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (GDI.IsMouseInBounds(e.Location, knobRectangle))
            {
                if (focused && rotating)
                {
                    // change value is allowed only only after 2nd click                   
                    Value = GetValueFromPosition(new Point(e.X, e.Y));
                }
                else
                {
                    // 1st click = only focus
                    focused = true;
                    rotating = true;
                }
            }

            Cursor = Cursors.Default;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (focused && rotating && GDI.IsMouseInBounds(e.Location, knobRectangle))
            {
                // the Delta value is always 120, as explained in MSDN
                int v = ((e.Delta / 120) * (maximum - minimum)) / mouseWheelBarPartitions;
                SetValue(Value + v);

                // Avoid to send MouseWheel event to the parent container
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = GDI.Initialize(e, CompositingMode.SourceOver, CompositingQuality.Default, InterpolationMode.Default, PixelOffsetMode.Default, SmoothingMode.HighQuality, TextRenderingHint.AntiAliasGridFit);
            offGraphics = graphics;
            offGraphics.Clear(BackColor);

            gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };

            DrawScale();
            DrawKnob();
            DrawKnobTop();
            DrawPointer(offGraphics);
            DrawDivisions(offGraphics, knobRectangle);

            graphics.DrawImage(offScreenImage, 0, 0);

            if (valueVisible)
            {
                string value = _value.ToString("0");
                Size textAreaSize = GDI.GetTextSize(e.Graphics, value, Font);
                graphics.DrawString(value, Font, new SolidBrush(ForeColor), (Width / 2) - (textAreaSize.Width / 2), (Height / 2) - (textAreaSize.Height / 2));
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Empty To avoid Flickring due do background Drawing.
        }

        private void ConfigureDimensions()
        {
            int size = Width;
            Height = size;

            // Rectangle
            float x, y, w, h;
            x = 0;
            y = 0;
            w = Size.Width;
            h = Size.Height;

            // Calculate ratio
            drawRatio = Math.Min(w, h) / 200;
            if (drawRatio == 0.0)
            {
                drawRatio = 1;
            }

            if (showLargeScale)
            {
                float fSize = 6F * drawRatio;
                if (fSize < 6)
                {
                    fSize = 6;
                }

                knobFont = new Font(Font.FontFamily, fSize);
                double val = maximum;
                string str = string.Format("{0,0:D}", (int)val);

                Graphics Gr = CreateGraphics();
                SizeF strsize = Gr.MeasureString(str, knobFont);
                int strw = (int)strsize.Width + 4;
                var strh = (int)strsize.Height;

                // allow 10% gap on all side to determine size of knob    
                // this.rKnob = new Rectangle((int)(size * 0.10), (int)(size * 0.15), (int)(size * 0.80), (int)(size * 0.80));
                x = strw;

                // y = x;
                y = 2 * strh;
                w = size - (2 * strw);
                if (w <= 0)
                {
                    w = 1;
                }

                h = w;
                knobRectangle = new Rectangle((int)x, (int)y, (int)w, (int)h);
                Gr.Dispose();
            }
            else
            {
                // rKnob = new Rectangle(0, 0, Width, Height);
            }

            // Center of knob
            knobPoint = new Point(knobRectangle.X + (knobRectangle.Width / 2), knobRectangle.Y + (knobRectangle.Height / 2));

            offScreenImage = new Bitmap(Width, Height);
            offGraphics = Graphics.FromImage(offScreenImage);
        }

        private bool DrawDivisions(Graphics graphics, RectangleF rectangleF)
        {
            if (this == null)
            {
                return false;
            }

            float cx = knobPoint.X;
            float cy = knobPoint.Y;

            float w = rectangleF.Width;
            float h = rectangleF.Height;

            float tx;
            float ty;

            float incr = MathManager.DegreeToRadian((endAngle - startAngle) / ((scaleDivisions - 1) * (scaleSubDivisions + 1)));
            float currentAngle = MathManager.DegreeToRadian(startAngle);

            float radius = knobRectangle.Width / 2;
            float rulerValue = minimum;

            Pen penL = new Pen(TickColor, 2 * drawRatio);
            Pen penS = new Pen(TickColor, 1 * drawRatio);

            SolidBrush br = new SolidBrush(ForeColor);

            PointF ptStart = new PointF(0, 0);
            PointF ptEnd = new PointF(0, 0);
            var n = 0;

            if (showLargeScale)
            {
                for (; n < scaleDivisions; n++)
                {
                    // draw divisions
                    ptStart.X = (float)(cx + (radius * Math.Cos(currentAngle)));
                    ptStart.Y = (float)(cy + (radius * Math.Sin(currentAngle)));

                    ptEnd.X = (float)(cx + ((radius + (w / 50)) * Math.Cos(currentAngle)));
                    ptEnd.Y = (float)(cy + ((radius + (w / 50)) * Math.Sin(currentAngle)));

                    graphics.DrawLine(penL, ptStart, ptEnd);

                    // Draw graduations Strings                    
                    float fSize = 6F * drawRatio;
                    if (fSize < 6)
                    {
                        fSize = 6;
                    }

                    Font font = new Font(Font.FontFamily, fSize);

                    double val = Math.Round(rulerValue);
                    string str = string.Format("{0,0:D}", (int)val);
                    SizeF size = graphics.MeasureString(str, font);

                    if (drawDivInside)
                    {
                        // graduations strings inside the knob
                        tx = (float)(cx + ((radius - (11 * drawRatio)) * Math.Cos(currentAngle)));
                        ty = (float)(cy + ((radius - (11 * drawRatio)) * Math.Sin(currentAngle)));
                    }
                    else
                    {
                        // graduation strings outside the knob
                        tx = (float)(cx + ((radius + (11 * drawRatio)) * Math.Cos(currentAngle)));
                        ty = (float)(cy + ((radius + (11 * drawRatio)) * Math.Sin(currentAngle)));
                    }

                    graphics.DrawString(str,
                        font,
                        br,
                        tx - (float)(size.Width * 0.5),
                        ty - (float)(size.Height * 0.5));

                    rulerValue += (maximum - minimum) / (scaleDivisions - 1);

                    if (n == scaleDivisions - 1)
                    {
                        font.Dispose();
                        break;
                    }

                    // Subdivisions
                    if (scaleDivisions <= 0)
                    {
                        currentAngle += incr;
                    }
                    else
                    {
                        for (var j = 0; j <= scaleSubDivisions; j++)
                        {
                            currentAngle += incr;

                            // if user want to display small graduations
                            if (showSmallScale)
                            {
                                ptStart.X = (float)(cx + (radius * Math.Cos(currentAngle)));
                                ptStart.Y = (float)(cy + (radius * Math.Sin(currentAngle)));
                                ptEnd.X = (float)(cx + ((radius + (w / 50)) * Math.Cos(currentAngle)));
                                ptEnd.Y = (float)(cy + ((radius + (w / 50)) * Math.Sin(currentAngle)));

                                graphics.DrawLine(penS, ptStart, ptEnd);
                            }
                        }
                    }

                    font.Dispose();
                }
            }

            return true;
        }

        private void DrawKnob()
        {
            Point knobPoint = new Point((this.knobRectangle.X + (this.knobRectangle.Width / 2)) - (KnobSize.Width / 2), (this.knobRectangle.Y + (this.knobRectangle.Height / 2)) - (KnobSize.Height / 2));
            Rectangle knobRectangle = new Rectangle(knobPoint, KnobSize);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(knob.Colors, gradientPoints, knob.Angle, knob.Positions);
            offGraphics.FillEllipse(gradientBrush, knobRectangle);

            if (knobBorder.Visible)
            {
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddEllipse(knobRectangle);
                GDI.DrawBorderType(offGraphics, controlState, borderPath, knobBorder.Thickness, knobBorder.Color, knobBorder.HoverColor, knobBorder.Visible);
            }
        }

        private void DrawKnobTop()
        {
            Point knobTopPoint = new Point((knobRectangle.X + (knobRectangle.Width / 2)) - (KnobTopSize.Width / 2), (knobRectangle.Y + (knobRectangle.Height / 2)) - (KnobTopSize.Height / 2));
            Rectangle knobTopRectangle = new Rectangle(knobTopPoint, KnobTopSize);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(knobTop.Colors, gradientPoints, knobTop.Angle, knobTop.Positions);
            offGraphics.FillEllipse(gradientBrush, knobTopRectangle);

            if (knobTopBorder.Visible)
            {
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddEllipse(knobTopRectangle);
                GDI.DrawBorderType(offGraphics, controlState, borderPath, knobTopBorder.Thickness, knobTopBorder.Color, knobTopBorder.HoverColor, knobTopBorder.HoverVisible);
            }

            float cx = knobPoint.X;
            float cy = knobPoint.Y;

            float w = KnobTopSize.Width;

            // TODO: Adjust
            float incr = MathManager.DegreeToRadian((startAngle - endAngle) / ((buttonDivisions - 1) * (scaleSubDivisions + 1)));
            float currentAngle = MathManager.DegreeToRadian(0);

            float radius = KnobTickSize.Width / 2;

            Pen penL = new Pen(TickColor, 2 * drawRatio);

            PointF ptStart = new PointF(0, 0);
            PointF ptEnd = new PointF(0, 0);
            var n = 0;

            for (; n < buttonDivisions; n++)
            {
                // draw divisions
                ptStart.X = (float)(cx + (radius * Math.Cos(currentAngle)));
                ptStart.Y = (float)(cy + (radius * Math.Sin(currentAngle)));

                ptEnd.X = (float)(cx + ((radius + (w / 50)) * Math.Cos(currentAngle)));
                ptEnd.Y = (float)(cy + ((radius + (w / 50)) * Math.Sin(currentAngle)));

                // TODO: draw lines along button border
                // gOffScreen.DrawLine(penL, ptStart, ptEnd);

                // Draw graduations Strings                    
                float fSize = 6F * drawRatio;
                if (fSize < 6)
                {
                    fSize = 6;
                }

                Font font = new Font(Font.FontFamily, fSize);

                if (n == buttonDivisions - 1)
                {
                    font.Dispose();
                    break;
                }

                // Subdivisions
                if (buttonDivisions <= 0)
                {
                    currentAngle += incr;
                }
                else
                {
                    for (var j = 0; j <= scaleSubDivisions; j++)
                    {
                        currentAngle += incr;
                    }
                }

                font.Dispose();
            }
        }

        private void DrawPointer(Graphics graphics)
        {
            try
            {
                if (pointerStyle == PointerStyle.Line)
                {
                    float radius = knobRectangle.Width / 2;

                    int l = ((int)radius / 2) + lineSize.Height;
                    int w = (l / 4) + lineSize.Width;
                    var pt = GetKnobLine(l);

                    graphics.DrawLine(new Pen(pointerColor, w), pt[0], pt[1]);
                }
                else
                {
                    int w;
                    int h;

                    w = knobRectangle.Width / 10;
                    if (w < 7)
                    {
                        w = 7;
                    }

                    h = w;

                    Point Arrow = GetKnobPosition(w);
                    Rectangle rPointer = new Rectangle(Arrow.X - (w / 2), Arrow.Y - (w / 2), w, h);
                    graphics.FillEllipse(new SolidBrush(pointerColor), rPointer);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void DrawScale()
        {
            Size scaleSize = new Size(knobRectangle.Width, knobRectangle.Height);
            Point scalePoint = new Point(knobRectangle.X, knobRectangle.Y);
            Rectangle scaleRectangle = new Rectangle(scalePoint, scaleSize);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(scale.Colors, gradientPoints, scale.Angle, scale.Positions);
            offGraphics.FillEllipse(gradientBrush, scaleRectangle);
        }

        private Point[] GetKnobLine(int l)
        {
            var pret = new Point[2];

            float cx = knobPoint.X;
            float cy = knobPoint.Y;

            float radius = knobRectangle.Width / 2;

            float degree = (deltaAngle * Value) / (maximum - minimum);
            degree = MathManager.DegreeToRadian(degree + startAngle);

            Point Pos = new Point(0, 0);

            Pos.X = (int)(cx + ((radius - (drawRatio * 10)) * Math.Cos(degree)));
            Pos.Y = (int)(cy + ((radius - (drawRatio * 10)) * Math.Sin(degree)));

            pret[0] = new Point(Pos.X, Pos.Y);

            Pos.X = (int)(cx + ((radius - (drawRatio * 10) - l) * Math.Cos(degree)));
            Pos.Y = (int)(cy + ((radius - (drawRatio * 10) - l) * Math.Sin(degree)));

            pret[1] = new Point(Pos.X, Pos.Y);

            return pret;
        }

        private Point GetKnobPosition(int l)
        {
            float cx = knobPoint.X;
            float cy = knobPoint.Y;

            float radius = knobRectangle.Width / 2;

            float degree = (deltaAngle * Value) / (maximum - minimum);
            degree = MathManager.DegreeToRadian(degree + startAngle);

            Point Pos = new Point(0, 0)
                {
                    X = (int)(cx + ((radius - (KnobDistance * drawRatio)) * Math.Cos(degree))),
                    Y = (int)(cy + ((radius - (KnobDistance * drawRatio)) * Math.Sin(degree)))
                };

            return Pos;
        }

        private int GetValueFromPosition(Point point)
        {
            float degree = 0;
            var v = 0;

            if (point.X <= knobPoint.X)
            {
                degree = (knobPoint.Y - point.Y) / (float)(knobPoint.X - point.X);
                degree = (float)Math.Atan(degree);

                degree = (degree * (float)(180 / Math.PI)) + (180 - startAngle);
            }
            else if (point.X > knobPoint.X)
            {
                degree = (point.Y - knobPoint.Y) / (float)(point.X - knobPoint.X);
                degree = (float)Math.Atan(degree);

                degree = ((degree * (float)(180 / Math.PI)) + 360) - startAngle;
            }

            // round to the nearest value (when you click just before or after a graduation!)
            v = (int)Math.Round((degree * (maximum - minimum)) / deltaAngle);

            if (v > maximum)
            {
                v = maximum;
            }

            if (v < minimum)
            {
                v = minimum;
            }

            return v;
        }

        private void InitializeComponent()
        {
            ImeMode = ImeMode.On;
            Name = "VisualKnob";
            Resize += KnobControl_Resize;
        }

        private void KnobControl_Resize(object sender, EventArgs e)
        {
            ConfigureDimensions();

            // Refresh();
            Invalidate();
        }

        private void OnValueChanged(object sender)
        {
            if (ValueChanged != null)
            {
                ValueChanged(sender);
            }
        }

        private void SetValue(int value)
        {
            if (value < minimum)
            {
                Value = minimum;
            }
            else if (value > maximum)
            {
                Value = maximum;
            }
            else
            {
                Value = value;
            }
        }

        #endregion
    }
}