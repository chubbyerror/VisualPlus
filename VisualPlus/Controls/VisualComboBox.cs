namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    public enum DropDownButtons
    {
        /// <summary>Use arrow button.</summary>
        Arrow,

        /// <summary>Use bars button.</summary>
        Bars
    }

    /// <summary>The visual ComboBox.</summary>
    [ToolboxBitmap(typeof(ComboBox)), Designer(VSDesignerBinding.VisualComboBox)]
    public class VisualComboBox : ComboBox
    {
        #region  ${0} Variables

        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private static ControlState controlState = ControlState.Normal;
        private Color backgroundColor = StylesManager.DefaultValue.Style.BackgroundColor(0);
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color buttonColor = StylesManager.DefaultValue.Style.DropDownButtonColor;
        private Color controlDisabledColor = StylesManager.DefaultValue.Style.ControlDisabled;
        private GraphicsPath controlGraphicsPath;
        private DropDownButtons dropDownButton = DropDownButtons.Arrow;
        private bool dropDownButtonsVisible = StylesManager.DefaultValue.TextVisible;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color menuItemHover = StylesManager.DefaultValue.Style.ItemHover(0);
        private Color menuItemNormal = StylesManager.DefaultValue.Style.BackgroundColor(0);
        private Color menuTextColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color separatorColor = StylesManager.DefaultValue.Style.LineColor;
        private Color separatorShadowColor = StylesManager.DefaultValue.Style.ShadowColor;
        private bool separatorVisible = StylesManager.DefaultValue.TextVisible;
        private int startIndex;
        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;

        #endregion

        #region ${0} Properties

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
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderHoverColor)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumRounding, StylesManager.MaximumRounding))
                {
                    borderRounding = value;
                }

                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
         Description(Localize.Description.ComponentShape)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderSize), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderSize)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumBorderSize, StylesManager.MaximumBorderSize))
                {
                    borderSize = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderVisible)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
        public Color ControlDisabledColor
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.DropDownButton)]
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

        [DefaultValue(StylesManager.DefaultValue.TextVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.ComponentVisible)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [DefaultValue(StylesManager.DefaultValue.TextVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.ComponentVisible)]
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

        [Category(Localize.Category.Behavior), Description(Localize.Description.StartIndex)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        #endregion

        #region ${0} Events

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Draws the menu items
            e.Graphics.FillRectangle(
                (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? new SolidBrush(menuItemHover)
                    : new SolidBrush(menuItemNormal),
                e.Bounds);

            if (e.Index != -1)
            {
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(menuTextColor), e.Bounds);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            SuspendLayout();
            Update();
            ResumeLayout();
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
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            UpdateLocationPoints();

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlCheckTemp = Enabled ? backgroundColor : controlDisabledColor;

            // Draw the combobox background
            graphics.FillPath(new SolidBrush(controlCheckTemp), controlGraphicsPath);

            // Setup combobox border
            if (borderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderColor);
                }
            }

            Rectangle buttonRectangle = new Rectangle(0, 0, 25, 25);
            buttonRectangle = buttonRectangle.AlignCenterY(ClientRectangle);
            buttonRectangle = buttonRectangle.AlignRight(ClientRectangle, 0);

            if (dropDownButtonsVisible)
            {
                // Draw drop down button
                switch (dropDownButton)
                {
                    case DropDownButtons.Arrow:
                        {
                            graphics.DrawString(
                                "6",
                                new Font("Marlett", 13, FontStyle.Regular),
                                new SolidBrush(buttonColor),
                                buttonRectangle,
                                new StringFormat
                                    {
                                        LineAlignment = StringAlignment.Center,
                                        Alignment = StringAlignment.Far
                                    });
                            break;
                        }

                    case DropDownButtons.Bars:
                        {
                            var spacing = 5;

                            graphics.DrawLine(
                                new Pen(buttonColor, 2),
                                new Point(spacing + buttonRectangle.X, Height / 2 - 4),
                                new Point(buttonRectangle.X + buttonRectangle.Width - spacing - 2, Height / 2 - 4));

                            graphics.DrawLine(
                                new Pen(buttonColor, 2),
                                new Point(spacing + buttonRectangle.X, Height / 2 + 0),
                                new Point(buttonRectangle.X + buttonRectangle.Width - spacing - 2, Height / 2 + 0));

                            graphics.DrawLine(
                                new Pen(buttonColor, 2),
                                new Point(spacing + buttonRectangle.X, Height / 2 + 4),
                                new Point(buttonRectangle.X + buttonRectangle.Width - spacing - 2, Height / 2 + 4));
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
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private void UpdateLocationPoints()
        {
            // Update paths
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion
    }
}