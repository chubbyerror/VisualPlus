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

    /// <summary>The visual TextBox.</summary>
    [ToolboxBitmap(typeof(TextBox)), Designer(VSDesignerBinding.VisualTextBox)]
    public class VisualTextBox : TextBox
    {
        #region  ${0} Variables

        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        public TextBox TextBoxObject = new TextBox();
        private Color backgroundColor = StylesManager.DefaultValue.Style.BackgroundColor(3);
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color controlDisabled = StylesManager.DefaultValue.Style.TextDisabled;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color textColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color textDisabled = StylesManager.DefaultValue.Style.TextDisabled;

        #endregion

        #region ${0} Properties

        public VisualTextBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);
            BorderStyle = BorderStyle.None;
            AutoSize = false;

            CreateTextBox();
            Controls.Add(TextBoxObject);
            Size = new Size(135, 19);
            BackColor = Color.Transparent;
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

                controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
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
                controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
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

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Color textTemp;
            Color controlTemp;

            // Draw control state
            if (Enabled)
            {
                textTemp = textColor;
                controlTemp = backgroundColor;
            }
            else
            {
                textTemp = textDisabled;
                controlTemp = controlDisabled;
            }

            TextBoxObject.BackColor = controlTemp;
            TextBoxObject.ForeColor = textTemp;

            // Setup internal textbox object
            TextBoxObject.Width = Width - 10;
            TextBoxObject.TextAlign = TextAlign;
            TextBoxObject.UseSystemPasswordChar = UseSystemPasswordChar;

            // Draw background backcolor
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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
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
                // Auto adjust the textbox height depending on the font size.
                Height = Convert.ToInt32(Font.Size) * 2 + 1;
            }

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            TextBoxObject.Text = Text;
            Invalidate();
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

        #endregion

        #region ${0} Methods

        public void CreateTextBox()
        {
            // BackColor = backgroundColor;
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

        public void TextBoxTextChanged(object s, EventArgs e)
        {
            TextBoxObject.Text = Text;
        }

        #endregion
    }
}