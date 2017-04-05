namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

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

    /// <summary>The visual Trackbar.</summary>
    [DefaultEvent("ValueChanged"), ToolboxBitmap(typeof(TrackBar)), Designer(VSDesignerBinding.VisualTrackBar)]
    public sealed class VisualTrackBar : TrackBar
    {
        #region  ${0} Variables

        private static Color progressColor1 = StylesManager.DefaultValue.Style.ProgressColor;
        private static Orientation trackBarType = Orientation.Horizontal;
        private static RectangleF trackerRectangleF = RectangleF.Empty;
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color buttonColor = StylesManager.DefaultValue.Style.ButtonNormalColor;
        private GraphicsPath buttonPath = new GraphicsPath();
        private Color buttonTextColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color controlDisabledColor = StylesManager.DefaultValue.Style.ControlDisabled;
        private ControlState controlState = ControlState.Normal;
        private ValueDivisor dividedValue = ValueDivisor.By1;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private int indentHeight = 6;
        private int indentWidth = 6;
        private bool leftButtonDown;
        private float mouseStartPos = -1;
        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;
        private Font textFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        private Color tickColor = StylesManager.DefaultValue.Style.LineColor;
        private int tickHeight = 2;
        private Size trackerSize = new Size(27, 20);
        private Color trackLineColor = StylesManager.DefaultValue.Style.LineColor;
        private int trackLineHeight = 5;
        private bool valueButtonVisible;
        private bool valueTickVisible = StylesManager.DefaultValue.TextVisible;

        #endregion

        #region ${0} Properties

        public VisualTrackBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            UpdateStyles();
            AutoSize = false;
            Size = new Size(180, 50);
            MinimumSize = new Size(180, 50);
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderHoverColor)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumRounding, StylesManager.MaximumRounding))
                {
                    borderRounding = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
         Description(Localize.Description.ComponentShape)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderSize), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderSize)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumBorderSize, StylesManager.MaximumBorderSize))
                {
                    borderSize = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderVisible)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
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

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentSize)]
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

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentSize)]
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

                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentFont)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentSize)]
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

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentSize)]
        public Size TrackerSize
        {
            get
            {
                return trackerSize;
            }

            set
            {
                trackerSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentSize)]
        public int TrackLineHeight
        {
            get
            {
                return trackLineHeight;
            }

            set
            {
                trackLineHeight = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentVisible)]
        public bool ValueButtonVisible
        {
            get
            {
                return valueButtonVisible;
            }

            set
            {
                valueButtonVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior), Description(Localize.Description.ValueDivisor)]
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

        [DefaultValue(StylesManager.DefaultValue.TextVisible), Category(Localize.Category.Appearance), Description(Localize.Description.TextVisible)]
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

        /// <summary>Gets or sets the value to set.</summary>
        [Browsable(false)]
        public float ValueToSet
        {
            get
            {
                return (float)(Value / (double)dividedValue);
            }

            set
            {
                Value = (int)Math.Round(value * (float)dividedValue);
            }
        }

        #endregion

        #region ${0} Events

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
                                break;
                            }

                        case Orientation.Vertical:
                            {
                                mouseStartPos = currentPoint.Y - trackerRectangleF.Y;
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
                        if ((currentPoint.X + trackerSize.Width) / 2 >= Width - indentWidth)
                        {
                            offsetValue = Maximum - Minimum;
                        }
                        else if ((currentPoint.X - trackerSize.Width) / 2 <= indentWidth)
                        {
                            offsetValue = 0;
                        }
                        else
                        {
                            offsetValue =
                                (int)
                                ((currentPoint.X - indentWidth - trackerSize.Width) / 2 * (Maximum - Minimum) /
                                 (Width - 2 * indentWidth - trackerSize.Width) + 0.5);
                        }

                        break;

                    case Orientation.Vertical:
                        if ((currentPoint.Y + trackerSize.Width) / 2 >= Height - indentHeight)
                        {
                            offsetValue = 0;
                        }
                        else if ((currentPoint.Y - trackerSize.Width) / 2 <= indentHeight)
                        {
                            offsetValue = Maximum - Minimum;
                        }
                        else
                        {
                            offsetValue =
                                (int)
                                ((Height - currentPoint.Y - indentHeight - trackerSize.Width) / 2 * (Maximum - Minimum) /
                                 (Height - 2 * indentHeight - trackerSize.Width) + 0.5);
                        }

                        break;
                }

                int oldValue = Value;
                Value = Minimum + offsetValue;
                Invalidate();

                if (oldValue != Value)
                {
                    // OnScroll();
                    // OnValueChanged(Value);
                }
            }

            Invalidate();
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
                            if (currentPoint.X + trackerSize.Width - mouseStartPos >= Width - indentWidth)
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
                                     (Width - 2 * indentWidth - trackerSize.Width) + 0.5);
                            }

                            break;

                        case Orientation.Vertical:
                            if ((currentPoint.Y + trackerSize.Width) / 2 >= Height - indentHeight)
                            {
                                offsetValue = 0;
                            }
                            else if ((currentPoint.Y + trackerSize.Width) / 2 <= indentHeight)
                            {
                                offsetValue = Maximum - Minimum;
                            }
                            else
                            {
                                offsetValue =
                                    (int)
                                    (((Height - currentPoint.Y + trackerSize.Width) / 2 - mouseStartPos - indentHeight) * (Maximum - Minimum) /
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
                    Value = Minimum + offsetValue;
                    Invalidate();

                    if (oldValue != Value)
                    {
                        // OnScroll();
                        // OnValueChanged(Value);
                    }
                }
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            leftButtonDown = false;
            Capture = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

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
                    drawRect.Inflate(-trackerSize.Width / 2, 0);
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
                    drawRect.Inflate(-trackerSize.Width / 2, 0);
                    currentUsedPos += tickHeight + 1;

                    GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                // Calculate the tracker's rectangle
                float currentTrackerPos;
                if (Maximum == Minimum)
                {
                    currentTrackerPos = workingRect.Left;
                }
                else
                {
                    currentTrackerPos = (workingRect.Width - trackerSize.Width) * (Value - Minimum) / (Maximum - Minimum) + workingRect.Left;
                }

                // Remember this for drawing the Tracker later
                trackerRectangleF = new RectangleF(currentTrackerPos, currentUsedPos, trackerSize.Width, trackerSize.Height);

                // trackerRectangleF.Inflate(0,-1);
                // Draw the Track Line
                drawRect = new RectangleF(workingRect.Left, currentUsedPos + trackerSize.Height / 2 - trackLineHeight / 2, workingRect.Width,
                    trackLineHeight);

                // Draws the track line
                DrawTrackLine(e.Graphics, drawRect);

                Rectangle trackLineRectangle = Rectangle.Round(drawRect);

                // Trackline border
                if (borderVisible)
                {
                    GDI.DrawBorder(graphics, GDI.GetBorderShape(trackLineRectangle, borderShape, 3), 1, borderColor);
                }

                currentUsedPos += trackerSize.Height;

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw tick line.
                    currentUsedPos += 1;
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, tickHeight);
                    drawRect.Inflate(-trackerSize.Width / 2, 0);
                    currentUsedPos += tickHeight;

                    GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw the text
                    // Get Height of Text Area
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, textAreaSize);
                    drawRect.Inflate(-trackerSize.Width / 2, 0);
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
                    drawRect.Inflate(0, -trackerSize.Width / 2);
                    currentUsedPos += textAreaSize;

                    GDI.DrawTickTextLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }

                if (TickStyle == TickStyle.TopLeft || TickStyle == TickStyle.Both)
                {
                    // Draw the tick line.
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, tickHeight, workingRect.Height);
                    drawRect.Inflate(0, -trackerSize.Width / 2);
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
                    currentTrackerPos = (workingRect.Height - trackerSize.Width) * (Value - Minimum) / (Maximum - Minimum);
                }

                // Remember this for drawing the Tracker later
                trackerRectangleF = new RectangleF(currentUsedPos, workingRect.Bottom - currentTrackerPos - trackerSize.Width, trackerSize.Height,
                    trackerSize.Width);

                // trackerRectangleF.Inflate(-1,0);
                // Draw the track line
                drawRect = new RectangleF(currentUsedPos + trackerSize.Height / 2 - trackLineHeight / 2, workingRect.Top, trackLineHeight,
                    workingRect.Height);

                // Draw the track line
                DrawTrackLine(e.Graphics, drawRect);

                Rectangle trackLineRectangle = Rectangle.Round(drawRect);

                // Track line border
                if (borderVisible)
                {
                    GDI.DrawBorder(graphics, GDI.GetBorderShape(trackLineRectangle, borderShape, 3), 1, borderColor);
                }

                currentUsedPos += trackerSize.Height;

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw the 2st Tick Line.
                    currentUsedPos += 1;
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, tickHeight, workingRect.Height);
                    drawRect.Inflate(0, -trackerSize.Width / 2);
                    currentUsedPos += tickHeight;

                    GDI.DrawTickLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, tickColor, trackBarType);
                }

                if (TickStyle == TickStyle.BottomRight || TickStyle == TickStyle.Both)
                {
                    // Draw the 2st Text Line.
                    // Get Height of Text Area
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, textAreaSize, workingRect.Height);
                    drawRect.Inflate(0, -trackerSize.Width / 2);
                    currentUsedPos += textAreaSize;

                    GDI.DrawTickTextLine(e.Graphics, drawRect, TickFrequency, Minimum, Maximum, foreColor, textFont, trackBarType);
                }
            }

            // Draw the Tracker
            DrawTracker(e.Graphics);
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

        /// <summary>Draws the tracker button.</summary>
        /// <param name="graphics">Graphics controller.</param>
        private void DrawTracker(Graphics graphics)
        {
            // Convert from RectangleF to Rectangle.
            Rectangle buttonRectangle = Rectangle.Round(trackerRectangleF);
            Color controlCheckTemp = Enabled ? buttonColor : controlDisabledColor;

            buttonPath = GDI.GetBorderShape(buttonRectangle, borderShape, borderRounding);

            // Draw button background
            graphics.FillPath(new SolidBrush(controlCheckTemp), buttonPath);

            // Draw button border
            GDI.DrawBorderType(graphics, controlState, buttonPath, borderSize, borderColor, borderHoverColor, borderVisible);

            // Draw the value on the tracker button
            if (valueButtonVisible)
            {
                // Get Height of Text Area
                float textAreaSize = graphics.MeasureString(Maximum.ToString(), textFont).
                                              Height;
                var stringValue = (float)(Value / (double)dividedValue);

                graphics.DrawString(stringValue.ToString("0"), textFont, new SolidBrush(buttonTextColor),
                    new PointF(buttonRectangle.X + buttonRectangle.Width / 2 - textAreaSize / 2,
                        buttonRectangle.Y + buttonRectangle.Height / 2 - textAreaSize / 2));
            }
        }

        /// <summary>Draws the track line.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="trackLineRectangleF">Track line rectangle.</param>
        private void DrawTrackLine(Graphics graphics, RectangleF trackLineRectangleF)
        {
            // Convert from RectangleF to Rectangle.
            Rectangle trackLineRectangle = Rectangle.Round(trackerRectangleF);
            GDI.GetBorderShape(trackLineRectangle, borderShape, borderRounding);

            GDI.DrawAquaPillSingleLayer(graphics, trackLineRectangleF, trackLineColor, trackBarType);
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