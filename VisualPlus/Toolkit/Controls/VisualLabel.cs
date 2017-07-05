namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Label))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [Description("The Visual Label")]
    [Designer(ControlManager.FilterProperties.VisualLabel)]
    public sealed class VisualLabel : VisualControlBase
    {
        #region Variables

        private Gradient _foreDisabledGradient = new Gradient();
        private Gradient _foreGradient = new Gradient();

        private bool autoSize;
        private bool gradientString;
        private Orientation orientation = Orientation.Horizontal;
        private bool outline;
        private Color outlineColor = Color.Red;
        private Point outlineLocation = new Point(0, 0);
        private bool reflection;
        private Color reflectionColor = Color.FromArgb(120, 0, 0, 0);
        private int reflectionSpacing;
        private bool shadow;
        private Color shadowColor = Color.Black;
        private int ShadowDepth = 4;
        private int shadowDirection = 315;
        private Point shadowLocation = new Point(0, 0);
        private int shadowOpacity = 100;
        private float ShadowSmooth = 1.5f;
        private Rectangle textBoxRectangle;

        #endregion

        #region Constructors

        public VisualLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            UpdateStyles();
            BackColor = Color.Transparent;

            ConfigureStyleManager();
            DefaultGradient();
        }

        #endregion

        #region Properties

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.AutoSize)]
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient ForeGradient
        {
            get
            {
                return _foreGradient;
            }

            set
            {
                _foreGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient ForeGradientDisabled
        {
            get
            {
                return _foreDisabledGradient;
            }

            set
            {
                _foreDisabledGradient = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
        public bool GradientString
        {
            get
            {
                return gradientString;
            }

            set
            {
                gradientString = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Orientation)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Outline)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Point)]
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
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Spacing)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Toggle)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Direction)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Point)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Opacity)]
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

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(false)]
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        #endregion

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

            ConfigureStyleManager();

            Gradient foreGradient = Enabled ? _foreGradient : _foreDisabledGradient;

            if (reflection && (orientation == Orientation.Vertical))
            {
                textBoxRectangle = new Rectangle(GDI.GetTextSize(graphics, Text, Font).Height, 0, ClientRectangle.Width, ClientRectangle.Height);
            }
            else
            {
                textBoxRectangle = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
            }

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush gradientBrush = Gradient.CreateGradientBrush(foreGradient.Colors, gradientPoints, _foreGradient.Angle, _foreGradient.Positions);

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
            if (gradientString)
            {
                graphics.DrawString(Text, Font, gradientBrush, textBoxRectangle, GetStringFormat());
            }
            else
            {
                graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textBoxRectangle, GetStringFormat());
            }
        }

        private void ConfigureStyleManager()
        {
            if (StyleManager != null)
            {
                // IFont fontStyle = StyleManager.VisualStylesManager.VisualStylesInterface.FontStyle;
                // Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
            }
            else
            {
                // Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
            }
        }

        private void DefaultGradient()
        {
            Color[] foreColor =
                {
                    StyleManager.FontStyle.ForeColor,
                    StyleManager.FontStyle.ForeColor
                };

            Color[] textDisabledColor =
                {
                    ControlPaint.Light(StyleManager.FontStyle.ForeColorDisabled),
                    StyleManager.FontStyle.ForeColorDisabled
                };

            float[] gradientPosition = { 0, 1 };

            _foreGradient.Colors = foreColor;
            _foreGradient.Positions = gradientPosition;

            _foreDisabledGradient.Colors = textDisabledColor;
            _foreDisabledGradient.Positions = gradientPosition;
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