namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Managers;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ProgressBar))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual Circle ProgressBar")]
    [Designer(ControlManager.FilterProperties.VisualCircleProgressBar)]
    public class VisualCircleProgressBar : ProgressBase
    {
        #region Variables

        private Color backgroundCircleColor;
        private bool backgroundCircleVisible = true;
        private Color foregroundCircleColor;
        private bool foregroundCircleVisible = true;
        private float gradientRotation;
        private Point iconPoint;
        private Rectangle iconRectangle;
        private Size iconSize = new Size(16, 16);
        private Image image;
        private GraphicsPath imagePath;
        private Color progressGradient1;
        private Color progressGradient2;
        private ProgressShape progressShapeVal = ProgressShape.Round;
        private float progressSize = 5F;
        private bool textVisible;

        #endregion

        #region Constructors

        public VisualCircleProgressBar()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            MinimumSize = new Size(100, 100);
            Maximum = 100;
            textVisible = true;

            // Attempt to center icon
            iconPoint = new Point((Width / 2) - (iconRectangle.Width / 2), (Height / 2) - (iconRectangle.Height / 2));

            backgroundCircleColor = StyleManager.ProgressStyle.BackCircle;
            foregroundCircleColor = StyleManager.ProgressStyle.ForeCircle;

            progressGradient1 = StyleManager.ProgressStyle.Progress.Colors[0];
            progressGradient2 = StyleManager.ProgressStyle.Progress.Colors[1];
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
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [DefaultValue(true)]
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Rotation)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Image)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Image)]
        public Image Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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
        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
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

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            DrawCircles(graphics);
            DrawImage(graphics);
            DrawText(graphics);
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

        private void DrawCircles(Graphics graphics)
        {
            if (backgroundCircleVisible)
            {
                // Draw background circle
                graphics.FillEllipse(new SolidBrush(backgroundCircleColor), progressSize, progressSize, Width - progressSize - 1, Height - progressSize - 1);
            }

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
        }

        private void DrawImage(Graphics graphics)
        {
            imagePath = new GraphicsPath();
            imagePath.AddRectangle(iconRectangle);
            imagePath.CloseAllFigures();

            if (Image != null)
            {
                // Point iconPoint = new Point(Width / 2 - iconRectangle.Width / 2, Height / 2 - iconRectangle.Height / 2);
                iconRectangle = new Rectangle(iconPoint, iconSize);
                graphics.DrawImage(Image, iconRectangle);
            }
        }

        private void DrawText(Graphics graphics)
        {
            if (textVisible)
            {
                SizeF measuredString = graphics.MeasureString(Convert.ToString(Convert.ToInt32((100 / Maximum) * Value)), Font);
                Point textPoint = new Point(Convert.ToInt32((Width / 2) - (measuredString.Width / 2)), Convert.ToInt32((Height / 2) - (measuredString.Height / 2)));
                string stringValue = Convert.ToString(Convert.ToInt32((100 / Maximum) * Value)) + @"%";
                graphics.DrawString(stringValue, Font, new SolidBrush(ForeColor), textPoint);
            }
        }

        private void SetStandardSize()
        {
            Size = new Size(Math.Max(Width, Height), Math.Max(Width, Height));
        }

        #endregion
    }
}