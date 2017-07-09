namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;
    using VisualPlus.Handlers;
    using VisualPlus.Structure;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ContextMenuStrip))]
    [DefaultEvent("Opening")]
    [DefaultProperty("Items")]
    [Description("The Visual Context Menu Strip")]
    public sealed class VisualContextMenuStrip : ContextMenuStrip
    {
        #region Variables

        private StyleManager _styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

        private ToolStripItemClickedEventArgs clickedEventArgs;

        #endregion

        #region Constructors

        public VisualContextMenuStrip()
        {
            Renderer = new VisualToolStripRender();
            ConfigureStyleManager();
        }

        public delegate void ClickedEventHandler(object sender);

        public event ClickedEventHandler Clicked;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ArrowColor
        {
            get
            {
                return arrowColor;
            }

            set
            {
                arrowColor = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ArrowDisabledColor
        {
            get
            {
                return arrowDisabledColor;
            }

            set
            {
                arrowDisabledColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
        public bool ArrowVisible
        {
            get
            {
                return arrowVisible;
            }

            set
            {
                arrowVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color Background
        {
            get
            {
                return background;
            }

            set
            {
                background = value;
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
        [Description(Localize.Description.Strings.Font)]
        public Font MenuFont
        {
            get
            {
                return contextMenuFont;
            }

            set
            {
                contextMenuFont = value;
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

        #endregion

        #region Events

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if ((e.ClickedItem != null) && !(e.ClickedItem is ToolStripSeparator))
            {
                if (ReferenceEquals(e, clickedEventArgs))
                {
                    OnItemClicked(e);
                }
                else
                {
                    clickedEventArgs = e;
                    if (Clicked != null)
                    {
                        Clicked(this);
                    }
                }
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = Cursors.Hand;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cursor = Cursors.Hand;
            Invalidate();
        }

        private static Color arrowColor;
        private static Color arrowDisabledColor;
        private static bool arrowVisible = Settings.DefaultValue.TextVisible;

        private static Color background;
        private static Border border;
        private static Font contextMenuFont;
        private static Color foreColor;
        private static Color textDisabledColor;

        private void ConfigureStyleManager()
        {
            border = new Border
                {
                    HoverVisible = false,
                    Type = ShapeType.Rectangle
                };

            Font = _styleManager.Font;
            foreColor = _styleManager.FontStyle.ForeColor;
            textDisabledColor = _styleManager.FontStyle.ForeColorDisabled;

            BackColor = background;
            arrowColor = _styleManager.ControlStyle.FlatButtonEnabled;
            arrowDisabledColor = _styleManager.ControlStyle.FlatButtonDisabled;
            contextMenuFont = Font;

            background = _styleManager.ControlStyle.Background(0);
        }

        #endregion

        #region Methods

        public sealed class VisualToolStripMenuItem : ToolStripMenuItem
        {
            #region Constructors

            public VisualToolStripMenuItem()
            {
                AutoSize = false;
                Size = new Size(160, 30);
                Font = contextMenuFont;
            }

            #endregion

            #region Events

            protected override ToolStripDropDown CreateDefaultDropDown()
            {
                if (DesignMode)
                {
                    return base.CreateDefaultDropDown();
                }

                VisualContextMenuStrip defaultDropDown = new VisualContextMenuStrip();
                defaultDropDown.Items.AddRange(base.CreateDefaultDropDown().Items);
                return defaultDropDown;
            }

            #endregion
        }

        public sealed class VisualToolStripRender : ToolStripProfessionalRenderer
        {
            #region Events

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                if (arrowVisible)
                {
                    int arrowX = e.Item.ContentRectangle.X + e.Item.ContentRectangle.Width;
                    int arrowY = (e.ArrowRectangle.Y + e.ArrowRectangle.Height) / 2;

                    Point[] arrowPoints =
                        {
                            new Point(arrowX - 5, arrowY - 5),
                            new Point(arrowX, arrowY),
                            new Point(arrowX - 5, arrowY + 5)
                        };

                    // Set control state color
                    foreColor = e.Item.Enabled ? foreColor : textDisabledColor;
                    Color controlCheckTemp = e.Item.Enabled ? arrowColor : arrowDisabledColor;

                    // Draw the arrowButton
                    e.Graphics.FillPolygon(new SolidBrush(controlCheckTemp), arrowPoints);
                }
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                // Allow to add images to ToolStrips
                // MyBase.OnRenderImageMargin(e) 
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Rectangle textRect = new Rectangle(25, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - (24 + 16), e.Item.ContentRectangle.Height - 4);

                // Set control state color
                foreColor = e.Item.Enabled ? foreColor : textDisabledColor;

                StringFormat stringFormat = new StringFormat
                    {
                        // Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                e.Graphics.DrawString(e.Text, contextMenuFont, new SolidBrush(foreColor), textRect, stringFormat);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.Clear(background);
                Rectangle menuItemRectangle = new Rectangle(0, e.Item.ContentRectangle.Y - 2, e.Item.ContentRectangle.Width + 4, e.Item.ContentRectangle.Height + 3);
                e.Graphics.FillRectangle(e.Item.Selected && e.Item.Enabled ? new SolidBrush(Color.FromArgb(130, background)) : new SolidBrush(background), menuItemRectangle);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawLine(new Pen(Color.FromArgb(200, border.Color), border.Thickness), new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2), new Point(e.Item.Bounds.Right - 5, e.Item.Bounds.Height / 2));
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                base.OnRenderToolStripBackground(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.Clear(background);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (border.Visible)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.High;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle borderRectangle = new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - border.Thickness, e.AffectedBounds.Height - border.Thickness);
                    GraphicsPath borderPath = new GraphicsPath();
                    borderPath.AddRectangle(borderRectangle);
                    borderPath.CloseAllFigures();

                    e.Graphics.SetClip(borderPath);
                    e.Graphics.DrawPath(new Pen(border.Color), borderPath);
                    e.Graphics.ResetClip();
                }
            }

            #endregion
        }

        #endregion
    }
}