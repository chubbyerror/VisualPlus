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
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(GroupBox))]
    [DefaultEvent("Enter")]
    [DefaultProperty("Text")]
    [Description("The Visual GroupBox")]
    public sealed class VisualGroupBox : ContainerBase
    {
        #region Variables

        private GraphicsPath borderGraphicsPath;
        private Expander expander;
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

            expander = new Expander(this, 25)
                {
                    Visible = false
                };

            InitializeTheme();
        }

        [Description("Occours when the expander toggle has changed.")]
        public delegate void ToggleChangedEventHandler();

        public event ToggleChangedEventHandler ToggleExpanderChanged;

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

        [TypeConverter(typeof(ExpanderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Expander Expander
        {
            get
            {
                return expander;
            }

            set
            {
                expander = value;
                Invalidate();
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (expander.MouseOnButton)
            {
                expander.Expanded = !expander.Expanded;
                ToggleExpanderChanged?.Invoke();
            }
            else
            {
                Focus();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (expander.Visible)
            {
                expander.GetMouseOnButton(e.Location);
                Cursor = expander.MouseOnButton ? expander.Cursor : Cursors.Default;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            if (StyleManager.LockedStyle)
            {
                InitializeTheme();
            }

            Size textArea = GDI.GetTextSize(graphics, Text, Font);
            Rectangle group = ConfigureStyleBox(textArea);
            Rectangle title = ConfigureStyleTitleBox(textArea);

            titleBoxRectangle = new Rectangle(title.X, title.Y, title.Width, title.Height);
            titleBoxPath = Border.GetBorderShape(titleBoxRectangle, titleBorder.Type, titleBorder.Rounding);

            borderGraphicsPath = Border.GetBorderShape(group, Border.Type, Border.Rounding);

            graphics.FillPath(new SolidBrush(Background), borderGraphicsPath);

            Border.DrawBorderStyle(graphics, Border, MouseState, borderGraphicsPath);

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

            if (expander.Visible)
            {
                Point buttonPoint = expander.GetAlignmentPoint(Size);
                expander.Draw(graphics, buttonPoint);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            expander?.UpdateOriginal(Size);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            expander?.UpdateOriginal(Size);
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

        private void InitializeTheme()
        {
            if (StyleManager.VisualStylesManager != null)
            {
                IBorder borderStyle = StyleManager.VisualStylesManager.BorderStyle;
                IControl controlStyle = StyleManager.VisualStylesManager.ControlStyle;

                titleBorder.Color = borderStyle.Color;
                titleBorder.HoverColor = borderStyle.HoverColor;
                titleBorder.HoverVisible = StyleManager.VisualStylesManager.BorderHoverVisible;
                titleBorder.Rounding = StyleManager.VisualStylesManager.BorderRounding;
                titleBorder.Type = StyleManager.VisualStylesManager.BorderType;
                titleBorder.Thickness = StyleManager.VisualStylesManager.BorderThickness;
                titleBorder.Visible = StyleManager.VisualStylesManager.BorderVisible;

                titleGradient = new Gradient
                    {
                        Colors = controlStyle.ControlDisabled.Colors,
                        Positions = controlStyle.ControlDisabled.Positions
                    };
            }
            else
            {
                titleBorder = new Border();

                titleGradient = new Gradient
                    {
                        Colors = Settings.DefaultValue.Control.ControlDisabled.Colors,
                        Positions = Settings.DefaultValue.Control.ControlDisabled.Positions
                    };
            }
        }

        #endregion
    }
}