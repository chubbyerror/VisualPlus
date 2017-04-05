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
    public class VisualTabControl : TabControl
    {
        #region  ${0} Variables

        private ControlState controlState = ControlState.Normal;

        private Point mouseLocation;
        private bool selectorVisible = true;
        private Color separator = StylesManager.DefaultValue.Style.TabSelected;
        private Color tabHover = StylesManager.DefaultValue.Style.TabHover;
        private Color tabMenu = StylesManager.DefaultValue.Style.TabMenu;
        private Color tabNormal = StylesManager.DefaultValue.Style.TabNormal;
        private Color tabSelected = StylesManager.DefaultValue.Style.TabSelected;
        private Color tabSelector = StylesManager.DefaultValue.Style.MainColor;

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

            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(40, 140);

            // DrawMode = TabDrawMode.OwnerDrawFixed;
            foreach (TabPage page in TabPages)
            {
                page.BackColor = Background;
            }
        }

        [Category(Localize.Category.Appearance)]
        public Color Background { get; set; } = StylesManager.DefaultValue.Style.BackgroundColor(2);

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
                        BackColor = Background;
                    }
                }
            }
            finally
            {
                e.Control.BackColor = Background;
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

            graphics.Clear(Background);
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.CompositingMode = CompositingMode.SourceOver;

            // Draw tab selector background body
            graphics.FillRectangle(new SolidBrush(tabMenu), new Rectangle(-5, 0, ItemSize.Height + 4, Height));

            // Draw vertical line at the end of the tab selector rectangle
            graphics.DrawLine(new Pen(separator), ItemSize.Height - 1, 0, ItemSize.Height - 1, Height);

            // ------------------------------- >
            for (var tabIndex = 0; tabIndex <= TabCount - 1; tabIndex++)
            {
                Rectangle tabRect = new Rectangle(
                    new Point(
                        GetTabRect(tabIndex).
                            Location.X - 2,
                        GetTabRect(tabIndex).
                            Location.Y - 2),
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
                            Location.Y + 5),
                    new Size(
                        4,
                        GetTabRect(tabIndex).
                            Height / 2));

                Rectangle textRect = new Rectangle(tabRect.Left + 40, tabRect.Top + 12, tabRect.Width - 40, tabRect.Height);

                if (tabIndex == SelectedIndex)
                {
                    // Draw selected tab
                    graphics.FillRectangle(new SolidBrush(tabSelected), tabRect);

                    if (selectorVisible)
                    {
                        // Tab Selector
                        graphics.FillRectangle(new SolidBrush(tabSelector), tabHighlighter);
                    }

                    // Draw tab text
                    graphics.DrawString(
                        TabPages[tabIndex].Text,
                        Font,
                        new SolidBrush(textSelected),
                        textRect,
                        new StringFormat { Alignment = StringAlignment.Near });

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabRect.X + 12, tabRect.Y + 11, ImageList.Images[tabIndex].Size.Height,
                            ImageList.Images[tabIndex].Size.Width);
                    }
                }
                else
                {
                    // Draw other TabPages
                    if (controlState == ControlState.Hover && GetTabRect(tabIndex).
                            Contains(mouseLocation))
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

                    // Unselected TabPages
                    // graphics.FillRectangle(new SolidBrush(tabNormal), tabRect);
                    graphics.DrawString(
                        TabPages[tabIndex].Text,
                        Font,
                        new SolidBrush(textNormal),
                        textRect,
                        new StringFormat { Alignment = StringAlignment.Near });

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabRect.X + 12, tabRect.Y + 11, ImageList.Images[tabIndex].Size.Height,
                            ImageList.Images[tabIndex].Size.Width);
                    }
                }
            }

            // Draw divider that separates the panels.
            // e.Graphics.DrawLine(new Pen(separator, 2), ItemSize.Height + 2, 0, ItemSize.Height + 2, Height);
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