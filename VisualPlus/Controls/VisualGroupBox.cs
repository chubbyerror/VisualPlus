namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(GroupBox))]
    [DefaultEvent("Enter")]
    [DefaultProperty("Text")]
    [Description("The Visual GroupBox")]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public sealed class VisualGroupBox : GroupBox
    {
        #region Variables

        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private Border border = new Border();
        private GraphicsPath borderGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);

        private GroupBoxStyle groupBoxStyle = GroupBoxStyle.Default;
        private StringAlignment stringAlignment = StringAlignment.Center;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private TitleAlignments titleAlign = TitleAlignments.Top;
        private Border titleBorder = new Border();

        private Color[] titleBoxColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0)
            };

        private int titleBoxHeight = 25;
        private GraphicsPath titleBoxPath;
        private Rectangle titleBoxRectangle;
        private bool titleBoxVisible = Settings.DefaultValue.TitleBoxVisible;
        private Gradient titleGradient = new Gradient();

        #endregion

        #region Constructors

        public VisualGroupBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            ForeColor = Settings.DefaultValue.Style.ForeColor(0);
            Size = new Size(220, 180);
            MinimumSize = new Size(136, 50);
            Padding = new Padding(5, 28, 5, 5);
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            UpdateStyles();

            float[] gradientPosition = { 0, 1 };

            titleGradient.Colors = titleBoxColor;
            titleGradient.Positions = gradientPosition;
        }

        public enum GroupBoxStyle
        {
            /// <summary>The default.</summary>
            Default,

            /// <summary>The classic.</summary>
            Classic
        }

        public enum TitleAlignments
        {
            /// <summary>The bottom.</summary>
            Bottom,

            /// <summary>The top.</summary>
            Top
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
                ExceptionHandler.ApplyContainerBackColorChange(this, backgroundColor);
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
        [Description("The group box type.")]
        public GroupBoxStyle BoxStyle
        {
            get
            {
                return groupBoxStyle;
            }

            set
            {
                groupBoxStyle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.StringAlignment)]
        public StringAlignment TextAlignment
        {
            get
            {
                return stringAlignment;
            }

            set
            {
                stringAlignment = value;
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
        [Description(Localize.Description.ComponentColor)]
        public Color TextDisabledColor
        {
            get
            {
                return textDisabledColor;
            }

            set
            {
                textDisabledColor = value;
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
        [Description(Localize.Description.ComponentNoName)]
        public TitleAlignments TitleAlignment
        {
            get
            {
                return titleAlign;
            }

            set
            {
                titleAlign = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border TitleBorder
        {
            get
            {
                return titleBorder;
            }

            set
            {
                titleBorder = value;
                Invalidate();
            }
        }

        [DefaultValue("25")]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int TitleBoxHeight
        {
            get
            {
                return titleBoxHeight;
            }

            set
            {
                titleBoxHeight = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TitleBoxVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.TitleBoxVisible)]
        public bool TitleBoxVisible
        {
            get
            {
                return titleBoxVisible;
            }

            set
            {
                titleBoxVisible = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient TitleGradient
        {
            get
            {
                return titleGradient;
            }

            set
            {
                titleGradient = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnControlAdded(ControlEventArgs e)
        {
            ExceptionHandler.SetControlBackColor(e.Control, backgroundColor, false);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            ExceptionHandler.SetControlBackColor(e.Control, backgroundColor, true);
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
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            Size textArea = GDI.GetTextSize(graphics, Text, Font);

            Rectangle group = ConfigureStyleBox(textArea);
            Rectangle title = ConfigureStyleTitleBox(textArea);

            titleBoxRectangle = new Rectangle(title.X, title.Y, title.Width, title.Height);
            titleBoxPath = GDI.GetBorderShape(titleBoxRectangle, titleBorder.Shape, titleBorder.Rounding);

            borderGraphicsPath = GDI.GetBorderShape(group, border.Shape, border.Rounding);

            foreColor = Enabled ? foreColor : textDisabledColor;
            graphics.FillPath(new SolidBrush(backgroundColor), borderGraphicsPath);

            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, borderGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }

            if (titleBoxVisible)
            {
                var gradientPoints = new[] { new Point { X = titleBoxRectangle.Width, Y = 0 }, new Point { X = titleBoxRectangle.Width, Y = titleBoxRectangle.Height } };
                LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(titleGradient.Colors, gradientPoints, titleGradient.Angle, titleGradient.Positions);

                graphics.FillPath(gradientBrush, titleBoxPath);

                if (titleBorder.Visible)
                {
                    if ((controlState == ControlState.Hover) && titleBorder.HoverVisible)
                    {
                        GDI.DrawBorder(graphics, titleBoxPath, titleBorder.Thickness, titleBorder.HoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, titleBoxPath, titleBorder.Thickness, titleBorder.Color);
                    }
                }
            }

            if (groupBoxStyle == GroupBoxStyle.Classic)
            {
                graphics.FillRectangle(new SolidBrush(backgroundColor), titleBoxRectangle);
                graphics.DrawString(Text, Font, new SolidBrush(foreColor), titleBoxRectangle);
            }
            else
            {
                StringFormat stringFormat = new StringFormat
                    {
                        Alignment = stringAlignment,
                        LineAlignment = StringAlignment.Center
                    };

                graphics.DrawString(Text, Font, new SolidBrush(foreColor), titleBoxRectangle, stringFormat);
            }
        }

        private Rectangle ConfigureStyleBox(Size textArea)
        {
            Size groupBoxSize = new Size(Width, Height);
            Point groupBoxPoint = new Point(0, 0);

            switch (groupBoxStyle)
            {
                case GroupBoxStyle.Default:
                    {
                        break;
                    }

                case GroupBoxStyle.Classic:
                    {
                        if (titleAlign == TitleAlignments.Top)
                        {
                            groupBoxPoint = new Point(0, textArea.Height / 2);
                            groupBoxSize = new Size(Width, Height - (textArea.Height / 2));
                        }
                        else
                        {
                            groupBoxPoint = new Point(0, 0);
                            groupBoxSize = new Size(Width, Height - (textArea.Height / 2));
                        }

                        break;
                    }
            }

            Rectangle groupBoxRectangle = new Rectangle(groupBoxPoint, groupBoxSize);

            return groupBoxRectangle;
        }

        private Rectangle ConfigureStyleTitleBox(Size textArea)
        {
            Size titleSize = new Size(Width, titleBoxHeight);
            Point titlePoint = new Point(0, 0);

            switch (groupBoxStyle)
            {
                case GroupBoxStyle.Default:
                    {
                        // Declare Y
                        if (titleAlign == TitleAlignments.Top)
                        {
                            titlePoint = new Point(titlePoint.X, 0);
                        }
                        else
                        {
                            titlePoint = new Point(titlePoint.X, Height - titleBoxHeight);
                        }

                        break;
                    }

                case GroupBoxStyle.Classic:
                    {
                        titleBoxVisible = false;
                        var spacing = 5;

                        if (titleAlign == TitleAlignments.Top)
                        {
                            titlePoint = new Point(titlePoint.X, 0);
                        }
                        else
                        {
                            titlePoint = new Point(titlePoint.X, Height - textArea.Height);
                        }

                        titleSize = new Size(textArea.Width, textArea.Height);

                        switch (stringAlignment)
                        {
                            case StringAlignment.Near:
                                {
                                    titlePoint.X += 5;
                                    break;
                                }

                            case StringAlignment.Center:
                                {
                                    titlePoint.X = (Width / 2) - (textArea.Width / 2);
                                    break;
                                }

                            case StringAlignment.Far:
                                {
                                    titlePoint.X = Width - textArea.Width - spacing;
                                    break;
                                }
                        }

                        break;
                    }
            }

            Rectangle titleRectangle = new Rectangle(titlePoint, titleSize);
            return titleRectangle;
        }

        #endregion
    }
}