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
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Properties;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual TextBox")]
    [Designer(ControlManager.FilterProperties.VisualTextBox)]
    public sealed class VisualTextBox : TextBox
    {
        #region Variables

        public TextBox TextBoxObject = new TextBox();

        #endregion

        #region Variables

        private readonly MouseState mouseState;

        private Color backgroundColor;
        private Border border;
        private Border buttonBorder;
        private Color buttonColor = Settings.DefaultValue.Control.FlatButtonEnabled;
        private Image buttonImage = Resources.search;
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private bool buttonVisible;
        private int buttonWidth = 19;
        private Color controlDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
        private GraphicsPath controlGraphicsPath;
        private Color foreColor;
        private Size iconSize = new Size(13, 13);
        private StyleManager styleManager = new StyleManager();
        private int textBoxHeight = 20;
        private Color textDisabledColor;
        private TextRenderingHint textRendererHint;
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

            mouseState = new MouseState(this);

            BorderStyle = BorderStyle.None;
            AutoSize = false;
            Size = new Size(135, 25);
            CreateTextBox();
            Controls.Add(TextBoxObject);
            BackColor = Color.Transparent;
            UpdateStyles();

            waterMarkContainer = null;

            ConfigureStyleManager();

            if (watermark.Visible)
            {
                DrawWaterMark();
            }
        }

        public delegate void ButtonClickedEventHandler();

        public event ButtonClickedEventHandler ButtonClicked;

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Layout)]
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

        [TypeConverter(typeof(WatermarkConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Behavior)]
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
            tb.UseSystemPasswordChar = UseSystemPasswordChar;
            tb.Multiline = false;

            TextBoxObject.KeyDown += OnKeyDown;
            TextBoxObject.TextChanged += OnBaseTextBoxChanged;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            mouseState.State = MouseStates.Hover;
            Invalidate();

            if (watermark.Visible)
            {
                // If focused use focus color
                watermark.Brush = new SolidBrush(watermark.ActiveColor);

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
            mouseState.State = MouseStates.Normal;
            Invalidate();

            if (watermark.Visible)
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

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

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
                Point imagePoint = new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - (imageSize.Width / 2), (buttonRectangle.Y + (buttonRectangle.Height / 2)) - (imageSize.Height / 2));

                Rectangle imageRectangle = new Rectangle(imagePoint, imageSize);

                graphics.SetClip(buttonPath);
                graphics.DrawImage(buttonImage, imageRectangle);
                graphics.SetClip(controlGraphicsPath);

                Border.DrawBorderStyle(graphics, buttonBorder, mouseState.State, buttonPath);

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
                if ((mouseState.State == MouseStates.Hover) && border.HoverVisible)
                {
                    Border.DrawBorder(graphics, controlGraphicsPath, border.Thickness, border.HoverColor);
                }
                else
                {
                    Border.DrawBorder(graphics, controlGraphicsPath, border.Thickness, border.Color);
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
            if (Multiline)
            {
                TextBoxObject.Height = Height - 6;
            }
            else
            {
                // Auto adjust the text box height depending on the font size.
                textBoxHeight = (Convert.ToInt32(Font.Size) * 2) + 1;
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

            if (watermark.Visible)
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
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                backgroundColor = controlStyle.Background(0);
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;
            }
            else
            {
                // Load default settings
                border = new Border();
                buttonBorder = new Border();
                backgroundColor = Settings.DefaultValue.Control.Background(3);

                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        private void DrawWaterMark()
        {
            if ((waterMarkContainer == null) && (TextLength <= 0))
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
                Height = textBoxHeight;
            }

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, border.Type, border.Rounding);
            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            buttonPath = new GraphicsPath();
            buttonPath.AddRectangle(buttonRectangle);
            buttonPath.CloseAllFigures();
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