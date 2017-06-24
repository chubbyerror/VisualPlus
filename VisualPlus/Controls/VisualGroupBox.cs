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

    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

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

        private readonly MouseState mouseState;

        private Color backgroundColor;
        private Border border = new Border();

        private GraphicsPath borderGraphicsPath;

        private Expander expander;

        private Color foreColor;
        private GroupBoxStyle groupBoxStyle = GroupBoxStyle.Default;
        private StringAlignment stringAlignment = StringAlignment.Center;

        private StyleManager styleManager = new StyleManager();
        private Color textDisabledColor;
        private TextRenderingHint textRendererHint;
        private TitleAlignments titleAlign = TitleAlignments.Top;
        private Border titleBorder = new Border();
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

            mouseState = new MouseState(this);
            Size = new Size(220, 180);
            Padding = new Padding(5, 28, 5, 5);
            UpdateStyles();

            expander = new Expander(this, 25)
                {
                    Visible = false
                };

            ConfigureStyleManager();
            DefaultGradient();
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
                ExceptionManager.ApplyContainerBackColorChange(this, backgroundColor);
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

        [TypeConverter(typeof(ExpanderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
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
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        public MouseStates MouseState
        {
            get
            {
                return mouseState.State;
            }

            set
            {
                mouseState.State = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
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
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
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
        [Category(Localize.Category.Behavior)]
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
            ExceptionManager.SetControlBackColor(e.Control, backgroundColor, false);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            ExceptionManager.SetControlBackColor(e.Control, backgroundColor, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (expander.MouseOnButton)
            {
                if (expander.Expanded)
                {
                    expander.Expanded = false;
                }
                else
                {
                    expander.Expanded = true;
                }

                ToggleExpanderChanged?.Invoke();
            }
            else
            {
                Focus();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseState.State = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (expander.Visible)
            {
                expander.GetMouseOnButton(e.Location);

                if (expander.MouseOnButton)
                {
                    Cursor = expander.Cursor;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            Size textArea = GDI.GetTextSize(graphics, Text, Font);
            Rectangle group = ConfigureStyleBox(textArea);
            Rectangle title = ConfigureStyleTitleBox(textArea);

            titleBoxRectangle = new Rectangle(title.X, title.Y, title.Width, title.Height);
            titleBoxPath = GDI.GetBorderShape(titleBoxRectangle, titleBorder.Type, titleBorder.Rounding);

            borderGraphicsPath = GDI.GetBorderShape(group, border.Type, border.Rounding);

            foreColor = Enabled ? foreColor : textDisabledColor;
            graphics.FillPath(new SolidBrush(backgroundColor), borderGraphicsPath);

            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, mouseState.State, borderGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }

            if (titleBoxVisible)
            {
                var gradientPoints = new[] { new Point { X = titleBoxRectangle.Width, Y = 0 }, new Point { X = titleBoxRectangle.Width, Y = titleBoxRectangle.Height } };
                LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(titleGradient.Colors, gradientPoints, titleGradient.Angle, titleGradient.Positions);

                graphics.FillPath(gradientBrush, titleBoxPath);

                if (titleBorder.Visible)
                {
                    if ((mouseState.State == MouseStates.Hover) && titleBorder.HoverVisible)
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

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.BorderStyle;
                IControl controlStyle = styleManager.VisualStylesManager.ControlStyle;
                IFont fontStyle = styleManager.VisualStylesManager.FontStyle;

                border.Color = borderStyle.Color;
                border.HoverColor = borderStyle.HoverColor;
                border.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                border.Rounding = styleManager.VisualStylesManager.BorderRounding;
                border.Type = styleManager.VisualStylesManager.BorderType;
                border.Thickness = styleManager.VisualStylesManager.BorderThickness;
                border.Visible = styleManager.VisualStylesManager.BorderVisible;

                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;
                backgroundColor = controlStyle.Background(0);
            }
            else
            {
                // Load default settings
                border.HoverVisible = Settings.DefaultValue.BorderHoverVisible;
                border.Rounding = Settings.DefaultValue.Rounding.Default;
                border.Type = Settings.DefaultValue.BorderShape;
                border.Thickness = Settings.DefaultValue.BorderThickness;
                border.Visible = Settings.DefaultValue.BorderVisible;
                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
                backgroundColor = Settings.DefaultValue.Control.Background(0);
            }
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

        private void DefaultGradient()
        {
            titleGradient.Colors = Settings.DefaultValue.Control.ControlDisabled.Colors;
            titleGradient.Positions = Settings.DefaultValue.Control.ControlDisabled.Positions;
        }

        #endregion
    }
}