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

    /// <summary>The visual RichTextBox.</summary>
    [ToolboxBitmap(typeof(RichTextBox)), Designer(VSDesignerBinding.VisualRichTextBox), DefaultEvent("TextChanged")]
    public class VisualRichTextBox : RichTextBox
    {
        #region  ${0} Variables

        private static BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private static ControlState controlState = ControlState.Normal;
        public RichTextBox RichObject = new RichTextBox();
        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(3);
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private int borderSize = Settings.DefaultValue.BorderSize;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Color controlDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private GraphicsPath controlGraphicsPath;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = TextRenderingHint.AntiAlias;

        #endregion

        #region ${0} Properties

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
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
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

        [DefaultValue(Settings.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
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

                controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
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
                controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderSize), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderSize)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderSize = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible), Category(Localize.Category.Behavior),
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

        [Category(Localize.Category.Appearance), Description("Visual Text Renderer.")]
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

        #endregion

        #region ${0} Events

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
            graphics.TextRenderingHint = textRendererHint;

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlTempColor = Enabled ? backgroundColor : controlDisabledColor;

            RichObject.BackColor = controlTempColor;
            RichObject.ForeColor = foreColor;

            // Draw background color
            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            // Draw border
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

            graphics.SetClip(controlGraphicsPath);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RichObject.Size = new Size(Width - 13, Height - 11);
        }

        #endregion

        #region ${0} Methods

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

        #endregion
    }
}