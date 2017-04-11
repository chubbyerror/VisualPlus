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

    [ToolboxBitmap(typeof(Label))]
    public class VisualLabel : Label
    {
        #region  ${0} Variables

        private Color hoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private Color mirrorColor = Color.Black;
        private bool hoverVisible;
        private bool mirrored;
        private int mirrorSpacing = 3;
        private Rectangle textBoxRectangle;
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

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
        public bool BorderHoverVisible
        {
            get
            {
                return hoverVisible;
            }

            set
            {
                hoverVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.HoverColor)]
        public Color HoverColor
        {
            get
            {
                return hoverColor;
            }

            set
            {
                hoverColor = value;
                Invalidate();
            }
        }

        [DefaultValue(false), Category(Localize.Category.Behavior),
         Description("Draws a reflection of the text.")]
        public bool Mirrored
        {
            get
            {
                return mirrored;
            }

            set
            {
                mirrored = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description("The spacing between the mirror.")]
        public int MirrorSpacing
        {
            get
            {
                return mirrorSpacing;
            }

            set
            {
                mirrorSpacing = value;
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.HoverColor)]
        public Color MirrorColor
        {
            get
            {
                return mirrorColor;
            }

            set
            {
                mirrorColor = value;
                Invalidate();
            }
        }

        #endregion

        #region ${0} Events

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (hoverVisible)
            {
                ForeColor = hoverColor;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ForeColor = Settings.DefaultValue.Style.ForeColor(0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            // String format
            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

            textBoxRectangle = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);

            // Draw the text
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textBoxRectangle, stringFormat);

            // Draw the mirrored text.
            if (mirrored)
            {
                // Mirror location
                Point mirrorLocation = new Point(0, -textBoxRectangle.Y - textBoxRectangle.Height / 2 - (int)Font.SizeInPoints + mirrorSpacing);

                graphics.TranslateTransform(0, Font.Size);
                graphics.ScaleTransform(1, -1);
                graphics.DrawString(Text, Font, new SolidBrush(mirrorColor), mirrorLocation);
                graphics.ResetTransform();
                
            }
        }

        #endregion
    }
}