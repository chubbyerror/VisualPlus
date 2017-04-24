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

    /// <summary>The visual NumericUpDown.</summary>
    [ToolboxBitmap(typeof(NumericUpDown))]
    [Designer(VSDesignerBinding.VisualNumericUpDown)]
    [Description("Displays a single numeric value that the user can increment and decrement by clicking up and down button on the control.")]
    public sealed class VisualNumericUpDown : Control
    {
        #region Variables

        private static BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private int borderThickness = Settings.DefaultValue.BorderSize;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Color buttonColor = Settings.DefaultValue.Style.ButtonNormalColor;
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private int buttonWidth = 19;
        private Color controlDisabledColor = Settings.DefaultValue.Style.ControlDisabled;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private bool keyboardNum;
        private long maximumValue;
        private long minimumValue;
        private long numericValue;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private int xValue;
        private int yValue;

        #endregion

        #region Constructors

        public VisualNumericUpDown()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            minimumValue = 0;
            maximumValue = 100;
            Size = new Size(70, 29);
            MinimumSize = new Size(62, 29);
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            UpdateStyles();
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor
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

                UpdateLocationPoints();
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
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderSize)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int ButtonWidth
        {
            get
            {
                return buttonWidth;
            }

            set
            {
                buttonWidth = value;
                UpdateLocationPoints();
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
        public Color InputField
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
        public long MaximumValue
        {
            get
            {
                return maximumValue;
            }

            set
            {
                if (value > minimumValue)
                {
                    maximumValue = value;
                }

                if (numericValue > maximumValue)
                {
                    numericValue = maximumValue;
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        public long MinimumValue
        {
            get
            {
                return minimumValue;
            }

            set
            {
                if (value < maximumValue)
                {
                    minimumValue = value;
                }

                if (numericValue < minimumValue)
                {
                    numericValue = MinimumValue;
                }

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

        [Category(Localize.Category.Behavior)]
        public long Value
        {
            get
            {
                return numericValue;
            }

            set
            {
                if ((value <= maximumValue) & (value >= minimumValue))
                {
                    numericValue = value;
                }

                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            try
            {
                if (keyboardNum)
                {
                    numericValue = long.Parse(numericValue + e.KeyChar.ToString());
                }

                if (numericValue > maximumValue)
                {
                    numericValue = maximumValue;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Back)
            {
                string temporaryValue = numericValue.ToString();
                temporaryValue = temporaryValue.Remove(Convert.ToInt32(temporaryValue.Length - 1));
                if (temporaryValue.Length == 0)
                {
                    temporaryValue = "0";
                }

                numericValue = Convert.ToInt32(temporaryValue);
            }

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnMouseClick(e);

            // Check if mouse in X position.
            if (xValue > Width - buttonRectangle.Width && xValue < Width)
            {
                // Determine the button middle separator by checking for the Y position.
                if (yValue > buttonRectangle.Y && yValue < Height / 2)
                {
                    if (Value + 1 <= maximumValue)
                    {
                        numericValue++;
                    }
                }
                else if (yValue > Height / 2 && yValue < Height)
                {
                    if (Value - 1 >= minimumValue)
                    {
                        numericValue--;
                    }
                }
            }
            else
            {
                keyboardNum = !keyboardNum;
                Focus();
            }

            Invalidate();
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
            xValue = e.Location.X;
            yValue = e.Location.Y;
            Invalidate();

            // IBeam cursor toggle
            if (e.X < buttonRectangle.X)
            {
                Cursor = Cursors.IBeam;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                if (Value + 1 <= maximumValue)
                {
                    numericValue++;
                }

                Invalidate();
            }
            else
            {
                if (Value - 1 >= minimumValue)
                {
                    numericValue--;
                }

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlCheckTemp = Enabled ? buttonColor : controlDisabledColor;

            graphics.SetClip(controlGraphicsPath);

            // Draw background
            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            // Buttons background
            graphics.FillPath(new SolidBrush(controlCheckTemp), buttonPath);

            // Setup buttons border
            if (borderVisible)
            {
                // Draw buttons border
                GDI.DrawBorder(graphics, buttonPath, 1, Settings.DefaultValue.Style.BorderColor(0));
            }

            graphics.ResetClip();

            // Draw string
            graphics.DrawString("+", Font, new SolidBrush(foreColor), new Point(buttonRectangle.X + buttonRectangle.Width / 2 - (int)Font.SizeInPoints / 2, Height / 4 - buttonRectangle.Height / 4));
            graphics.DrawString("-", Font, new SolidBrush(foreColor), new Point(buttonRectangle.X + buttonRectangle.Width / 2 - (int)Font.SizeInPoints / 2 + 1, Height / 2));

            // Button separator
            graphics.DrawLine(new Pen(Settings.DefaultValue.Style.BorderColor(0)), buttonRectangle.X, buttonRectangle.Y + buttonRectangle.Height / 2, buttonRectangle.X + buttonRectangle.Width, buttonRectangle.Y + buttonRectangle.Height / 2);

            // Draw control border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            // Draw value string
            Rectangle textboxRectangle = new Rectangle(6, 0, Width - 1, Height - 1);

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Convert.ToString(Value), Font, new SolidBrush(foreColor), textboxRectangle, stringFormat);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private void UpdateLocationPoints()
        {
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            buttonPath = new GraphicsPath();
            buttonPath.AddRectangle(buttonRectangle);
            buttonPath.CloseAllFigures();
        }

        #endregion

        #region ${0} Methods

        public void Decrement(int value)
        {
            numericValue -= value;
            Invalidate();
        }

        public void Increment(int value)
        {
            numericValue += value;
            Invalidate();
        }

        #endregion
    }
}