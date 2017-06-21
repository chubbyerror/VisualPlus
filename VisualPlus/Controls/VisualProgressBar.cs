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
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ProgressBar))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual ProgressBar")]
    [Designer(DesignManager.VisualProgressBar)]
    public sealed class VisualProgressBar : ProgressBar
    {
        #region Variables

        protected int bars = 5;
        protected int barSpacing = 10;

        #endregion

        #region Variables

        private Gradient backgroundGradient = new Gradient();
        private Point barLocation = new Point(0, 0);
        private Point barSize = new Point(15, 15);
        private BarTypes barStyle = BarTypes.Horizontal;
        private Border border = new Border();
        private ControlState controlState = ControlState.Normal;
        private Color foreColor;
        private Point[] gradientPoints;
        private GraphicsPath graphicsDefaultBorderPath;
        private Color hatchBackColor;
        private Color hatchForeColor;
        private GraphicsPath hatchPath = new GraphicsPath();
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;
        private Timer marqueeTimer;
        private bool marqueeTimerEnabled;
        private int marqueeX;
        private int marqueeY;
        private Size minimumSize = new Size(100, 20);
        private bool percentageVisible;
        private Font progressFont;
        private Gradient progressGradient = new Gradient();
        private StyleManager styleManager = new StyleManager();
        private TextRenderingHint textRendererHint;
        private StringAlignment valueAlignment = StringAlignment.Center;

        #endregion

        #region Constructors

        public VisualProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Maximum = 100;

            Size = minimumSize;
            MinimumSize = minimumSize;

            percentageVisible = true;
            DoubleBuffered = true;
            UpdateStyles();

            DefaultGradient();
            ConfigureStyleManager();
        }

        public enum BarTypes
        {
            /// <summary>Bars type.</summary>
            Bars,

            /// <summary>The horizontal progressbar.</summary>
            Horizontal,

            /// <summary>The vertical progressbar.</summary>
            Vertical
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ProgressBarStyle)]
        public BarTypes BarStyle
        {
            get
            {
                return barStyle;
            }

            set
            {
                barStyle = value;

                if (barStyle == BarTypes.Horizontal)
                {
                    Size = GDI.FlipOrientationSize(Orientation.Horizontal, Size);
                }
                else if (barStyle == BarTypes.Vertical)
                {
                    Size = GDI.FlipOrientationSize(Orientation.Vertical, Size);
                }

                // Resize check
                OnResize(EventArgs.Empty);

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

        public new Color ForeColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                base.ForeColor = value;
                foreColor = value;
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

        [TypeConverter(typeof(VisualStyleManagerConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public StyleManager StyleManager
        {
            get
            {
                return styleManager;
            }

            set
            {
                styleManager = value;
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

        private int ProgressBarMarqueeHeight
        {
            get
            {
                return ClientRectangle.Height / 3;
            }
        }

        private int ProgressBarMarqueeWidth
        {
            get
            {
                return ClientRectangle.Width / 3;
            }
        }

        private double ProgressBarWidth
        {
            get
            {
                return ((double)Value / Maximum) * ClientRectangle.Width;
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

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            if ((barStyle == BarTypes.Horizontal) || (barStyle == BarTypes.Vertical))
            {
                // Draw default progress
                DrawDefaultProgress(barStyle, graphics);
            }
            else
            {
                // Draw styled progress
                SetStyleSettings(barStyle);

                // Draw total bars
                DrawStyledProgress(graphics, bars, false);

                SetStyleSettings(barStyle);

                // Draw current progressbars
                DrawStyledProgress(graphics, MathManager.GetFactor(Convert.ToDouble(Value), bars), true);
            }

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.ResetClip();
        }

        protected override void OnResize(EventArgs e)
        {
            switch (barStyle)
            {
                case BarTypes.Bars:
                    {
                        Height = barSize.Y;
                        MinimumSize = new Size(bars * barSize.X, barSize.Y + 2);
                        break;
                    }

                case BarTypes.Horizontal:
                    {
                        MinimumSize = minimumSize;
                        break;
                    }

                case BarTypes.Vertical:
                    {
                        MinimumSize = new Size(minimumSize.Height, minimumSize.Width);
                        break;
                    }
            }
        }

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.BorderStyle;
                IFont fontStyle = styleManager.VisualStylesManager.FontStyle;

                border.Color = borderStyle.Color;
                border.HoverColor = borderStyle.HoverColor;
                border.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                border.Rounding = styleManager.VisualStylesManager.BorderRounding;
                border.Shape = styleManager.VisualStylesManager.BorderShape;
                border.Thickness = styleManager.VisualStylesManager.BorderThickness;
                border.Visible = styleManager.VisualStylesManager.BorderVisible;
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                progressFont = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                foreColor = fontStyle.ForeColor;
                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
            }
            else
            {
                // Load default settings
                border.HoverVisible = Settings.DefaultValue.BorderHoverVisible;
                border.Rounding = Settings.DefaultValue.Rounding.Default;
                border.Shape = Settings.DefaultValue.BorderShape;
                border.Thickness = Settings.DefaultValue.BorderThickness;
                border.Visible = Settings.DefaultValue.BorderVisible;
                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                progressFont = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                hatchBackColor = Settings.DefaultValue.Progress.Hatch;
                hatchForeColor = Color.FromArgb(40, hatchBackColor);

                foreColor = Settings.DefaultValue.Font.ForeColor;
                textRendererHint = Settings.DefaultValue.TextRenderingHint;
            }
        }

        private void DefaultGradient()
        {
            backgroundGradient.Colors = Settings.DefaultValue.Progress.Background.Colors;
            backgroundGradient.Positions = Settings.DefaultValue.Progress.Background.Positions;

            progressGradient.Colors = Settings.DefaultValue.Progress.Progress.Colors;
            progressGradient.Positions = Settings.DefaultValue.Progress.Progress.Positions;
        }

        private void DrawDefaultProgress(BarTypes style, Graphics graphics)
        {
            graphicsDefaultBorderPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
            GraphicsPath progressPath = null;

            if (Style == ProgressBarStyle.Marquee)
            {
                if (!DesignMode && Enabled)
                {
                    StartTimer();
                }

                if (!Enabled)
                {
                    StopTimer();
                }

                if (Value == Maximum)
                {
                    StopTimer();
                    DrawProgressContinuous(graphics);
                }
                else
                {
                    DrawProgressMarquee(graphics);
                }
            }
            else
            {
                var i1 = new int();

                switch (style)
                {
                    case BarTypes.Horizontal:
                        {
                            i1 = (int)Math.Round(((Value - Minimum) / (double)(Maximum - Minimum)) * (Width - 2));

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
                    case BarTypes.Vertical:
                        {
                            i1 = (int)Math.Round(((Value - Minimum) / (double)(Maximum - Minimum)) * (Height - 2));

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
            }

            // Draw border
            if (border.Visible)
            {
                if ((controlState == ControlState.Hover) && border.HoverVisible)
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

        private void DrawProgressContinuous(Graphics graphics)
        {
            LinearGradientBrush backgroundGradientBrush = GDI.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillPath(backgroundGradientBrush, graphicsDefaultBorderPath);

            graphics.SetClip(graphicsDefaultBorderPath);

            LinearGradientBrush progressGradientBrush = GDI.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);
            graphics.FillRectangle(progressGradientBrush, 0, 0, (int)ProgressBarWidth, ClientRectangle.Height);

            graphics.ResetClip();
        }

        private void DrawProgressMarquee(Graphics graphics)
        {
            LinearGradientBrush backgroundGradientBrush = GDI.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillPath(backgroundGradientBrush, graphicsDefaultBorderPath);

            graphics.SetClip(graphicsDefaultBorderPath);

            LinearGradientBrush progressGradientBrush = GDI.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);

            Rectangle progressRectangle = new Rectangle();

            if (barStyle == BarTypes.Horizontal)
            {
                progressRectangle = new Rectangle(marqueeX, 0, ProgressBarMarqueeWidth, ClientRectangle.Height);
            }
            else if (barStyle == BarTypes.Vertical)
            {
                progressRectangle = new Rectangle(0, marqueeY, ClientRectangle.Width, ClientRectangle.Height);
            }

            graphics.FillRectangle(progressGradientBrush, progressRectangle);

            GraphicsPath progressPath = new GraphicsPath();
            progressPath.AddRectangle(progressRectangle);

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

            graphics.ResetClip();
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
                switch (this.barStyle)
                {
                    case BarTypes.Bars:
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

                    case BarTypes.Horizontal:
                        {
                            // Default progress bar
                            barStyle = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);
                            break;
                        }

                    case BarTypes.Vertical:
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
                    if ((controlState == ControlState.Hover) && border.HoverVisible)
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

        private void marqueeTimer_Tick(object sender, EventArgs e)
        {
            if (barStyle == BarTypes.Horizontal)
            {
                marqueeX++;
                if (marqueeX > ClientRectangle.Width)
                {
                    marqueeX = -ProgressBarMarqueeWidth;
                }
            }
            else if (barStyle == BarTypes.Vertical)
            {
                marqueeY++;
                if (marqueeY > ClientRectangle.Height)
                {
                    marqueeY = -ProgressBarMarqueeHeight;
                }
            }

            Invalidate();
        }

        /// <summary>Sets the style settings.</summary>
        /// <param name="style">Current ProgressBarType.</param>
        private void SetStyleSettings(BarTypes style)
        {
            switch (style)
            {
                case BarTypes.Bars:
                    {
                        barLocation = new Point(0, 0);
                        barSize = new Point(10, 10);
                        barSpacing = 15;
                        break;
                    }

                case BarTypes.Horizontal:
                    {
                        break;
                    }

                case BarTypes.Vertical:
                    {
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        private void StartTimer()
        {
            if (marqueeTimerEnabled)
            {
                return;
            }

            if (marqueeTimer == null)
            {
                marqueeTimer = new Timer { Interval = 10 };
                marqueeTimer.Tick += marqueeTimer_Tick;
            }

            if (barStyle == BarTypes.Horizontal)
            {
                marqueeX = -ProgressBarMarqueeWidth;
            }
            else if (barStyle == BarTypes.Vertical)
            {
                marqueeY = -ProgressBarMarqueeHeight;
            }

            marqueeTimer.Stop();
            marqueeTimer.Start();

            marqueeTimerEnabled = true;

            Invalidate();
        }

        private void StopTimer()
        {
            if (marqueeTimer == null)
            {
                return;
            }

            marqueeTimer.Stop();

            Invalidate();
        }

        #endregion
    }
}