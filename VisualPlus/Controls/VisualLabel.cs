namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Localization;

    /// <summary>The visual Label.</summary>
    [ToolboxBitmap(typeof(Label)), Designer(VSDesignerBinding.VisualLabel)]
    public class VisualLabel : Label
    {
        #region  ${0} Variables

        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color reflectionColor = Color.FromArgb(120, 0, 0, 0);
        private bool reflection;
        private int reflectionSpacing = 3;
        private bool shadow;
        private Color shadowColor = Settings.DefaultValue.Style.ShadowColor;
        private int shadowDepth = 4;
        private int shadowDirection = 315;
        private int shadowOpacity = 100;
        private float shadowSmooth = 2f;
        private Rectangle textBoxRectangle;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

        #endregion

        #region ${0} Properties

        public VisualLabel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            BackColor = Color.Transparent;
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.MirrorColor)]
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

        [DefaultValue(false), Category(Localize.Category.Behavior),
         Description("Draws a reflection of the text.")]
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

        [Category(Localize.Category.Layout), Description("The spacing between the mirror.")]
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

        // Shadow Properties
        [DefaultValue(false), Category(Localize.Category.Appearance),
         Description("Draws a shadow to the text.")]
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

        [Category(Localize.Category.Appearance), Description("Sets the shadow color.")]
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

        [Category(Localize.Category.Appearance), Description("Shadow direction")]
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

        [Category(Localize.Category.Appearance), Description("Shadow opacity.")]
        public int ShadowOpacity
        {
            get
            {
                return shadowOpacity;
            }

            set
            {
                shadowOpacity = value;
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextRenderingHint)]
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
            graphics.TextRenderingHint = textRendererHint;

            // Set control color state
            foreColor = Enabled ? foreColor : textDisabledColor;

            // String format
            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

            textBoxRectangle = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);

            // Draw the text
            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle, stringFormat);

            // Draw the shadow
            if (shadow)
            {
                DrawShadow(e);
            }

            // Draw the reflection text.
            if (reflection)
            {
                DrawReflection(graphics);
            }
        }

        private void DrawReflection(Graphics graphics)
        {
            // Mirror location
            Point mirrorLocation = new Point(0, -textBoxRectangle.Y - textBoxRectangle.Height / 2 - (int)Font.SizeInPoints + reflectionSpacing);

            graphics.TranslateTransform(0, Font.Size);
            graphics.ScaleTransform(1, -1);
            graphics.DrawString(Text, Font, new SolidBrush(reflectionColor), mirrorLocation);
            graphics.ResetTransform();
        }

        private void DrawShadow(PaintEventArgs e)
        {
            Graphics screenGraphics = e.Graphics;
            Bitmap shadowBitmap = new Bitmap(Math.Max((int)(Width / shadowSmooth), 1), Math.Max((int)(Height / shadowSmooth), 1));
            using (Graphics imageGraphics = Graphics.FromImage(shadowBitmap))
            {
                imageGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                Matrix transformMatrix = new Matrix();
                transformMatrix.Scale(1 / shadowSmooth, 1 / shadowSmooth);
                transformMatrix.Translate((float)(shadowDepth * Math.Cos(shadowDirection)),
                    (float)(shadowDepth * Math.Sin(shadowDirection)));
                imageGraphics.Transform = transformMatrix;
                imageGraphics.DrawString(Text, Font,
                    new SolidBrush(Color.FromArgb(shadowOpacity, shadowColor)), 0, 0,
                    StringFormat.GenericTypographic);
            }

            screenGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            screenGraphics.DrawImage(shadowBitmap, ClientRectangle, 0, 0,
                shadowBitmap.Width, shadowBitmap.Height, GraphicsUnit.Pixel);
            screenGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        #endregion
    }
}