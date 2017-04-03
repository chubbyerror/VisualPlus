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
    using VisualPlus.Localization;

    public enum ProgressBarTypes
    {
        /// <summary>Bars type.</summary>
        Bars,

        /// <summary>The horizontal progressbar.</summary>
        Horizontal,

        /// <summary>The vertical progressbar.</summary>
        Vertical,

        /// <summary>Rating type.</summary>
        Rating,

        /// <summary>Signal type.</summary>
        Signal
    }

    /// <summary>The visual ProgressBar.</summary>
    [ToolboxBitmap(typeof(ProgressBar)), Designer(VSDesignerBinding.VisualProgressBar)]
    public class VisualProgressBar : ProgressBar
    {
        #region  ${0} Variables

        private static int backgroundRotation = 90;
        private static int bars = 5;
        private static int barSpacing = 10;
        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private static Color hatchBackColor = StylesManager.DefaultValue.Style.HatchColor;
        private static ProgressBarTypes progressBarStyle = ProgressBarTypes.Horizontal;
        private static Color progressColor1 = StylesManager.DefaultValue.Style.ProgressColor;
        private static int progressRotation;
        private bool backColorGradient = true;
        private Color backgroundColor1 = StylesManager.DefaultValue.Style.BackgroundColor(0);
        private Color backgroundColor2 = StylesManager.DefaultValue.Style.BackgroundColor(1);
        private Point barLocation = new Point(0, 0);
        private Point barSize = new Point(15, 15);
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private ControlState controlState = ControlState.Normal;
        private GraphicsPath graphicsDefaultBorderPath;
        private Color hatchForeColor = Color.FromArgb(40, hatchBackColor);
        private GraphicsPath hatchPath = new GraphicsPath();
        private float hatchSize = StylesManager.DefaultValue.HatchSize;
        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;
        private bool hatchVisible = StylesManager.DefaultValue.HatchVisible;
        private bool percentageVisible;
        private Color progressColor2 = ControlPaint.Light(progressColor1);
        private BrushType progressColorStyle = BrushType.Gradient;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private StringAlignment valueAlignment = StringAlignment.Center;

        #endregion

        #region ${0} Properties

        public VisualProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Maximum = 100;
            Width = 100;
            Height = 20;
            percentageVisible = true;
            BackColor = Color.Transparent;
            ForeColor = foreColor;
            DoubleBuffered = true;
            UpdateStyles();
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor2
        {
            get
            {
                return backgroundColor2;
            }

            set
            {
                backgroundColor2 = value;
                Invalidate();
            }
        }

        [DefaultValue(true), Category(Localize.Category.Behavior)]
        public bool BackgroundGradient
        {
            get
            {
                return backColorGradient;
            }

            set
            {
                backColorGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description(Localize.Description.Rotation)]
        public int BackgroundRotation
        {
            get
            {
                return backgroundRotation;
            }

            set
            {
                backgroundRotation = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BarAmount), Category(Localize.Category.Behavior), Description(Localize.Description.BarAmount)]
        public int BarAmount
        {
            get
            {
                return bars;
            }

            set
            {
                bars = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description(Localize.Description.BarSize)]
        public Point BarSize
        {
            get
            {
                return barSize;
            }

            set
            {
                barSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description(Localize.Description.BarSpacing)]
        public int BarSpacing
        {
            get
            {
                return barSpacing;
            }

            set
            {
                barSpacing = value;
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

                graphicsDefaultBorderPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
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
                graphicsDefaultBorderPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
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
        public Color HatchBackColor
        {
            get
            {
                return hatchBackColor;
            }

            set
            {
                hatchBackColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color HatchForeColor
        {
            get
            {
                return hatchForeColor;
            }

            set
            {
                hatchForeColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), DefaultValue(StylesManager.DefaultValue.HatchSize), Description(Localize.Description.HatchSize)]
        public float HatchSize
        {
            get
            {
                return hatchSize;
            }

            set
            {
                hatchSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.HatchStyle)]
        public HatchStyle HatchStyle
        {
            get
            {
                return hatchStyle;
            }

            set
            {
                hatchStyle = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.HatchVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.ComponentVisible)]
        public bool HatchVisible
        {
            get
            {
                return hatchVisible;
            }

            set
            {
                hatchVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.TextVisible), Category(Localize.Category.Appearance), Description(Localize.Description.TextVisible)]
        public bool PercentageVisible
        {
            get
            {
                return percentageVisible;
            }

            set
            {
                percentageVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior), Description(Localize.Description.ProgressBarStyle)]
        public ProgressBarTypes ProgressBarStyle
        {
            get
            {
                return progressBarStyle;
            }

            set
            {
                progressBarStyle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color ProgressColor1
        {
            get
            {
                return progressColor1;
            }

            set
            {
                progressColor1 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color ProgressColor2
        {
            get
            {
                return progressColor2;
            }

            set
            {
                progressColor2 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior), Description(Localize.Description.ComponentColor)]
        public BrushType ProgressColorStyle
        {
            get
            {
                return progressColorStyle;
            }

            set
            {
                progressColorStyle = value;
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

        [Category(Localize.Category.Layout), Description(Localize.Description.Alignment)]
        public StringAlignment ValueAlignment
        {
            get
            {
                return valueAlignment;
            }

            set
            {
                valueAlignment = value;
                Invalidate();
            }
        }

        #endregion

        #region ${0} Events

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
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (progressBarStyle == ProgressBarTypes.Horizontal || progressBarStyle == ProgressBarTypes.Vertical)
            {
                // Draw default progress
                DrawDefaultProgress(progressBarStyle, graphics);
            }
            else
            {
                // Draw styled progress
                SetStyleSettings(progressBarStyle);

                // Draw total bars
                DrawStyledProgress(graphics, bars, false);

                SetStyleSettings(progressBarStyle);

                // Draw current progressbars
                DrawStyledProgress(graphics, MathHelper.GetFactor(Convert.ToDouble(Value), bars), true);
            }

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.ResetClip();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            switch (progressBarStyle)
            {
                case ProgressBarTypes.Bars:
                    {
                        Height = barSize.Y;
                        Size minimumSize = new Size(bars * barSize.X, barSize.Y + 2);
                        MinimumSize = minimumSize;
                        break;
                    }

                case ProgressBarTypes.Horizontal:
                    {
                        Size minimumSize = new Size(60, 10);
                        MinimumSize = minimumSize;
                        break;
                    }

                case ProgressBarTypes.Rating:
                    {
                        Size minimumSize = new Size(bars * barSize.X, barSize.Y + 2);
                        MinimumSize = minimumSize;
                        break;
                    }

                case ProgressBarTypes.Signal:
                    {
                        Size minimumSize = new Size(60, 20);
                        MinimumSize = minimumSize;
                        break;
                    }

                case ProgressBarTypes.Vertical:
                    {
                        Size minimumSize = new Size(10, 60);
                        MinimumSize = minimumSize;
                        break;
                    }
            }
        }

        private void DrawDefaultProgress(ProgressBarTypes style, Graphics graphics)
        {
            graphicsDefaultBorderPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
            GraphicsPath progressPath = null;
            Rectangle backgroundRect = new Rectangle();

            // Declare progress.Value
            // int intValue = Convert.ToInt32(Convert.ToDouble(Value) / Convert.ToDouble(Maximum) * Width);
            var i1 = new int();

            switch (style)
            {
                case ProgressBarTypes.Horizontal:
                    {
                        i1 = (int)Math.Round((Value - Minimum) / (double)(Maximum - Minimum) * (Width - 2));
                        progressRotation = 0;

                        // Progress path
                        if (borderShape == BorderShape.Rectangle)
                        {
                            progressPath = new GraphicsPath();
                            progressPath.AddRectangle(new Rectangle(0, 0, i1 + 1, Height));
                            progressPath.CloseAllFigures();
                        }
                        else
                        {
                            progressPath = GDI.DrawRoundedRectangle(new Rectangle(1, 1, i1, Height - 2), borderRounding);
                        }

                        backgroundRect = new Rectangle(1, 1, i1, Height - 3);
                    }

                    break;
                case ProgressBarTypes.Vertical:
                    {
                        i1 = (int)Math.Round((Value - Minimum) / (double)(Maximum - Minimum) * (Height - 2));
                        progressRotation = -90;

                        // Progress path
                        if (borderShape == BorderShape.Rectangle)
                        {
                            progressPath = new GraphicsPath();
                            progressPath.AddRectangle(new Rectangle(0, Height - i1 - 2, Width, i1));
                            progressPath.CloseAllFigures();
                        }
                        else
                        {
                            progressPath = GDI.DrawRoundedRectangle(new Rectangle(0, Height - i1 - 2, Width, i1), borderRounding);
                        }

                        backgroundRect = new Rectangle(1, 1, i1, Height - 3);
                    }

                    break;
            }

            // Draw background
            GDI.FillBackground(
                graphics,
                ClientRectangle,
                graphicsDefaultBorderPath,
                backgroundColor1,
                backgroundColor2,
                backgroundRotation,
                backColorGradient);

            // Draw progress
            if (i1 > 1)
            {
                // Draw progress
                if (progressColorStyle == BrushType.Gradient)
                {
                    // Draw gradient progress
                    graphics.FillPath(new LinearGradientBrush(backgroundRect, progressColor1, progressColor2, progressRotation), progressPath);
                }
                else
                {
                    // Solid color progress
                    graphics.FillPath(new SolidBrush(progressColor1), progressPath);
                }

                hatchPath = progressPath;

                // Toggle hatch
                if (hatchVisible)
                {
                    HatchBrush hatchBrush = new HatchBrush(hatchStyle, hatchForeColor, hatchBackColor);
                    using (TextureBrush textureBrush = GDI.DrawTextureUsingHatch(hatchBrush))
                    {
                        textureBrush.ScaleTransform(hatchSize, hatchSize);
                        graphics.FillPath(textureBrush, hatchPath);
                    }
                }

                graphics.SetClip(progressPath);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.ResetClip();
            }

            // Draw border
            if (borderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, graphicsDefaultBorderPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, graphicsDefaultBorderPath, borderSize, borderColor);
                }
            }

            // Draw value as a string
            string percentValue = Convert.ToString(Convert.ToInt32(Value)) + "%";

            // Location
            var textX = (int)(Width - graphics.MeasureString(percentValue, Font).
                                               Width - 1);
            int textY = Height / 2 - (Convert.ToInt32(graphics.MeasureString(percentValue, Font).
                                                               Height / 2) - 2);

            // Toggle percentage
            if (percentageVisible)
            {
                StringFormat stringFormat = new StringFormat
                    {
                        Alignment = valueAlignment,
                        LineAlignment = StringAlignment.Center
                    };

                graphics.DrawString(
                    percentValue,
                    Font,
                    new SolidBrush(foreColor),
                    new Rectangle(0, 0, Width, Height + 2),
                    stringFormat);
            }
        }

        /// <summary>Draw styled progressbar.</summary>
        /// <param name="graphics">Graphics processor.</param>
        /// <param name="barCount">Amount of bars.</param>
        /// <param name="colored">Toggle coloring.</param>
        private void DrawStyledProgress(Graphics graphics, int barCount, bool colored)
        {
            GraphicsPath barStyle = new GraphicsPath();

            for (var i = 0; i < barCount; i++)
            {
                // Move the bar right
                if (i != 0)
                {
                    barLocation = new Point(barLocation.X + barSpacing, barLocation.Y);
                }

                // Create Bar
                switch (progressBarStyle)
                {
                    case ProgressBarTypes.Bars:
                        {
                            // Create bars
                            if (borderShape == BorderShape.Rounded)
                            {
                                // Rounded rectangle - makes it possible to make circles with full roundness.
                                barStyle.AddPath(
                                    GDI.DrawRoundedRectangle(barLocation.X, barLocation.Y, barSize.X, barSize.Y, borderRounding),
                                    true);
                            }
                            else
                            {
                                // Rectangle
                                barStyle.AddRectangle(new Rectangle(barLocation.X, barLocation.Y, barSize.X, barSize.Y));
                            }

                            barStyle.CloseAllFigures();
                            break;
                        }

                    case ProgressBarTypes.Horizontal:
                        {
                            // Default progress bar
                            barStyle = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
                            break;
                        }

                    case ProgressBarTypes.Rating:
                        {
                            // Create rating bar
                            barStyle.AddPolygon(GDI.Calculate5PointStar(barLocation, 10, 5));
                            barStyle.CloseAllFigures();

                            break;
                        }

                    case ProgressBarTypes.Signal:
                        {
                            break;
                        }

                    case ProgressBarTypes.Vertical:
                        {
                            barStyle = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Draw shape
                if (colored)
                {
                    GDI.FillBackground(
                        graphics,
                        ClientRectangle,
                        barStyle,
                        progressColor1,
                        progressColor2,
                        backgroundRotation,
                        backColorGradient);
                }
                else
                {
                    GDI.FillBackground(
                        graphics,
                        ClientRectangle,
                        barStyle,
                        backgroundColor1,
                        backgroundColor2,
                        backgroundRotation,
                        backColorGradient);
                }

                // Draw border
                if (borderVisible)
                {
                    if (controlState == ControlState.Hover && borderHoverVisible)
                    {
                        GDI.DrawBorder(graphics, barStyle, borderSize, borderHoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, barStyle, borderSize, borderColor);
                    }
                }
            }
        }

        /// <summary>Sets the style settings.</summary>
        /// <param name="style">Current ProgressBarType.</param>
        private void SetStyleSettings(ProgressBarTypes style)
        {
            switch (style)
            {
                case ProgressBarTypes.Bars:
                    {
                        barLocation = new Point(0, 0);
                        barSize = new Point(10, 10);
                        barSpacing = 15;
                        break;
                    }

                case ProgressBarTypes.Horizontal:
                    {
                        break;
                    }

                case ProgressBarTypes.Rating:
                    {
                        barLocation = new Point(10, 10);
                        barSpacing = 25;
                        break;
                    }

                case ProgressBarTypes.Signal:
                    {
                        break;
                    }

                case ProgressBarTypes.Vertical:
                    {
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        #endregion

        #region ${0} Methods

        /// <summary>Decreases the progress value.</summary>
        /// <param name="curValue">Value amount.</param>
        public void Deincrement(int curValue)
        {
            Value -= curValue;
            Invalidate();
        }

        #endregion
    }
}