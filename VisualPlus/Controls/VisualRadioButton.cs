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
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual radio button.</summary>
    [ToolboxBitmap(typeof(RadioButton))]
    [Designer(VSDesignerBinding.VisualRadioButton)]
    public sealed class VisualRadioButton : RadioButton
    {
        #region Variables

        private const int Spacing = 2;

        private Color[] backgroundColor =
        {
            ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3)),
            Settings.DefaultValue.Style.BackgroundColor(3),
            ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3))
        };

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private GraphicsPath boxGraphicsPath;
        private Point boxLocation = new Point(2, 2);
        private Size boxSize = new Size(10, 10);
        private Point checkLocation = new Point(0, 0);
        private Color checkMarkColor = Settings.DefaultValue.Style.StyleColor;
        private Size checkSize = new Size(6, 6);
        private Color controlDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Point backroundStartPoint;
        private Point backroundEndPoint;
        private float gradientBackgroundAngle;
        private LinearGradientBrush gradientBackgroundBrush;
        private float[] gradientBackgroundPosition = { 0, 1 / 2f, 1 };

        #endregion

        #region Constructors

        public VisualRadioButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Width = 132;
            UpdateStyles();
            Cursor = Cursors.Hand;
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] BackgroundColor
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
        [Description(Localize.Description.BorderColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderHoverColor)]
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

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderHoverVisible)]
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

        [DefaultValue(Settings.DefaultValue.BorderThickness)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderThickness)]
        public int BorderThickness
        {
            get
            {
                return borderThickness;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderThickness = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderVisible)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientBackroundAngle
        {
            get
            {
                return gradientBackgroundAngle;
            }

            set
            {
                gradientBackgroundAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientBackgroundPosition
        {
            get
            {
                return gradientBackgroundPosition;
            }

            set
            {
                gradientBackgroundPosition = value;
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

        #endregion

        #region Events

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
            graphics.TextRenderingHint = textRendererHint;

            // gradient
            gradientBackgroundBrush = GDI.CreateGradientBrush(backgroundColor, gradientBackgroundPosition, gradientBackgroundAngle, backroundStartPoint, backroundEndPoint);

            // CheckMark background color
            graphics.FillPath(gradientBackgroundBrush, boxGraphicsPath);

            // Draw border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, boxGraphicsPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlCheckTemp = Enabled ? checkMarkColor : controlDisabledColor;

            // Draw an ellipse inside the body
            if (Checked)
            {
                graphics.FillEllipse(new SolidBrush(controlCheckTemp), new Rectangle(checkLocation, checkSize));
            }

            // Draw the string specified in 'Text' property
            Point textPoint = new Point(boxLocation.X + boxSize.Width + Spacing, boxSize.Height / 2 - (int)Font.Size / 2);

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

            // points
            backroundStartPoint = new Point(box.Width, 0);
            backroundEndPoint = new Point(box.Width, box.Height);
        }

        #endregion
    }
}