namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Controls.Bases;
    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(NumericUpDown))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual NumericUpDown")]
    [Designer(ControlManager.FilterProperties.VisualNumericUpDown)]
    public sealed class VisualNumericUpDown : ControlBase
    {
        #region Variables

        private Gradient backgroundGradient = new Gradient();
        private Border buttonBorder;
        private Color buttonColorText;
        private Gradient buttonGradient = new Gradient();
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private int buttonWidth = 19;
        private GraphicsPath controlGraphicsPath = new GraphicsPath();
        private bool keyboardNum;
        private long maximumValue;
        private long minimumValue;
        private long numericValue;
        private int xValue;
        private int yValue;

        #endregion

        #region Constructors

        public VisualNumericUpDown()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            minimumValue = 0;
            maximumValue = 100;
            Size = new Size(70, 29);
            MinimumSize = new Size(62, 29);
            UpdateStyles();

            InitializeTheme();
            DefaultGradient();
        }

        public enum ButtonSetup
        {
            /// <summary>The above.</summary>
            Above,

            /// <summary>The beside.</summary>
            Beside
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return ControlBorder;
            }

            set
            {
                ControlBorder = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ButtonColorText
        {
            get
            {
                return buttonColorText;
            }

            set
            {
                buttonColorText = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
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
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Behavior)]
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

        [Category(Localize.PropertiesCategory.Behavior)]
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

        [Category(Localize.PropertiesCategory.Behavior)]
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
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (StyleManager.LockedStyle)
            {
                InitializeTheme();
            }

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            buttonPath = new GraphicsPath();
            buttonPath.AddRectangle(buttonRectangle);
            buttonPath.CloseAllFigures();

            Gradient backgroundCheckTemp = Enabled ? backgroundGradient : ControlBrushCollection[3];
            Gradient buttonCheckTemp = Enabled ? buttonGradient : ControlBrushCollection[3];

            graphics.SetClip(controlGraphicsPath);

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };

            LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backgroundCheckTemp.Colors, gradientPoints, backgroundCheckTemp.Angle, backgroundCheckTemp.Positions);
            graphics.FillPath(backgroundGradientBrush, controlGraphicsPath);

            LinearGradientBrush buttonGradientBrush = Gradient.CreateGradientBrush(buttonCheckTemp.Colors, gradientPoints, buttonCheckTemp.Angle, buttonCheckTemp.Positions);
            graphics.FillPath(buttonGradientBrush, buttonPath);

            Border.DrawBorderStyle(graphics, buttonBorder, MouseState, buttonPath);

            graphics.ResetClip();

            // Draw string
            graphics.DrawString("+", Font, new SolidBrush(buttonColorText), new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - ((int)Font.SizeInPoints / 2) - 2, (Height / 4) - (buttonRectangle.Height / 4)));
            graphics.DrawString("-", Font, new SolidBrush(buttonColorText), new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - ((int)Font.SizeInPoints / 2), Height / 2));

            // Button separator
            graphics.DrawLine(new Pen(Settings.DefaultValue.Border.Color), buttonRectangle.X, buttonRectangle.Y + (buttonRectangle.Height / 2), buttonRectangle.X + buttonRectangle.Width, buttonRectangle.Y + (buttonRectangle.Height / 2));

            Border.DrawBorderStyle(graphics, ControlBorder, MouseState, controlGraphicsPath);

            // Draw value string
            Rectangle textboxRectangle = new Rectangle(6, 0, Width - 1, Height - 1);

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Convert.ToString(Value), Font, new SolidBrush(ForeColor), textboxRectangle, stringFormat);
        }

        private void DefaultGradient()
        {
            float[] backgroundGradientPosition = { 0, 1 };

            Color[] backgroundColor =
                {
                    Settings.DefaultValue.Control.Background(0),
                    ControlPaint.Light(Settings.DefaultValue.Control.Background(0))
                };

            buttonGradient = Settings.DefaultValue.ControlState.ControlEnabled;

            backgroundGradient.Colors = backgroundColor;
            backgroundGradient.Positions = backgroundGradientPosition;
        }

        private void InitializeTheme()
        {
            if (StyleManager.VisualStylesManager != null)
            {
                IControl controlStyle = StyleManager.VisualStylesManager.ControlStyle;
                IControlState controlStateStyle = StyleManager.VisualStylesManager.ControlStateStyle;
                IFont fontStyle = StyleManager.VisualStylesManager.FontStyle;

                buttonGradient = controlStateStyle.ControlEnabled;
                buttonColorText = fontStyle.ForeColor;

                float[] backgroundGradientPosition = { 0, 1 };

                Color[] backgroundColor =
                    {
                        controlStyle.Background(0),
                        controlStyle.Background(1)
                    };

                backgroundGradient.Colors = backgroundColor;
                backgroundGradient.Positions = backgroundGradientPosition;
            }
            else
            {
                buttonBorder = new Border
                    {
                        HoverVisible = false,
                        Type = BorderType.Rectangle
                    };

                buttonColorText = Settings.DefaultValue.Font.ForeColor;
            }
        }

        #endregion
    }
}