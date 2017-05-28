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
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Label))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [Description("The Visual Label")]
    [Designer(DesignManager.VisualLabel)]
    public sealed class VisualLabel : Label
    {
        #region Variables

        private const int ShadowDepth = 4;
        private const float ShadowSmooth = 1.5f;

        private readonly Color[] foreColor =
            {
                Settings.DefaultValue.Style.ForeColor(0),
                Settings.DefaultValue.Style.ForeColor(0)
            };

        private readonly Color[] textDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TextDisabled),
                Settings.DefaultValue.Style.TextDisabled
            };

        private bool autoSize;

        private Orientation orientation = Orientation.Horizontal;
        private bool outline;
        private Color outlineColor = Color.Red;
        private Point outlineLocation = new Point(0, 0);
        private bool reflection;
        private Color reflectionColor = Color.FromArgb(120, 0, 0, 0);
        private int reflectionSpacing;
        private bool shadow;
        private Color shadowColor = Color.Black;
        private int shadowDirection = 315;
        private Point shadowLocation = new Point(0, 0);
        private int shadowOpacity = 100;
        private Rectangle textBoxRectangle;
        private Gradient textDisabledGradient = new Gradient();
        private Gradient textGradient = new Gradient();
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

        #endregion

        #region Constructors

        public VisualLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            UpdateStyles();
            BackColor = Color.Transparent;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);

            float[] gradientPosition = { 0, 1 };

            textGradient.Colors = foreColor;
            textGradient.Positions = gradientPosition;

            textDisabledGradient.Colors = textDisabledColor;
            textDisabledGradient.Positions = gradientPosition;
        }

        #endregion

        #region Properties

        [DefaultValue(false)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.AutoSize)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }

            set
            {
                if ((autoSize != value) & (autoSize == false))
                {
                    base.AutoSize = false;
                    autoSize = value;
                }
                else
                {
                    base.AutoSize = value;
                }
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Orientation)]
        public Orientation Orientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
                Size = GDI.FlipOrientationSize(orientation, Size);
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Outline)]
        public bool Outline
        {
            get
            {
                return outline;
            }

            set
            {
                outline = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color OutlineColor
        {
            get
            {
                return outlineColor;
            }

            set
            {
                outlineColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentLocation)]
        public Point OutlineLocation
        {
            get
            {
                return outlineLocation;
            }

            set
            {
                outlineLocation = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Reflection)]
        public bool Reflection
        {
            get
            {
                return reflection;
            }

            set
            {
                reflection = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.MirrorColor)]
        public Color ReflectionColor
        {
            get
            {
                return reflectionColor;
            }

            set
            {
                reflectionColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ReflectionSpacing)]
        public int ReflectionSpacing
        {
            get
            {
                return reflectionSpacing;
            }

            set
            {
                reflectionSpacing = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Shadow)]
        public bool Shadow
        {
            get
            {
                return shadow;
            }

            set
            {
                shadow = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ShadowColor)]
        public Color ShadowColor
        {
            get
            {
                return shadowColor;
            }

            set
            {
                shadowColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ShadowDirection)]
        public int ShadowDirection
        {
            get
            {
                return shadowDirection;
            }

            set
            {
                shadowDirection = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentLocation)]
        public Point ShadowLocation
        {
            get
            {
                return shadowLocation;
            }

            set
            {
                shadowLocation = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ShadowOpacity)]
        public int ShadowOpacity
        {
            get
            {
                return shadowOpacity;
            }

            set
            {
                if (ExceptionManager.ArgumentOutOfRangeException(value, Settings.MinimumAlpha, Settings.MaximumAlpha))
                {
                    shadowOpacity = value;
                }

                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient TextColor
        {
            get
            {
                return textGradient;
            }

            set
            {
                textGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient TextDisabledColor
        {
            get
            {
                return textDisabledGradient;
            }

            set
            {
                textDisabledGradient = value;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            Gradient foreGradient = Enabled ? textGradient : textDisabledGradient;

            if (reflection && (orientation == Orientation.Vertical))
            {
                textBoxRectangle = new Rectangle(GDI.GetTextSize(graphics, Text, Font).Height, 0, ClientRectangle.Width, ClientRectangle.Height);
            }
            else
            {
                textBoxRectangle = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
            }

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(foreGradient.Colors, gradientPoints, textGradient.Angle, textGradient.Positions);

            // Draw the text outline
            if (outline)
            {
                DrawOutline(graphics);
            }

            // Draw the shadow
            if (shadow)
            {
                DrawShadow(graphics);
            }

            // Draw the reflection text.
            if (reflection)
            {
                DrawReflection(graphics);
            }

            // Draw text
            graphics.DrawString(Text, Font, gradientBrush, textBoxRectangle, GetStringFormat());
        }

        private void DrawOutline(Graphics graphics)
        {
            GraphicsPath outlinePath = new GraphicsPath();

            switch (orientation)
            {
                case Orientation.Horizontal:
                    {
                        outlinePath.AddString(
                            Text,
                            Font.FontFamily,
                            (int)Font.Style,
                            (graphics.DpiY * Font.SizeInPoints) / 72,
                            outlineLocation,
                            new StringFormat());

                        break;
                    }

                case Orientation.Vertical:
                    {
                        outlinePath.AddString(
                            Text,
                            Font.FontFamily,
                            (int)Font.Style,
                            (graphics.DpiY * Font.SizeInPoints) / 72,
                            outlineLocation,
                            new StringFormat(StringFormatFlags.DirectionVertical));

                        break;
                    }
            }

            graphics.DrawPath(new Pen(OutlineColor), outlinePath);
        }

        private void DrawReflection(Graphics graphics)
        {
            Point reflectionLocation = new Point(0, 0);
            Bitmap reflectionBitmap = new Bitmap(Width, Height);
            Graphics imageGraphics = Graphics.FromImage(reflectionBitmap);

            // Setup text render
            imageGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Rotate reflection
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        imageGraphics.TranslateTransform(0, GDI.GetTextSize(graphics, Text, Font).Height);
                        imageGraphics.ScaleTransform(1, -1);

                        reflectionLocation = new Point(0, textBoxRectangle.Y - (GDI.GetTextSize(graphics, Text, Font).Height / 2) - reflectionSpacing);
                        break;
                    }

                case Orientation.Vertical:
                    {
                        imageGraphics.ScaleTransform(-1, 1);
                        reflectionLocation = new Point((textBoxRectangle.X - (GDI.GetTextSize(graphics, Text, Font).Width / 2)) + reflectionSpacing, 0);
                        break;
                    }
            }

            // Draw reflected string
            imageGraphics.DrawString(Text, Font, new SolidBrush(reflectionColor), reflectionLocation, GetStringFormat());

            // Draw the reflection image
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(reflectionBitmap, ClientRectangle, 0, 0, reflectionBitmap.Width, reflectionBitmap.Height, GraphicsUnit.Pixel);
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        private void DrawShadow(Graphics graphics)
        {
            // Create shadow into a bitmap
            Bitmap shadowBitmap = new Bitmap(Math.Max((int)(Width / ShadowSmooth), 1), Math.Max((int)(Height / ShadowSmooth), 1));
            Graphics imageGraphics = Graphics.FromImage(shadowBitmap);

            // Setup text render
            imageGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Create transformation matrix
            Matrix transformMatrix = new Matrix();
            transformMatrix.Scale(1 / ShadowSmooth, 1 / ShadowSmooth);
            transformMatrix.Translate((float)(ShadowDepth * Math.Cos(shadowDirection)), (float)(ShadowDepth * Math.Sin(shadowDirection)));
            imageGraphics.Transform = transformMatrix;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    {
                        imageGraphics.DrawString(Text, Font, new SolidBrush(Color.FromArgb(shadowOpacity, shadowColor)), shadowLocation);
                        break;
                    }

                case Orientation.Vertical:
                    {
                        imageGraphics.DrawString(Text, Font, new SolidBrush(Color.FromArgb(shadowOpacity, shadowColor)), shadowLocation, new StringFormat(StringFormatFlags.DirectionVertical));
                        break;
                    }
            }

            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(shadowBitmap, ClientRectangle, 0, 0, shadowBitmap.Width, shadowBitmap.Height, GraphicsUnit.Pixel);
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        private StringFormat GetStringFormat()
        {
            StringFormat stringFormat = new StringFormat();

            switch (orientation)
            {
                case Orientation.Horizontal:
                    {
                        stringFormat = new StringFormat();
                        break;
                    }

                case Orientation.Vertical:
                    {
                        stringFormat = new StringFormat(StringFormatFlags.DirectionVertical);
                        break;
                    }
            }

            return stringFormat;
        }

        #endregion
    }
}