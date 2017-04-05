namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Localization;

    public enum ProgressShape
    {
        /// <summary>Rectangle shape.</summary>
        Flat,

        /// <summary>Round shape.</summary>
        Round
    }

    /// <summary>The visual CircleProgressBar.</summary>
    [ToolboxBitmap(typeof(ProgressBar)), Designer(VSDesignerBinding.VisualCircleProgressBar)]
    public sealed class VisualCircleProgressBar : ProgressBar
    {
        #region  ${0} Variables

        private Color backgroundCircleColor = StylesManager.DefaultValue.Style.BackgroundProgressCircle;
        private bool backgroundCircleVisible = true;
        private Color foregroundCircleColor = StylesManager.DefaultValue.Style.ForegroundProgressCircle;
        private bool foregroundCircleVisible = true;
        private float gradientRotation;
        private Color progressGradient1 = StylesManager.DefaultValue.Style.ProgressColor;
        private Color progressGradient2 = ControlPaint.LightLight(StylesManager.DefaultValue.Style.ProgressColor);
        private ProgressShape progressShapeVal = ProgressShape.Round;
        private float progressSize = 5F;
        private bool textVisible;

        #endregion

        #region ${0} Properties

        public VisualCircleProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            MinimumSize = new Size(100, 100);
            Size = new Size(130, 130);
            ForeColor = StylesManager.DefaultValue.Style.ForeColor(0);
            textVisible = true;
            BackColor = Color.Transparent;

            UpdateStyles();
        }

        [DefaultValue(true), Category(Localize.Category.Appearance), Description(Localize.Description.ComponentVisible)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [DefaultValue(true), Category(Localize.Category.Appearance), Description(Localize.Description.ComponentVisible)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Layout), Description(Localize.Description.Rotation)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentShape)]
        public ProgressShape ProgressShape
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

        [DefaultValue(StylesManager.DefaultValue.ProgressSize), Category(Localize.Category.Layout), Description(Localize.Description.ProgressSize)]
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

        [DefaultValue(StylesManager.DefaultValue.TextVisible), Category(Localize.Category.Appearance), Description(Localize.Description.TextVisible)]
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

        #region ${0} Events

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;

            if (backgroundCircleVisible)
            {
                // Draw background circle
                graphics.FillEllipse(new SolidBrush(backgroundCircleColor), 0x18 - 4, 0x18 - 4, Width - 0x30 + 8, Height - 0x30 + 8);
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
                    graphics.DrawArc(progressPen, 0x18 - 2, 0x18 - 2, Width - 0x30 + 4, Height - 0x30 + 4, -90,
                        (int)Math.Round(360.0 / Maximum * Value));
                }
            }

            if (foregroundCircleVisible)
            {
                // Draw foreground circle
                graphics.FillEllipse(new SolidBrush(foregroundCircleColor), 0x18, 0x18, Width - 0x30, Height - 0x30);
            }

            // String percentage
            if (textVisible)
            {
                SizeF measuredString = new SizeF();

                // String setup
                measuredString = graphics.MeasureString(Convert.ToString(Convert.ToInt32(100 / Maximum * Value)), Font);

                // Draw value string
                Point textPoint = new Point(Convert.ToInt32(Width / 2 - measuredString.Width / 2),
                    Convert.ToInt32(Height / 2 - measuredString.Height / 2));
                string stringValue = Convert.ToString(Convert.ToInt32(100 / Maximum * Value)) + @"%";

                StringFormat stringFormat = new StringFormat();

                // stringFormat.Alignment = StringAlignment.Center;
                // stringFormat.LineAlignment = StringAlignment.Center;
                graphics.DrawString(stringValue, Font, new SolidBrush(ForeColor), textPoint, stringFormat);
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

        private void SetStandardSize()
        {
            Size = new Size(Math.Max(Width, Height), Math.Max(Width, Height));
        }

        #endregion

        #region ${0} Methods

        public void Decrement(int val)
        {
            Value -= val;
            Invalidate();
        }

        #endregion
    }
}