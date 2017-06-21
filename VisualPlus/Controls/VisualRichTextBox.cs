namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
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
    [ToolboxBitmap(typeof(RichTextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual RichTextBox")]
    [Designer(DesignManager.VisualRichTextBox)]
    public sealed class VisualRichTextBox : RichTextBox
    {
        #region Variables

        public RichTextBox RichObject = new RichTextBox();

        #endregion

        #region Variables

        private Color backgroundColor;
        private Border border = new Border();
        private Color controlDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
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

        [TypeConverter(typeof(VisualStyleManagerConverter))]
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
            controlState = ControlState.Hover;
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
            controlState = ControlState.Normal;
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
            Color controlTempColor = Enabled ? backgroundColor : controlDisabledColor;

            RichObject.BackColor = controlTempColor;
            RichObject.ForeColor = foreColor;

            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            if (border.Visible)
            {
                if ((controlState == ControlState.Hover) && border.HoverVisible)
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, border.Thickness, border.HoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, border.Thickness, border.Color);
                }
            }

            graphics.SetClip(controlGraphicsPath);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
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
                border.Shape = styleManager.VisualStylesManager.BorderShape;
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
                border.HoverVisible = Settings.DefaultValue.BorderHoverVisible;
                border.Rounding = Settings.DefaultValue.Rounding.Default;
                border.Shape = Settings.DefaultValue.BorderShape;
                border.Thickness = Settings.DefaultValue.BorderThickness;
                border.Visible = Settings.DefaultValue.BorderVisible;
                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                backgroundColor = Settings.DefaultValue.Control.Background(3);
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        #endregion
    }
}