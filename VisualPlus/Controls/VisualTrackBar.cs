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

    /// <summary>The visual TrackBar.</summary>
    [DefaultEvent("ValueChanged")]
    [ToolboxBitmap(typeof(TrackBar))]
    [Designer(VSDesignerBinding.VisualTrackBar)]
    public sealed class VisualTrackBar : TrackBar
    {
        #region Variables

        private static Color hatchBackColor = Settings.DefaultValue.Style.HatchColor;

        private static Color[] progressColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ProgressColor),
                Settings.DefaultValue.Style.ProgressColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ProgressColor)
            };

        private static Orientation trackBarType = Orientation.Horizontal;
        private static Rectangle trackerRectangle = Rectangle.Empty;

        private Color[] backgroundColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private int barTickSpacing = 8;
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;

        private Color[] buttonColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ButtonNormalColor),
                Settings.DefaultValue.Style.ButtonNormalColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ButtonNormalColor)
            };

        private GraphicsPath buttonPath = new GraphicsPath();
        private Rectangle buttonRectangle;
        private Size buttonSize = new Size(27, 20);
        private Color buttonTextColor = Settings.DefaultValue.Style.ForeColor(0);
        private bool buttonVisible = true;

        private Color[] controlDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled,
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled)
            };

        private ControlState controlState = ControlState.Normal;
        private int currentUsedPos;
        private ValueDivisor dividedValue = ValueDivisor.By1;
        private Point endButtonPoint;
        private Point endPoint;
        private Point endProgressPoint;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private float gradientBackgroundAngle = 90;
        private float[] gradientBackgroundPosition = { 0, 1 / 2f, 1 };
        private LinearGradientBrush gradientBrush;
        private float gradientButtonAngle;
        private float[] gradientPosition = { 0, 1 / 2f, 1 };
        private float gradientProgressAngle = 90;
        private LinearGradientBrush gradientProgressBrush;
        private float[] gradientProgressPosition = { 0, 1 / 2f, 1 };
        private Color hatchForeColor = Color.FromArgb(40, hatchBackColor);
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;
        private int indentHeight;
        private int indentWidth;
        private bool leftButtonDown;
        private bool lineTicksVisible = Settings.DefaultValue.TextVisible;
        private float mouseStartPos = -1;
        private string prefix;
        private BrushType progressColorStyle = BrushType.Gradient;
        private bool progressFilling;
        private bool progressValueVisible;
        private bool progressVisible = Settings.DefaultValue.TextVisible;
        private Point startButtonPoint;
        private Point startPoint;
        private Point startProgressPoint;
        private string suffix;
        private Size textAreaSize;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private Font textFont = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Regular);
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color tickColor = Settings.DefaultValue.Style.LineColor;
        private int tickHeight = 4;
        private GraphicsPath trackBarPath;
        private Rectangle trackBarRectangle;
        private int trackLineThickness = 10;
        private bool valueTicksVisible = Settings.DefaultValue.TextVisible;
        private Rectangle workingRectangle;

        #endregion

        #region Constructors

        public VisualTrackBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            UpdateStyles();
            AutoSize = false;
            Size = new Size(200, 80);
            MinimumSize = new Size(10, 10);
        }

        public enum ValueDivisor
        {
            /// <summary>The by 1.</summary>
            By1 = 1,

            /// <summary>The by 10.</summary>
            By10 = 10,

            /// <summary>The by 100.</summary>
            By100 = 100,

            /// <summary>The by 1000.</summary>
            By1000 = 1000
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int BarTickSpacing
        {
            get
            {
                return barTickSpacing;
            }

            set
            {
                barTickSpacing = value;
                Invalidate();
            }
        }

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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderHoverColor)]
        public Color BorderHoverColor
        {
            get
            {
                return borderHoverColor;
            }

            set
            {
                borderHoverColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderHoverVisible)]
        public bool BorderHoverVisible
        {
            get
            {
                return borderHoverVisible;
            }

            set
            {
                borderHoverVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    borderRounding = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] ButtonColor
        {
            get
            {
                return buttonColor;
            }

            set
            {
                buttonColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public Size ButtonSize
        {
            get
            {
                return buttonSize;
            }

            set
            {
                buttonSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ButtonTextColor
        {
            get
            {
                return buttonTextColor;
            }

            set
            {
                buttonTextColor = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ButtonVisible
        {
            get
            {
                return buttonVisible;
            }

            set
            {
                buttonVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
        public Color[] ControlDisabledColor
        {
            get
            {
                return controlDisabledColor;
            }

            set
            {
                controlDisabledColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientBackgroundAngle
        {
            get
            {
                return gradientBackgroundAngle;
            }

            set
            {
                gradientBackgroundAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientBackgroundPosition
        {
            get
            {
                return gradientBackgroundPosition;
            }

            set
            {
                gradientBackgroundPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientButtonAngle
        {
            get
            {
                return gradientButtonAngle;
            }

            set
            {
                gradientButtonAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientPosition
        {
            get
            {
                return gradientPosition;
            }

            set
            {
                gradientPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientProgressAngle
        {
            get
            {
                return gradientProgressAngle;
            }

            set
            {
                gradientProgressAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientProgressPosition
        {
            get
            {
                return gradientProgressPosition;
            }

            set
            {
                gradientProgressPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color HatchBackColor
        {
            get
            {
                return hatchBackColor;
            }

            set
            {
                hatchBackColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color HatchForeColor
        {
            get
            {
                return hatchForeColor;
            }

            set
            {
                hatchForeColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [DefaultValue(Settings.DefaultValue.HatchSize)]
        [Description(Localize.Description.HatchSize)]
        public float HatchSize
        {
            get
            {
                return hatchSize;
            }

            set
            {
                hatchSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.HatchStyle)]
        public HatchStyle HatchStyle
        {
            get
            {
                return hatchStyle;
            }

            set
            {
                hatchStyle = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.HatchVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool HatchVisible
        {
            get
            {
                return hatchVisible;
            }

            set
            {
                hatchVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int IndentHeight
        {
            get
            {
                return indentHeight;
            }

            set
            {
                indentHeight = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int IndentWidth
        {
            get
            {
                return indentWidth;
            }

            set
            {
                indentWidth = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool LineTicksVisible
        {
            get
            {
                return lineTicksVisible;
            }

            set
            {
                lineTicksVisible = value;
                Invalidate();
            }
        }

        public new Orientation Orientation
        {
            get
            {
                return trackBarType;
            }

            set
            {
                trackBarType = value;

                // Flip separator size on orientation change.
                if (trackBarType == Orientation.Horizontal)
                {
                    // Horizontal
                    if (Width < Height)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }
                }
                else
                {
                    // Vertical
                    if (Width > Height)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }
                }
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public string Prefix
        {
            get
            {
                return prefix;
            }

            set
            {
                prefix = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] ProgressColor
        {
            get
            {
                return progressColor;
            }

            set
            {
                progressColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentColor)]
        public BrushType ProgressColorStyle
        {
            get
            {
                return progressColorStyle;
            }

            set
            {
                progressColorStyle = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ProgressFilling
        {
            get
            {
                return progressFilling;
            }

            set
            {
                progressFilling = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ProgressValueVisible
        {
            get
            {
                return progressValueVisible;
            }

            set
            {
                progressValueVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ProgressVisible
        {
            get
            {
                return progressVisible;
            }

            set
            {
                progressVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public string Suffix
        {
            get
            {
                return suffix;
            }

            set
            {
                suffix = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextColor)]
        public Color TextColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                foreColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TextDisabledColor
        {
            get
            {
                return textDisabledColor;
            }

            set
            {
                textDisabledColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font TextFont
        {
            get
            {
                return textFont;
            }

            set
            {
                textFont = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextRenderingHint)]
        public TextRenderingHint TextRendering
        {
            get
            {
                return textRendererHint;
            }

            set
            {
                textRendererHint = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int TickHeight
        {
            get
            {
                return tickHeight;
            }

            set
            {
                tickHeight = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int TrackLineThickness
        {
            get
            {
                return trackLineThickness;
            }

            set
            {
                trackLineThickness = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ValueDivisor)]
        public ValueDivisor ValueDivision
        {
            get
            {
                return dividedValue;
            }

            set
            {
                dividedValue = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextVisible)]
        public bool ValueTicksVisible
        {
            get
            {
                return valueTicksVisible;
            }

            set
            {
                valueTicksVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        /// <summary>Call the Decrement() method to decrease the value displayed by an integer you specify.</summary>
        /// <param name="value">The value to decrement.</param>
        public void Decrement(int value)
        {
            if (Value > Minimum)
            {
                Value -= value;
                if (Value < Minimum)
                {
                    Value = Minimum;
                }
            }
            else
            {
                Value = Minimum;
            }

            Invalidate();
        }

        /// <summary>Call the Increment() method to increase the value displayed by an integer you specify.</summary>
        /// <param name="value">The value to increment.</param>
        public void Increment(int value)
        {
            if (Value < Maximum)
            {
                Value += value;
                if (Value > Maximum)
                {
                    Value = Maximum;
                }
            }
            else
            {
                Value = Maximum;
            }

            Invalidate();
        }

        /// <summary>Sets a new range value.</summary>
        /// <param name="minimumValue">The minimum.</param>
        /// <param name="maximumValue">The maximum.</param>
        public new void SetRange(int minimumValue, int maximumValue)
        {
            Minimum = minimumValue;

            if (Minimum > Value)
            {
                Value = Minimum;
            }

            Maximum = maximumValue;

            if (Maximum < Value)
            {
                Value = Maximum;
            }

            if (Maximum < Minimum)
            {
                Minimum = Maximum;
            }

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var offsetValue = 0;
            Point currentPoint = new Point(e.X, e.Y);

            if (trackerRectangle.Contains(currentPoint))
            {
                if (!leftButtonDown)
                {
                    leftButtonDown = true;
                    Capture = true;
                    switch (trackBarType)
                    {
                        case Orientation.Horizontal:
                            {
                                mouseStartPos = currentPoint.X - trackerRectangle.X;
                                Invalidate();
                                break;
                            }

                        case Orientation.Vertical:
                            {
                                mouseStartPos = currentPoint.Y - trackerRectangle.Y;
                                Invalidate();
                                break;
                            }
                    }
                }
            }
            else
            {
                switch (trackBarType)
                {
                    case Orientation.Horizontal:
                        {
                            if ((currentPoint.X + buttonSize.Width) / 2 >= Width - indentWidth)
                            {
                                offsetValue = Maximum - Minimum;
                            }
                            else if ((currentPoint.X - buttonSize.Width) / 2 <= indentWidth)
                            {
                                offsetValue = 0;
                            }
                            else
                            {
                                offsetValue = (int)((currentPoint.X - indentWidth - buttonSize.Width) / 2 * (Maximum - Minimum) / (Width - 2 * indentWidth - buttonSize.Width) + 0.5);
                            }

                            break;
                        }

                    case Orientation.Vertical:
                        {
                            if ((currentPoint.Y + buttonSize.Width) / 2 >= Height - indentHeight)
                            {
                                offsetValue = 0;
                            }
                            else if ((currentPoint.Y - buttonSize.Width) / 2 <= indentHeight)
                            {
                                offsetValue = Maximum - Minimum;
                            }
                            else
                            {
                                offsetValue = (int)((Height - currentPoint.Y - indentHeight - buttonSize.Width) / 2 * (Maximum - Minimum) / (Height - 2 * indentHeight - buttonSize.Width) + 0.5);
                            }

                            break;
                        }
                }

                int oldValue = Value;
                Value = Minimum + offsetValue;

                Invalidate();

                if (oldValue != Value)
                {
                    OnScroll(e);
                    OnValueChanged(e);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            OnEnter(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = trackBarType == Orientation.Vertical ? Cursors.SizeNS : Cursors.SizeWE;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            OnLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var offsetValue = 0;
            PointF currentPoint = new PointF(e.X, e.Y);

            if (leftButtonDown)
            {
                try
                {
                    switch (trackBarType)
                    {
                        case Orientation.Horizontal:
                            {
                                if (currentPoint.X + buttonSize.Width - mouseStartPos >= Width - indentWidth)
                                {
                                    offsetValue = Maximum - Minimum;
                                }
                                else if (currentPoint.X - mouseStartPos <= indentWidth)
                                {
                                    offsetValue = 0;
                                }
                                else
                                {
                                    offsetValue =
                                        (int)
                                        ((currentPoint.X - mouseStartPos - indentWidth) * (Maximum - Minimum) /
                                         (Width - 2 * indentWidth - buttonSize.Width) + 0.5);
                                }

                                break;
                            }

                        case Orientation.Vertical:
                            {
                                if ((currentPoint.Y + buttonSize.Height) / 2 >= Height - indentHeight)
                                {
                                    offsetValue = 0;
                                }
                                else if ((currentPoint.Y + buttonSize.Height) / 2 <= indentHeight)
                                {
                                    offsetValue = Maximum - Minimum;
                                }
                                else
                                {
                                    offsetValue =
                                        (int)
                                        (((Height - currentPoint.Y + buttonSize.Height) / 2 - mouseStartPos - indentHeight) * (Maximum - Minimum) /
                                         (Height - 2 * indentHeight) + 0.5);
                                }

                                break;
                            }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    int oldValue = Value;

                    // TODO: Vertical exception is caused when trying to scroll passed the bottom
                    Value = Minimum + offsetValue;
                    Invalidate();

                    if (oldValue != Value)
                    {
                        OnScroll(e);
                        OnValueChanged(e);
                    }
                }
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            leftButtonDown = false;
            Capture = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            workingRectangle = Rectangle.Inflate(ClientRectangle, -indentWidth, -indentHeight);

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            // Step 1 - Configure tick style
            ConfigureTickStyle(graphics);

            // Step 2 - Draw the progress
            if (progressVisible)
            {
                DrawProgress(graphics);
            }

            // Step 3 - Draw the Tracker
            DrawButton(graphics);

            // Step 4 - Draw progress value
            if (progressValueVisible)
            {
                // Get Height of Text Area
                float textAreaSizeWidth = graphics.MeasureString(prefix + Maximum + suffix, textFont).Width;
                float textAreaSizeHeight = graphics.MeasureString(prefix + Maximum + suffix, textFont).Height;
                var stringValue = (float)(Value / (double)dividedValue);

                PointF newPointF = new PointF(buttonRectangle.X + buttonRectangle.Width / 2 - textAreaSizeWidth / 2, buttonRectangle.Y + buttonRectangle.Height / 2 - textAreaSizeHeight / 2);
                graphics.DrawString(prefix + stringValue.ToString("0") + suffix, textFont, new SolidBrush(buttonTextColor), newPointF);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
        }

        protected override void OnScroll(EventArgs e)
        {
            base.OnScroll(e);
            Invalidate();
        }

        protected override void OnValueChanged(EventArgs e)
        {
            Invalidate();
        }

        /// <summary>This member overrides <see cref="Control.ProcessCmdKey">Control.ProcessCmdKey</see>.</summary>
        /// <param name="msg">The msg.</param>
        /// <param name="keyData">The key Data.</param>
        /// <returns>The <see cref="bool" />.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var result = true;

            // Specified WM_KEYDOWN enumeration value.
            const int WM_KEYDOWN = 0x0100;

            // Specified WM_SYSKEYDOWN enumeration value.
            const int WM_SYSKEYDOWN = 0x0104;

            if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.Left:
                    case Keys.Down:
                        {
                            Decrement(SmallChange);
                            break;
                        }

                    case Keys.Right:
                    case Keys.Up:
                        {
                            Increment(SmallChange);
                            break;
                        }

                    case Keys.PageUp:
                        {
                            Increment(LargeChange);
                            break;
                        }

                    case Keys.PageDown:
                        {
                            Decrement(LargeChange);
                            break;
                        }

                    case Keys.Home:
                        {
                            Value = Maximum;
                            break;
                        }

                    case Keys.End:
                        {
                            Value = Minimum;
                            break;
                        }

                    default:
                        {
                            result = base.ProcessCmdKey(ref msg, keyData);
                            break;
                        }
                }
            }

            return result;
        }

        /// <summary>Configures the tick style.</summary>
        /// <param name="graphics">Graphics input.</param>
        private void ConfigureTickStyle(Graphics graphics)
        {
            int currentTrackerPos;
            Point trackBarLocation;
            Size trackBarSize;
            Point trackerLocation;
            Size trackerSize;

            // Draw tick by orientation
            if (trackBarType == Orientation.Horizontal)
            {
                // Start location
                currentUsedPos = indentHeight;

                // Draw value tick
                if (valueTicksVisible)
                {
                    HorizontalStyle(graphics, workingRectangle, false);
                }

                // Draw line tick
                if (lineTicksVisible)
                {
                    HorizontalStyle(graphics, workingRectangle, true);
                }

                // Setup track bar
                if (TickStyle == TickStyle.None)
                {
                    trackBarLocation = new Point(0, indentHeight);
                    Size = new Size(ClientRectangle.Width, indentHeight);
                }
                else
                {
                    trackBarLocation = new Point(0, indentHeight + textAreaSize.Height + tickHeight + barTickSpacing);
                }

                trackBarSize = new Size(workingRectangle.Width, trackLineThickness);
                trackBarRectangle = new Rectangle(trackBarLocation, trackBarSize);

                // Get tracker position
                currentTrackerPos = RetrieveTrackerPosition(workingRectangle);

                // Remember this for drawing the Tracker later
                trackerLocation = new Point(currentTrackerPos, currentUsedPos);
                trackerSize = new Size(buttonSize.Width, buttonSize.Height);
                trackerRectangle = new Rectangle(trackerLocation, trackerSize);

                // Draws track bar
                DrawBar(graphics, trackBarRectangle);

                // Update current position
                currentUsedPos += buttonSize.Height;

                // Draw value tick
                if (valueTicksVisible)
                {
                    HorizontalStyle(graphics, workingRectangle, false);
                }

                // Draw line tick
                if (lineTicksVisible)
                {
                    HorizontalStyle(graphics, workingRectangle, true);
                }
            }
            else
            {
                // Start location
                currentUsedPos = indentWidth;

                // Draw value tick
                if (valueTicksVisible)
                {
                    VerticalStyle(graphics, workingRectangle, false);
                }

                // Draw line tick
                if (lineTicksVisible)
                {
                    VerticalStyle(graphics, workingRectangle, true);
                }

                // Setup track bar
                if (TickStyle == TickStyle.None)
                {
                    trackBarLocation = new Point(indentWidth, indentHeight);
                    Size = new Size(indentWidth, ClientRectangle.Height);
                }
                else
                {
                    trackBarLocation = new Point(currentUsedPos + buttonSize.Height / 2 - trackLineThickness / 2, indentHeight + buttonSize.Height / 2);
                }

                trackBarSize = new Size(trackLineThickness, ClientRectangle.Height - indentHeight);
                trackBarRectangle = new Rectangle(trackBarLocation, trackBarSize);

                // Get tracker position
                currentTrackerPos = RetrieveTrackerPosition(workingRectangle);

                // Remember this for drawing the Tracker later
                trackerLocation = new Point(currentUsedPos, workingRectangle.Bottom - currentTrackerPos - buttonSize.Height);
                trackerSize = new Size(buttonSize.Width, buttonSize.Height);
                trackerRectangle = new Rectangle(trackerLocation, trackerSize);

                // Draw the track bar
                DrawBar(graphics, trackBarRectangle);

                // Update current position
                currentUsedPos += buttonSize.Height;

                // Draw value tick
                if (valueTicksVisible)
                {
                    VerticalStyle(graphics, workingRectangle, false);
                }

                // Draw line tick
                if (lineTicksVisible)
                {
                    VerticalStyle(graphics, workingRectangle, true);
                }
            }
        }

        /// <summary>Draws the bar.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <param name="barRectangle">Bar rectangle.</param>
        private void DrawBar(Graphics graphics, Rectangle barRectangle)
        {
            // Define TrackBar line properties
            Point trackLocation;
            Size trackSize;

            // Determine track size by orientation
            if (Orientation == Orientation.Horizontal)
            {
                trackLocation = new Point(indentWidth + barRectangle.Left, barRectangle.Top);
                trackSize = new Size(barRectangle.Left + barRectangle.Width - 1, barRectangle.Height);

                // Gradient track bar positions
                startPoint = new Point(ClientRectangle.Width, 0);
                endPoint = new Point(ClientRectangle.Width, ClientRectangle.Width);
            }
            else
            {


                trackLocation = new Point(indentWidth, barRectangle.Top);


                trackSize = new Size(barRectangle.Width, barRectangle.Height);

                // Gradient track bar positions
                startPoint = new Point(ClientRectangle.Width, 0);
                endPoint = new Point(ClientRectangle.Width, ClientRectangle.Height);
            }

            // New track bar line
            trackBarRectangle = new Rectangle(trackLocation, trackSize);

            // Get track bar shape
            trackBarPath = GDI.GetBorderShape(trackBarRectangle, borderShape, borderRounding);

            // Get gradient track bar brush
            LinearGradientBrush gradientTrackBarBrush = GDI.CreateGradientBrush(backgroundColor, gradientBackgroundPosition, GradientBackgroundAngle, startPoint, endPoint);

            // Fill trackBar background
            graphics.FillPath(gradientTrackBarBrush, trackBarPath);

            // Track bar line border
            if (borderVisible)
            {
                GDI.DrawBorder(graphics, trackBarPath, borderThickness, borderColor);
            }
        }

        /// <summary>Draws the button.</summary>
        /// <param name="graphics">Graphics input.</param>
        private void DrawButton(Graphics graphics)
        {
            Point buttonLocation = new Point();
            graphics.ResetClip();

            // Setup button colors
            var controlCheckTemp = Enabled ? buttonColor : controlDisabledColor;

            // Determine button location by orientation
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        buttonLocation = new Point(trackerRectangle.X, trackBarRectangle.Top + trackLineThickness / 2 - buttonSize.Height / 2);
                        break;
                    }

                case Orientation.Vertical:
                    {
                        buttonLocation = new Point(trackBarRectangle.Left + trackLineThickness / 2 - buttonSize.Width / 2, trackerRectangle.Y);
                        break;
                    }
            }

            buttonRectangle = new Rectangle(buttonLocation, buttonSize);

            if (buttonVisible)
            {
                // Button gradient positions
                startButtonPoint = new Point(buttonRectangle.Width, 0);
                endButtonPoint = new Point(buttonRectangle.Width, buttonRectangle.Height);

                // Get button shape
                buttonPath = GDI.GetBorderShape(buttonRectangle, borderShape, borderRounding);

                // Get gradient button brush
                gradientBrush = GDI.CreateGradientBrush(controlCheckTemp, gradientPosition, gradientButtonAngle, startButtonPoint, endButtonPoint);

                // Draw button background
                graphics.FillPath(gradientBrush, buttonPath);

                // Draw button border
                GDI.DrawBorderType(graphics, controlState, buttonPath, borderThickness, borderColor, borderHoverColor, borderVisible);
            }
        }

        /// <summary>Draws the TrackBar progress.</summary>
        /// <param name="graphics">Graphics input.</param>
        private void DrawProgress(Graphics graphics)
        {
            // Rectangle workingRect = Rectangle.Inflate(ClientRectangle, -indentWidth, -indentHeight);
            GraphicsPath progressPath = new GraphicsPath();
            Rectangle progressRectangle = new Rectangle();
            Point progressLocation;
            Size progressSize;

            var barProgress = 0;

            // Progress setup
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        // Draws the progress to the middle of the button
                        barProgress = buttonRectangle.X + buttonRectangle.Width / 2;

                        progressLocation = new Point(0, 0);
                        progressSize = new Size(barProgress, Height);

                        if (Value == Minimum && progressFilling)
                        {
                            progressLocation = new Point(barProgress, Height);
                        }

                        if (Value == Maximum && progressFilling)
                        {
                            progressSize = new Size(barProgress + textAreaSize.Width, Height);
                        }

                        progressRectangle = new Rectangle(progressLocation, progressSize);
                        progressPath = GDI.GetBorderShape(progressRectangle, borderShape, borderRounding);
                    }

                    break;
                case Orientation.Vertical:
                    {
                        // Draws the progress to the middle of the button
                        barProgress = buttonRectangle.Y + buttonRectangle.Height / 2;

                        progressLocation = new Point(0, barProgress);

                        if (Value == Minimum && progressFilling)
                        {
                            progressLocation = new Point(0, barProgress + textAreaSize.Height);
                        }

                        if (Value == Maximum && progressFilling)
                        {
                            progressLocation = new Point(0, barProgress + textAreaSize.Height - 26);
                        }

                        progressSize = new Size(Width, Height + textAreaSize.Height);
                        progressRectangle = new Rectangle(progressLocation, progressSize);
                        progressPath = GDI.GetBorderShape(progressRectangle, borderShape, borderRounding);
                    }

                    break;
            }

            // Clip to the track bar
            graphics.SetClip(trackBarPath);

            startProgressPoint = new Point(ClientRectangle.Width, 0);
            endProgressPoint = new Point(ClientRectangle.Width, ClientRectangle.Width);

            // Progress
            if (barProgress > 1)
            {
                // Draw progress
                if (progressColorStyle == BrushType.Gradient)
                {
                    // Draw gradient progress
                    gradientProgressBrush = GDI.CreateGradientBrush(progressColor, gradientProgressPosition, gradientProgressAngle, startProgressPoint, endProgressPoint);
                    graphics.FillPath(gradientProgressBrush, progressPath);
                }
                else
                {
                    // Solid color progress
                    graphics.FillPath(new SolidBrush(progressColor[0]), progressPath);
                }

                // Draw hatch
                if (hatchVisible)
                {
                    HatchBrush hatchBrush = new HatchBrush(hatchStyle, hatchForeColor, hatchBackColor);
                    using (TextureBrush textureBrush = GDI.DrawTextureUsingHatch(hatchBrush))
                    {
                        textureBrush.ScaleTransform(hatchSize, hatchSize);
                        graphics.FillPath(textureBrush, progressPath);
                        graphics.ResetClip();
                    }
                }

                graphics.ResetClip();
            }
        }

        /// <summary>Gets the size of the value barRectangle.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <returns>Returns text size.</returns>
        private Size GetTextSize(Graphics graphics)
        {
            int width = Convert.ToInt32(graphics.MeasureString(Maximum.ToString(), textFont).Width);
            int height = Convert.ToInt32(graphics.MeasureString(Maximum.ToString(), textFont).Height);
            Size textSize = new Size(width, height);

            return textSize;
        }

        /// <summary>Configures horizontal style.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <param name="workingRectangle">Working barRectangle.</param>
        /// <param name="line">Line</param>
        private void HorizontalStyle(Graphics graphics, Rectangle workingRectangle, bool line)
        {
            Rectangle tickRectangle;
            currentUsedPos = indentHeight;
            Point location;
            Size size;
            textAreaSize = GetTextSize(graphics);

            if (line)
            {
                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Retrieve tick barRectangle
                    location = new Point(workingRectangle.Left, currentUsedPos + textAreaSize.Height);
                    size = new Size(workingRectangle.Width, tickHeight);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Retrieve tick barRectangle
                    location = new Point(workingRectangle.Left, trackBarRectangle.Bottom + barTickSpacing);
                    size = new Size(workingRectangle.Width, tickHeight);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }
            }
            else
            {
                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Retrieve text barRectangle
                    location = new Point(workingRectangle.Left, currentUsedPos);
                    size = new Size(workingRectangle.Width, textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Retrieve text barRectangle
                    location = new Point(workingRectangle.Left, trackBarRectangle.Y + trackBarRectangle.Height + borderRounding + barTickSpacing + tickHeight + currentUsedPos);
                    size = new Size(workingRectangle.Width, tickHeight);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }
            }
        }

        /// <summary>Retrieves the trackers current position.</summary>
        /// <param name="workingRect">Working Rectangle.</param>
        /// <returns>Tracker position.</returns>
        private int RetrieveTrackerPosition(Rectangle workingRect)
        {
            var currentTrackerPos = 0;

            if (Orientation == Orientation.Horizontal)
            {
                if (Maximum == Minimum)
                {
                    currentTrackerPos = workingRect.Left;
                }
                else
                {
                    currentTrackerPos = (workingRect.Width - buttonSize.Width) * (Value - Minimum) / (Maximum - Minimum) + workingRect.Left;
                }
            }
            else if (Orientation == Orientation.Vertical)
            {
                if (Maximum == Minimum)
                {
                    currentTrackerPos = workingRect.Top;
                }
                else
                {
                    currentTrackerPos = (workingRect.Height - buttonSize.Height) * (Value - Minimum) / (Maximum - Minimum);
                }
            }

            return currentTrackerPos;
        }

        /// <summary>Configures vertical style.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <param name="workingRectangle">Working barRectangle.</param>
        /// <param name="line">Line.</param>
        private void VerticalStyle(Graphics graphics, Rectangle workingRectangle, bool line)
        {
            Rectangle tickRectangle;
            currentUsedPos = indentWidth;
            Point location;
            Size size;
            textAreaSize = GetTextSize(graphics);

            if (line)
            {
                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Retrieve tick barRectangle
                    location = new Point(currentUsedPos + textAreaSize.Width, textAreaSize.Height / 2);
                    size = new Size(tickHeight, trackBarRectangle.Height - textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Retrieve tick barRectangle
                    location = new Point(trackBarRectangle.X + trackBarRectangle.Width + barTickSpacing, workingRectangle.Top + textAreaSize.Height / 2);
                    size = new Size(tickHeight, trackBarRectangle.Height - textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }
            }
            else
            {
                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Retrieve text barRectangle
                    location = new Point(currentUsedPos, textAreaSize.Height / 2);
                    size = new Size(textAreaSize.Width, trackBarRectangle.Height - textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Retrieve text barRectangle
                    location = new Point(trackBarRectangle.X + trackBarRectangle.Width + barTickSpacing + tickHeight, workingRectangle.Top + textAreaSize.Height / 2);
                    size = new Size(textAreaSize.Width, workingRectangle.Height - textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }
            }
        }

        #endregion
    }
}