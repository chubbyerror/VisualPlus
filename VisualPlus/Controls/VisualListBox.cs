namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

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

        private Color foreColor = Settings.DefaultValue.Font.ForeColor;
        private Color itemBackground = Settings.DefaultValue.Control.Background(0);
        private Color itemBackground2 = Settings.DefaultValue.Border.Color;
        private Color itemSelected = Settings.DefaultValue.Border.HoverColor;
        private bool rotateItemColor = true;
        private StyleManager styleManager = new StyleManager();
        private Color textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

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

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

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
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IFont fontStyle = styleManager.VisualStylesManager.VisualStylesInterface.FontStyle;

                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;
            }
            else
            {
                // Load default settings
                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        #endregion
    }
}