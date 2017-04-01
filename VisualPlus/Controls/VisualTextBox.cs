namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Styles;
    using VisualPlus.Localization;

    /// <summary>The visual TextBox.</summary>
    [ToolboxBitmap(typeof(TextBox)), Designer(VSDesignerBinding.VisualTextBox)]
    public class VisualTextBox : TextBox
    {
        #region  ${0} Variables

        private static readonly IStyle Style = new Visual();
        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        public TextBox TextBoxObject = new TextBox();
        private Color backgroundColor1 = Style.BackgroundColor(3);
        private Color borderColor = Style.BorderColor(0);
        private Color borderHoverColor = Style.BorderColor(1);
        private bool borderHoverVisible = true;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;

        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = true;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;

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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor1
        {
            get
            {
                return backgroundColor1;
            }

            set
            {
                backgroundColor1 = value;
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
            Bitmap bitmap = new Bitmap(Width, Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);

            // Setup internal textbox object
            TextBoxObject.Width = Width - 10;
            TextBoxObject.TextAlign = TextAlign;
            TextBoxObject.UseSystemPasswordChar = UseSystemPasswordChar;
            TextBoxObject.BackColor = backgroundColor1;

            // Draw background backcolor
            graphics.FillPath(new SolidBrush(backgroundColor1), controlGraphicsPath);

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

            e.Graphics.DrawImage((Image)bitmap.Clone(), 0, 0);
            graphics.Dispose();
            bitmap.Dispose();
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
            // BackColor = backgroundColor1;
            TextBox tb = TextBoxObject;
            tb.Size = new Size(Width, Height);
            tb.Location = new Point(5, 2);
            tb.Text = string.Empty;
            tb.BorderStyle = BorderStyle.None;
            tb.TextAlign = HorizontalAlignment.Left;
            tb.Font = Font;
            tb.ForeColor = ForeColor;
            tb.BackColor = backgroundColor1;
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