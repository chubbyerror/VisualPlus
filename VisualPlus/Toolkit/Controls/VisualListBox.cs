namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Managers;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ListBox))]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("Items")]
    [Description("The Visual ListBox")]
    [Designer(ControlManager.FilterProperties.VisualListBox)]
    public sealed class VisualListBox : ListBox
    {
        #region Variables

        private StyleManager _styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

        private Color foreColor;
        private Color itemBackground;
        private Color itemBackground2;
        private Color itemSelected;
        private bool rotateItemColor = true;
        private Color textDisabledColor;
        private TextRenderingHint textRendererHint;

        #endregion

        #region Constructors

        public VisualListBox()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

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
            ScrollAlwaysVisible = true;

            ConfigureStyleManager();
        }

        #endregion

        #region Properties

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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [DefaultValue(true)]
        [Category(Localize.PropertiesCategory.Behavior)]
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

        #endregion

        #region Events

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Invalidate();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

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

                Rectangle background = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

                // Draw the background
                e.Graphics.FillRectangle(new SolidBrush(color), background);

                StringFormat stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };

                // Draw the text
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(foreColor), e.Bounds, stringFormat);
            }
        }

        private void ConfigureStyleManager()
        {
            textRendererHint = Settings.DefaultValue.TextRenderingHint;
            Font = _styleManager.Font;
            foreColor = _styleManager.FontStyle.ForeColor;
            textDisabledColor = _styleManager.FontStyle.ForeColorDisabled;
            itemBackground = _styleManager.ControlStyle.Background(0);
            itemBackground2 = _styleManager.BorderStyle.Color;
            itemSelected = _styleManager.BorderStyle.HoverColor;
        }

        #endregion
    }
}