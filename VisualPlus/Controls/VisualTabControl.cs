namespace VisualPlus.Controls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Localization;

    /// <summary>The visual TabControl.</summary>
    // [ToolboxBitmap(typeof(TabControl)), Designer(StylesManager.BindedDesignerControls.VisualTab)]
    [ToolboxBitmap(typeof(TabControl))]
    public sealed class VisualTabControl : TabControl
    {
        #region  ${0} Variables

        private Color backgroundColor = StylesManager.DefaultValue.Style.BackgroundColor(0);

        private ControlState controlState = ControlState.Normal;
        private StringAlignment lineAlignment = StringAlignment.Near;

        private Point mouseLocation;
        private bool selectorVisible;
        private Color separator = StylesManager.DefaultValue.Style.TabSelected;
        private Color tabHover = StylesManager.DefaultValue.Style.TabHover;
        private Color tabMenu = StylesManager.DefaultValue.Style.TabMenu;
        private Color tabNormal = StylesManager.DefaultValue.Style.TabNormal;
        private Color tabSelected = StylesManager.DefaultValue.Style.TabSelected;
        private Color tabSelector = StylesManager.DefaultValue.Style.MainColor;

        private StringAlignment textAlignment = StringAlignment.Center;

        // private Color textDisabled = StylesManager.DefaultValue.Style.TextDisabled;
        private Color textNormal = StylesManager.DefaultValue.Style.TabTextNormal;

        private Color textSelected = StylesManager.DefaultValue.Style.TabTextSelected;

        #endregion

        #region ${0} Properties

        public VisualTabControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            ItemSize = new Size(40, 140);
            MinimumSize = new Size(144, 85);

            foreach (TabPage page in TabPages)
            {
                page.BackColor = backgroundColor;
            }
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
                foreach (TabPage page in TabPages)
                {
                    page.BackColor = backgroundColor;
                }

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

        [DefaultValue(true), Category(Localize.Category.Behavior)]
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

        [Category(Localize.Category.Appearance)]
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

        #region ${0} Events

        protected override void CreateHandle()
        {
            base.CreateHandle();

            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;
            Alignment = TabAlignment.Left;
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
            foreach (TabPage Tab in TabPages)
            {
                if (Tab.DisplayRectangle.Contains(mouseLocation))
                {
                    Invalidate();
                    break;
                }
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            mouseLocation = e.Location;
            foreach (TabPage Tab in TabPages)
            {
                if (Tab.DisplayRectangle.Contains(e.Location))
                {
                    Invalidate();
                    break;
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingMode = CompositingMode.SourceOver;

            // Draw tab selector background body
            graphics.FillRectangle(new SolidBrush(tabMenu), new Rectangle(0, 0, Width, Height));

            // ------------------------------- >
            for (var tabIndex = 0; tabIndex <= TabCount - 1; tabIndex++)
            {
                Rectangle tabRect = new Rectangle(
                    new Point(
                        GetTabRect(tabIndex).
                            Location.X,
                        GetTabRect(tabIndex).
                            Location.Y),
                    new Size(
                        GetTabRect(tabIndex).
                            Width,
                        GetTabRect(tabIndex).
                            Height));

                Rectangle tabHighlighter = new Rectangle(
                    new Point(
                        GetTabRect(tabIndex).
                            X,
                        GetTabRect(tabIndex).
                            Y),
                    new Size(
                        4,
                        tabRect.Height));

                Rectangle textRect = new Rectangle(tabRect.Left + 20, tabRect.Top + 12, tabRect.Width - 40, tabRect.Height);

                if (tabIndex == SelectedIndex)
                {
                    // Draw selected tab
                    graphics.FillRectangle(new SolidBrush(tabSelected), tabRect);

                    if (selectorVisible)
                    {
                        // Tab Selector
                        graphics.FillRectangle(new SolidBrush(tabSelector), tabHighlighter);
                    }

                    StringFormat stringFormat = new StringFormat
                        {
                            Alignment = textAlignment,
                            LineAlignment = lineAlignment
                        };

                    // Draw selected tab text
                    graphics.DrawString(
                        TabPages[tabIndex].
                            Text,
                        Font,
                        new SolidBrush(textSelected),
                        textRect,
                        stringFormat);

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabRect.X + 12, tabRect.Y + 11, ImageList.Images[tabIndex].
                                                                                                                 Size.Height,
                            ImageList.Images[tabIndex].
                                      Size.Width);
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
                    }

                    StringFormat stringFormat = new StringFormat
                        {
                            Alignment = textAlignment,
                            LineAlignment = lineAlignment
                        };

                    graphics.DrawString(
                        TabPages[tabIndex].
                            Text,
                        Font,
                        new SolidBrush(textNormal),
                        textRect,
                        stringFormat);

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabRect.X + 12, tabRect.Y + 11, ImageList.Images[tabIndex].
                                                                                                                 Size.Height,
                            ImageList.Images[tabIndex].
                                      Size.Width);
                    }
                }
            }

            // Draw divider that separates the panels.
            e.Graphics.DrawLine(new Pen(separator, 2), ItemSize.Height + 2, 0, ItemSize.Height + 2, Height);
        }

        private TabPage GetPageByPoint(TabControl tabControl, Point point)
        {
            for (var i = 0; i < tabControl.TabPages.Count; i++)
            {
                TabPage page = tabControl.TabPages[i];
                if (tabControl.GetTabRect(i).
                               Contains(point))
                {
                    return page;
                }
            }

            return null;
        }

        #endregion
    }
}