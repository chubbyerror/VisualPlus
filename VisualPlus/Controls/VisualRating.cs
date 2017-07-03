namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Controls.Bases;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Control))]
    [DefaultEvent("RatingChanged")]
    [DefaultProperty("Value")]
    [Description("The Visual Rating")]
    [Designer(ControlManager.FilterProperties.VisualRating)]
    public sealed class VisualRating : ControlBase
    {
        #region Variables

        private readonly BufferedGraphicsContext _bufferedContext = BufferedGraphicsManager.Current;
        private bool allowHalfStar = true;
        private BufferedGraphics bufferedGraphics;
        private int maximum = 5;
        private float mouseOverIndex = -1;
        private StarType ratingType = StarType.Thick;
        private bool settingRating;
        private SolidBrush starBrush = new SolidBrush(Color.Yellow);
        private SolidBrush starDullBrush = new SolidBrush(Color.Silver);
        private Pen starDullStroke = new Pen(Color.Gray, 3f);
        private int starSpacing = 1;
        private Pen starStroke = new Pen(Color.Gold, 3f);
        private int starWidth = 25;
        private float value;

        #endregion

        #region Constructors

        public VisualRating()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

            SetPenBrushDefaults();
            Size = new Size(200, 100);
            UpdateGraphicsBuffer();
        }

        [Description("Occurs when the star rating of the strip has changed (Typically by a click operation)")]
        public event EventHandler RatingChanged;

        [Description("Occurs when a different number of stars are illuminated (does not include mouseleave un-ilum)")]
        public event EventHandler StarsPanned;

        public enum StarType
        {
            /// <summary>Default star.</summary>
            Default,

            /// <summary>Detailed star.</summary>
            Detailed,

            /// <summary>Thick star.</summary>
            Thick
        }

        #endregion

        #region Properties

        [Description("Determines whether the user can rate with a half a star of specificity")]
        [Category(Localize.PropertiesCategory.Behavior)]
        [DefaultValue(false)]
        public bool AllowHalfStar
        {
            get
            {
                return allowHalfStar;
            }

            set
            {
                bool disabled = !value && allowHalfStar;
                allowHalfStar = value;

                if (disabled)
                {
                    // Only set rating if half star was enabled and now disabled
                    Value = (int)(Value + 0.5);
                }
            }
        }

        [Description("The number of stars to display")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(5)]
        public int Maximum
        {
            get
            {
                return maximum;
            }

            set
            {
                bool changed = maximum != value;
                maximum = value;

                if (changed)
                {
                    UpdateSize();
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public float MouseOverStarIndex
        {
            get
            {
                return mouseOverIndex;
            }
        }

        /// <summary>
        ///     Gets or sets the preset appearance of the star
        /// </summary>
        [Description("The star style to use")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(StarType.Thick)]
        public StarType RatingType
        {
            get
            {
                return ratingType;
            }

            set
            {
                ratingType = value;
                Invalidate();
            }
        }

        [Description("The color to use for the star borders when they are illuminated")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(typeof(Color), "Gold")]
        public Color StarBorderColor
        {
            get
            {
                return starStroke.Color;
            }

            set
            {
                starStroke.Color = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the width of the border around the star (including the dull version)
        /// </summary>
        [Description("The width of the star border")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(3f)]
        public float StarBorderWidth
        {
            get
            {
                return starStroke.Width;
            }

            set
            {
                starStroke.Width = value;
                starDullStroke.Width = value;
                UpdateSize();
                Invalidate();
            }
        }

        [Browsable(false)]
        public SolidBrush StarBrush
        {
            get
            {
                return starBrush;
            }

            set
            {
                starBrush = value;
            }
        }

        [Description("The color to use for the star when they are illuminated")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(typeof(Color), "Yellow")]
        public Color StarColor
        {
            get
            {
                return starBrush.Color;
            }

            set
            {
                starBrush.Color = value;
                Invalidate();
            }
        }

        [Description("The color to use for the star borders when they are not illuminated")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(typeof(Color), "Gray")]
        public Color StarDullBorderColor
        {
            get
            {
                return starDullStroke.Color;
            }

            set
            {
                starDullStroke.Color = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public SolidBrush StarDullBrush
        {
            get
            {
                return starDullBrush;
            }

            set
            {
                starDullBrush = value;
            }
        }

        [Description("The color to use for the stars when they are not illuminated")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(typeof(Color), "Silver")]
        public Color StarDullColor
        {
            get
            {
                return starDullBrush.Color;
            }

            set
            {
                starDullBrush.Color = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public Pen StarDullStroke
        {
            get
            {
                return starDullStroke;
            }

            set
            {
                starDullStroke = value;
            }
        }

        [Description("The amount of space between each star")]
        [Category(Localize.PropertiesCategory.Layout)]
        [DefaultValue(1)]
        public int StarSpacing
        {
            get
            {
                return starSpacing;
            }

            set
            {
                starSpacing = starSpacing < 0 ? 0 : value;
                UpdateSize();
                Invalidate();
            }
        }

        [Browsable(false)]
        public Pen StarStroke
        {
            get
            {
                return starStroke;
            }

            set
            {
                starStroke = value;
            }
        }

        [Description("The width and height of the star in pixels (not including the border)")]
        [Category(Localize.PropertiesCategory.Layout)]
        [DefaultValue(25)]
        public int StarWidth
        {
            get
            {
                return starWidth;
            }

            set
            {
                starWidth = starWidth < 1 ? 1 : value;
                UpdateSize();
                Invalidate();
            }
        }

        [Description("The number of stars selected (Note: 0 is considered un-rated")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [DefaultValue(0f)]
        public float Value
        {
            get
            {
                return value;
            }

            set
            {
                if (value > maximum)
                {
                    value = maximum; // bounds check
                }
                else if (value < 0)
                {
                    value = 0;
                }
                else
                {
                    // Rounding to whole number or near .5 appropriately
                    if (allowHalfStar)
                    {
                        value = RoundToNearestHalf(value);
                    }
                    else
                    {
                        value = (int)(value + 0.5f);
                    }
                }

                bool changed = value != this.value;
                this.value = value;

                if (changed)
                {
                    if (!settingRating)
                    {
                        mouseOverIndex = this.value;
                        if (!allowHalfStar)
                        {
                            mouseOverIndex -= 1f;
                        }
                    }

                    OnRatingChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>Gets all of the spacing between the stars.</summary>
        private int TotalSpacing
        {
            get
            {
                return (maximum - 1) * starSpacing;
            }
        }

        /// <summary>Gets the sum of all star widths.</summary>
        private int TotalStarWidth
        {
            get
            {
                return maximum * starWidth;
            }
        }

        /// <summary>Gets the sum of the width of the stroke for each star.</summary>
        private float TotalStrokeWidth
        {
            get
            {
                return maximum * starStroke.Width;
            }
        }

        #endregion

        #region Events

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (value == 0f)
            {
                settingRating = true;
                Value = allowHalfStar ? mouseOverIndex : mouseOverIndex + 1f;
                settingRating = false;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (value > 0)
            {
                return;
            }

            mouseOverIndex = -1; // No stars will be highlighted
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (value > 0)
            {
                return;
            }

            float index = GetHoveredStarIndex(e.Location);

            if (index != mouseOverIndex)
            {
                mouseOverIndex = index;
                OnStarsPanned();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            bufferedGraphics.Graphics.Clear(BackColor);
            DrawDullStars();
            DrawIlluminatedStars();
            bufferedGraphics.Render(e.Graphics);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateSize();
            UpdateGraphicsBuffer();
        }

        /// <summary>Gets half of the detailed star polygon as a point[].</summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Star shape.</returns>
        private static PointF[] GetDetailedSemiStar(RectangleF rect)
        {
            return new[]
                {
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.4f), rect.Y + (rect.Height * 0.73f)),
                    new PointF(rect.X + (rect.Width * 0.17f), rect.Y + (rect.Height * 0.83f)),
                    new PointF(rect.X + (rect.Width * 0.27f), rect.Y + (rect.Height * 0.6f)),
                    new PointF(rect.X + (rect.Width * 0f), rect.Y + (rect.Height * 0.5f)),
                    new PointF(rect.X + (rect.Width * 0.27f), rect.Y + (rect.Height * 0.4f)),
                    new PointF(rect.X + (rect.Width * 0.17f), rect.Y + (rect.Height * 0.17f)),
                    new PointF(rect.X + (rect.Width * 0.4f), rect.Y + (rect.Height * 0.27f))
                };
        }

        /// <summary>Gets a detailed star polygon as a point[].</summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Star shape.</returns>
        private static PointF[] GetDetailedStar(RectangleF rect)
        {
            return new[]
                {
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0f)),
                    new PointF(rect.X + (rect.Width * 0.6f), rect.Y + (rect.Height * 0.27f)),
                    new PointF(rect.X + (rect.Width * 0.83f), rect.Y + (rect.Height * 0.17f)),
                    new PointF(rect.X + (rect.Width * 0.73f), rect.Y + (rect.Height * 0.4f)),
                    new PointF(rect.X + (rect.Width * 1f), rect.Y + (rect.Height * 0.5f)),
                    new PointF(rect.X + (rect.Width * 0.73f), rect.Y + (rect.Height * 0.6f)),
                    new PointF(rect.X + (rect.Width * 0.83f), rect.Y + (rect.Height * 0.83f)),
                    new PointF(rect.X + (rect.Width * 0.6f), rect.Y + (rect.Height * 0.73f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.4f), rect.Y + (rect.Height * 0.73f)),
                    new PointF(rect.X + (rect.Width * 0.17f), rect.Y + (rect.Height * 0.83f)),
                    new PointF(rect.X + (rect.Width * 0.27f), rect.Y + (rect.Height * 0.6f)),
                    new PointF(rect.X + (rect.Width * 0f), rect.Y + (rect.Height * 0.5f)),
                    new PointF(rect.X + (rect.Width * 0.27f), rect.Y + (rect.Height * 0.4f)),
                    new PointF(rect.X + (rect.Width * 0.17f), rect.Y + (rect.Height * 0.17f)),
                    new PointF(rect.X + (rect.Width * 0.4f), rect.Y + (rect.Height * 0.27f))
                };
        }

        /// <summary>Gets half of a fat star polygon as a point[].</summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Star shape.</returns>
        private static PointF[] GetFatSemiStar(RectangleF rect)
        {
            return new[]
                {
                    new PointF(rect.X + (rect.Width * 0.31f), rect.Y + (rect.Height * 0.33f)),
                    new PointF(rect.X + (rect.Width * 0f), rect.Y + (rect.Height * 0.37f)),
                    new PointF(rect.X + (rect.Width * 0.25f), rect.Y + (rect.Height * 0.62f)),
                    new PointF(rect.X + (rect.Width * 0.19f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0.81f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0f))
                };
        }

        /// <summary>Gets a fat star polygon as a point[].</summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Star shape.</returns>
        private static PointF[] GetFatStar(RectangleF rect)
        {
            return new[]
                {
                    new PointF(rect.X + (rect.Width * 0.31f), rect.Y + (rect.Height * 0.33f)),
                    new PointF(rect.X + (rect.Width * 0f), rect.Y + (rect.Height * 0.37f)),
                    new PointF(rect.X + (rect.Width * 0.25f), rect.Y + (rect.Height * 0.62f)),
                    new PointF(rect.X + (rect.Width * 0.19f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0.81f)),
                    new PointF(rect.X + (rect.Width * 0.81f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.75f), rect.Y + (rect.Height * 0.62f)),
                    new PointF(rect.X + (rect.Width * 1f), rect.Y + (rect.Height * 0.37f)),
                    new PointF(rect.X + (rect.Width * 0.69f), rect.Y + (rect.Height * 0.33f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0f))
                };
        }

        /// <summary>Gets half of a typical thin star polygon as a point[].</summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Star shape.</returns>
        private static PointF[] GetNormalSemiStar(RectangleF rect)
        {
            return new[]
                {
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0f)),
                    new PointF(rect.X + (rect.Width * 0.38f), rect.Y + (rect.Height * 0.38f)),
                    new PointF(rect.X + (rect.Width * 0f), rect.Y + (rect.Height * 0.38f)),
                    new PointF(rect.X + (rect.Width * 0.31f), rect.Y + (rect.Height * 0.61f)),
                    new PointF(rect.X + (rect.Width * 0.19f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0.77f))
                };
        }

        /// <summary>Gets a typical thin star polygon as a point[].</summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Star shape.</returns>
        private static PointF[] GetNormalStar(RectangleF rect)
        {
            return new[]
                {
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0f)),
                    new PointF(rect.X + (rect.Width * 0.38f), rect.Y + (rect.Height * 0.38f)),
                    new PointF(rect.X + (rect.Width * 0f), rect.Y + (rect.Height * 0.38f)),
                    new PointF(rect.X + (rect.Width * 0.31f), rect.Y + (rect.Height * 0.61f)),
                    new PointF(rect.X + (rect.Width * 0.19f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0.77f)),
                    new PointF(rect.X + (rect.Width * 0.8f), rect.Y + (rect.Height * 1f)),
                    new PointF(rect.X + (rect.Width * 0.69f), rect.Y + (rect.Height * 0.61f)),
                    new PointF(rect.X + (rect.Width * 1f), rect.Y + (rect.Height * 0.38f)),
                    new PointF(rect.X + (rect.Width * 0.61f), rect.Y + (rect.Height * 0.38f))
                };
        }

        /// <summary>Rounds precise numbers to a number no more precise than .5.</summary>
        /// <param name="f">The value.</param>
        /// <returns>Star shape.</returns>
        private static float RoundToNearestHalf(float f)
        {
            return (float)Math.Round(f / 5.0, 1) * 5f;
        }

        private void DrawDullStars()
        {
            float height = Height - starStroke.Width;
            float lastX = starStroke.Width / 2f; // Start off at stroke size and increment
            float width = (Width - TotalSpacing - TotalStrokeWidth) / maximum;

            // Draw stars
            for (var i = 0; i < maximum; i++)
            {
                RectangleF rect = new RectangleF(lastX, starStroke.Width / 2f, width, height);
                var polygon = GetStarPolygon(rect);
                bufferedGraphics.Graphics.FillPolygon(starDullBrush, polygon);
                bufferedGraphics.Graphics.DrawPolygon(starDullStroke, polygon);
                lastX += starWidth + starSpacing + starStroke.Width;
                bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                bufferedGraphics.Graphics.FillPolygon(starDullBrush, polygon);
                bufferedGraphics.Graphics.DrawPolygon(starDullStroke, polygon);
                bufferedGraphics.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        private void DrawIlluminatedStars()
        {
            float height = Height - starStroke.Width;
            float lastX = starStroke.Width / 2f; // Start off at stroke size and increment
            float width = (Width - TotalSpacing - TotalStrokeWidth) / maximum;

            if (allowHalfStar)
            {
                // Draw stars
                for (var i = 0; i < maximum; i++)
                {
                    RectangleF rect = new RectangleF(lastX, starStroke.Width / 2f, width, height);

                    if (i < mouseOverIndex - 0.5f)
                    {
                        var polygon = GetStarPolygon(rect);
                        bufferedGraphics.Graphics.FillPolygon(starBrush, polygon);
                        bufferedGraphics.Graphics.DrawPolygon(starStroke, polygon);
                    }
                    else if (i == mouseOverIndex - 0.5f)
                    {
                        var polygon = GetSemiStarPolygon(rect);
                        bufferedGraphics.Graphics.FillPolygon(starBrush, polygon);
                        bufferedGraphics.Graphics.DrawPolygon(starStroke, polygon);
                    }
                    else
                    {
                        break;
                    }

                    lastX += starWidth + starSpacing + starStroke.Width;
                }
            }
            else
            {
                // Draw stars
                for (var i = 0; i < maximum; i++)
                {
                    RectangleF rect = new RectangleF(lastX, starStroke.Width / 2f, width, height);
                    var polygon = GetStarPolygon(rect);

                    if (i <= mouseOverIndex)
                    {
                        bufferedGraphics.Graphics.FillPolygon(starBrush, polygon);
                        bufferedGraphics.Graphics.DrawPolygon(starStroke, polygon);
                    }
                    else
                    {
                        break;
                    }

                    lastX += starWidth + starSpacing + starStroke.Width;
                }
            }
        }

        private float GetHoveredStarIndex(Point pos)
        {
            if (allowHalfStar)
            {
                float widthSection = Width / (float)maximum / 2f;

                for (var i = 0f; i < maximum; i += 0.5f)
                {
                    float starX = i * widthSection * 2f;

                    // If cursor is within the x region of the iterated star
                    if ((pos.X >= starX) && (pos.X <= starX + widthSection))
                    {
                        return i + 0.5f;
                    }
                }

                return -1;
            }
            else
            {
                var widthSection = (int)((Width / (double)maximum) + 0.5);

                for (var i = 0; i < maximum; i++)
                {
                    float starX = i * widthSection;

                    // If cursor is within the x region of the iterated star
                    if ((pos.X >= starX) && (pos.X <= starX + widthSection))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private PointF[] GetSemiStarPolygon(RectangleF rect)
        {
            switch (ratingType)
            {
                case StarType.Default: return GetNormalSemiStar(rect);
                case StarType.Thick: return GetFatSemiStar(rect);
                case StarType.Detailed: return GetDetailedSemiStar(rect);
                default: return null;
            }
        }

        private PointF[] GetStarPolygon(RectangleF rect)
        {
            switch (ratingType)
            {
                case StarType.Default: return GetNormalStar(rect);
                case StarType.Thick: return GetFatStar(rect);
                case StarType.Detailed: return GetDetailedStar(rect);
                default: return null;
            }
        }

        private void OnRatingChanged()
        {
            RatingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnStarsPanned()
        {
            StarsPanned?.Invoke(this, EventArgs.Empty);
        }

        private void SetPenBrushDefaults()
        {
            starStroke.LineJoin = LineJoin.Round;
            starStroke.Alignment = PenAlignment.Outset;
            starDullStroke.LineJoin = LineJoin.Round;
            starDullStroke.Alignment = PenAlignment.Outset;
        }

        private void UpdateGraphicsBuffer()
        {
            if ((Width > 0) && (Height > 0))
            {
                _bufferedContext.MaximumBuffer = new Size(Width + 1, Height + 1);
                bufferedGraphics = _bufferedContext.Allocate(CreateGraphics(), ClientRectangle);
                bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
        }

        private void UpdateSize()
        {
            var height = (int)(starWidth + starStroke.Width + 0.5);
            var width = (int)(TotalStarWidth + TotalSpacing + TotalStrokeWidth + 0.5);
            Size = new Size(width, height);
        }

        #endregion
    }
}