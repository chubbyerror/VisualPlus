namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;
    using VisualPlus.Managers;
    using VisualPlus.Properties;
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual TextBox")]
    [Designer(ControlManager.FilterProperties.VisualTextBox)]
    public sealed class VisualTextBox : InputFieldBase
    {
        #region Variables

        public TextBox TextBoxObject = new TextBox();

        #endregion

        #region Variables

        private Color backgroundColor;
        private Border buttonBorder;
        private Color buttonColor;
        private Image buttonImage = Resources.search;
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private bool buttonVisible;
        private int buttonWidth = 19;
        private Color controlDisabledColor;
        private GraphicsPath controlGraphicsPath;
        private Size iconSize = new Size(13, 13);
        private int textBoxHeight = 20;
        private Watermark watermark = new Watermark();
        private Panel waterMarkContainer;
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

            buttonColor = StyleManager.ControlStyle.FlatButtonEnabled;
            controlDisabledColor = StyleManager.FontStyle.ForeColorDisabled;

            Border = new Border();
            buttonBorder = new Border();

            AutoSize = false;
            Size = new Size(135, 25);
            CreateTextBox();
            Controls.Add(TextBoxObject);
            BackColor = Color.Transparent;
            UpdateStyles();

            waterMarkContainer = null;

            backgroundColor = StyleManager.ControlStyle.Background(3);

            if (watermark.Visible)
            {
                DrawWaterMark();
            }
        }

        public delegate void ButtonClickedEventHandler();

        public event ButtonClickedEventHandler ButtonClicked;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Image)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
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

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(false)]
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        [TypeConverter(typeof(WatermarkConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Watermark Watermark
        {
            get
            {
                return watermark;
            }

            set
            {
                watermark = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

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

            // tb.UseSystemPasswordChar = UseSystemPasswordChar;
            tb.Multiline = false;

            TextBoxObject.KeyDown += OnKeyDown;
            TextBoxObject.TextChanged += OnBaseTextBoxChanged;

            TextBoxObject.MouseMove += OnMouseMove;
            TextBoxObject.Leave += OnLeave;
            TextBoxObject.Enter += OnEnter;
            TextBoxObject.GotFocus += OnEnter;
            TextBoxObject.LostFocus += OnLeave;
            TextBoxObject.MouseLeave += OnLeave;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (watermark.Visible)
            {
                // If focused use focus color
                watermark.Brush = new SolidBrush(watermark.ActiveColor);

                // Don't draw watermark if contains text.
                if (TextBoxObject.TextLength <= 0)
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

            if (watermark.Visible)
            {
                // If the user has written something and left the control
                if (TextBoxObject.TextLength > 0)
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
                if ((xValue > buttonRectangle.X) && (xValue < Width))
                {
                    // Determine the button middle separator by checking for the Y position.
                    if ((yValue > buttonRectangle.Y) && (yValue < Height))
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
            MouseState = MouseStates.Hover;
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

            if (!TextBoxObject.Multiline)
            {
                Height = textBoxHeight;
            }

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            buttonPath = new GraphicsPath();
            buttonPath.AddRectangle(buttonRectangle);
            buttonPath.CloseAllFigures();

            graphics.SetClip(controlGraphicsPath);

            // Set control state color
            Color controlTempColor = Enabled ? backgroundColor : controlDisabledColor;

            TextBoxObject.BackColor = controlTempColor;
            TextBoxObject.ForeColor = ForeColor;

            // Setup internal text box object
            // TextBoxObject.TextAlign = TextAlign;
            // TextBoxObject.UseSystemPasswordChar = UseSystemPasswordChar;

            // Draw background back color
            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            // Setup button
            if (!TextBoxObject.Multiline && buttonVisible)
            {
                // Buttons background
                graphics.FillPath(new SolidBrush(buttonColor), buttonPath);

                Size imageSize = new Size(iconSize.Width, iconSize.Height);
                Point imagePoint = new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - (imageSize.Width / 2), (buttonRectangle.Y + (buttonRectangle.Height / 2)) - (imageSize.Height / 2));

                Rectangle imageRectangle = new Rectangle(imagePoint, imageSize);

                graphics.SetClip(buttonPath);
                graphics.DrawImage(buttonImage, imageRectangle);
                graphics.SetClip(controlGraphicsPath);

                Border.DrawBorderStyle(graphics, buttonBorder, MouseState, buttonPath);

                TextBoxObject.Width = buttonRectangle.X - 10;
            }
            else
            {
                TextBoxObject.Width = Width - 10;
            }

            graphics.ResetClip();

            // Draw border
            if (ControlBorder.Visible)
            {
                if ((MouseState == MouseStates.Hover) && ControlBorder.HoverVisible)
                {
                    Border.DrawBorder(graphics, controlGraphicsPath, ControlBorder.Thickness, ControlBorder.HoverColor);
                }
                else
                {
                    Border.DrawBorder(graphics, controlGraphicsPath, ControlBorder.Thickness, ControlBorder.Color);
                }
            }

            if (watermark.Visible)
            {
                DrawWaterMark();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Multiline sizing
            if (TextBoxObject.Multiline)
            {
                TextBoxObject.Height = Height - 6;
            }
            else
            {
                // Auto adjust the text box height depending on the font size.
                textBoxHeight = (Convert.ToInt32(Font.Size) * 2) + 1;
            }

            // TODO Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            TextBoxObject.Text = Text;
            Invalidate();

            if (watermark.Visible)
            {
                // If the text of the text box is not empty.
                if (TextBoxObject.TextLength > 0)
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
            if ((waterMarkContainer == null) && (TextBoxObject.TextLength <= 0))
            {
                waterMarkContainer = new Panel(); // Creates the new panel instance
                waterMarkContainer.Paint += WaterMarkContainer_Paint;
                waterMarkContainer.Invalidate();
                waterMarkContainer.Click += WaterMarkContainer_Click;
                Controls.Add(waterMarkContainer); // adds the control
                waterMarkContainer.BringToFront();
            }
        }

        private void OnBaseTextBoxChanged(object s, EventArgs e)
        {
            Text = TextBoxObject.Text;
        }

        private void OnEnter(object sender, EventArgs e)
        {
            MouseState = MouseStates.Hover;
        }

        private void OnKeyDown(object obj, KeyEventArgs e)
        {
            // Select all
            if (e.Control && (e.KeyCode == Keys.A))
            {
                TextBoxObject.SelectAll();
                e.SuppressKeyPress = true;
            }

            // Copy
            if (e.Control && (e.KeyCode == Keys.C))
            {
                TextBoxObject.Copy();
                e.SuppressKeyPress = true;
            }
        }

        private void OnLeave(object sender, EventArgs e)
        {
            if (!TextBoxObject.Focused)
            {
                MouseState = MouseStates.Normal;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseState = MouseStates.Hover;
        }

        private void RemoveWaterMark()
        {
            if (waterMarkContainer != null)
            {
                Controls.Remove(waterMarkContainer);
                waterMarkContainer = null;
            }
        }

        private void WaterMarkContainer_Click(object sender, EventArgs e)
        {
            Focus();
        }

        private void WaterMarkContainer_Paint(object sender, PaintEventArgs e)
        {
            // Configure the watermark
            waterMarkContainer.Location = new Point(2, 0);
            waterMarkContainer.Height = Height;
            waterMarkContainer.Width = Width;

            // Forces it to resize with the parent control
            waterMarkContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            // Set color
            watermark.Brush = ContainsFocus ? new SolidBrush(watermark.ActiveColor) : new SolidBrush(watermark.InactiveColor);

            // Draws the string on the panel
            e.Graphics.DrawString(watermark.Text, watermark.Font, watermark.Brush, new PointF(-2f, 1f));
        }

        #endregion
    }
}