namespace VisualPlus.Toolkit.Controls
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
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ComboBox))]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("Items")]
    [Description("The Visual ComboBox")]
    [Designer(ControlManager.FilterProperties.VisualComboBox)]
    public sealed class VisualComboBox : ComboBox
    {
        #region Variables

        private Border border;
        private Color buttonColor;
        private Alignment.Horizontal buttonHorizontal = Alignment.Horizontal.Right;
        private DropDownButtons buttonStyles = DropDownButtons.Arrow;
        private bool buttonVisible = Settings.DefaultValue.TextVisible;
        private int buttonWidth = 30;
        private Gradient controlDisabledGradient = new Gradient();
        private Gradient controlGradient = new Gradient();
        private GraphicsPath controlGraphicsPath;
        private Color foreColor;
        private Border itemBorder;

        private Size itemSize;
        private Color menuItemHover;
        private Color menuItemNormal;
        private Color menuTextColor;

        private MouseStates mouseState;
        private Color separatorColor;
        private Color separatorShadowColor;
        private bool separatorVisible = Settings.DefaultValue.TextVisible;
        private int startIndex;
        private StyleManager styleManager = new StyleManager();
        private StringAlignment textAlignment = StringAlignment.Center;
        private Color textDisabledColor;
        private TextRenderingHint textRendererHint;
        private Watermark watermark = new Watermark();

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

            DefaultGradient();
            ConfigureStyleManager();
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
        [Category(Localize.PropertiesCategory.Appearance)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Direction)]
        public Alignment.Horizontal ButtonHorizontal
        {
            get
            {
                return buttonHorizontal;
            }

            set
            {
                buttonHorizontal = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
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
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Size)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.StartIndex)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.MouseState)]
        public MouseStates State
        {
            get
            {
                return mouseState;
            }

            set
            {
                mouseState = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(StyleManagerConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public StyleManager StyleManager
        {
            get
            {
                return styleManager;
            }

            set
            {
                styleManager = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Alignment)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
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

        [TypeConverter(typeof(WatermarkConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Watermark Watermark
        {
            get
            {
                return watermark;
            }

            set
            {
                watermark = value;
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

            itemSize = GetItemSize(e.Bounds);

            Point itemPoint = new Point(e.Bounds.X, e.Bounds.Y);
            Rectangle itemBorderRectangle = new Rectangle(itemPoint, itemSize);
            GraphicsPath itemBorderPath = new GraphicsPath();
            itemBorderPath.AddRectangle(itemBorderRectangle);

            if (itemBorder.Visible)
            {
                Border.DrawBorder(e.Graphics, itemBorderPath, itemBorder.Thickness, border.Color);
            }

            if (e.Index != -1)
            {
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(menuTextColor), e.Bounds);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            watermark.Brush = new SolidBrush(watermark.ActiveColor);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            watermark.Brush = new SolidBrush(watermark.InactiveColor);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            SuspendLayout();
            Update();
            ResumeLayout();
            State = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, border.Type, border.Rounding);

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
            LinearGradientBrush gradientBackgroundBrush = Gradient.CreateGradientBrush(controlCheckTemp.Colors, gradientPoints, controlCheckTemp.Angle, controlCheckTemp.Positions);
            graphics.FillPath(gradientBackgroundBrush, controlGraphicsPath);

            Border.DrawBorderStyle(graphics, border, State, controlGraphicsPath);

            Point textBoxPoint;
            Point buttonPoint;
            Size buttonSize = new Size(buttonWidth, Height);

            if (buttonHorizontal == Alignment.Horizontal.Right)
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

            if (Text.Length == 0)
            {
                Watermark.DrawWatermark(graphics, textBoxRectangle, stringFormat, watermark);
            }
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            OnLostFocus(e);
        }

        private void ConfigureDirection(Rectangle textBoxRectangle, Rectangle buttonRectangle)
        {
            if (buttonHorizontal == Alignment.Horizontal.Right)
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

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.VisualStylesInterface.BorderStyle;
                IControl controlStyle = styleManager.VisualStylesManager.VisualStylesInterface.ControlStyle;
                IFont fontStyle = styleManager.VisualStylesManager.VisualStylesInterface.FontStyle;

                buttonColor = controlStyle.FlatButtonEnabled;
                menuTextColor = fontStyle.ForeColor;

                menuItemNormal = controlStyle.ItemEnabled;
                menuItemHover = controlStyle.ItemHover;

                separatorColor = controlStyle.Line;
                separatorShadowColor = controlStyle.Shadow;

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

                controlGradient.Colors = controlStyle.BoxEnabled.Colors;
                controlGradient.Positions = controlStyle.BoxEnabled.Positions;
                controlDisabledGradient.Colors = controlStyle.BoxDisabled.Colors;
                controlDisabledGradient.Positions = controlStyle.BoxDisabled.Positions;
            }
            else
            {
                // Load default settings
                border = new Border();
                itemBorder = new Border { HoverVisible = false };

                buttonColor = Settings.DefaultValue.Control.FlatButtonEnabled;
                menuTextColor = Settings.DefaultValue.Font.ForeColor;

                menuItemNormal = Settings.DefaultValue.Control.ItemEnabled;
                menuItemHover = Settings.DefaultValue.Control.ItemHover;

                separatorColor = Settings.DefaultValue.Control.Line;
                separatorShadowColor = Settings.DefaultValue.Control.Shadow;

                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = Settings.DefaultValue.DefaultFont;
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        private void DefaultGradient()
        {
            controlGradient.Colors = Settings.DefaultValue.Control.BoxEnabled.Colors;
            controlGradient.Positions = Settings.DefaultValue.Control.BoxEnabled.Positions;
            controlDisabledGradient.Colors = Settings.DefaultValue.Control.BoxDisabled.Colors;
            controlDisabledGradient.Positions = Settings.DefaultValue.Control.BoxDisabled.Positions;
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
                if (buttonHorizontal == Alignment.Horizontal.Right)
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

        /// <summary>Gets the item size.</summary>
        /// <param name="itemBounds">Item bounds.</param>
        /// <returns>Item size.</returns>
        private Size GetItemSize(Rectangle itemBounds)
        {
            return new Size(itemBounds.Width - itemBorder.Thickness, itemBounds.Height - itemBorder.Thickness);
        }

        #endregion
    }
}