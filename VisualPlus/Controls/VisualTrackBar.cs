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
        private static Color progressColor1 = Settings.DefaultValue.Style.ProgressColor;
        private static int progressRotation;
        private static Orientation trackBarType = Orientation.Horizontal;
        private static RectangleF trackerRectangleF = RectangleF.Empty;
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Color buttonColor = Settings.DefaultValue.Style.ButtonNormalColor;
        private GraphicsPath buttonPath = new GraphicsPath();
        private Rectangle buttonRectangle;
        private Size buttonSize = new Size(27, 20);
        private Color buttonTextColor = Settings.DefaultValue.Style.ForeColor(0);
        private bool buttonValueVisible;
        private bool buttonVisible = true;
        private char charExtension;
        private Color controlDisabledColor = Settings.DefaultValue.Style.ControlDisabled;
        private ControlState controlState = ControlState.Normal;
        private ValueDivisor dividedValue = ValueDivisor.By1;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color hatchForeColor = Color.FromArgb(40, hatchBackColor);
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;
        private int indentHeight = 6;
        private int indentWidth = 6;
        private bool leftButtonDown;
        private float mouseStartPos = -1;
        private Color progressColor2 = ControlPaint.Light(progressColor1);
        private BrushType progressColorStyle = BrushType.Gradient;
        private bool progressValueVisible;
        private bool progressVisible = Settings.DefaultValue.TextVisible;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private Font textFont = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Regular);
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color tickColor = Settings.DefaultValue.Style.LineColor;
        private int tickHeight = 2;
        private bool tickVisible = Settings.DefaultValue.TextVisible;
        private Color trackLineColor = Settings.DefaultValue.Style.LineColor;
        private int trackLineThickness = 5;
        private bool valueTickVisible = Settings.DefaultValue.TextVisible;

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
            Size = new Size(180, 50);
            MinimumSize = new Size(25, 25);
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
        public Color ButtonColor
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ButtonValueVisible
        {
            get
            {
                return buttonValueVisible;
            }

            set
            {
                buttonValueVisible = value;
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public char CharExtension
        {
            get
            {
                return charExtension;
            }

            set
            {
                charExtension = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
        public Color ControlDisabledColor
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ProgressColor1
        {
            get
            {
                return progressColor1;
            }

            set
            {
                progressColor1 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ProgressColor2
        {
            get
            {
                return progressColor2;
            }

            set
            {
                progressColor2 = value;
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

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool TickVisible
        {
            get
            {
                return tickVisible;
            }

            set
            {
                tickVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TrackLineColor
        {
            get
            {
                return trackLineColor;
            }

            set
            {
                trackLineColor = value;
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
        public bool ValueTickVisible
        {
            get
            {
                return valueTickVisible;
            }

            set
            {
                valueTickVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var offsetValue = 0;
            PointF currentPoint = new PointF(e.X, e.Y);

            if (trackerRectangleF.Contains(currentPoint))
            {
                if (!leftButtonDown)
                {
                    leftButtonDown = true;
                    Capture = true;
                    switch (trackBarType)
                    {
                        case Orientation.Horizontal:
                            {
                                mouseStartPos = currentPoint.X - trackerRectangleF.X;
                                Invalidate();
                                break;
                            }

                        case Orientation.Vertical:
                            {
                                mouseStartPos = currentPoint.Y - trackerRectangleF.Y;
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
                            offsetValue =
                                (int)
                                ((currentPoint.X - indentWidth - buttonSize.Width) / 2 * (Maximum - Minimum) /
                                 (Width - 2 * indentWidth - buttonSize.Width) + 0.5);
                        }

                        break;

                    case Orientation.Vertical:
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
                            offsetValue =
                                (int)
                                ((Height - currentPoint.Y - indentHeight - buttonSize.Width) / 2 * (Maximum - Minimum) /
                                 (Height - 2 * indentHeight - buttonSize.Width) + 0.5);
                        }

                        break;
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

                        case Orientation.Vertical:
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

            RectangleF drawRect;
            Rectangle workingRect = Rectangle.Inflate(ClientRectangle, -indentWidth, -indentHeight);
            float textAreaSize;
            float currentUsedPos;

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            if (trackBarType == Orientation.Horizontal)
            {
                currentUsedPos = indentHeight;

                // Get Height of Text Area
                textAreaSize = e.Graphics.MeasureString(Maximum.ToString(), textFont).
                                 Height;

                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Draw text
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, textAreaSize);
                    drawRect.Inflate(-buttonSize.Width / 2, 0);
                    currentUsedPos += textAreaSize;

                    if (valueTickVisible)
                    {
                        GDI.DrawTickTextLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                    }
                }

                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Draw tick line.
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, tickHeight);
                    drawRect.Inflate(-buttonSize.Width / 2, 0);
                    currentUsedPos += tickHeight + 1;

                    if (tickVisible)
                    {
                        GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                    }
                }

                // Calculate the tracker's rectangle
                float currentTrackerPos;
                if (Maximum == Minimum)
                {
                    currentTrackerPos = workingRect.Left;
                }
                else
                {
                    currentTrackerPos = (workingRect.Width - buttonSize.Width) * (Value - Minimum) / (Maximum - Minimum) + workingRect.Left;
                }

                // Remember this for drawing the Tracker later
                trackerRectangleF = new RectangleF(currentTrackerPos, currentUsedPos, buttonSize.Width, buttonSize.Height);

                // trackerRectangleF.Inflate(0,-1);
                // Draw the Track Line
                drawRect = new RectangleF(workingRect.Left, currentUsedPos + buttonSize.Height / 2 - trackLineThickness / 2, workingRect.Width, trackLineThickness);

                // Draws the track line
                DrawTrackLine(e.Graphics, drawRect);

                Rectangle trackLineRectangle = Rectangle.Round(drawRect);

                // Track line border
                if (borderVisible)
                {
                    GDI.DrawBorder(graphics, GDI.GetBorderShape(trackLineRectangle, borderShape, 3), 1, borderColor);
                }

                currentUsedPos += buttonSize.Height;

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw tick line.
                    currentUsedPos += 1;
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, tickHeight);
                    drawRect.Inflate(-buttonSize.Width / 2, 0);
                    currentUsedPos += tickHeight;

                    if (tickVisible)
                    {
                        GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                    }
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw the text
                    // Get Height of Text Area
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, textAreaSize);
                    drawRect.Inflate(-buttonSize.Width / 2, 0);
                    currentUsedPos += textAreaSize;

                    if (valueTickVisible)
                    {
                        GDI.DrawTickTextLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                    }
                }
            }
            else
            {
                // trackBarType == Orientation.Vertical
                currentUsedPos = indentWidth;

                // Get Width of Text Area
                textAreaSize = e.Graphics.MeasureString(Maximum.ToString(), textFont).
                                 Width;

                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Draw text
                    // Get Height of Text Area
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, textAreaSize, workingRect.Height);
                    drawRect.Inflate(0, -buttonSize.Width / 2);
                    currentUsedPos += textAreaSize;

                    GDI.DrawTickTextLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }

                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Draw the tick line.
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, tickHeight, workingRect.Height);
                    drawRect.Inflate(0, -buttonSize.Width / 2);
                    currentUsedPos += tickHeight + 1;

                    GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                // Calculate the tracker's rectangle
                float currentTrackerPos;
                if (Maximum == Minimum)
                {
                    currentTrackerPos = workingRect.Top;
                }
                else
                {
                    currentTrackerPos = (workingRect.Height - buttonSize.Width) * (Value - Minimum) / (Maximum - Minimum);
                }

                // Remember this for drawing the Tracker later
                trackerRectangleF = new RectangleF(currentUsedPos, workingRect.Bottom - currentTrackerPos - buttonSize.Width, buttonSize.Height, buttonSize.Width);

                // trackerRectangleF.Inflate(-1,0);
                // Draw the track line
                drawRect = new RectangleF(currentUsedPos + buttonSize.Height / 2 - trackLineThickness / 2, workingRect.Top, trackLineThickness, workingRect.Height);

                // Draw the track line
                DrawTrackLine(e.Graphics, drawRect);

                Rectangle trackLineRectangle = Rectangle.Round(drawRect);

                // Track line border
                if (borderVisible)
                {
                    GDI.DrawBorder(graphics, GDI.GetBorderShape(trackLineRectangle, borderShape, 3), 1, borderColor);
                }

                currentUsedPos += buttonSize.Height;

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw the 2st Tick Line.
                    currentUsedPos += 1;
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, tickHeight, workingRect.Height);
                    drawRect.Inflate(0, -buttonSize.Width / 2);
                    currentUsedPos += tickHeight;

                    GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw the 2st Text Line.
                    // Get Height of Text Area
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, textAreaSize, workingRect.Height);
                    drawRect.Inflate(0, -buttonSize.Width / 2);
                    currentUsedPos += textAreaSize;

                    GDI.DrawTickTextLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }
            }

            if (progressVisible)
            {
                // Draw the progress
                DrawProgress(e.Graphics);
            }

            if (progressValueVisible)
            {
                // Get Height of Text Area
                float textAreaSizeWidth = graphics.MeasureString(Maximum.ToString() + charExtension, textFont).
                                                   Width;
                float textAreaSizeHeight = graphics.MeasureString(Maximum.ToString() + charExtension, textFont).
                                                    Height;
                var stringValue = (float)(Value / (double)dividedValue);

                PointF progressValuePoint = new PointF();

                // Determine draw position on orientation
                if (Orientation == Orientation.Horizontal)
                {
                    progressValuePoint = new PointF(Width / 2 - textAreaSizeWidth / 2, buttonRectangle.Y + buttonRectangle.Height / 2 - textAreaSizeHeight / 2 + 2);
                }
                else
                {
                    progressValuePoint = new PointF(Width / 2 - textAreaSizeWidth, Height / 2 - textAreaSizeHeight / 2 + 2);
                }

                // Draws the progress value on the progress bar
                graphics.DrawString(stringValue.ToString("0") + charExtension, textFont, new SolidBrush(buttonTextColor), progressValuePoint);
            }

            // Draw the Tracker
            DrawTracker(e.Graphics);
        }

        protected override void OnScroll(EventArgs e)
        {
            base.OnScroll(e);
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
                        Decrement(SmallChange);
                        break;
                    case Keys.Right:
                    case Keys.Up:
                        Increment(SmallChange);
                        break;

                    case Keys.PageUp:
                        Increment(LargeChange);
                        break;
                    case Keys.PageDown:
                        Decrement(LargeChange);
                        break;

                    case Keys.Home:
                        Value = Maximum;
                        break;
                    case Keys.End:
                        Value = Minimum;
                        break;

                    default:
                        result = base.ProcessCmdKey(ref msg, keyData);
                        break;
                }
            }

            return result;
        }

        private void DrawProgress(Graphics graphics)
        {
            GraphicsPath progressPath = null;
            Rectangle backgroundRect = new Rectangle();

            // Convert from RectangleF to Rectangle.
            Rectangle buttonRectangle = Rectangle.Round(trackerRectangleF);

            // Setup pipe clip
            Rectangle workingRect = Rectangle.Inflate(ClientRectangle, -indentWidth, -indentHeight);
            RectangleF progressRect = new RectangleF();

            var i1 = 0;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        progressRect = new RectangleF(workingRect.Left, indentHeight + buttonSize.Height / 2 - trackLineThickness / 2, workingRect.Width, trackLineThickness);

                        // Draws the progress to the middle of the button
                        i1 = buttonRectangle.X + buttonRectangle.Width / 2;
                        progressRotation = 0;

                        // Progress path
                        if (borderShape == BorderShape.Rectangle)
                        {
                            progressPath = new GraphicsPath();
                            progressPath.AddRectangle(new Rectangle(0, 0, i1 + 1, Height));
                            progressPath.CloseAllFigures();
                        }
                        else
                        {
                            progressPath = GDI.DrawRoundedRectangle(new Rectangle(1, 1, i1, Height - 2), borderRounding);
                        }

                        backgroundRect = new Rectangle(1, 1, i1, Height - 3);
                    }

                    break;
                case Orientation.Vertical:
                    {
                        progressRect = new RectangleF(indentWidth + workingRect.Y + trackLineThickness / 2, indentHeight, trackLineThickness, workingRect.Height);

                        // Draws the progress to the middle of the button
                        i1 = buttonRectangle.Y + buttonRectangle.Height / 2;
                        progressRotation = -90;

                        // Progress path
                        if (borderShape == BorderShape.Rectangle)
                        {
                            progressPath = new GraphicsPath();
                            progressPath.AddRectangle(new Rectangle(1, i1, Width - 3, Height));
                            progressPath.CloseAllFigures();
                        }
                        else
                        {
                            progressPath = GDI.DrawRoundedRectangle(new Rectangle(1, i1, Width - 3, Height), borderRounding);
                        }

                        backgroundRect = new Rectangle(1, i1, Width - 3, Height);
                    }

                    break;
            }

            // Clip to the TrackLine
            graphics.SetClip(progressRect);

            // Draw progress
            if (i1 > 1)
            {
                // Draw progress
                if (progressColorStyle == BrushType.Gradient)
                {
                    // Draw gradient progress
                    graphics.FillPath(new LinearGradientBrush(backgroundRect, progressColor1, progressColor2, progressRotation), progressPath);
                }
                else
                {
                    // Solid color progress
                    graphics.FillPath(new SolidBrush(progressColor1), progressPath);
                }

                // Toggle hatch
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

        /// <summary>Draws the tracker button.</summary>
        /// <param name="graphics">Graphics controller.</param>
        private void DrawTracker(Graphics graphics)
        {
            if (buttonVisible)
            {
                // Convert from RectangleF to Rectangle.
                buttonRectangle = Rectangle.Round(trackerRectangleF);
                Color controlCheckTemp = Enabled ? buttonColor : controlDisabledColor;

                buttonPath = GDI.GetBorderShape(buttonRectangle, borderShape, borderRounding);

                // Draw button background
                graphics.FillPath(new SolidBrush(controlCheckTemp), buttonPath);

                // Draw button border
                GDI.DrawBorderType(graphics, controlState, buttonPath, borderThickness, borderColor, borderHoverColor, borderVisible);

                // Draw the value on the tracker button
                if (buttonValueVisible)
                {
                    // Get Height of Text Area
                    float textAreaSizeWidth = graphics.MeasureString(Maximum.ToString() + charExtension, textFont).
                                                       Width;
                    float textAreaSizeHeight = graphics.MeasureString(Maximum.ToString() + charExtension, textFont).
                                                        Height;
                    var stringValue = (float)(Value / (double)dividedValue);

                    graphics.DrawString(stringValue.ToString("0") + charExtension, textFont, new SolidBrush(buttonTextColor),
                        new PointF(buttonRectangle.X + buttonRectangle.Width / 2 - textAreaSizeWidth / 2, buttonRectangle.Y + buttonRectangle.Height / 2 - textAreaSizeHeight / 2));
                }
            }
        }

        /// <summary>Draws the track line.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="trackLineRectangleF">Track line rectangle.</param>
        private void DrawTrackLine(Graphics graphics, RectangleF trackLineRectangleF)
        {
            GDI.DrawTrackBarLine(graphics, trackLineRectangleF, trackLineColor, trackBarType);
        }

        #endregion

        #region ${0} Methods

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

        #endregion
    }
}