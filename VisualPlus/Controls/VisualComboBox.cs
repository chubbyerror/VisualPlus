namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Components.Symbols;
    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ComboBox))]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("Items")]
    [Description("The Visual ComboBox")]
    [Designer(DesignManager.VisualComboBox)]
    public sealed class VisualComboBox : ComboBox
    {
        #region Variables

        private readonly Color[] controlColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private readonly Color[] controlDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled,
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled)
            };

        private Border border = new Border();
        private Color buttonColor = Settings.DefaultValue.Style.DropDownButtonColor;
        private Direction buttonDirection = Direction.Right;
        private DropDownButtons buttonStyles = DropDownButtons.Arrow;
        private bool buttonVisible = Settings.DefaultValue.TextVisible;
        private int buttonWidth = 30;
        private Gradient controlDisabledGradient = new Gradient();
        private Gradient controlGradient = new Gradient();
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Border itemBorder = new Border();
        private Color menuItemHover = Settings.DefaultValue.Style.ItemHover(0);
        private Color menuItemNormal = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color menuTextColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color separatorColor = Settings.DefaultValue.Style.LineColor;
        private Color separatorShadowColor = Settings.DefaultValue.Style.ShadowColor;
        private bool separatorVisible = Settings.DefaultValue.TextVisible;
        private int startIndex;
        private StringAlignment textAlignment = StringAlignment.Center;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color waterMarkActiveColor = Color.Gray;
        private SolidBrush waterMarkBrush;
        private Color waterMarkColor = Color.LightGray;
        private Font waterMarkFont;
        private string waterMarkText = Settings.DefaultValue.WatermarkText;
        private bool watermarkVisible = Settings.DefaultValue.WatermarkVisible;

        #endregion

        #region Constructors

        public VisualComboBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            SetStyle((ControlStyles)139286, true);
            SetStyle(ControlStyles.Selectable, false);
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            Size = new Size(135, 26);
            ItemHeight = 20;
            UpdateStyles();
            DropDownHeight = 100;
            BackColor = Color.Transparent;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            itemBorder.HoverVisible = false;
            float[] gradientPosition = { 0, 1 / 2f, 1 };
            controlGradient.Colors = controlColor;
            controlGradient.Positions = gradientPosition;
            controlDisabledGradient.Colors = controlDisabledColor;
            controlDisabledGradient.Positions = gradientPosition;
            waterMarkFont = Font;
            waterMarkBrush = new SolidBrush(waterMarkActiveColor);
        }

        public enum DropDownButtons
        {
            /// <summary>Use arrow button.</summary>
            Arrow,

            /// <summary>Use bars button.</summary>
            Bars
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Background
        {
            get
            {
                return controlGradient;
            }

            set
            {
                controlGradient = value;
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
        [Description(Localize.Description.ComponentColor)]
        public Color ButtonColor
        {
            get
            {
                return buttonColor;
            }

            set
            {
                buttonColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Direction)]
        public Direction ButtonDirection
        {
            get
            {
                return buttonDirection;
            }

            set
            {
                buttonDirection = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.DropDownButton)]
        public DropDownButtons ButtonStyles
        {
            get
            {
                return buttonStyles;
            }

            set
            {
                buttonStyles = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ButtonVisible
        {
            get
            {
                return buttonVisible;
            }

            set
            {
                buttonVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentSize)]
        public int ButtonWidth
        {
            get
            {
                return buttonWidth;
            }

            set
            {
                buttonWidth = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient DisabledBackground
        {
            get
            {
                return controlDisabledGradient;
            }

            set
            {
                controlDisabledGradient = value;
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

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Item
        {
            get
            {
                return itemBorder;
            }

            set
            {
                itemBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color MenuItemHover
        {
            get
            {
                return menuItemHover;
            }

            set
            {
                menuItemHover = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color MenuItemNormal
        {
            get
            {
                return menuItemNormal;
            }

            set
            {
                menuItemNormal = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color MenuTextColor
        {
            get
            {
                return menuTextColor;
            }

            set
            {
                menuTextColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color SeparatorColor
        {
            get
            {
                return separatorColor;
            }

            set
            {
                separatorColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color SeparatorShadowColor
        {
            get
            {
                return separatorShadowColor;
            }

            set
            {
                separatorShadowColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool SeparatorVisible
        {
            get
            {
                return separatorVisible;
            }

            set
            {
                separatorVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.StartIndex)]
        public int StartIndex
        {
            get
            {
                return startIndex;
            }

            set
            {
                startIndex = value;
                try
                {
                    SelectedIndex = value;
                }
                catch (Exception)
                {
                    // ignored
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Alignment)]
        public StringAlignment TextAlignment
        {
            get
            {
                return textAlignment;
            }

            set
            {
                textAlignment = value;
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Watermark)]
        public string WaterMark
        {
            get
            {
                return waterMarkText;
            }

            set
            {
                waterMarkText = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color WaterMarkActiveForeColor
        {
            get
            {
                return waterMarkActiveColor;
            }

            set
            {
                waterMarkActiveColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font WaterMarkFont
        {
            get
            {
                return waterMarkFont;
            }

            set
            {
                waterMarkFont = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color WaterMarkForeColor
        {
            get
            {
                return waterMarkColor;
            }

            set
            {
                waterMarkColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool WatermarkVisible
        {
            get
            {
                return watermarkVisible;
            }

            set
            {
                watermarkVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(
                (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? new SolidBrush(menuItemHover)
                    : new SolidBrush(menuItemNormal),
                e.Bounds);

            Size itemSize = new Size(e.Bounds.Width - itemBorder.Thickness, e.Bounds.Height - itemBorder.Thickness);
            Point itemPoint = new Point(e.Bounds.X, e.Bounds.Y);
            Rectangle itemBorderRectangle = new Rectangle(itemPoint, itemSize);
            GraphicsPath itemBorderPath = new GraphicsPath();
            itemBorderPath.AddRectangle(itemBorderRectangle);

            if (itemBorder.Visible)
            {
                GDI.DrawBorder(e.Graphics, itemBorderPath, itemBorder.Thickness, border.Color);
            }

            if (e.Index != -1)
            {
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(menuTextColor), e.Bounds);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            waterMarkBrush = new SolidBrush(waterMarkActiveColor);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            waterMarkBrush = new SolidBrush(waterMarkColor);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            SuspendLayout();
            Update();
            ResumeLayout();
            controlState = ControlState.Normal;
            Invalidate();
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

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);

            foreColor = Enabled ? foreColor : textDisabledColor;
            Gradient controlCheckTemp;

            if (Enabled)
            {
                controlCheckTemp = controlGradient;
            }
            else
            {
                controlCheckTemp = controlDisabledGradient;
            }

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush gradientBackgroundBrush = GDI.CreateGradientBrush(controlCheckTemp.Colors, gradientPoints, controlCheckTemp.Angle, controlCheckTemp.Positions);
            graphics.FillPath(gradientBackgroundBrush, controlGraphicsPath);

            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }

            Point textBoxPoint;
            Point buttonPoint;
            Size buttonSize = new Size(buttonWidth, Height);

            if (buttonDirection == Direction.Right)
            {
                buttonPoint = new Point(Width - buttonWidth, 0);
                textBoxPoint = new Point(0, 0);
            }
            else
            {
                buttonPoint = new Point(0, 0);
                textBoxPoint = new Point(buttonWidth, 0);
            }

            Rectangle buttonRectangle = new Rectangle(buttonPoint, buttonSize);
            Rectangle textBoxRectangle = new Rectangle(textBoxPoint.X, textBoxPoint.Y, Width - buttonWidth, Height);

            DrawButton(graphics, buttonRectangle);
            DrawSeparator(graphics, buttonRectangle);

            StringFormat stringFormat = new StringFormat
                {
                    Alignment = textAlignment,
                    LineAlignment = StringAlignment.Center
                };

            ConfigureDirection(textBoxRectangle, buttonRectangle);

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle, stringFormat);

            if (watermarkVisible && (Text.Length == 0))
            {
                graphics.DrawString(waterMarkText, WaterMarkFont, waterMarkBrush, textBoxRectangle, stringFormat);
            }
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            OnLostFocus(e);
        }

        private void ConfigureDirection(Rectangle textBoxRectangle, Rectangle buttonRectangle)
        {
            if (buttonDirection == Direction.Right)
            {
                if (textAlignment == StringAlignment.Far)
                {
                    textBoxRectangle.Width -= buttonRectangle.Width;
                }
                else if (textAlignment == StringAlignment.Near)
                {
                    textBoxRectangle.X = 0;
                }
            }
            else
            {
                if (textAlignment == StringAlignment.Far)
                {
                    textBoxRectangle.Width -= buttonRectangle.Width;
                    textBoxRectangle.X = Width - textBoxRectangle.Width;
                }
                else if (textAlignment == StringAlignment.Near)
                {
                    textBoxRectangle.X = buttonWidth;
                }
            }
        }

        private void DrawButton(Graphics graphics, Rectangle buttonRectangle)
        {
            if (buttonVisible)
            {
                Point buttonImagePoint;
                Size buttonImageSize;

                switch (buttonStyles)
                {
                    case DropDownButtons.Arrow:
                        {
                            buttonImageSize = new Size(25, 25);
                            buttonImagePoint = new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - (buttonImageSize.Width / 2), (buttonRectangle.Y + (buttonRectangle.Height / 2)) - (buttonImageSize.Height / 2));
                            Arrow.DrawArrow(graphics, buttonImagePoint, buttonImageSize, buttonColor, 13);
                            break;
                        }

                    case DropDownButtons.Bars:
                        {
                            buttonImageSize = new Size(18, 10);
                            buttonImagePoint = new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - (buttonImageSize.Width / 2), (buttonRectangle.Y + (buttonRectangle.Height / 2)) - buttonImageSize.Height);
                            Bars.DrawBars(graphics, buttonImagePoint, buttonImageSize, buttonColor, 3, 5);
                            break;
                        }
                }
            }
        }

        private void DrawSeparator(Graphics graphics, Rectangle buttonRectangle)
        {
            if (separatorVisible)
            {
                if (buttonDirection == Direction.Right)
                {
                    graphics.DrawLine(new Pen(separatorColor), buttonRectangle.X - 1, 4, buttonRectangle.X - 1, Height - 5);
                    graphics.DrawLine(new Pen(separatorShadowColor), buttonRectangle.X, 4, buttonRectangle.X, Height - 5);
                }
                else
                {
                    graphics.DrawLine(new Pen(separatorColor), buttonRectangle.Width - 1, 4, buttonRectangle.Width - 1, Height - 5);
                    graphics.DrawLine(new Pen(separatorShadowColor), buttonRectangle.Width, 4, buttonRectangle.Width, Height - 5);
                }
            }
        }

        #endregion
    }
}