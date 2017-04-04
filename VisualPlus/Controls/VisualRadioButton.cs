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

    /// <summary>The visual radio button.</summary>
    [ToolboxBitmap(typeof(RadioButton)), Designer(VSDesignerBinding.VisualRadioButton)]
    public class VisualRadioButton : RadioButton
    {
        #region  ${0} Variables

        private const int spacing = 2;
        private Color backgroundColor = StylesManager.DefaultValue.Style.BackgroundColor(3);
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private GraphicsPath boxGraphicsPath;
        private Point boxLocation = new Point(2, 2);
        private Size boxSize = new Size(10, 10);
        private Point checkLocation = new Point(0, 0);
        private Color checkMarkColor = StylesManager.DefaultValue.Style.MainColor;
        private Size checkSize = new Size(6, 6);
        private Color controlDisabled = StylesManager.DefaultValue.Style.ControlDisabled;
        private Color controlDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;

        #endregion

        #region ${0} Properties

        public VisualRadioButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Width = 132;
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color CheckMark
        {
            get
            {
                return checkMarkColor;
            }

            set
            {
                checkMarkColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
        public Color ControlDisabledColorColor
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

        #endregion

        #region ${0} Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Checked)
            {
                Checked = true;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // CheckMark background color
            graphics.FillPath(new SolidBrush(backgroundColor), boxGraphicsPath);

            // Draw border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, boxGraphicsPath, borderSize, borderColor, borderHoverColor, borderHoverVisible);
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlCheckTemp = Enabled ? checkMarkColor : controlDisabled;

            // Draw an ellipse inside the body
            if (Checked)
            {
                graphics.FillEllipse(new SolidBrush(controlCheckTemp), new Rectangle(checkLocation, checkSize));
            }

            // Draw the string specified in 'Text' property
            Point textPoint = new Point(boxLocation.X + boxSize.Width + spacing, boxSize.Height / 2 - (int)Font.Size / 2);

            StringFormat stringFormat = new StringFormat();

            // stringFormat.Alignment = StringAlignment.Center;
            // stringFormat.LineAlignment = StringAlignment.Center;
            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textPoint, stringFormat);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        private void UpdateLocationPoints()
        {
            boxGraphicsPath = new GraphicsPath();
            boxGraphicsPath.AddEllipse(boxLocation.X, boxLocation.Y, boxSize.Width, boxSize.Height);
            boxGraphicsPath.CloseAllFigures();

            // Centers the check location according to the checkbox
            Rectangle box = new Rectangle(boxLocation.X, boxLocation.Y, boxSize.Width, boxSize.Height);

            Rectangle check = new Rectangle(box.X, box.Y, checkSize.Width / 2, checkSize.Height / 2);
            check = check.AlignCenterX(box);
            check = check.AlignCenterY(box);
            checkLocation = new Point(check.X, check.Y);
        }

        #endregion
    }
}