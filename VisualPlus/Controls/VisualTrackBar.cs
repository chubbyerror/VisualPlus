namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TrackBar))]
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    [Description("The Visual TrackBar")]
    [Designer(ControlManager.FilterProperties.VisualTrackBar)]
    public sealed class VisualTrackBar : TrackBar
    {
        #region Variables

        protected Orientation orientation = Orientation.Horizontal;

        #endregion

        #region Variables

        private readonly MouseState mouseState;

        private Gradient backgroundGradient = new Gradient();
        private int barThickness = 10;
        private int barTickSpacing = 8;
        private bool buttonAutoSize = true;
        private Border buttonBorder = new Border();
        private Gradient buttonGradient = new Gradient();
        private GraphicsPath buttonPath = new GraphicsPath();
        private Rectangle buttonRectangle;
        private Size buttonSize = new Size(27, 20);
        private Color buttonTextColor = Settings.DefaultValue.Font.ForeColor;
        private bool buttonVisible = true;
        private int currentUsedPos;
        private ValueDivisor dividedValue = ValueDivisor.By1;
        private int fillingValue = 25;
        private Color foreColor = Settings.DefaultValue.Font.ForeColor;
        private Point[] gradientPoints;
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
        private bool progressFilling;
        private Gradient progressGradient = new Gradient();
        private bool progressValueVisible;
        private bool progressVisible = Settings.DefaultValue.TextVisible;
        private StyleManager styleManager = new StyleManager();
        private string suffix;
        private Size textAreaSize;
        private Color textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
        private Font textFont = Settings.DefaultValue.DefaultFont;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color tickColor = Settings.DefaultValue.Control.Line;
        private int tickHeight = 4;
        private Border trackBarBorder = new Border();
        private Gradient trackBarDisabledGradient = new Gradient();
        private GraphicsPath trackBarPath;
        private Rectangle trackBarRectangle;
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

            mouseState = new MouseState(this);
            UpdateStyles();
            Font = Settings.DefaultValue.DefaultFont;
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            UpdateStyles();
            AutoSize = false;
            Size = new Size(200, 50);
            MinimumSize = new Size(0, 0);

            DefaultGradient();
            ConfigureStyleManager();
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Background
        {
            get
            {
                return backgroundGradient;
            }

            set
            {
                backgroundGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
        public int BarThickness
        {
            get
            {
                return barThickness;
            }

            set
            {
                barThickness = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
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

        [DefaultValue(true)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Common.AutoSize)]
        public bool ButtonAutoSize
        {
            get
            {
                return buttonAutoSize;
            }

            set
            {
                buttonAutoSize = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border ButtonBorder
        {
            get
            {
                return buttonBorder;
            }

            set
            {
                buttonBorder = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient ButtonGradient
        {
            get
            {
                return buttonGradient;
            }

            set
            {
                buttonGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
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
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Common.Visible)]
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Disabled
        {
            get
            {
                return trackBarDisabledGradient;
            }

            set
            {
                trackBarDisabledGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Data)]
        [Description("Experiemental: Filling Value.")]
        public int FillingValue
        {
            get
            {
                return fillingValue;
            }

            set
            {
                fillingValue = value;
                Invalidate();
            }
        }

        public new Font Font
        {
            get
            {
                return textFont;
            }

            set
            {
                base.Font = value;
                textFont = value;
                Invalidate();
            }
        }

        public new Color ForeColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                base.ForeColor = value;
                foreColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Common.Size)]
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
        [Description(Localize.Description.Common.Type)]
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
        [Description(Localize.Description.Common.Visible)]
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
        [Description(Localize.Description.Common.Size)]
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
        [Description(Localize.Description.Common.Size)]
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
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.Category.Appearance)]
        public MouseStates MouseState
        {
            get
            {
                return mouseState.State;
            }

            set
            {
                mouseState.State = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Orientation)]
        public new Orientation Orientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
                Size = GDI.FlipOrientationSize(orientation, Size);
                Invalidate();
            }
        }

        [Category(Localize.Category.Data)]
        [Description(Localize.Description.Common.Visible)]
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Progress
        {
            get
            {
                return progressGradient;
            }

            set
            {
                progressGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Visible)]
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
        [Description(Localize.Description.Common.Visible)]
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
        [Description(Localize.Description.Common.Visible)]
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

        [TypeConverter(typeof(StyleManagerConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public StyleManager StyleManager
        {
            get
            {
                return styleManager;
            }

            set
            {
                styleManager = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Data)]
        [Description(Localize.Description.Common.Visible)]
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
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Strings.TextRenderingHint)]
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
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Common.Size)]
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

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border TrackBar
        {
            get
            {
                return trackBarBorder;
            }

            set
            {
                trackBarBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Common.ValueDivisor)]
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
        [Description(Localize.Description.Common.Visible)]
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

        /// <summary>Get's the formatted progress value.</summary>
        /// <returns>Formatted progress value.</returns>
        public string GetFormattedProgressValue()
        {
            var value = (float)(Value / (double)dividedValue);
            string formattedString = $"{Prefix}{value}{Suffix}";

            return formattedString;
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

            // TODO: Improve location accuracy
            if (trackerRectangle.Contains(currentPoint))
            {
                if (!leftButtonDown)
                {
                    leftButtonDown = true;
                    Capture = true;
                    switch (orientation)
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
                switch (orientation)
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
                                offsetValue = (int)(((((currentPoint.X - indentWidth - buttonSize.Width) / 2) * (Maximum - Minimum)) / (Width - (2 * indentWidth) - buttonSize.Width)) + 0.5);
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
                                offsetValue = (int)(((((Height - currentPoint.Y - indentHeight - buttonSize.Width) / 2) * (Maximum - Minimum)) / (Height - (2 * indentHeight) - buttonSize.Width)) + 0.5);
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
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = orientation == Orientation.Vertical ? Cursors.SizeNS : Cursors.SizeWE;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            OnLeave(e);
            mouseState.State = MouseStates.Normal;
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
                    // TODO: Improve location accuracy
                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            {
                                if ((currentPoint.X + buttonSize.Width) - mouseStartPos >= Width - indentWidth)
                                {
                                    offsetValue = Maximum - Minimum;
                                }
                                else if (currentPoint.X - mouseStartPos <= indentWidth)
                                {
                                    offsetValue = 0;
                                }
                                else
                                {
                                    offsetValue = (int)((((currentPoint.X - mouseStartPos - indentWidth) * (Maximum - Minimum)) / (Width - (2 * indentWidth) - buttonSize.Width)) + 0.5);
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
                                    offsetValue = (int)(((((((Height - currentPoint.Y) + buttonSize.Height) / 2) - mouseStartPos - indentHeight) * (Maximum - Minimum)) / (Height - (2 * indentHeight))) + 0.5);
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

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            // Step 1 - Configure tick style
            ConfigureTickStyle(graphics);

            // Step 2 - Draw the progress
            if (progressVisible)
            {
                DrawProgress(graphics);
            }

            Size formattedProgressValue = GDI.GetTextSize(graphics, Maximum.ToString(), textFont);

            // Step 3 - Draw the Tracker
            DrawButton(graphics, formattedProgressValue);

            // Step 4 - Draw progress value
            if (progressValueVisible)
            {
                string value = GetFormattedProgressValue();

                // Position
                Point progressValueLocation = new Point();
                if (buttonVisible)
                {
                    progressValueLocation = new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - (formattedProgressValue.Width / 2), (buttonRectangle.Y + (buttonRectangle.Height / 2)) - (formattedProgressValue.Height / 2));
                }
                else
                {
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            {
                                progressValueLocation = new Point(trackerRectangle.X, (trackBarRectangle.Y + (trackBarRectangle.Height / 2)) - (formattedProgressValue.Height / 2));
                                break;
                            }

                        case Orientation.Vertical:
                            {
                                progressValueLocation = new Point(trackBarRectangle.X, trackerRectangle.Y);
                                break;
                            }
                    }
                }

                // Draw the formatted progress value
                graphics.DrawString(value, textFont, new SolidBrush(buttonTextColor), progressValueLocation);
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

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
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

        private static Color hatchBackColor = Settings.DefaultValue.Progress.Hatch;

        private static Rectangle trackerRectangle = Rectangle.Empty;

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.BorderStyle;
                IFont fontStyle = styleManager.VisualStylesManager.FontStyle;

                trackBarBorder.Color = borderStyle.Color;
                trackBarBorder.HoverColor = borderStyle.HoverColor;
                trackBarBorder.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                trackBarBorder.Rounding = styleManager.VisualStylesManager.BorderRounding;
                trackBarBorder.Type = styleManager.VisualStylesManager.BorderType;
                trackBarBorder.Thickness = styleManager.VisualStylesManager.BorderThickness;
                trackBarBorder.Visible = styleManager.VisualStylesManager.BorderVisible;

                buttonBorder.Color = borderStyle.Color;
                buttonBorder.HoverColor = borderStyle.HoverColor;
                buttonBorder.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                buttonBorder.Rounding = styleManager.VisualStylesManager.BorderRounding;
                buttonBorder.Type = styleManager.VisualStylesManager.BorderType;
                buttonBorder.Thickness = styleManager.VisualStylesManager.BorderThickness;
                buttonBorder.Visible = styleManager.VisualStylesManager.BorderVisible;

                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);

                foreColor = fontStyle.ForeColor;
                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
            }
            else
            {
                // Load default settings
                trackBarBorder = new Border();
                buttonBorder = new Border();

                Font = Settings.DefaultValue.DefaultFont;
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textRendererHint = Settings.DefaultValue.TextRenderingHint;
            }
        }

        /// <summary>Configures the tick style.</summary>
        /// <param name="graphics">Graphics input.</param>
        private void ConfigureTickStyle(Graphics graphics)
        {
            int currentTrackerPos;
            Point trackBarLocation = new Point();
            Size trackBarSize;
            Point trackerLocation;
            Size trackerSize;

            // Draw tick by orientation
            if (orientation == Orientation.Horizontal)
            {
                gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Width } };

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

                // Setups the location & sizing
                switch (TickStyle)
                {
                    case TickStyle.TopLeft:
                        {
                            if (buttonVisible)
                            {
                                trackBarLocation = new Point(0, indentHeight + textAreaSize.Height + tickHeight + barTickSpacing);
                                Size = new Size(ClientRectangle.Width, indentHeight + textAreaSize.Height + tickHeight + barTickSpacing + barThickness + (buttonSize.Height / 2));
                            }
                            else
                            {
                                trackBarLocation = new Point(0, indentHeight + textAreaSize.Height + tickHeight + barTickSpacing);
                                Size = new Size(ClientRectangle.Width, indentHeight + textAreaSize.Height + tickHeight + barTickSpacing + barThickness);
                            }

                            break;
                        }

                    case TickStyle.BottomRight:
                        {
                            if (buttonVisible)
                            {
                                trackBarLocation = new Point(0, indentHeight + (buttonSize.Height / 2));
                                Size = new Size(ClientRectangle.Width, indentHeight + barThickness + barTickSpacing + tickHeight + textAreaSize.Height + (textAreaSize.Height / 2));
                            }
                            else
                            {
                                trackBarLocation = new Point(0, indentHeight);
                                Size = new Size(ClientRectangle.Width, indentHeight + barThickness + barTickSpacing + tickHeight + textAreaSize.Height);
                            }

                            break;
                        }

                    case TickStyle.None:
                        {
                            if (buttonVisible)
                            {
                                trackBarLocation = new Point(0, indentHeight + (buttonSize.Height / 2));
                                Size = new Size(ClientRectangle.Width, indentHeight + barThickness + buttonSize.Height);
                            }
                            else
                            {
                                trackBarLocation = new Point(0, indentHeight);
                                Size = new Size(ClientRectangle.Width, indentHeight + barThickness);
                            }

                            break;
                        }

                    case TickStyle.Both:
                        {
                            int totalHeight = indentHeight + textAreaSize.Height + tickHeight + barTickSpacing + barThickness + barTickSpacing + tickHeight + textAreaSize.Height + (textAreaSize.Height / 2);

                            trackBarLocation = new Point(0, indentHeight + textAreaSize.Height + tickHeight + barTickSpacing);
                            Size = new Size(ClientRectangle.Width, totalHeight);

                            break;
                        }
                }

                trackBarSize = new Size(workingRectangle.Width, barThickness);
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
                gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };

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

                // Setups the location & sizing
                switch (TickStyle)
                {
                    case TickStyle.TopLeft:
                        {
                            if (buttonVisible)
                            {
                                trackBarLocation = new Point(indentWidth + textAreaSize.Width + tickHeight + barTickSpacing, 0);
                                Size = new Size(indentWidth + textAreaSize.Width + tickHeight + barTickSpacing + barThickness + (buttonSize.Width / 2), ClientRectangle.Height);
                            }
                            else
                            {
                                trackBarLocation = new Point(indentWidth + textAreaSize.Width + tickHeight + barTickSpacing, 0);
                                Size = new Size(indentWidth + textAreaSize.Width + tickHeight + barTickSpacing + barThickness, ClientRectangle.Height);
                            }

                            break;
                        }

                    case TickStyle.BottomRight:
                        {
                            if (buttonVisible)
                            {
                                trackBarLocation = new Point(indentWidth + (buttonSize.Width / 2), 0);
                                Size = new Size(indentWidth + barThickness + barTickSpacing + tickHeight + textAreaSize.Width + (buttonSize.Width / 2), ClientRectangle.Height);
                            }
                            else
                            {
                                trackBarLocation = new Point(0, indentWidth);
                                Size = new Size(indentWidth + barThickness + barTickSpacing + tickHeight + textAreaSize.Width, ClientRectangle.Height);
                            }

                            break;
                        }

                    case TickStyle.None:
                        {
                            if (buttonVisible)
                            {
                                trackBarLocation = new Point(indentWidth + (buttonSize.Width / 2), indentHeight);
                                Size = new Size(indentWidth + barThickness + buttonSize.Width, ClientRectangle.Height);
                            }
                            else
                            {
                                trackBarLocation = new Point(indentWidth, indentHeight);
                                Size = new Size(indentWidth + barThickness, ClientRectangle.Height);
                            }

                            break;
                        }

                    case TickStyle.Both:
                        {
                            int totalWidth = indentWidth + textAreaSize.Width + tickHeight + barTickSpacing + barThickness + barTickSpacing + tickHeight + textAreaSize.Width;

                            trackBarLocation = new Point(indentWidth + textAreaSize.Width + tickHeight + barTickSpacing, 0);
                            Size = new Size(totalWidth, ClientRectangle.Height);

                            break;
                        }
                }

                trackBarSize = new Size(barThickness, ClientRectangle.Height - indentHeight);
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

        private void DefaultGradient()
        {
            backgroundGradient.Angle = Settings.DefaultValue.Progress.Background.Angle;
            backgroundGradient.Colors = Settings.DefaultValue.Progress.Background.Colors;
            backgroundGradient.Positions = Settings.DefaultValue.Progress.Background.Positions;

            trackBarDisabledGradient.Angle = Settings.DefaultValue.Progress.Background.Angle;
            trackBarDisabledGradient.Colors = Settings.DefaultValue.Progress.Background.Colors;
            trackBarDisabledGradient.Positions = Settings.DefaultValue.Progress.Background.Positions;

            progressGradient.Angle = Settings.DefaultValue.Progress.Progress.Angle;
            progressGradient.Colors = Settings.DefaultValue.Progress.Progress.Colors;
            progressGradient.Positions = Settings.DefaultValue.Progress.Progress.Positions;

            buttonGradient.Angle = Settings.DefaultValue.Control.ControlEnabled.Angle;
            buttonGradient.Colors = Settings.DefaultValue.Control.ControlEnabled.Colors;
            buttonGradient.Positions = Settings.DefaultValue.Control.ControlEnabled.Positions;
        }

        /// <summary>Draws the bar.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <param name="barRectangle">Bar rectangle.</param>
        private void DrawBar(Graphics graphics, Rectangle barRectangle)
        {
            Point trackLocation;
            Size trackSize;

            if (Orientation == Orientation.Horizontal)
            {
                trackLocation = new Point(indentWidth + barRectangle.Left, indentHeight + barRectangle.Top);
                trackSize = new Size(barRectangle.Width, barRectangle.Height);
            }
            else
            {
                trackLocation = new Point(indentWidth + barRectangle.Left, indentHeight + barRectangle.Top);
                trackSize = new Size(barRectangle.Width, barRectangle.Height);
            }

            trackBarRectangle = new Rectangle(trackLocation, trackSize);
            trackBarPath = GDI.GetBorderShape(trackBarRectangle, trackBarBorder.Type, trackBarBorder.Rounding);

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillPath(gradientBrush, trackBarPath);

            if (trackBarBorder.Visible)
            {
                GDI.DrawBorderType(graphics, mouseState.State, trackBarPath, trackBarBorder.Thickness, trackBarBorder.Color, trackBarBorder.HoverColor, trackBarBorder.HoverVisible);
            }
        }

        /// <summary>Draws the button.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <param name="progressValue">The progress Value.</param>
        private void DrawButton(Graphics graphics, Size progressValue)
        {
            Point buttonLocation = new Point();
            graphics.ResetClip();

            // Setup button colors
            Gradient controlCheckTemp = Enabled ? buttonGradient : trackBarDisabledGradient;

            // Determine button location by orientation
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        buttonLocation = new Point(trackerRectangle.X, (trackBarRectangle.Top + (barThickness / 2)) - (buttonSize.Height / 2));

                        if (buttonAutoSize)
                        {
                            buttonSize = new Size(progressValue.Width, buttonSize.Height);
                        }
                        else
                        {
                            buttonSize = new Size(buttonSize.Width, buttonSize.Height);
                        }

                        break;
                    }

                case Orientation.Vertical:
                    {
                        buttonLocation = new Point((trackBarRectangle.Left + (barThickness / 2)) - (buttonSize.Width / 2), trackerRectangle.Y);

                        if (buttonAutoSize)
                        {
                            buttonSize = new Size(buttonSize.Width, progressValue.Height);
                        }
                        else
                        {
                            buttonSize = new Size(buttonSize.Width, buttonSize.Height);
                        }

                        break;
                    }
            }

            buttonRectangle = new Rectangle(buttonLocation, buttonSize);

            if (buttonVisible)
            {
                LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(controlCheckTemp.Colors, gradientPoints, controlCheckTemp.Angle, controlCheckTemp.Positions);
                buttonPath = GDI.GetBorderShape(buttonRectangle, buttonBorder.Type, buttonBorder.Rounding);
                graphics.FillPath(gradientBrush, buttonPath);

                if (buttonBorder.Visible)
                {
                    GDI.DrawBorderType(graphics, mouseState.State, buttonPath, buttonBorder.Thickness, buttonBorder.Color, buttonBorder.HoverColor, buttonBorder.HoverVisible);
                }
            }
        }

        /// <summary>Draws the TrackBar progress.</summary>
        /// <param name="graphics">Graphics input.</param>
        private void DrawProgress(Graphics graphics)
        {
            GraphicsPath progressPath = new GraphicsPath();
            Rectangle progressRectangle;
            Point progressLocation;
            Size progressSize;

            var barProgress = 0;

            // Progress setup
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        // Draws the progress to the middle of the button
                        barProgress = buttonRectangle.X + (buttonRectangle.Width / 2);

                        progressLocation = new Point(0, 0);
                        progressSize = new Size(barProgress, Height);

                        if ((Value == Minimum) && progressFilling)
                        {
                            progressLocation = new Point(barProgress, Height);
                        }

                        if ((Value == Maximum) && progressFilling)
                        {
                            progressSize = new Size(barProgress + fillingValue, Height);
                        }

                        progressRectangle = new Rectangle(progressLocation, progressSize);
                        progressPath = GDI.GetBorderShape(progressRectangle, trackBarBorder.Type, trackBarBorder.Rounding);
                    }

                    break;
                case Orientation.Vertical:
                    {
                        // Draws the progress to the middle of the button
                        barProgress = buttonRectangle.Y + (buttonRectangle.Height / 2);

                        progressLocation = new Point(0, barProgress);

                        if ((Value == Minimum) && progressFilling)
                        {
                            progressLocation = new Point(0, barProgress + fillingValue);
                        }

                        if ((Value == Maximum) && progressFilling)
                        {
                            progressLocation = new Point(0, barProgress - fillingValue);
                        }

                        progressSize = new Size(Width, Height + textAreaSize.Height);
                        progressRectangle = new Rectangle(progressLocation, progressSize);
                        progressPath = GDI.GetBorderShape(progressRectangle, trackBarBorder.Type, trackBarBorder.Rounding);
                    }

                    break;
            }

            graphics.SetClip(trackBarPath);

            if (barProgress > 1)
            {
                LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);
                graphics.FillPath(gradientBrush, progressPath);

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

        private void HorizontalStyle(Graphics graphics, Rectangle rectangle, bool line)
        {
            Rectangle tickRectangle;
            currentUsedPos = indentHeight;
            Point location;
            Size size;
            textAreaSize = GDI.GetTextSize(graphics, Maximum.ToString(), textFont);

            if (line)
            {
                if ((TickStyle == TickStyle.TopLeft) || (TickStyle == TickStyle.Both))
                {
                    // Retrieve tick barRectangle
                    location = new Point(rectangle.Left, currentUsedPos + textAreaSize.Height);
                    size = new Size(rectangle.Width, tickHeight);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, orientation);
                }

                if ((TickStyle == TickStyle.BottomRight) || (TickStyle == TickStyle.Both))
                {
                    // Retrieve tick barRectangle
                    location = new Point(rectangle.Left, trackBarRectangle.Bottom + barTickSpacing);
                    size = new Size(rectangle.Width, tickHeight);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, orientation);
                }
            }
            else
            {
                if ((TickStyle == TickStyle.TopLeft) || (TickStyle == TickStyle.Both))
                {
                    // Retrieve text barRectangle
                    location = new Point(rectangle.Left, currentUsedPos);
                    size = new Size(rectangle.Width, textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, orientation);
                }

                if ((TickStyle == TickStyle.BottomRight) || (TickStyle == TickStyle.Both))
                {
                    // Retrieve text barRectangle
                    location = new Point(rectangle.Left, trackBarRectangle.Y + trackBarRectangle.Height + trackBarBorder.Rounding + barTickSpacing + tickHeight + currentUsedPos);
                    size = new Size(rectangle.Width, tickHeight);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, orientation);
                }
            }
        }

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
                    currentTrackerPos = (((workingRect.Width - buttonSize.Width) * (Value - Minimum)) / (Maximum - Minimum)) + workingRect.Left;
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
                    currentTrackerPos = ((workingRect.Height - buttonSize.Height) * (Value - Minimum)) / (Maximum - Minimum);
                }
            }

            return currentTrackerPos;
        }

        private void VerticalStyle(Graphics graphics, Rectangle rectangle, bool line)
        {
            Rectangle tickRectangle;
            currentUsedPos = indentWidth;
            Point location;
            Size size;
            textAreaSize = GDI.GetTextSize(graphics, Maximum.ToString(), textFont);

            if (line)
            {
                if ((TickStyle == TickStyle.TopLeft) || (TickStyle == TickStyle.Both))
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
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, orientation);
                }

                if ((TickStyle == TickStyle.BottomRight) || (TickStyle == TickStyle.Both))
                {
                    // Retrieve tick barRectangle
                    location = new Point(trackBarRectangle.Right + barTickSpacing, rectangle.Top + (textAreaSize.Height / 2));
                    size = new Size(tickHeight, trackBarRectangle.Height - textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge tick barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next tick area
                    currentUsedPos += tickHeight;

                    // Draw tick line
                    GDI.DrawTickLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, tickColor, orientation);
                }
            }
            else
            {
                if ((TickStyle == TickStyle.TopLeft) || (TickStyle == TickStyle.Both))
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
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, orientation);
                }

                if ((TickStyle == TickStyle.BottomRight) || (TickStyle == TickStyle.Both))
                {
                    // Retrieve text barRectangle
                    location = new Point(trackBarRectangle.Right + barTickSpacing + tickHeight, rectangle.Top + (textAreaSize.Height / 2));
                    size = new Size(textAreaSize.Width, rectangle.Height - textAreaSize.Height);
                    tickRectangle = new Rectangle(location, size);

                    // Enlarge text barRectangle
                    // tickRectangle.Inflate(-buttonSize.Width / 2, 0);

                    // Move next text area
                    currentUsedPos += tickHeight;

                    // Draw text 
                    GDI.DrawTickTextLine(graphics, tickRectangle, TickFrequency, Minimum, Maximum, foreColor, textFont, orientation);
                }
            }
        }

        #endregion
    }
}