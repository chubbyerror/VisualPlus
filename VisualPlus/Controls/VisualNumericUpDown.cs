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
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(NumericUpDown))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual NumericUpDown")]
    [Designer(DesignManager.VisualNumericUpDown)]
    public sealed class VisualNumericUpDown : Control
    {
        #region Variables

        private readonly MouseState mouseState;

        private Gradient backgroundDisabledGradient = new Gradient();
        private Gradient backgroundGradient = new Gradient();
        private Border border = new Border();
        private Border buttonBorder = new Border();
        private Gradient buttonGradient = new Gradient();
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private int buttonWidth = 19;
        private GraphicsPath controlGraphicsPath;
        private Color foreColor;
        private bool keyboardNum;
        private long maximumValue;
        private long minimumValue;
        private long numericValue;
        private StyleManager styleManager = new StyleManager();
        private Color textDisabledColor;
        private TextRenderingHint textRendererHint;
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

            mouseState = new MouseState(this);
            BackColor = Color.Transparent;
            minimumValue = 0;
            maximumValue = 100;
            Size = new Size(70, 29);
            MinimumSize = new Size(62, 29);
            UpdateStyles();

            buttonBorder.HoverVisible = false;
            buttonBorder.Type = BorderType.Rectangle;

            ConfigureStyleManager();
            DefaultGradient();
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient BackgroundDisabled
        {
            get
            {
                return backgroundDisabledGradient;
            }

            set
            {
                backgroundDisabledGradient = value;
                Invalidate();
            }
        }

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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Button
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
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
            if ((xValue > Width - buttonRectangle.Width) && (xValue < Width))
            {
                // Determine the button middle separator by checking for the Y position.
                if ((yValue > buttonRectangle.Y) && (yValue < Height / 2))
                {
                    if (Value + 1 <= maximumValue)
                    {
                        numericValue++;
                    }
                }
                else if ((yValue > Height / 2) && (yValue < Height))
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
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseState.State = MouseStates.Normal;
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

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            Gradient backgroundCheckTemp = Enabled ? backgroundGradient : backgroundDisabledGradient;
            Gradient buttonCheckTemp = Enabled ? buttonGradient : backgroundDisabledGradient;

            graphics.SetClip(controlGraphicsPath);

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };

            LinearGradientBrush backgroundGradientBrush = GDI.CreateGradientBrush(backgroundCheckTemp.Colors, gradientPoints, backgroundCheckTemp.Angle, backgroundCheckTemp.Positions);
            graphics.FillPath(backgroundGradientBrush, controlGraphicsPath);

            LinearGradientBrush buttonGradientBrush = GDI.CreateGradientBrush(buttonCheckTemp.Colors, gradientPoints, buttonCheckTemp.Angle, buttonCheckTemp.Positions);
            graphics.FillPath(buttonGradientBrush, buttonPath);

            // Setup buttons border
            if (buttonBorder.Visible)
            {
                GDI.DrawBorderType(graphics, mouseState.State, buttonPath, buttonBorder.Thickness, buttonBorder.Color, buttonBorder.HoverColor, buttonBorder.HoverVisible);
            }

            graphics.ResetClip();

            // Draw string
            graphics.DrawString("+", Font, new SolidBrush(foreColor), new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - ((int)Font.SizeInPoints / 2) - 2, (Height / 4) - (buttonRectangle.Height / 4)));
            graphics.DrawString("-", Font, new SolidBrush(foreColor), new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - ((int)Font.SizeInPoints / 2), Height / 2));

            // Button separator
            graphics.DrawLine(new Pen(Settings.DefaultValue.Border.Color), buttonRectangle.X, buttonRectangle.Y + (buttonRectangle.Height / 2), buttonRectangle.X + buttonRectangle.Width, buttonRectangle.Y + (buttonRectangle.Height / 2));

            // Draw control border
            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, mouseState.State, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
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

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.BorderStyle;
                IControl controlStyle = styleManager.VisualStylesManager.ControlStyle;
                IFont fontStyle = styleManager.VisualStylesManager.FontStyle;

                border.Color = borderStyle.Color;
                border.HoverColor = borderStyle.HoverColor;
                border.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                border.Rounding = styleManager.VisualStylesManager.BorderRounding;
                border.Type = styleManager.VisualStylesManager.BorderType;
                border.Thickness = styleManager.VisualStylesManager.BorderThickness;
                border.Visible = styleManager.VisualStylesManager.BorderVisible;
                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;

                buttonGradient.Colors = controlStyle.ControlEnabled.Colors;
                buttonGradient.Positions = controlStyle.ControlEnabled.Positions;

                float[] backgroundGradientPosition = { 0, 1 };

                Color[] backgroundColor =
                    {
                        controlStyle.Background(0),
                        controlStyle.Background(1)
                    };

                backgroundGradient.Colors = backgroundColor;
                backgroundGradient.Positions = backgroundGradientPosition;
                backgroundDisabledGradient.Colors = controlStyle.ControlDisabled.Colors;
                backgroundDisabledGradient.Positions = controlStyle.ControlDisabled.Positions;
            }
            else
            {
                // Load default settings
                border = new Border();

                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = Settings.DefaultValue.DefaultFont;
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        private void DefaultGradient()
        {
            float[] backgroundGradientPosition = { 0, 1 };

            Color[] backgroundColor =
                {
                    Settings.DefaultValue.Control.Background(0),
                    ControlPaint.Light(Settings.DefaultValue.Control.Background(0))
                };

            buttonGradient.Colors = Settings.DefaultValue.Control.ControlEnabled.Colors;
            buttonGradient.Positions = Settings.DefaultValue.Control.ControlEnabled.Positions;

            backgroundGradient.Colors = backgroundColor;
            backgroundGradient.Positions = backgroundGradientPosition;

            backgroundDisabledGradient.Colors = Settings.DefaultValue.Control.ControlDisabled.Colors;
            backgroundDisabledGradient.Positions = Settings.DefaultValue.Control.ControlDisabled.Positions;
        }

        private void UpdateLocationPoints()
        {
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Type, border.Rounding);
            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            buttonPath = new GraphicsPath();
            buttonPath.AddRectangle(buttonRectangle);
            buttonPath.CloseAllFigures();
        }

        #endregion
    }
}