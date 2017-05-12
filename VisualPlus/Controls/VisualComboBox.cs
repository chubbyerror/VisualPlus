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
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual ComboBox.</summary>
    [ToolboxBitmap(typeof(ComboBox))]
    [Designer(VSDesignerBinding.VisualComboBox)]
    public sealed class VisualComboBox : ComboBox
    {
        #region Variables

        private static BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private static ControlState controlState = ControlState.Normal;

        private Color[] backgroundColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Color buttonColor = Settings.DefaultValue.Style.DropDownButtonColor;

        private Color[] controlDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled,
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled)
            };

        private GraphicsPath controlGraphicsPath;
        private DropDownButtons dropDownButton = DropDownButtons.Arrow;
        private bool dropDownButtonsVisible = Settings.DefaultValue.TextVisible;
        private Point endPoint;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private float gradientAngle;
        private LinearGradientBrush gradientBrush;
        private float[] gradientPosition = { 0, 1 / 2f, 1 };

        private bool itemBorderVisible;
        private Color menuItemHover = Settings.DefaultValue.Style.ItemHover(0);
        private Color menuItemNormal = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color menuTextColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color separatorColor = Settings.DefaultValue.Style.LineColor;
        private Color separatorShadowColor = Settings.DefaultValue.Style.ShadowColor;
        private bool separatorVisible = Settings.DefaultValue.TextVisible;
        private int startIndex;
        private Point startPoint;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

        private Color waterMarkActiveColor = Color.Gray;
        private SolidBrush waterMarkBrush;
        private Color waterMarkColor = Color.LightGray;
        private Font waterMarkFont;
        private string waterMarkText = "Custom text...";
        private bool watermarkVisible;

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
            UpdateLocationPoints();
            BackColor = Color.Transparent;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);

            // Sets some default values to the watermark properties
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
                Invalidate();
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
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderHoverColor)]
        public Color BorderHoverColor
        {
            get
            {
                return borderHoverColor;
            }

            set
            {
                borderHoverColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderHoverVisible)]
        public bool BorderHoverVisible
        {
            get
            {
                return borderHoverVisible;
            }

            set
            {
                borderHoverVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    borderRounding = value;
                }

                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
                UpdateLocationPoints();
                Invalidate();
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

                Invalidate();
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
        public Color[] ControlDisabledColor
        {
            get
            {
                return controlDisabledColor;
            }

            set
            {
                controlDisabledColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
        public Color[] DisabledColor
        {
            get
            {
                return controlDisabledColor;
            }

            set
            {
                controlDisabledColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.DropDownButton)]
        public DropDownButtons DropDownButton
        {
            get
            {
                return dropDownButton;
            }

            set
            {
                dropDownButton = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool DropDownButtonVisible
        {
            get
            {
                return dropDownButtonsVisible;
            }

            set
            {
                dropDownButtonsVisible = value;
                Invalidate();
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
                Invalidate();
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
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderVisible)]
        public bool ItemBorderVisible
        {
            get
            {
                return itemBorderVisible;
            }

            set
            {
                itemBorderVisible = value;
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
            // Draws the menu items
            e.Graphics.FillRectangle(
                (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? new SolidBrush(menuItemHover)
                    : new SolidBrush(menuItemNormal),
                e.Bounds);

            // Setup item
            Size itemSize = new Size(e.Bounds.Width - borderThickness, e.Bounds.Height - borderThickness);
            Point itemPoint = new Point(e.Bounds.X, e.Bounds.Y);
            Rectangle itemBorderRectangle = new Rectangle(itemPoint, itemSize);
            GraphicsPath itemBorderPath = new GraphicsPath();
            itemBorderPath.AddRectangle(itemBorderRectangle);

            // Draw item border
            if (itemBorderVisible)
            {
                GDI.DrawBorder(e.Graphics, itemBorderPath, borderThickness, borderColor);
            }

            // Draw item string
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

            UpdateLocationPoints();

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            var controlCheckTemp = Enabled ? backgroundColor : controlDisabledColor;

            gradientBrush = GDI.CreateGradientBrush(controlCheckTemp, gradientPosition, gradientAngle, startPoint, endPoint);

            // Draw the background
            graphics.FillPath(gradientBrush, controlGraphicsPath);

            // Create border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            Rectangle buttonRectangle = new Rectangle(0, 0, 25, 25);
            buttonRectangle = buttonRectangle.AlignCenterY(ClientRectangle);
            buttonRectangle = buttonRectangle.AlignRight(ClientRectangle, 0);

            if (dropDownButtonsVisible)
            {
                Point buttonImagePoint;
                Size buttonImageSize;

                // Draw drop down button
                switch (dropDownButton)
                {
                    case DropDownButtons.Arrow:
                        {
                            buttonImageSize = new Size(25, 25);
                            buttonImagePoint = new Point(buttonRectangle.X, buttonRectangle.Y);

                            Arrow.DrawArrow(graphics, buttonImagePoint, buttonImageSize, buttonColor, 13);
                            break;
                        }

                    case DropDownButtons.Bars:
                        {
                            buttonImageSize = new Size(18, 10);
                            buttonImagePoint = new Point(buttonRectangle.X + 2, buttonRectangle.Y + buttonRectangle.Width / 2 - buttonImageSize.Height);
                            Bars.DrawBars(graphics, buttonImagePoint, buttonImageSize, buttonColor, 3, 5);
                            break;
                        }
                }
            }

            if (separatorVisible)
            {
                // Draw the separator
                graphics.DrawLine(new Pen(separatorColor), buttonRectangle.X - 2, 4, buttonRectangle.X - 2, Height - 5);
                graphics.DrawLine(new Pen(separatorShadowColor), buttonRectangle.X - 1, 4, buttonRectangle.X - 1, Height - 5);
            }

            // Draw string
            Rectangle textBoxRectangle = new Rectangle(3, 0, Width - 20, Height);

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle, stringFormat);

            // Draw the watermark
            if (watermarkVisible && Text.Length == 0)
            {
                graphics.DrawString(waterMarkText, WaterMarkFont, waterMarkBrush, textBoxRectangle, stringFormat);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            OnLostFocus(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private void UpdateLocationPoints()
        {
            startPoint = new Point(ClientRectangle.Width, 0);
            endPoint = new Point(ClientRectangle.Width, ClientRectangle.Height);

            // Update paths
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion
    }
}