namespace VisualPlus.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual ToolTip.</summary>
    [ToolboxBitmap(typeof(ToolTip))]
    [Designer(VSDesignerBinding.VisualToolTip)]
    public sealed class VisualToolTip : ToolTip
    {
        #region Variables

        private Color[] backgroundColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private bool contentsVisible = true;
        private Point endPoint;
        private Font font = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Regular);
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private float gradientAngle;
        private LinearGradientBrush gradientBrush;
        private float[] gradientPosition = { 0, 1 / 2f, 1 };
        private Point startPoint;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private bool textShadow;

        #endregion

        #region Constructors

        public VisualToolTip()
        {
            OwnerDraw = true;
            Popup += VisualToolTip_Popup;
            Draw += VisualToolTip_Draw;
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
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ContentsVisible
        {
            get
            {
                return contentsVisible;
            }

            set
            {
                contentsVisible = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font Font
        {
            get
            {
                return font;
            }

            set
            {
                font = value;
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
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool TextShadow
        {
            get
            {
                return textShadow;
            }

            set
            {
                textShadow = value;
            }
        }

        #endregion

        #region Events

        private void VisualToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            startPoint = new Point(e.Bounds.Width, 0);
            endPoint = new Point(e.Bounds.Width, e.Bounds.Height);

            gradientBrush = GDI.CreateGradientBrush(backgroundColor, gradientPosition, gradientAngle, startPoint, endPoint);

            // Background
            graphics.FillRectangle(gradientBrush, e.Bounds);

            // Create border
            if (borderVisible)
            {
                Rectangle border = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddRectangle(border);

                // Draw border
                graphics.DrawPath(new Pen(borderColor, borderThickness), borderPath);
            }

            if (textShadow)
            {
                graphics.DrawString(e.ToolTipText, new Font(e.Font, FontStyle.Bold), Brushes.Silver, new PointF(e.Bounds.X + 3, e.Bounds.Y + 3));
            }

            if (contentsVisible)
            {
                graphics.DrawString(e.ToolTipText, new Font(e.Font, FontStyle.Bold), new SolidBrush(foreColor), new PointF(e.Bounds.X + 2, e.Bounds.Y + 2));
            }

            gradientBrush.Dispose();
        }

        private void VisualToolTip_Popup(object sender, PopupEventArgs e)
        {
            // e.ToolTipSize = new Size(TextRenderer.MeasureText(contents, font).Width, TextRenderer.MeasureText(contents, font).Height);
        }

        #endregion
    }
}