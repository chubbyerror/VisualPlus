namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RichTextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual RichTextBox")]
    [Designer(ControlManager.FilterProperties.VisualRichTextBox)]
    public sealed class VisualRichTextBox : RichTextBox
    {
        #region Variables

        public RichTextBox RichObject = new RichTextBox();

        #endregion

        #region Variables

        private readonly MouseState mouseState;

        private Color backgroundColor;
        private Color backgroundDisabledColor;
        private Border border;
        private GraphicsPath controlGraphicsPath;
        private Color foreColor;
        private StyleManager styleManager = new StyleManager();
        private Color textDisabledColor;

        #endregion

        #region Constructors

        public VisualRichTextBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            mouseState = new MouseState(this);

            CreateRichTextBox();
            Controls.Add(RichObject);
            BackColor = Color.Transparent;
            Size = new Size(150, 100);
            WordWrap = true;
            AutoWordSelection = false;
            BorderStyle = BorderStyle.None;
            TextChanged += TextBoxTextChanged;
            UpdateStyles();

            ConfigureStyleManager();
        }

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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color BackgroundDisabledColor
        {
            get
            {
                return backgroundDisabledColor;
            }

            set
            {
                backgroundDisabledColor = value;
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
        public override string Text
        {
            get
            {
                return RichObject.Text;
            }

            set
            {
                RichObject.Text = value;
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

        #endregion

        #region Events

        public void CreateRichTextBox()
        {
            // BackColor = Color.Transparent;
            RichTextBox rtb = RichObject;
            rtb.BackColor = backgroundColor;
            rtb.ForeColor = ForeColor;
            rtb.Size = new Size(Width - 10, 100);
            rtb.Location = new Point(7, 5);
            rtb.Text = string.Empty;
            rtb.BorderStyle = BorderStyle.None;
            rtb.Font = Font;
            rtb.Multiline = true;
        }

        public void TextBoxTextChanged(object s, EventArgs e)
        {
            RichObject.Text = Text;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RichObject.Font = Font;
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            RichObject.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            mouseState.State = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlTempColor = Enabled ? backgroundColor : backgroundDisabledColor;

            RichObject.BackColor = controlTempColor;
            RichObject.ForeColor = foreColor;

            graphics.FillPath(new SolidBrush(controlTempColor), controlGraphicsPath);

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

            graphics.SetClip(controlGraphicsPath);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, border.Type, border.Rounding);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RichObject.Size = new Size(Width - 13, Height - 11);
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
                backgroundDisabledColor = controlStyle.FlatButtonDisabled;
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;
            }
            else
            {
                // Load default settings
                border = new Border();

                backgroundColor = Settings.DefaultValue.Control.Background(3);
                backgroundDisabledColor = Settings.DefaultValue.Control.FlatButtonDisabled;

                Font = Settings.DefaultValue.DefaultFont;
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        #endregion
    }
}