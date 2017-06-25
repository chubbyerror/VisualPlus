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
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Properties;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ToolTip))]
    [DefaultEvent("Popup")]
    [DefaultProperty("Text")]
    [Description("The Visual ToolTip")]
    [Designer(ControlManager.FilterProperties.VisualToolTip)]
    public sealed class VisualToolTip : ToolTip
    {
        #region Variables

        private bool autoSize = true;
        private Gradient backgroundGradient = new Gradient();
        private Border border = new Border();
        private Font font = Settings.DefaultValue.DefaultFont;
        private Color foreColor = Settings.DefaultValue.Font.ForeColor;
        private Image icon = Resources.Icon;
        private bool iconBorder;
        private GraphicsPath iconGraphicsPath;
        private Point iconPoint = new Point(0, 0);
        private Rectangle iconRectangle;
        private Size iconSize = new Size(24, 24);
        private Color lineColor = Settings.DefaultValue.Control.Line;
        private Padding padding = new Padding(4, 4, 4, 4);
        private Rectangle separator;
        private int separatorThickness = 1;
        private int spacing = 2;
        private string text = "Enter your custom text here.";
        private Point textPoint;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private bool textShadow;
        private string title = "Title";
        private Color titleColor = Color.Gray;
        private Font titleFont = Settings.DefaultValue.DefaultFont;
        private Point titlePoint;
        private Size toolTipSize = new Size(100, 40);
        private ToolTipType toolTipType = ToolTipType.Default;
        private int xWidth;
        private int yHeight;

        #endregion

        #region Constructors

        public VisualToolTip()
        {
            backgroundGradient.Colors = Settings.DefaultValue.Control.ControlEnabled.Colors;
            backgroundGradient.Positions = Settings.DefaultValue.Control.ControlEnabled.Positions;

            IsBalloon = false;
            OwnerDraw = true;
            Popup += VisualToolTip_Popup;
            Draw += VisualToolTip_Draw;
        }

        public enum ToolTipType
        {
            /// <summary>The default.</summary>
            Default = 0,

            /// <summary>The image.</summary>
            Image = 1,

            /// <summary>The text.</summary>
            Text = 2
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Common.AutoSize)]
        public bool AutoSize
        {
            get
            {
                return autoSize;
            }

            set
            {
                autoSize = value;
            }
        }

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
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Strings.Font)]
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
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Image)]
        public Image Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Visible)]
        public bool IconBorder
        {
            get
            {
                return iconBorder;
            }

            set
            {
                iconBorder = value;
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
        public Size IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                iconSize = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color LineColor
        {
            get
            {
                return lineColor;
            }

            set
            {
                lineColor = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Padding)]
        public Padding Padding
        {
            get
            {
                return padding;
            }

            set
            {
                padding = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Size)]
        public int SeparatorThickness
        {
            get
            {
                return separatorThickness;
            }

            set
            {
                separatorThickness = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Spacing)]
        public int Spacing
        {
            get
            {
                return spacing;
            }

            set
            {
                spacing = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Strings.Text)]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Strings.TextRenderingHint)]
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
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Type)]
        public ToolTipType TipType
        {
            get
            {
                return toolTipType;
            }

            set
            {
                toolTipType = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Strings.Text)]
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color TitleColor
        {
            get
            {
                return titleColor;
            }

            set
            {
                titleColor = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Strings.Font)]
        public Font TitleFont
        {
            get
            {
                return titleFont;
            }

            set
            {
                titleFont = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Size)]
        public Size ToolTipSize
        {
            get
            {
                return toolTipSize;
            }

            set
            {
                toolTipSize = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Input the text height to compare it to the icon height.</summary>
        /// <param name="textHeight">The text height.</param>
        /// <returns>New height.</returns>
        private int GetTipHeight(int textHeight)
        {
            int tipHeight = textHeight > iconSize.Height ? textHeight : iconSize.Height;
            return tipHeight;
        }

        /// <summary>Input the title and text width to retrieve total width.</summary>
        /// <param name="titleWidth">The title width.</param>
        /// <param name="textWidth">The text width.</param>
        /// <returns>New width.</returns>
        private int GetTipWidth(int titleWidth, int textWidth)
        {
            int tipWidth = titleWidth > iconSize.Width + textWidth ? titleWidth : iconSize.Width + textWidth;
            return tipWidth;
        }

        private void VisualToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            var gradientPoints = new[] { new Point { X = e.Bounds.Width, Y = 0 }, new Point { X = e.Bounds.Width, Y = e.Bounds.Height } };

            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(backgroundGradient.Colors, gradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            graphics.FillRectangle(gradientBrush, e.Bounds);

            if (border.Visible)
            {
                Rectangle boxRectangle = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddRectangle(boxRectangle);
                graphics.DrawPath(new Pen(border.Color, border.Thickness), borderPath);
            }

            if ((textShadow && (toolTipType == ToolTipType.Text)) || (textShadow && (toolTipType == ToolTipType.Default)))
            {
                // Draw shadow text
                graphics.DrawString(text, new Font(Font, FontStyle.Regular), Brushes.Silver, new PointF(textPoint.X + 1, textPoint.Y + 1));
            }

            switch (toolTipType)
            {
                case ToolTipType.Default:
                    {
                        // Draw the title
                        graphics.DrawString(title, titleFont, new SolidBrush(titleColor), new PointF(titlePoint.X, titlePoint.Y));

                        // Draw the separator
                        graphics.DrawLine(new Pen(lineColor), separator.X, separator.Y, separator.Width, separator.Y);

                        // Draw the text
                        graphics.DrawString(text, Font, new SolidBrush(foreColor), new PointF(textPoint.X, textPoint.Y));

                        if (Icon != null)
                        {
                            // Update point
                            iconRectangle.Location = iconPoint;

                            // Draw icon border
                            if (iconBorder)
                            {
                                graphics.DrawPath(new Pen(border.Color), iconGraphicsPath);
                            }

                            // Draw icon
                            graphics.DrawImage(Icon, iconRectangle);
                        }

                        break;
                    }

                case ToolTipType.Image:
                    {
                        if (Icon != null)
                        {
                            // Update point
                            iconRectangle.Location = iconPoint;

                            // Draw icon border
                            if (iconBorder)
                            {
                                graphics.DrawPath(new Pen(border.Color), iconGraphicsPath);
                            }

                            // Draw icon
                            graphics.DrawImage(Icon, iconRectangle);
                        }

                        break;
                    }

                case ToolTipType.Text:
                    {
                        // Draw the text
                        graphics.DrawString(text, Font, new SolidBrush(foreColor), new PointF(textPoint.X, textPoint.Y));
                        break;
                    }
            }

            gradientBrush.Dispose();
        }

        private void VisualToolTip_Popup(object sender, PopupEventArgs e)
        {
            switch (toolTipType)
            {
                case ToolTipType.Default:
                    {
                        if (!autoSize)
                        {
                            xWidth = toolTipSize.Width;
                            yHeight = toolTipSize.Height;
                        }
                        else
                        {
                            xWidth = GetTipWidth(TextRenderer.MeasureText(title, Font).Width, TextRenderer.MeasureText(text, Font).Width);
                            yHeight = TextRenderer.MeasureText(title, Font).Height + SeparatorThickness + GetTipHeight(TextRenderer.MeasureText(text, Font).Height);
                        }

                        titlePoint.X = padding.Left;
                        titlePoint.Y = padding.Top;

                        Point separatorPoint = new Point(padding.Left + Spacing, TextRenderer.MeasureText(title, Font).Height + 5);
                        Size separatorSize = new Size(xWidth, SeparatorThickness);
                        separator = new Rectangle(separatorPoint, separatorSize);

                        textPoint.X = padding.Left + iconSize.Width + Spacing;
                        textPoint.Y = separator.Y + Spacing;

                        iconPoint = new Point(padding.Left, textPoint.Y);
                        break;
                    }

                case ToolTipType.Image:
                    {
                        iconPoint = new Point(padding.Left, padding.Top);
                        xWidth = iconSize.Width + 1;
                        yHeight = iconSize.Height + 1;
                        break;
                    }

                case ToolTipType.Text:
                    {
                        textPoint = new Point(padding.Left, padding.Top);
                        xWidth = TextRenderer.MeasureText(text, Font).Width;
                        yHeight = TextRenderer.MeasureText(text, Font).Height;
                        break;
                    }
            }

            // Create icon rectangle
            iconRectangle = new Rectangle(iconPoint, iconSize);

            // Create icon path
            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(iconRectangle);
            iconGraphicsPath.CloseAllFigures();

            // Initialize new size
            e.ToolTipSize = new Size(padding.Left + xWidth + padding.Right, padding.Top + yHeight + padding.Bottom);
        }

        #endregion
    }
}