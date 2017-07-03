namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Controls.Bases;
    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ProgressBar))]
    [DefaultEvent("Click")]
    [DefaultProperty("Value")]
    [Description("The Visual ProgressBar")]
    [Designer(ControlManager.FilterProperties.VisualProgressBar)]
    public sealed class VisualProgressBar : ProgressBase
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
        private ProgressBarStyle progressBarStyle = ProgressBarStyle.Blocks;
        private Gradient progressGradient = new Gradient();
        private StringAlignment valueAlignment = StringAlignment.Center;

        #endregion

        #region Constructors

        public VisualProgressBar()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            Maximum = 100;

            Size = minimumSize;
            MinimumSize = minimumSize;

            percentageVisible = true;
            DoubleBuffered = true;

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
        [Category(Localize.PropertiesCategory.Appearance)]
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
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Progressbar.Bars)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Spacing)]
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

        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Type)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return ControlBorder;
            }

            set
            {
                ControlBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [DefaultValue(Settings.DefaultValue.HatchSize)]
        [Description(Localize.Description.Common.Size)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
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
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [DefaultValue(typeof(ProgressBarStyle))]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description("This property allows the user to set the style of the ProgressBar.")]
        public ProgressBarStyle Style
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Alignment)]
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };

            if (StyleManager.LockedStyle)
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
            if (StyleManager.VisualStylesManager != null)
            {
            }
            else
            {
                hatchBackColor = Settings.DefaultValue.Progress.Hatch;
                hatchForeColor = Color.FromArgb(40, hatchBackColor);
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
            graphicsDefaultBorderPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
            GraphicsPath progressPath = null;

            if (progressBarStyle == ProgressBarStyle.Marquee)
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

                            if (ControlBorder.Type == BorderType.Rectangle)
                            {
                                progressPath = new GraphicsPath();
                                progressPath.AddRectangle(new Rectangle(0, 0, i1 + 1, Height));
                                progressPath.CloseAllFigures();
                            }
                            else
                            {
                                progressPath = GDI.DrawRoundedRectangle(new Rectangle(1, 1, i1, Height - 2), ControlBorder.Rounding);
                            }
                        }

                        break;
                    case BarTypes.Vertical:
                        {
                            i1 = (int)Math.Round(((Value - Minimum) / (double)(Maximum - Minimum)) * (Height - 2));

                            if (ControlBorder.Type == BorderType.Rectangle)
                            {
                                progressPath = new GraphicsPath();
                                progressPath.AddRectangle(new Rectangle(0, Height - i1 - 2, Width, i1));
                                progressPath.CloseAllFigures();
                            }
                            else
                            {
                                progressPath = GDI.DrawRoundedRectangle(new Rectangle(0, Height - i1 - 2, Width, i1), ControlBorder.Rounding);
                            }
                        }

                        break;
                }

                LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
                graphics.FillPath(backgroundGradientBrush, graphicsDefaultBorderPath);

                // Draw progress
                if (i1 > 1)
                {
                    LinearGradientBrush progressGradientBrush = Gradient.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);
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
            if (ControlBorder.Visible)
            {
                if ((MouseState == MouseStates.Hover) && ControlBorder.HoverVisible)
                {
                    Border.DrawBorder(graphics, graphicsDefaultBorderPath, ControlBorder.Thickness, ControlBorder.HoverColor);
                }
                else
                {
                    Border.DrawBorder(graphics, graphicsDefaultBorderPath, ControlBorder.Thickness, ControlBorder.Color);
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
                    Font,
                    new SolidBrush(ForeColor),
                    new Rectangle(0, 0, Width, Height + 2),
                    stringFormat);
            }
        }

        private void DrawProgressContinuous(Graphics graphics)
        {
            LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillPath(backgroundGradientBrush, graphicsDefaultBorderPath);

            graphics.SetClip(graphicsDefaultBorderPath);

            LinearGradientBrush progressGradientBrush = Gradient.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);
            graphics.FillRectangle(progressGradientBrush, 0, 0, (int)ProgressBarWidth, ClientRectangle.Height);

            graphics.ResetClip();
        }

        private void DrawProgressMarquee(Graphics graphics)
        {
            LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillPath(backgroundGradientBrush, graphicsDefaultBorderPath);

            graphics.SetClip(graphicsDefaultBorderPath);

            LinearGradientBrush progressGradientBrush = Gradient.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);

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
            GraphicsPath barStylePath = new GraphicsPath();

            for (var i = 0; i < barCount; i++)
            {
                // Move the bar right
                if (i != 0)
                {
                    barLocation = new Point(barLocation.X + barSpacing, barLocation.Y);
                }

                // Create Bar
                switch (barStyle)
                {
                    case BarTypes.Bars:
                        {
                            // Create bars
                            if (ControlBorder.Type == BorderType.Rounded)
                            {
                                // Rounded rectangle - makes it possible to make circles with full roundness.
                                barStylePath.AddPath(GDI.DrawRoundedRectangle(barLocation.X, barLocation.Y, barSize.X, barSize.Y, ControlBorder.Rounding), true);
                            }
                            else
                            {
                                // Rectangle
                                barStylePath.AddRectangle(new Rectangle(barLocation.X, barLocation.Y, barSize.X, barSize.Y));
                            }

                            barStylePath.CloseAllFigures();
                            break;
                        }

                    case BarTypes.Horizontal:
                        {
                            // Default progress bar
                            barStylePath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
                            break;
                        }

                    case BarTypes.Vertical:
                        {
                            barStylePath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Draw shape
                if (colored)
                {
                    // Draw progress
                    LinearGradientBrush progressGradientBrush = Gradient.CreateGradientBrush(progressGradient.Colors, gradientPoints, progressGradient.Angle, progressGradient.Positions);

                    // Draw the progress
                    graphics.FillPath(progressGradientBrush, barStylePath);
                }
                else
                {
                    // Draw background
                    LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);

                    // Draw the background
                    graphics.FillPath(backgroundGradientBrush, barStylePath);
                }

                // Draw border
                if (ControlBorder.Visible)
                {
                    if ((MouseState == MouseStates.Hover) && ControlBorder.HoverVisible)
                    {
                        Border.DrawBorder(graphics, barStylePath, ControlBorder.Thickness, ControlBorder.HoverColor);
                    }
                    else
                    {
                        Border.DrawBorder(graphics, barStylePath, ControlBorder.Thickness, ControlBorder.Color);
                    }
                }
            }
        }

        private void MarqueeTimer_Tick(object sender, EventArgs e)
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
                marqueeTimer.Tick += MarqueeTimer_Tick;
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