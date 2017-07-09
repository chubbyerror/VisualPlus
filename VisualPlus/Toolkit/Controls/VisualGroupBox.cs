namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(GroupBox))]
    [DefaultEvent("Enter")]
    [DefaultProperty("Text")]
    [Description("The Visual GroupBox")]
    public sealed class VisualGroupBox : ContainerBase
    {
        #region Variables

        private Drag _drag;
        private Expandable _expander;
        private GroupBoxStyle groupBoxStyle;
        private StringAlignment stringAlignment;
        private TitleAlignments titleAlign;
        private Border titleBorder;
        private int titleBoxHeight;
        private GraphicsPath titleBoxPath;
        private Rectangle titleBoxRectangle;
        private bool titleBoxVisible;
        private Gradient titleGradient;

        #endregion

        #region Constructors

        public VisualGroupBox()
        {
            groupBoxStyle = GroupBoxStyle.Default;
            stringAlignment = StringAlignment.Center;
            titleAlign = TitleAlignments.Top;
            titleBoxVisible = Settings.DefaultValue.TitleBoxVisible;
            titleBoxHeight = 25;
            titleGradient = new Gradient();

            Size = new Size(220, 180);
            Padding = new Padding(5, titleBoxHeight + Border.Thickness, 5, 5);

            _drag = new Drag(this, Settings.DefaultValue.Moveable);

            _expander = new Expandable(this, 25)
                {
                    Visible = false
                };

            titleBorder = new Border();
            titleGradient = StyleManager.ControlStatesStyle.ControlDisabled;
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
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

        [TypeConverter(typeof(DragConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Drag Drag
        {
            get
            {
                return _drag;
            }

            set
            {
                _drag = value;
            }
        }

        [TypeConverter(typeof(ExpandableConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Expandable Expandable
        {
            get
            {
                return _expander;
            }

            set
            {
                _expander = value;
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Alignment)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Alignment)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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
        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
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
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            Size textArea = GDI.GetTextSize(graphics, Text, Font);
            Rectangle group = ConfigureStyleBox(textArea);
            Rectangle title = ConfigureStyleTitleBox(textArea);

            titleBoxRectangle = new Rectangle(title.X, title.Y, title.Width, title.Height);
            titleBoxPath = Border.GetBorderShape(titleBoxRectangle, titleBorder.Type, titleBorder.Rounding);

            ControlGraphicsPath = Border.GetBorderShape(group, Border.Type, Border.Rounding);

            graphics.FillPath(new SolidBrush(Background), ControlGraphicsPath);

            Border.DrawBorderStyle(graphics, Border, MouseState, ControlGraphicsPath);

            if (titleBoxVisible)
            {
                var gradientPoints = new[] { new Point { X = titleBoxRectangle.Width, Y = 0 }, new Point { X = titleBoxRectangle.Width, Y = titleBoxRectangle.Height } };
                LinearGradientBrush gradientBrush = Gradient.CreateGradientBrush(titleGradient.Colors, gradientPoints, titleGradient.Angle, titleGradient.Positions);

                graphics.FillPath(gradientBrush, titleBoxPath);

                if (titleBorder.Visible)
                {
                    if ((MouseState == MouseStates.Hover) && titleBorder.HoverVisible)
                    {
                        Border.DrawBorder(graphics, titleBoxPath, titleBorder.Thickness, titleBorder.HoverColor);
                    }
                    else
                    {
                        Border.DrawBorder(graphics, titleBoxPath, titleBorder.Thickness, titleBorder.Color);
                    }
                }
            }

            if (groupBoxStyle == GroupBoxStyle.Classic)
            {
                graphics.FillRectangle(new SolidBrush(Background), titleBoxRectangle);
                graphics.DrawString(Text, Font, new SolidBrush(ForeColor), titleBoxRectangle);
            }
            else
            {
                StringFormat stringFormat = new StringFormat
                    {
                        Alignment = stringAlignment,
                        LineAlignment = StringAlignment.Center
                    };

                graphics.DrawString(Text, Font, new SolidBrush(ForeColor), titleBoxRectangle, stringFormat);
            }

            _expander.Draw(graphics, _expander.GetAlignmentPoint(Size));
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

                        // +1 extra whitespace in case of FontStyle=Bold
                        titleSize = new Size(textArea.Width + 1, textArea.Height);

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