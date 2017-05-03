namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Components.Symbols;
    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual ProgressBar.</summary>
    [ToolboxBitmap(typeof(ProgressBar))]
    [Designer(VSDesignerBinding.VisualProgressBar)]
    public sealed class VisualProgressBar : ProgressBar
    {
        #region Variables

        private static int bars = 5;
        private static int barSpacing = 10;
        private static BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private static Color hatchBackColor = Settings.DefaultValue.Style.HatchColor;
        private static ProgressBarTypes progressBarStyle = ProgressBarTypes.Horizontal;

        private static Color[] progressColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ProgressColor),
                Settings.DefaultValue.Style.ProgressColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ProgressColor)
            };

        private Color[] backgroundColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private Point barLocation = new Point(0, 0);
        private Point barSize = new Point(15, 15);
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private ControlState controlState = ControlState.Normal;

        private Point endPoint;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);

        private float gradientAngle;
        private LinearGradientBrush gradientBrush;
        private float[] gradientPosition = { 0, 1 / 2f, 1 };

        private float gradientProgressAngle;
        private LinearGradientBrush gradientProgressBrush;
        private float[] gradientProgressPosition = { 0, 1 / 2f, 1 };
        private GraphicsPath graphicsDefaultBorderPath;
        private Color hatchForeColor = Color.FromArgb(40, hatchBackColor);
        private GraphicsPath hatchPath = new GraphicsPath();
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;

        private Size minimumSize = new Size(100, 20);
        private bool percentageVisible;
        private BrushType progressColorStyle = BrushType.Gradient;
        private Font progressFont = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Regular);
        private Point startPoint;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private StringAlignment valueAlignment = StringAlignment.Center;

        #endregion

        #region Constructors

        public VisualProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Maximum = 100;

            Size = minimumSize;
            MinimumSize = minimumSize;

            percentageVisible = true;
            DoubleBuffered = true;
            UpdateStyles();
        }

        public enum ProgressBarTypes
        {
            /// <summary>Bars type.</summary>
            Bars,

            /// <summary>The horizontal progressbar.</summary>
            Horizontal,

            /// <summary>The vertical progressbar.</summary>
            Vertical,

            /// <summary>Rating type.</summary>
            Rating
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

        [DefaultValue(Settings.DefaultValue.BarAmount)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BarAmount)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BarSize)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BarSpacing)]
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

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
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

                graphicsDefaultBorderPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientAngle
        {
            get
            {
                return gradientAngle;
            }

            set
            {
                gradientAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientPosition
        {
            get
            {
                return gradientPosition;
            }

            set
            {
                gradientPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientProgressAngle
        {
            get
            {
                return gradientProgressAngle;
            }

            set
            {
                gradientProgressAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientProgressPosition
        {
            get
            {
                return gradientProgressPosition;
            }

            set
            {
                gradientProgressPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Layout)]
        [DefaultValue(Settings.DefaultValue.HatchSize)]
        [Description(Localize.Description.HatchSize)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.HatchStyle)]
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

        [DefaultValue(Settings.DefaultValue.HatchVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
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

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextVisible)]
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ProgressBarStyle)]
        public ProgressBarTypes ProgressBarStyle
        {
            get
            {
                return progressBarStyle;
            }

            set
            {
                progressBarStyle = value;

                if (progressBarStyle == ProgressBarTypes.Horizontal)
                {
                    Size = GDI.FlipOrientationSize(Orientation.Horizontal, Size);
                }
                else if (progressBarStyle == ProgressBarTypes.Vertical)
                {
                    Size = GDI.FlipOrientationSize(Orientation.Vertical, Size);
                }

                // Resize check
                OnResize(EventArgs.Empty);

                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] ProgressColor
        {
            get
            {
                return progressColor;
            }

            set
            {
                progressColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font ProgressFont
        {
            get
            {
                return progressFont;
            }

            set
            {
                progressFont = value;
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Alignment)]
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

        #region Events

        /// <summary>Decreases the progress value.</summary>
        /// <param name="curValue">Value amount.</param>
        public void Decrement(int curValue)
        {
            Value -= curValue;
            Invalidate();
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
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            startPoint = new Point(ClientRectangle.Width, 0);
            endPoint = new Point(ClientRectangle.Width, ClientRectangle.Height);

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
            switch (progressBarStyle)
            {
                case ProgressBarTypes.Bars:
                    {
                        Height = barSize.Y;
                        MinimumSize = new Size(bars * barSize.X, barSize.Y + 2);
                        break;
                    }

                case ProgressBarTypes.Horizontal:
                    {
                        MinimumSize = minimumSize;
                        break;
                    }

                case ProgressBarTypes.Rating:
                    {
                        MinimumSize = new Size(bars * barSize.X, barSize.Y + 2);
                        break;
                    }

                case ProgressBarTypes.Vertical:
                    {
                        MinimumSize = new Size(minimumSize.Height, minimumSize.Width);
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
            gradientBrush = GDI.CreateGradientBrush(backgroundColor, gradientPosition, gradientAngle, startPoint, endPoint);

            // Draw the background
            graphics.FillPath(gradientBrush, graphicsDefaultBorderPath);

            // Draw progress
            if (i1 > 1)
            {
                gradientProgressBrush = GDI.CreateGradientBrush(progressColor, gradientProgressPosition, gradientProgressAngle, startPoint, endPoint);

                // Draw progress
                if (progressColorStyle == BrushType.Gradient)
                {
                    // Draw gradient progress
                    graphics.FillPath(gradientProgressBrush, progressPath);
                }
                else
                {
                    // Solid color progress
                    graphics.FillPath(new SolidBrush(progressColor[0]), progressPath);
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
                    GDI.DrawBorder(graphics, graphicsDefaultBorderPath, borderThickness, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, graphicsDefaultBorderPath, borderThickness, borderColor);
                }
            }

            // Draw value as a string
            string percentValue = Convert.ToString(Convert.ToInt32(Value)) + "%";

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
                    progressFont,
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
                            barStyle.AddPolygon(Star.Calculate5PointStar(barLocation, 10, 5));
                            barStyle.CloseAllFigures();

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
                    // Draw progress
                    gradientProgressBrush = GDI.CreateGradientBrush(progressColor, gradientProgressPosition, gradientProgressAngle, startPoint, endPoint);

                    // Draw the background
                    graphics.FillPath(gradientProgressBrush, barStyle);
                }
                else
                {
                    // Draw background
                    gradientBrush = GDI.CreateGradientBrush(backgroundColor, gradientPosition, gradientAngle, startPoint, endPoint);

                    // Draw the background
                    graphics.FillPath(gradientBrush, barStyle);
                }

                // Draw border
                if (borderVisible)
                {
                    if (controlState == ControlState.Hover && borderHoverVisible)
                    {
                        GDI.DrawBorder(graphics, barStyle, borderThickness, borderHoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, barStyle, borderThickness, borderColor);
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

                case ProgressBarTypes.Vertical:
                    {
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        #endregion
    }
}