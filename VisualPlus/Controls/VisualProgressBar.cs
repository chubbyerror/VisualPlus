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
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ProgressBar))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual ProgressBar")]
    [Designer(VSDesignerBinding.VisualProgressBar)]
    public sealed class VisualProgressBar : ProgressBar
    {
        #region Variables

        protected int bars = 5;
        protected int barSpacing = 10;

        #endregion

        #region Variables

        private static Color hatchBackColor = Settings.DefaultValue.Style.HatchColor;

        private Color[] backgroundColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private Gradient backgroundGradient = new Gradient();

        private Point barLocation = new Point(0, 0);
        private Point barSize = new Point(15, 15);

        private Border border = new Border();
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);

        private Point[] gradientPoints;
        private GraphicsPath graphicsDefaultBorderPath;
        private Color hatchForeColor = Color.FromArgb(40, hatchBackColor);
        private GraphicsPath hatchPath = new GraphicsPath();
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;
        private Size minimumSize = new Size(100, 20);
        private bool percentageVisible;
        private ProgressBarTypes progressBarStyle = ProgressBarTypes.Horizontal;

        private Color[] progressColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ProgressColor),
                Settings.DefaultValue.Style.ProgressColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ProgressColor)
            };

        private Font progressFont = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Regular);
        private Gradient progressGradient = new Gradient();
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

            float[] gradientPosition = { 0, 1 / 2f, 1 };

            backgroundGradient.Colors = backgroundColor;
            backgroundGradient.Positions = gradientPosition;

            progressGradient.Colors = progressColor;
            progressGradient.Positions = gradientPosition;
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Background
        {
            get
            {
                return backgroundGradient;
            }

            set
            {
                backgroundGradient = value;
                Invalidate();
            }
        }

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

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Progress
        {
            get
            {
                return progressGradient;
            }

            set
            {
                progressGradient = value;
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

            gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };

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
            graphicsDefaultBorderPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
            GraphicsPath progressPath = null;

            var i1 = new int();

            switch (style)
            {
                case ProgressBarTypes.Horizontal:
                    {
                        i1 = (int)Math.Round((Value - Minimum) / (double)(Maximum - Minimum) * (Width - 2));

                        if (border.Shape == BorderShape.Rectangle)
                        {
                            progressPath = new GraphicsPath();
                            progressPath.AddRectangle(new Rectangle(0, 0, i1 + 1, Height));
                            progressPath.CloseAllFigures();
                        }
                        else
                        {
                            progressPath = GDI.DrawRoundedRectangle(new Rectangle(1, 1, i1, Height - 2), border.Rounding);
                        }
                    }

                    break;
                case ProgressBarTypes.Vertical:
                    {
                        i1 = (int)Math.Round((Value - Minimum) / (double)(Maximum - Minimum) * (Height - 2));

                        if (border.Shape == BorderShape.Rectangle)
                        {
                            progressPath = new GraphicsPath();
                            progressPath.AddRectangle(new Rectangle(0, Height - i1 - 2, Width, i1));
                            progressPath.CloseAllFigures();
                        }
                        else
                        {
                            progressPath = GDI.DrawRoundedRectangle(new Rectangle(0, Height - i1 - 2, Width, i1), border.Rounding);
                        }
                    }

                    break;
            }

            LinearGradientBrush backgroundGradientBrush = GDI.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillPath(backgroundGradientBrush, graphicsDefaultBorderPath);

            // Draw progress
            if (i1 > 1)
            {
                LinearGradientBrush progressGradientBrush = GDI.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);
                graphics.FillPath(progressGradientBrush, progressPath);

                hatchPath = progressPath;

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
            if (border.Visible)
            {
                if (controlState == ControlState.Hover && border.HoverVisible)
                {
                    GDI.DrawBorder(graphics, graphicsDefaultBorderPath, border.Thickness, border.HoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, graphicsDefaultBorderPath, border.Thickness, border.Color);
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
                            if (border.Shape == BorderShape.Rounded)
                            {
                                // Rounded rectangle - makes it possible to make circles with full roundness.
                                barStyle.AddPath(
                                    GDI.DrawRoundedRectangle(barLocation.X, barLocation.Y, barSize.X, barSize.Y, border.Rounding),
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
                            barStyle = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
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
                            barStyle = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Draw shape
                if (colored)
                {
                    // Draw progress
                    LinearGradientBrush progressGradientBrush = GDI.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);

                    // Draw the progress
                    graphics.FillPath(progressGradientBrush, barStyle);
                }
                else
                {
                    // Draw background
                    LinearGradientBrush backgroundGradientBrush = GDI.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);

                    // Draw the background
                    graphics.FillPath(backgroundGradientBrush, barStyle);
                }

                // Draw border
                if (border.Visible)
                {
                    if (controlState == ControlState.Hover && border.HoverVisible)
                    {
                        GDI.DrawBorder(graphics, barStyle, border.Thickness, border.HoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, barStyle, border.Thickness, border.Color);
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