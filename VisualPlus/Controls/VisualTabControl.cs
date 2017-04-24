namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Linq;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual TabControl.</summary>
    // [Designer(StylesManager.BindedDesignerControls.VisualTab)]
    [ToolboxBitmap(typeof(TabControl))]
    public sealed class VisualTabControl : TabControl
    {
        #region Variables

        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(3);

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;

        private int borderThickness = Settings.DefaultValue.BorderSize;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private ControlState controlState = ControlState.Normal;
        private StringAlignment lineAlignment = StringAlignment.Near;
        private Point mouseLocation;
        private bool selectorVisible;
        private Color separator = Settings.DefaultValue.Style.TabSelected;

        private int separatorSpacing = 2;
        private float separatorThickness = 2F;

        private bool separatorVisible;
        private Color tabHover = Settings.DefaultValue.Style.TabHover;
        private Color tabMenu = Settings.DefaultValue.Style.TabMenu;
        private Color tabNormal = Settings.DefaultValue.Style.TabNormal;
        private Color tabSelected = Settings.DefaultValue.Style.TabSelected;
        private Color tabSelector = Settings.DefaultValue.Style.StyleColor;

        private StringAlignment textAlignment = StringAlignment.Center;

        // private Color textDisabled = StylesManager.DefaultValue.Style.TextDisabled;
        private Color textNormal = Settings.DefaultValue.Style.TabTextNormal;

        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color textSelected = Settings.DefaultValue.Style.TabTextSelected;

        #endregion

        #region Constructors

        public VisualTabControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            Size = new Size(320, 160);
            ItemSize = new Size(100, 30);
            MinimumSize = new Size(144, 85);
            LineAlignment = StringAlignment.Center;

            foreach (TabPage page in TabPages)
            {
                page.BackColor = backgroundColor;
            }
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public float SeparatorThickness
        {
            get
            {
                return separatorThickness;
            }

            set
            {
                separatorThickness = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
                foreach (TabPage page in TabPages)
                {
                    page.BackColor = backgroundColor;
                }

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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int SeparatorSpacing
        {
            get
            {
                return separatorSpacing;
            }

            set
            {
                separatorSpacing = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderSize)]
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
        public StringAlignment LineAlignment
        {
            get
            {
                return lineAlignment;
            }

            set
            {
                lineAlignment = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Behavior)]
        public bool SelectorVisible
        {
            get
            {
                return selectorVisible;
            }

            set
            {
                selectorVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color Separator
        {
            get
            {
                return separator;
            }

            set
            {
                separator = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Behavior)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TabHover
        {
            get
            {
                return tabHover;
            }

            set
            {
                tabHover = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TabMenu
        {
            get
            {
                return tabMenu;
            }

            set
            {
                tabMenu = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TabNormal
        {
            get
            {
                return tabNormal;
            }

            set
            {
                tabNormal = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TabSelected
        {
            get
            {
                return tabSelected;
            }

            set
            {
                tabSelected = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TabSelector
        {
            get
            {
                return tabSelector;
            }

            set
            {
                tabSelector = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
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
        public Color TextNormal
        {
            get
            {
                return textNormal;
            }

            set
            {
                textNormal = value;
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
        [Description(Localize.Description.ComponentColor)]
        public Color TextSelected
        {
            get
            {
                return textSelected;
            }

            set
            {
                textSelected = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void CreateHandle()
        {
            base.CreateHandle();

            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (!(e.Control is TabPage))
            {
                return;
            }

            try
            {
                IEnumerator enumerator = Controls.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    using (new TabPage())
                    {
                        BackColor = backgroundColor;
                    }
                }
            }
            finally
            {
                e.Control.BackColor = backgroundColor;
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
            controlState = ControlState.Normal;
            if (TabPages.Cast<TabPage>().Any(Tab => Tab.DisplayRectangle.Contains(mouseLocation)))
            {
                Invalidate();
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            mouseLocation = e.Location;
            if (TabPages.Cast<TabPage>().Any(Tab => Tab.DisplayRectangle.Contains(e.Location)))
            {
                Invalidate();
            }

            Invalidate();
            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;
            graphics.CompositingMode = CompositingMode.SourceOver;

            // Draw tab selector background body
            graphics.FillRectangle(new SolidBrush(tabMenu), new Rectangle(0, 0, Width, Height));

            // ------------------------------- >
            for (var tabIndex = 0; tabIndex <= TabCount - 1; tabIndex++)
            {
                Rectangle tabRect;
                Rectangle textRect;

                if (Alignment == TabAlignment.Top && Alignment == TabAlignment.Bottom)
                {
                    // Top - Bottom
                    tabRect = new Rectangle(
                        new Point(
                            GetTabRect(tabIndex).Location.X,
                            GetTabRect(tabIndex).Location.Y),
                        new Size(
                            GetTabRect(tabIndex).Width,
                            GetTabRect(tabIndex).Height));

                    textRect = new Rectangle(tabRect.Left, tabRect.Top, tabRect.Width, tabRect.Height);
                }
                else
                {
                    // Left - Right
                    tabRect = new Rectangle(
                        new Point(
                            GetTabRect(tabIndex).Location.X,
                            GetTabRect(tabIndex).Location.Y),
                        new Size(
                            GetTabRect(tabIndex).Width,
                            GetTabRect(tabIndex).Height));

                    textRect = new Rectangle(tabRect.Left, tabRect.Top, tabRect.Width, tabRect.Height);
                }

                Rectangle tabHighlighter = new Rectangle(
                    new Point(
                        GetTabRect(tabIndex).X,
                        GetTabRect(tabIndex).Y),
                    new Size(
                        4,
                        tabRect.Height));

                if (tabIndex == SelectedIndex)
                {
                    // Draw selected tab
                    graphics.FillRectangle(new SolidBrush(tabSelected), tabRect);

                    if (selectorVisible)
                    {
                        // Tab Selector
                        graphics.FillRectangle(new SolidBrush(tabSelector), tabHighlighter);
                    }

                    // Draw border
                    if (borderVisible)
                    {
                        GraphicsPath borderPath = new GraphicsPath();
                        borderPath.AddRectangle(tabRect);
                        GDI.DrawBorderType(graphics, controlState, borderPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
                    }

                    StringFormat stringFormat = new StringFormat
                        {
                            Alignment = textAlignment,
                            LineAlignment = lineAlignment
                        };

                    // Draw selected tab text
                    graphics.DrawString(
                        TabPages[tabIndex].Text,
                        Font,
                        new SolidBrush(textSelected),
                        textRect,
                        stringFormat);

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabRect.X + 12, tabRect.Y + 11, ImageList.Images[tabIndex].Size.Height, ImageList.Images[tabIndex].Size.Width);
                    }
                }
                else
                {
                    // Draw other TabPages
                    graphics.FillRectangle(new SolidBrush(tabNormal), tabRect);

                    if (controlState == ControlState.Hover && tabRect.Contains(mouseLocation))
                    {
                        Cursor = Cursors.Hand;

                        // Draw hover background
                        graphics.FillRectangle(new SolidBrush(tabHover), tabRect);

                        if (selectorVisible)
                        {
                            // Draw hover separator
                            graphics.FillRectangle(new SolidBrush(tabSelector), tabHighlighter);
                        }

                        if (borderVisible)
                        {
                            GraphicsPath borderPath = new GraphicsPath();
                            borderPath.AddRectangle(tabRect);
                            GDI.DrawBorderType(graphics, controlState, borderPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
                        }
                    }

                    StringFormat stringFormat = new StringFormat
                        {
                            Alignment = textAlignment,
                            LineAlignment = lineAlignment
                        };

                    graphics.DrawString(
                        TabPages[tabIndex].Text,
                        Font,
                        new SolidBrush(textNormal),
                        textRect,
                        stringFormat);

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabRect.X + 12, tabRect.Y + 11, ImageList.Images[tabIndex].Size.Height, ImageList.Images[tabIndex].Size.Width);
                    }
                }
            }

            DrawSeparator(e);
        }

        private void DrawSeparator(PaintEventArgs e)
        {
            if (!separatorVisible)
            {
                return;
            }

            // Draw divider that separates the panels.
            switch (Alignment)
            {
                case TabAlignment.Top:
                    {
                        e.Graphics.DrawLine(new Pen(separator, separatorThickness), 0, ItemSize.Height + separatorSpacing, Width, ItemSize.Height + separatorSpacing);
                        break;
                    }

                case TabAlignment.Bottom:
                    {
                        e.Graphics.DrawLine(new Pen(separator, separatorThickness), 0, Height - ItemSize.Height - separatorSpacing, Width, Height - ItemSize.Height - separatorSpacing);
                        break;
                    }

                case TabAlignment.Left:
                    {
                        e.Graphics.DrawLine(new Pen(separator, separatorThickness), ItemSize.Height + separatorSpacing, 0, ItemSize.Height + separatorSpacing, Height);
                        break;
                    }

                case TabAlignment.Right:
                    {
                        e.Graphics.DrawLine(new Pen(separator, separatorThickness), Width - ItemSize.Height - separatorSpacing, 0, Width - ItemSize.Height - separatorSpacing, Height);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}