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
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Properties;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual TextBox")]
    [Designer(VSDesignerBinding.VisualTextBox)]
    public sealed class VisualTextBox : TextBox
    {
        #region Variables

        public TextBox TextBoxObject = new TextBox();

        #endregion

        #region Variables

        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(3);

        private Border border = new Border();
        private Border buttonBorder = new Border();
        private Color buttonColor = Settings.DefaultValue.Style.ButtonNormalColor;
        private Image buttonImage = Resources.search;
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private bool buttonVisible;
        private int buttonWidth = 19;
        private Color controlDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Size iconSize = new Size(13, 13);
        private int textboxHeight = 20;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color waterMarkActiveColor;
        private SolidBrush waterMarkBrush;
        private Color waterMarkColor;
        private Panel waterMarkContainer;
        private Font waterMarkFont;
        private string waterMarkText = "Custom text...";

        private bool watermarkVisible;
        private int xValue;
        private int yValue;

        #endregion

        #region Constructors

        public VisualTextBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            BorderStyle = BorderStyle.None;
            AutoSize = false;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Size = new Size(135, 25);
            CreateTextBox();
            Controls.Add(TextBoxObject);
            BackColor = Color.Transparent;
            UpdateStyles();

            // Sets some default values to the watermark properties
            waterMarkColor = Color.LightGray;
            waterMarkActiveColor = Color.Gray;
            waterMarkFont = Font;
            waterMarkBrush = new SolidBrush(waterMarkActiveColor);
            waterMarkContainer = null;

            // Draw the watermark, for design time
            if (watermarkVisible)
            {
                DrawWaterMark();
            }
        }

        public delegate void ButtonClickedEventHandler();

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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ButtonImage)]
        public Image ButtonImage
        {
            get
            {
                return buttonImage;
            }

            set
            {
                buttonImage = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public Size IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                iconSize = value;
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Watermark)]
        public string WaterMark
        {
            get
            {
                return waterMarkText;
            }

            set
            {
                waterMarkText = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color WaterMarkActiveForeColor
        {
            get
            {
                return waterMarkActiveColor;
            }

            set
            {
                waterMarkActiveColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font WaterMarkFont
        {
            get
            {
                return waterMarkFont;
            }

            set
            {
                waterMarkFont = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color WaterMarkForeColor
        {
            get
            {
                return waterMarkColor;
            }

            set
            {
                waterMarkColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool WatermarkVisible
        {
            get
            {
                return watermarkVisible;
            }

            set
            {
                watermarkVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        public event ButtonClickedEventHandler ButtonClicked;

        public void CreateTextBox()
        {
            TextBox tb = TextBoxObject;
            tb.Size = new Size(Width, Height);
            tb.Location = new Point(5, 2);
            tb.Text = string.Empty;
            tb.BorderStyle = BorderStyle.None;
            tb.TextAlign = HorizontalAlignment.Left;
            tb.Font = Font;
            tb.ForeColor = ForeColor;
            tb.BackColor = backgroundColor;
            tb.UseSystemPasswordChar = UseSystemPasswordChar;
            tb.Multiline = false;

            TextBoxObject.KeyDown += OnKeyDown;
            TextBoxObject.TextChanged += OnBaseTextBoxChanged;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            controlState = ControlState.Hover;
            Invalidate();

            if (watermarkVisible)
            {
                // If focused use focus color
                waterMarkBrush = new SolidBrush(waterMarkActiveColor);

                // Don't draw watermark if contains text.
                if (TextLength <= 0)
                {
                    RemoveWaterMark();
                    DrawWaterMark();
                }
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            TextBoxObject.Font = Font;
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            TextBoxObject.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            TextBoxObject.Focus();
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);

            // Check if there is a watermark
            if (waterMarkContainer != null)
            {
                // if there is a watermark it should also be invalidated();
                waterMarkContainer.Invalidate();
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            controlState = ControlState.Normal;
            Invalidate();

            if (watermarkVisible)
            {
                // If the user has written something and left the control
                if (TextLength > 0)
                {
                    // Remove the watermark
                    RemoveWaterMark();
                }
                else
                {
                    // But if the user didn't write anything, then redraw the control.
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnMouseClick(e);

            if (buttonVisible)
            {
                // Check if mouse in X position.
                if (xValue > buttonRectangle.X && xValue < Width)
                {
                    // Determine the button middle separator by checking for the Y position.
                    if (yValue > buttonRectangle.Y && yValue < Height)
                    {
                        ButtonClicked?.Invoke();
                    }
                }
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            UpdateLocationPoints();
            graphics.SetClip(controlGraphicsPath);

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlTempColor = Enabled ? backgroundColor : controlDisabledColor;

            TextBoxObject.BackColor = controlTempColor;
            TextBoxObject.ForeColor = foreColor;

            // Setup internal text box object
            TextBoxObject.TextAlign = TextAlign;
            TextBoxObject.UseSystemPasswordChar = UseSystemPasswordChar;

            // Draw background back color
            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            // Setup button
            if (!Multiline && buttonVisible)
            {
                // Buttons background
                graphics.FillPath(new SolidBrush(buttonColor), buttonPath);

                Size imageSize = new Size(iconSize.Width, iconSize.Height);
                Point imagePoint = new Point(buttonRectangle.X + buttonRectangle.Width / 2 - imageSize.Width / 2, buttonRectangle.Y + buttonRectangle.Height / 2 - imageSize.Height / 2);

                Rectangle imageRectangle = new Rectangle(imagePoint, imageSize);

                graphics.SetClip(buttonPath);
                graphics.DrawImage(buttonImage, imageRectangle);
                graphics.SetClip(controlGraphicsPath);

                if (buttonBorder.Visible)
                {
                    GDI.DrawBorderType(graphics, controlState, buttonPath, buttonBorder.Thickness, buttonBorder.Color, buttonBorder.HoverColor, buttonBorder.HoverVisible);
                }

                TextBoxObject.Width = buttonRectangle.X - 10;
            }
            else
            {
                TextBoxObject.Width = Width - 10;
            }

            graphics.ResetClip();

            // Draw border
            if (border.Visible)
            {
                if (controlState == ControlState.Hover && border.HoverVisible)
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, border.Thickness, border.HoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, border.Thickness, border.Color);
                }
            }

            if (watermarkVisible)
            {
                DrawWaterMark();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Multiline sizing
            if (Multiline)
            {
                TextBoxObject.Height = Height - 6;
            }
            else
            {
                // Auto adjust the text box height depending on the font size.
                textboxHeight = Convert.ToInt32(Font.Size) * 2 + 1;
            }

            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            TextBoxObject.Text = Text;
            Invalidate();

            if (watermarkVisible)
            {
                // If the text of the text box is not empty.
                if (TextLength > 0)
                {
                    // Remove the watermark
                    RemoveWaterMark();
                }
                else
                {
                    // But if the text is empty, draw the watermark again.
                    DrawWaterMark();
                }
            }
        }

        private void DrawWaterMark()
        {
            if (waterMarkContainer == null && TextLength <= 0)
            {
                waterMarkContainer = new Panel(); // Creates the new panel instance
                waterMarkContainer.Paint += waterMarkContainer_Paint;
                waterMarkContainer.Invalidate();
                waterMarkContainer.Click += waterMarkContainer_Click;
                Controls.Add(waterMarkContainer); // adds the control
                waterMarkContainer.BringToFront();
            }
        }

        private void OnBaseTextBoxChanged(object s, EventArgs e)
        {
            Text = TextBoxObject.Text;
        }

        private void OnKeyDown(object obj, KeyEventArgs e)
        {
            // Select all
            if (e.Control && e.KeyCode == Keys.A)
            {
                TextBoxObject.SelectAll();
                e.SuppressKeyPress = true;
            }

            // Copy
            if (e.Control && e.KeyCode == Keys.C)
            {
                TextBoxObject.Copy();
                e.SuppressKeyPress = true;
            }
        }

        private void RemoveWaterMark()
        {
            if (waterMarkContainer != null)
            {
                Controls.Remove(waterMarkContainer);
                waterMarkContainer = null;
            }
        }

        private void UpdateLocationPoints()
        {
            if (!Multiline)
            {
                Height = textboxHeight;
            }

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            buttonPath = new GraphicsPath();
            buttonPath.AddRectangle(buttonRectangle);
            buttonPath.CloseAllFigures();
        }

        private void waterMarkContainer_Click(object sender, EventArgs e)
        {
            Focus();
        }

        private void waterMarkContainer_Paint(object sender, PaintEventArgs e)
        {
            // Configure the watermark
            waterMarkContainer.Location = new Point(2, 0);
            waterMarkContainer.Height = Height;
            waterMarkContainer.Width = Width;

            // Forces it to resize with the parent control
            waterMarkContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            // Set color
            waterMarkBrush = ContainsFocus ? new SolidBrush(waterMarkActiveColor) : new SolidBrush(waterMarkColor);

            // Draws the string on the panel
            e.Graphics.DrawString(waterMarkText, waterMarkFont, waterMarkBrush, new PointF(-2f, 1f));
        }

        #endregion
    }
}