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
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ProgressBar))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual Circle ProgressBar")]
    [Designer(DesignManager.VisualCircleProgressBar)]
    public sealed class VisualCircleProgressBar : ProgressBar
    {
        #region Variables

        private Color backgroundCircleColor = Settings.DefaultValue.Style.BackgroundProgressCircle;
        private bool backgroundCircleVisible = true;
        private Font font = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Regular);
        private Color foregroundCircleColor = Settings.DefaultValue.Style.ForegroundProgressCircle;
        private bool foregroundCircleVisible = true;
        private float gradientRotation;
        private Image icon;
        private GraphicsPath iconGraphicsPath;
        private Point iconPoint;
        private Rectangle iconRectangle;
        private Size iconSize = new Size(16, 16);
        private Color progressGradient1 = Settings.DefaultValue.Style.ProgressColor;
        private Color progressGradient2 = ControlPaint.LightLight(Settings.DefaultValue.Style.ProgressColor);
        private ProgressShape progressShapeVal = ProgressShape.Round;
        private float progressSize = 5F;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private bool textVisible;

        #endregion

        #region Constructors

        public VisualCircleProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            MinimumSize = new Size(100, 100);

            ForeColor = Color.White;
            textVisible = true;
            BackColor = Color.Transparent;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            UpdateStyles();

            // Attempt to center icon
            iconPoint = new Point((Width / 2) - (iconRectangle.Width / 2), (Height / 2) - (iconRectangle.Height / 2));
        }

        public enum ProgressShape
        {
            /// <summary>Rectangle shape.</summary>
            Flat,

            /// <summary>Round shape.</summary>
            Round
        }

        #endregion

        #region Properties

        [DefaultValue(true)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool BackCircleVisible
        {
            get
            {
                return backgroundCircleVisible;
            }

            set
            {
                backgroundCircleVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color BackgroundCircle
        {
            get
            {
                return backgroundCircleColor;
            }

            set
            {
                backgroundCircleColor = value;
                Invalidate();
            }
        }

        public new Font Font
        {
            get
            {
                return font;
            }

            set
            {
                base.Font = value;
                font = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ForeCircleVisible
        {
            get
            {
                return foregroundCircleVisible;
            }

            set
            {
                foregroundCircleVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ForegroundCircle
        {
            get
            {
                return foregroundCircleColor;
            }

            set
            {
                foregroundCircleColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Rotation)]
        public float GradientRotation
        {
            get
            {
                return gradientRotation;
            }

            set
            {
                gradientRotation = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Icon)]
        public Image Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
                if (AutoSize)
                {
                    Size = GetPreferredSize();
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.IconPosition)]
        public Point IconPoint
        {
            get
            {
                return iconPoint;
            }

            set
            {
                iconPoint = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.IconSize)]
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
        [Description(Localize.Description.ComponentColor)]
        public Color ProgressGradient1
        {
            get
            {
                return progressGradient1;
            }

            set
            {
                progressGradient1 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ProgressGradient2
        {
            get
            {
                return progressGradient2;
            }

            set
            {
                progressGradient2 = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.ProgressSize)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ProgressSize)]
        public float ProgressSize
        {
            get
            {
                return progressSize;
            }

            set
            {
                progressSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public ProgressShape Shape
        {
            get
            {
                return progressShapeVal;
            }

            set
            {
                progressShapeVal = value;
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

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextVisible)]
        public bool TextVisible
        {
            get
            {
                return textVisible;
            }

            set
            {
                textVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        public void Decrement(int val)
        {
            Value -= val;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            if (backgroundCircleVisible)
            {
                // Draw background circle
                graphics.FillEllipse(new SolidBrush(backgroundCircleColor), progressSize, progressSize, Width - progressSize - 1, Height - progressSize - 1);
            }

            // Progress
            using (LinearGradientBrush progressBrush = new LinearGradientBrush(
                ClientRectangle,
                progressGradient1,
                progressGradient2,
                gradientRotation))
            {
                // Shape
                using (Pen progressPen = new Pen(progressBrush, progressSize))
                {
                    switch (progressShapeVal)
                    {
                        case ProgressShape.Round:
                            {
                                progressPen.StartCap = LineCap.Round;
                                progressPen.EndCap = LineCap.Round;
                                break;
                            }

                        case ProgressShape.Flat:
                            {
                                progressPen.StartCap = LineCap.Flat;
                                progressPen.EndCap = LineCap.Flat;
                                break;
                            }
                    }

                    // Draw progress
                    graphics.DrawArc(progressPen, progressSize + 2, progressSize + 2, Width - (progressSize * 2), Height - (progressSize * 2), -90, (int)Math.Round((360.0 / Maximum) * Value));
                }
            }

            if (foregroundCircleVisible)
            {
                // Draw foreground circle
                graphics.FillEllipse(new SolidBrush(foregroundCircleColor), progressSize + 4, progressSize + 4, Width - progressSize - 10, Height - progressSize - 10);
            }

            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(iconRectangle);
            iconGraphicsPath.CloseAllFigures();

            if (Icon != null)
            {
                // Point iconPoint = new Point(Width / 2 - iconRectangle.Width / 2, Height / 2 - iconRectangle.Height / 2);
                iconRectangle = new Rectangle(iconPoint, iconSize);

                // Draw icon
                graphics.DrawImage(Icon, iconRectangle);
            }

            // String percentage
            if (textVisible)
            {
                SizeF measuredString = graphics.MeasureString(Convert.ToString(Convert.ToInt32((100 / Maximum) * Value)), Font);
                Point textPoint = new Point(Convert.ToInt32((Width / 2) - (measuredString.Width / 2)), Convert.ToInt32((Height / 2) - (measuredString.Height / 2)));

                string stringValue = Convert.ToString(Convert.ToInt32((100 / Maximum) * Value)) + @"%";

                graphics.DrawString(stringValue, font, new SolidBrush(ForeColor), textPoint);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetStandardSize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
        }

        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }

        private void SetStandardSize()
        {
            Size = new Size(Math.Max(Width, Height), Math.Max(Width, Height));
        }

        #endregion
    }
}