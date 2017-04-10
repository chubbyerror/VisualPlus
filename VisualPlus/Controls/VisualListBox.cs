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

    /// <summary>The visual ListBox.</summary>
    [ToolboxBitmap(typeof(ListBox)), Designer(VSDesignerBinding.VisualListBox)]
    public sealed class VisualListBox : ListBox
    {
        #region  ${0} Variables

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderSize = Settings.DefaultValue.BorderSize;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color itemBackground = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color itemBackground2 = Settings.DefaultValue.Style.BorderColor(0);
        private Color itemSelected = Settings.DefaultValue.Style.BorderColor(1);
        private bool rotateItemColor = true;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;

        #endregion

        #region ${0} Properties

        public VisualListBox()
        {
            SetStyle(
                ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            UpdateStyles();

            DoubleBuffered = true;
            IntegralHeight = false;
            ItemHeight = 18;
            Font = new Font(Font.FontFamily, 10, FontStyle.Regular);
            ResizeRedraw = true;
            BorderStyle = BorderStyle.None;
            Size = new Size(250, 150);
            AutoSize = true;
            DrawMode = DrawMode.OwnerDrawVariable;
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

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
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

        [DefaultValue(Settings.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
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

        [DefaultValue(Settings.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
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

        [DefaultValue(Settings.DefaultValue.BorderSize), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderSize)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderSize = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible), Category(Localize.Category.Behavior),
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
        public Color ItemBackground
        {
            get
            {
                return itemBackground;
            }

            set
            {
                itemBackground = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color ItemBackground2
        {
            get
            {
                return itemBackground2;
            }

            set
            {
                itemBackground2 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color ItemSelected
        {
            get
            {
                return itemSelected;
            }

            set
            {
                itemSelected = value;
                Invalidate();
            }
        }

        [DefaultValue(true), Category(Localize.Category.Behavior)]
        public bool RotateItemColor
        {
            get
            {
                return rotateItemColor;
            }

            set
            {
                rotateItemColor = value;
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
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            BackColor = Parent.BackColor;

            UpdateLocationPoints();

            GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, borderSize, borderColor, borderHoverColor, borderHoverVisible);

            e.Graphics.SetClip(controlGraphicsPath);

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            if (e.Index > -1)
            {
                Color color;

                if (rotateItemColor)
                {
                    if (isSelected)
                    {
                        color = itemSelected;
                    }
                    else
                    {
                        if (e.Index % 2 == 0)
                        {
                            color = itemBackground;
                        }
                        else
                        {
                            color = itemBackground2;
                        }
                    }
                }
                else
                {
                    if (isSelected)
                    {
                        color = itemSelected;
                    }
                    else
                    {
                        color = itemBackground;
                    }
                }

                // Set control state color
                foreColor = Enabled ? foreColor : textDisabledColor;

                // Draw the background
                e.Graphics.FillRectangle(new SolidBrush(color), e.Bounds);

                StringFormat stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };

                // Draw the text
                e.Graphics.DrawString(Items[e.Index].
                    ToString(), e.Font, new SolidBrush(foreColor), e.Bounds, stringFormat);
            }
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
            Invalidate();
        }

        private void UpdateLocationPoints()
        {
            // Update paths
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion
    }
}