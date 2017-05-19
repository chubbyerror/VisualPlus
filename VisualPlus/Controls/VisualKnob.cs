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
    using VisualPlus.Localization;

    #endregion

    public delegate void ValueChangedEventHandler(object Sender);

    public sealed class VisualKnob : UserControl
    {
        #region Variables

        private bool _drawDivInside;
        private float _endAngle = 405;
        private Color _knobBackColor = Color.WhiteSmoke;
        private int _LargeChange = 5;
        private int _maximum = 25;

        private int _minimum;
        private int _mouseWheelBarPartitions = 10;

        private Color _PointerColor = Settings.DefaultValue.Style.ProgressColor;

        private int _scaleDivisions;
        private int _scaleSubDivisions;
        private bool _showLargeScale = true;

        private bool _showSmallScale = true;
        private int _SmallChange = 1;

        private float _startAngle = 135;

        private Color _tickColor = Color.DimGray;

        private int _Value;

        private Brush bKnobPoint;

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private int buttonDivisions = 30;

        private Container components = null;
        private ControlState controlState = ControlState.Normal;

        private float deltaAngle;

        private float drawRatio;

        private Graphics gOffScreen;
        private float gradientKnobAngle = 180;
        private float[] gradientKnobPosition = { 0, 1 };
        private float gradientKnobTopAngle;
        private float[] gradientKnobTopPosition = { 0, 1 };

        private float gradientScaleAngle;
        private float[] gradientScalePosition = { 0, 1 };

        private bool isFocused;
        private bool isKnobRotating;

        private Color[] knobColor =
            {
                Color.LightGray,
                Color.White
            };

        private int knobDistance = 35;

        private Font knobFont;

        private Size knobSize = new Size(90, 90);

        private Size knobTickSize = new Size(86, 86);

        private Color[] knobTopColor =
            {
                Color.White,
                Color.LightGray
            };

        private Size knobTopSize = new Size(75, 75);

        private Size lineSize = new Size(1, 1);

        private Image OffScreenImage;
        private Point pKnob;

        private PointerStyle pointerStyle = PointerStyle.Circle;
        private Rectangle rKnob;

        private Color[] scaleColor =
            {
                Color.LightGray,
                Color.White
            };

        private bool valueVisible = true;

        #endregion

        #region Constructors

        public VisualKnob()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            // This call is required by the Windows.Forms Form Designer.
            new Pen(ColorHelper.GetColorTint(ColorHelper.Brightness.Dark, BackColor, 40))
                {
                    DashStyle = DashStyle.Dash,
                    DashCap = DashCap.Flat
                };

            InitializeComponent();

            knobFont = Font;
            ForeColor = Color.DimGray;

            // "start angle" and "end angle" possible values:

            // 90 = bottom (minimum value for "start angle")
            // 180 = left
            // 270 = top
            // 360 = right
            // 450 = bottom again (maximum value for "end angle")

            // So the couple (90, 450) will give an entire circle and the couple (180, 360) will give half a circle.
            _startAngle = 135;
            _endAngle = 405;
            deltaAngle = _endAngle - _startAngle;

            _minimum = 0;
            _maximum = 100;
            _scaleDivisions = 11;
            _scaleSubDivisions = 4;
            _mouseWheelBarPartitions = 10;
            setDimensions();
        }

        public enum PointerStyle
        {
            /// <summary>The circle.</summary>
            Circle,

            /// <summary>The line.</summary>
            Line
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderColor)]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }

            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderThickness)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderThickness)]
        public int BorderThickness
        {
            get
            {
                return borderThickness;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderThickness = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderVisible)]
        public bool BorderVisible
        {
            get
            {
                return borderVisible;
            }

            set
            {
                borderVisible = value;
                Invalidate();
            }
        }

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
                return _drawDivInside;
            }

            set
            {
                _drawDivInside = value;
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
                return _endAngle;
            }

            set
            {
                if (value <= 450 && value > _startAngle)
                {
                    _endAngle = value;
                    deltaAngle = _endAngle - _startAngle;
                    Invalidate();
                }
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientKnobAngle
        {
            get
            {
                return gradientKnobAngle;
            }

            set
            {
                gradientKnobAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientKnobPosition
        {
            get
            {
                return gradientKnobPosition;
            }

            set
            {
                gradientKnobPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientKnobTopAngle
        {
            get
            {
                return gradientKnobTopAngle;
            }

            set
            {
                gradientKnobTopAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientKnobTopPosition
        {
            get
            {
                return gradientKnobTopPosition;
            }

            set
            {
                gradientKnobTopPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientScaleAngle
        {
            get
            {
                return gradientScaleAngle;
            }

            set
            {
                gradientScaleAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientScalePosition
        {
            get
            {
                return gradientScalePosition;
            }

            set
            {
                gradientScalePosition = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentColor)]
        [Category(Localize.Category.Appearance)]
        public Color knobBackColor
        {
            get
            {
                return _knobBackColor;
            }

            set
            {
                _knobBackColor = value;
                setDimensions();
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentColor)]
        [Category(Localize.Category.Appearance)]
        public Color[] KnobColor
        {
            get
            {
                return knobColor;
            }

            set
            {
                knobColor = value;
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

        [Description(Localize.Description.ComponentColor)]
        [Category(Localize.Category.Appearance)]
        public Color[] KnobTopColor
        {
            get
            {
                return knobTopColor;
            }

            set
            {
                knobTopColor = value;
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
                return _LargeChange;
            }

            set
            {
                _LargeChange = value;
                Invalidate();
            }
        }

        [Description("set the maximum value for the knob control")]
        [Category(Localize.Category.Behavior)]
        public int Maximum
        {
            get
            {
                return _maximum;
            }

            set
            {
                if (value > _minimum)
                {
                    _maximum = value;

                    if (_scaleSubDivisions > 0 && _scaleDivisions > 0 && (_maximum - _minimum) / (_scaleSubDivisions * _scaleDivisions) <= 0)
                    {
                        _showSmallScale = false;
                    }

                    setDimensions();
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
                return _minimum;
            }

            set
            {
                _minimum = value;
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
                return _mouseWheelBarPartitions;
            }

            set
            {
                if (value > 0)
                {
                    _mouseWheelBarPartitions = value;
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
                return _PointerColor;
            }

            set
            {
                _PointerColor = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.ComponentColor)]
        [Category(Localize.Category.Appearance)]
        public Color[] ScaleColor
        {
            get
            {
                return scaleColor;
            }

            set
            {
                scaleColor = value;
                Invalidate();
            }
        }

        [Description("Set the number of intervals between minimum and maximum")]
        [Category(Localize.Category.Behavior)]
        public int ScaleDivisions
        {
            get
            {
                return _scaleDivisions;
            }

            set
            {
                _scaleDivisions = value;
                Invalidate();
            }
        }

        [Description("Set the number of subdivisions between main divisions of graduation.")]
        [Category(Localize.Category.Behavior)]
        public int ScaleSubDivisions
        {
            get
            {
                return _scaleSubDivisions;
            }

            set
            {
                if (value > 0 && _scaleDivisions > 0 && (_maximum - _minimum) / (value * _scaleDivisions) > 0)
                {
                    _scaleSubDivisions = value;
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
                return _showLargeScale;
            }

            set
            {
                _showLargeScale = value;

                // need to redraw
                setDimensions();

                Invalidate();
            }
        }

        [Description("Show or hide subdivisions of graduations")]
        [Category(Localize.Category.Behavior)]
        public bool ShowSmallScale
        {
            get
            {
                return _showSmallScale;
            }

            set
            {
                if (value)
                {
                    if (_scaleDivisions > 0 && _scaleSubDivisions > 0 && (_maximum - _minimum) / (_scaleSubDivisions * _scaleDivisions) > 0)
                    {
                        _showSmallScale = value;
                        Invalidate();
                    }
                }
                else
                {
                    _showSmallScale = value;

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
                return _SmallChange;
            }

            set
            {
                _SmallChange = value;
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
                return _startAngle;
            }

            set
            {
                if (value >= 90 && value < _endAngle)
                {
                    _startAngle = value;
                    deltaAngle = _endAngle - StartAngle;
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
                return _tickColor;
            }

            set
            {
                _tickColor = value;
                Invalidate();
            }
        }

        [Description("set the current value of the knob control")]
        [Category(Localize.Category.Behavior)]
        public int Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;

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

        public event ValueChangedEventHandler ValueChanged;

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
            if (isFocused)
            {
                // --------------------------------------------------------
                // Handles knob rotation with up,down,left and right keys 
                // --------------------------------------------------------
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Right)
                {
                    if (_Value < _maximum)
                    {
                        Value = _Value + 1;
                    }

                    Refresh();
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Left)
                {
                    if (_Value > _minimum)
                    {
                        Value = _Value - 1;
                    }

                    Refresh();
                }
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            // unselect the control (remove dotted border)
            isFocused = false;
            isKnobRotating = false;
            Invalidate();

            base.OnLeave(new EventArgs());
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (GDI.IsMouseInBounds(e.Location, rKnob))
            {
                if (isFocused)
                {
                    // was already selected
                    // Start Rotation of knob only if it was selected before        
                    isKnobRotating = true;
                }
                else
                {
                    // Was not selected before => select it
                    Focus();
                    isFocused = true;
                    isKnobRotating = false; // disallow rotation, must click again

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
            if (e.Button == MouseButtons.Left && isKnobRotating)
            {
                Cursor = Cursors.Hand;
                Point p = new Point(e.X, e.Y);
                int posVal = getValueFromPosition(p);
                Value = posVal;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (GDI.IsMouseInBounds(e.Location, rKnob))
            {
                if (isFocused && isKnobRotating)
                {
                    // change value is allowed only only after 2nd click                   
                    Value = getValueFromPosition(new Point(e.X, e.Y));
                }
                else
                {
                    // 1st click = only focus
                    isFocused = true;
                    isKnobRotating = true;
                }
            }

            Cursor = Cursors.Default;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (isFocused && isKnobRotating && GDI.IsMouseInBounds(e.Location, rKnob))
            {
                // the Delta value is always 120, as explained in MSDN
                int v = e.Delta / 120 * (_maximum - _minimum) / _mouseWheelBarPartitions;
                SetProperValue(Value + v);

                // Avoid to send MouseWheel event to the parent container
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = GDI.Initialize(e, CompositingMode.SourceOver, CompositingQuality.Default, InterpolationMode.Default, PixelOffsetMode.Default, SmoothingMode.HighQuality, TextRenderingHint.AntiAliasGridFit);
            gOffScreen = graphics;
            gOffScreen.Clear(BackColor);

            DrawScale();
            DrawKnob();
            DrawKnobTop();

            // Draw border of knob                         
            // gOffScreen.DrawEllipse(new Pen(BackColor), rKnob);

            // if control is focused 
            if (isFocused)
            {
                // gOffScreen.DrawEllipse(DottedPen, rKnob);
            }

            // DrawPointer
            DrawPointer(gOffScreen);

            DrawDivisions(gOffScreen, rKnob);

            graphics.DrawImage(OffScreenImage, 0, 0);

            if (valueVisible)
            {
                string value = _Value.ToString("0");
                Size textAreaSize = GDI.GetTextSize(e.Graphics, value, Font);
                graphics.DrawString(value, Font, new SolidBrush(ForeColor), Width / 2 - textAreaSize.Width / 2, Height / 2 - textAreaSize.Height / 2);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Empty To avoid Flickring due do background Drawing.
        }

        private bool DrawDivisions(Graphics graphics, RectangleF rc)
        {
            if (this == null)
            {
                return false;
            }

            float cx = pKnob.X;
            float cy = pKnob.Y;

            float w = rc.Width;
            float h = rc.Height;

            float tx;
            float ty;

            float incr = GDI.GetRadian((_endAngle - _startAngle) / ((_scaleDivisions - 1) * (_scaleSubDivisions + 1)));
            float currentAngle = GDI.GetRadian(_startAngle);

            float radius = rKnob.Width / 2;
            float rulerValue = _minimum;

            Pen penL = new Pen(TickColor, 2 * drawRatio);
            Pen penS = new Pen(TickColor, 1 * drawRatio);

            SolidBrush br = new SolidBrush(ForeColor);

            PointF ptStart = new PointF(0, 0);
            PointF ptEnd = new PointF(0, 0);
            var n = 0;

            if (_showLargeScale)
            {
                for (; n < _scaleDivisions; n++)
                {
                    // draw divisions
                    ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
                    ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));

                    ptEnd.X = (float)(cx + (radius + w / 50) * Math.Cos(currentAngle));
                    ptEnd.Y = (float)(cy + (radius + w / 50) * Math.Sin(currentAngle));

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

                    if (_drawDivInside)
                    {
                        // graduations strings inside the knob
                        tx = (float)(cx + (radius - 11 * drawRatio) * Math.Cos(currentAngle));
                        ty = (float)(cy + (radius - 11 * drawRatio) * Math.Sin(currentAngle));
                    }
                    else
                    {
                        // graduation strings outside the knob
                        tx = (float)(cx + (radius + 11 * drawRatio) * Math.Cos(currentAngle));
                        ty = (float)(cy + (radius + 11 * drawRatio) * Math.Sin(currentAngle));
                    }

                    graphics.DrawString(str,
                        font,
                        br,
                        tx - (float)(size.Width * 0.5),
                        ty - (float)(size.Height * 0.5));

                    rulerValue += (_maximum - _minimum) / (_scaleDivisions - 1);

                    if (n == _scaleDivisions - 1)
                    {
                        font.Dispose();
                        break;
                    }

                    // Subdivisions
                    if (_scaleDivisions <= 0)
                    {
                        currentAngle += incr;
                    }
                    else
                    {
                        for (var j = 0; j <= _scaleSubDivisions; j++)
                        {
                            currentAngle += incr;

                            // if user want to display small graduations
                            if (_showSmallScale)
                            {
                                ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
                                ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));
                                ptEnd.X = (float)(cx + (radius + w / 50) * Math.Cos(currentAngle));
                                ptEnd.Y = (float)(cy + (radius + w / 50) * Math.Sin(currentAngle));

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
            Point knobPoint = new Point(rKnob.X + rKnob.Width / 2 - KnobSize.Width / 2, rKnob.Y + rKnob.Height / 2 - KnobSize.Height / 2);
            Rectangle knobRectangle = new Rectangle(knobPoint, KnobSize);

            Point startPoint = new Point(ClientRectangle.Width, 0);
            Point endPoint = new Point(ClientRectangle.Width, ClientRectangle.Height);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(knobColor, gradientKnobPosition, gradientKnobAngle, startPoint, endPoint);
            gOffScreen.FillEllipse(gradientBrush, knobRectangle);

            if (borderVisible)
            {
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddEllipse(knobRectangle);
                GDI.DrawBorder(gOffScreen, borderPath, borderThickness, borderColor);
            }
        }

        private void DrawKnobTop()
        {
            Point knobTopPoint = new Point(rKnob.X + rKnob.Width / 2 - KnobTopSize.Width / 2, rKnob.Y + rKnob.Height / 2 - KnobTopSize.Height / 2);
            Rectangle knobTopRectangle = new Rectangle(knobTopPoint, KnobTopSize);

            Point startPoint = new Point(ClientRectangle.Width, 0);
            Point endPoint = new Point(ClientRectangle.Width, ClientRectangle.Height);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(knobTopColor, gradientKnobTopPosition, gradientKnobTopAngle, startPoint, endPoint);
            gOffScreen.FillEllipse(gradientBrush, knobTopRectangle);

            if (borderVisible)
            {
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddEllipse(knobTopRectangle);
                GDI.DrawBorder(gOffScreen, borderPath, borderThickness, borderColor);
            }

            float cx = pKnob.X;
            float cy = pKnob.Y;

            float w = KnobTopSize.Width;

            // TODO: Adjust
            float incr = GDI.GetRadian((_startAngle - _endAngle) / ((buttonDivisions - 1) * (_scaleSubDivisions + 1)));
            float currentAngle = GDI.GetRadian(0);

            float radius = KnobTickSize.Width / 2;

            Pen penL = new Pen(TickColor, 2 * drawRatio);

            PointF ptStart = new PointF(0, 0);
            PointF ptEnd = new PointF(0, 0);
            var n = 0;

            for (; n < buttonDivisions; n++)
            {
                // draw divisions
                ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
                ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));

                ptEnd.X = (float)(cx + (radius + w / 50) * Math.Cos(currentAngle));
                ptEnd.Y = (float)(cy + (radius + w / 50) * Math.Sin(currentAngle));

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
                    for (var j = 0; j <= _scaleSubDivisions; j++)
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
                    float radius = rKnob.Width / 2;

                    int l = (int)radius / 2 + lineSize.Height;
                    int w = l / 4 + lineSize.Width;
                    var pt = getKnobLine(l);

                    graphics.DrawLine(new Pen(_PointerColor, w), pt[0], pt[1]);
                }
                else
                {
                    int w;
                    int h;

                    w = rKnob.Width / 10;
                    if (w < 7)
                    {
                        w = 7;
                    }

                    h = w;

                    Point Arrow = getKnobPosition(w);
                    Rectangle rPointer = new Rectangle(Arrow.X - w / 2, Arrow.Y - w / 2, w, h);
                    graphics.FillEllipse(new SolidBrush(_PointerColor), rPointer);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void DrawScale()
        {
            Size scaleSize = new Size(rKnob.Width, rKnob.Height);
            Point scalePoint = new Point(rKnob.X, rKnob.Y);
            Rectangle scaleRectangle = new Rectangle(scalePoint, scaleSize);

            Point startPoint = new Point(ClientRectangle.Width, 0);
            Point endPoint = new Point(ClientRectangle.Width, ClientRectangle.Height);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(scaleColor, gradientScalePosition, gradientScaleAngle, startPoint, endPoint);
            gOffScreen.FillEllipse(gradientBrush, scaleRectangle);
        }

        private Point[] getKnobLine(int l)
        {
            var pret = new Point[2];

            float cx = pKnob.X;
            float cy = pKnob.Y;

            float radius = rKnob.Width / 2;

            float degree = deltaAngle * Value / (_maximum - _minimum);
            degree = GDI.GetRadian(degree + _startAngle);

            Point Pos = new Point(0, 0);

            Pos.X = (int)(cx + (radius - drawRatio * 10) * Math.Cos(degree));
            Pos.Y = (int)(cy + (radius - drawRatio * 10) * Math.Sin(degree));

            pret[0] = new Point(Pos.X, Pos.Y);

            Pos.X = (int)(cx + (radius - drawRatio * 10 - l) * Math.Cos(degree));
            Pos.Y = (int)(cy + (radius - drawRatio * 10 - l) * Math.Sin(degree));

            pret[1] = new Point(Pos.X, Pos.Y);

            return pret;
        }

        private Point getKnobPosition(int l)
        {
            float cx = pKnob.X;
            float cy = pKnob.Y;

            float radius = rKnob.Width / 2;

            float degree = deltaAngle * Value / (_maximum - _minimum);
            degree = GDI.GetRadian(degree + _startAngle);

            Point Pos = new Point(0, 0)
                {
                    X = (int)(cx + (radius - KnobDistance * drawRatio) * Math.Cos(degree)),
                    Y = (int)(cy + (radius - KnobDistance * drawRatio) * Math.Sin(degree))
                };

            return Pos;
        }

        private int getValueFromPosition(Point p)
        {
            float degree = 0;
            var v = 0;

            if (p.X <= pKnob.X)
            {
                degree = (pKnob.Y - p.Y) / (float)(pKnob.X - p.X);
                degree = (float)Math.Atan(degree);

                degree = degree * (float)(180 / Math.PI) + (180 - _startAngle);
            }
            else if (p.X > pKnob.X)
            {
                degree = (p.Y - pKnob.Y) / (float)(p.X - pKnob.X);
                degree = (float)Math.Atan(degree);

                degree = degree * (float)(180 / Math.PI) + 360 - _startAngle;
            }

            // round to the nearest value (when you click just before or after a graduation!)
            v = (int)Math.Round(degree * (_maximum - _minimum) / deltaAngle);

            if (v > _maximum)
            {
                v = _maximum;
            }

            if (v < _minimum)
            {
                v = _minimum;
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
            setDimensions();

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

        private void setDimensions()
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

            if (_showLargeScale)
            {
                float fSize = 6F * drawRatio;
                if (fSize < 6)
                {
                    fSize = 6;
                }

                knobFont = new Font(Font.FontFamily, fSize);
                double val = _maximum;
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
                w = size - 2 * strw;
                if (w <= 0)
                {
                    w = 1;
                }

                h = w;
                rKnob = new Rectangle((int)x, (int)y, (int)w, (int)h);
                Gr.Dispose();
            }
            else
            {
                // rKnob = new Rectangle(0, 0, Width, Height);
            }

            // Center of knob
            pKnob = new Point(rKnob.X + rKnob.Width / 2, rKnob.Y + rKnob.Height / 2);

            OffScreenImage = new Bitmap(Width, Height);
            gOffScreen = Graphics.FromImage(OffScreenImage);
        }

        private void SetProperValue(int val)
        {
            if (val < _minimum)
            {
                Value = _minimum;
            }
            else if (val > _maximum)
            {
                Value = _maximum;
            }
            else
            {
                Value = val;
            }
        }

        #endregion
    }
}