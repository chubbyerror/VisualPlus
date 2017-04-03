namespace VisualPlus.Controls
{
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

    /// <summary>The visual NumericUpDown.</summary>
    [ToolboxBitmap(typeof(NumericUpDown)), Designer(VSDesignerBinding.VisualNumericUpDown)]
    public class VisualNumericUpDown : Control
    {
        #region  ${0} Variables

        private const int Spacing = 4;
        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color buttonColor1 = StylesManager.DefaultValue.Style.ButtonNormalColor;
        private Font buttonFont = new Font("Arial", 8);
        private Point buttonLocation = new Point(0, 4);
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private Size buttonSize = new Size(19, 19);
        private Color controlDisabled = StylesManager.DefaultValue.Style.ControlDisabled;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color inputFieldColor = StylesManager.DefaultValue.Style.BackgroundColor(0);
        private bool keyboardNum;
        private long maximumValue;
        private long minimumValue;
        private long numericValue;
        private Color textColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color textDisabled = StylesManager.DefaultValue.Style.TextDisabled;
        private int xval;
        private int yval;

        #endregion

        #region ${0} Properties

        public VisualNumericUpDown()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;

            minimumValue = 0;
            maximumValue = 100;

            Size = new Size(70, 28);
            MinimumSize = new Size(62, 28);

            UpdateStyles();
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

                UpdateLocationPoints();
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
                UpdateLocationPoints();
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

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentFont)]
        public Font ButtonFont
        {
            get
            {
                return buttonFont;
            }

            set
            {
                buttonFont = value;
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color InputField
        {
            get
            {
                return inputFieldColor;
            }

            set
            {
                inputFieldColor = value;
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
        public Color NumericUpDownDisabled
        {
            get
            {
                return controlDisabled;
            }

            set
            {
                controlDisabled = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextColor)]
        public Color TextColor
        {
            get
            {
                return textColor;
            }

            set
            {
                textColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color TextDisabled
        {
            get
            {
                return textDisabled;
            }

            set
            {
                textDisabled = value;
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

        #region ${0} Events

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
            if (xval > Width - Spacing - buttonRectangle.Width && xval < Width - Spacing)
            {
                if (yval < 15)
                {
                    if (Value + 1 <= maximumValue)
                    {
                        numericValue++;
                    }
                }
                else
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
            xval = e.Location.X;
            yval = e.Location.Y;
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
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Color tempTextColor;
            Color controlTemp;

            // Draw control state
            if (Enabled)
            {
                tempTextColor = textColor;
                controlTemp = buttonColor1;
            }
            else
            {
                tempTextColor = textDisabled;
                controlTemp = controlDisabled;
            }

            // Draw background
            graphics.FillPath(new SolidBrush(inputFieldColor), controlGraphicsPath);

            // Buttons background
            graphics.FillPath(new SolidBrush(controlTemp), buttonPath);

            // Setup buttons border
            if (borderVisible)
            {
                // Draw buttons border
                GDI.DrawBorder(graphics, buttonPath, 1, StylesManager.DefaultValue.Style.BorderColor(0));
            }

            // Buttons text
            TextRenderer.DrawText(graphics, "+", buttonFont, new Point(buttonRectangle.X + 5, buttonRectangle.Y - 2), textColor);
            TextRenderer.DrawText(graphics, "-", buttonFont, new Point(buttonRectangle.X + 6, buttonRectangle.Y + 6), textColor);

            // Button separator
            graphics.DrawLine(
                new Pen(StylesManager.DefaultValue.Style.BorderColor(0)),
                buttonRectangle.X,
                buttonRectangle.Y + buttonRectangle.Height / 2,
                buttonRectangle.X + buttonRectangle.Width,
                buttonRectangle.Y + buttonRectangle.Height / 2);

            // Draw control border
            if (borderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderColor);
                }
            }

            // Draw value string
            Rectangle textboxRectangle = new Rectangle(6, 0, Width - 1, Height - 1);

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Convert.ToString(Value), Font, new SolidBrush(tempTextColor), textboxRectangle, stringFormat);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = 28;
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private void UpdateLocationPoints()
        {
            buttonRectangle = new Rectangle(Width - buttonSize.Width - Spacing, buttonLocation.Y, buttonSize.Width, buttonSize.Height);

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);

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