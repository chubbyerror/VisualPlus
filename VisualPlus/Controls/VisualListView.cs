namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Styles;
    using VisualPlus.Localization;

    /// <summary>The visual ListView.</summary>
    [ToolboxBitmap(typeof(ListView))]

    // [ToolboxBitmap(typeof(ListView)), Designer(VSDesignerBinding.VisualListView)]
    public class VisualListView : ListView
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LogFont
        {
            #region  ${0} Variables

            public byte lfCharSet = 0;
            public byte lfClipPrecision = 0;
            public int lfEscapement = 0;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = string.Empty;

            public int lfHeight = 0;
            public byte lfItalic = 0;
            public int lfOrientation = 0;
            public byte lfOutPrecision = 0;
            public byte lfPitchAndFamily = 0;
            public byte lfQuality = 0;
            public byte lfStrikeOut = 0;
            public byte lfUnderline = 0;
            public int lfWeight = 0;
            public int lfWidth = 0;

            #endregion
        }

        #region  ${0} Variables

        private const int itemPadding = 12;

        private static readonly IStyle Style = new Visual();
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color columnBorder = Style.BorderColor(0);
        private Color columnHeaderBackground = Style.BackgroundColor(3);

        private ControlState controlState = ControlState.Normal;
        private bool drawFocusRectangle;

        private bool drawStandardHeader;

        private Font headerFont = new Font("Helvetica", 10, FontStyle.Regular);

        private Color headerText = StylesManager.DefaultValue.TextColor;
        private Color itemBackground = Style.BackgroundColor(3);
        private Color itemHover = Style.ItemHover(0);
        private Color itemSelected = Style.BorderColor(1);

        #endregion

        #region ${0} Properties

        public VisualListView()
        {
            SetStyle(
                ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            View = View.Details;

            LabelEdit = false;

            AllowColumnReorder = false;

            CheckBoxes = false;

            FullRowSelect = true;

            GridLines = true;

            HeaderStyle = ColumnHeaderStyle.Nonclickable;

            OwnerDraw = true;
            ResizeRedraw = true;
            BorderStyle = BorderStyle.None;
            GridLines = true;
            Size = new Size(250, 150);

            AutoSize = true;
            AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // Fix for hovers, by default it doesn't redraw
            // TODO: should only redraw when the hovered line changed, this to reduce unnecessary redraws
            MouseLocation = new Point(-1, -1);

            controlState = ControlState.Normal;
            MouseEnter += delegate
                {
                    controlState = ControlState.Hover;
                };
            MouseLeave += delegate
                {
                    controlState = ControlState.Normal;
                    MouseLocation = new Point(-1, -1);
                    HoveredItem = null;
                    Invalidate();
                };

            MouseDown += delegate
                {
                    controlState = ControlState.Down;
                };
            MouseUp += delegate
                {
                    controlState = ControlState.Hover;
                };
            MouseMove += delegate(object sender, MouseEventArgs args)
                {
                    MouseLocation = args.Location;
                    ListViewItem currentHoveredItem = GetItemAt(MouseLocation.X, MouseLocation.Y);
                    if (HoveredItem != currentHoveredItem)
                    {
                        HoveredItem = currentHoveredItem;
                        Invalidate();
                    }
                };
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
        public Color ColumnBackground
        {
            get
            {
                return columnHeaderBackground;
            }

            set
            {
                columnHeaderBackground = value;
                Invalidate();
            }
        }

        [DefaultValue(false), Category(Localize.Category.Behavior)]
        public bool FocusVisible
        {
            get
            {
                return drawFocusRectangle;
            }

            set
            {
                drawFocusRectangle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description(Localize.Description.ComponentFont)]
        public Font HeaderFont
        {
            get
            {
                return headerFont;
            }

            set
            {
                headerFont = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color HeaderText
        {
            get
            {
                return headerText;
            }

            set
            {
                headerText = value;
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
        public Color ItemHover
        {
            get
            {
                return itemHover;
            }

            set
            {
                itemHover = value;
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

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [DefaultValue(false), Category(Localize.Category.Behavior)]
        public bool StandardHeader
        {
            get
            {
                return drawStandardHeader;
            }

            set
            {
                drawStandardHeader = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        private ListViewItem HoveredItem { get; set; }

        #endregion

        #region ${0} Events

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;

            Rectangle columnHeaderRectangle = new Rectangle(e.Bounds.X, e.Bounds.Y, Width, e.Bounds.Height);
            GraphicsPath columnHeaderPath = new GraphicsPath();
            columnHeaderPath.AddRectangle(columnHeaderRectangle);
            columnHeaderPath.CloseAllFigures();

            if (drawStandardHeader)
            {
                // Draw the standard header background.
                e.DrawBackground();
            }
            else
            {
                // Draw column header background
                e.Graphics.FillRectangle(new SolidBrush(columnHeaderBackground), columnHeaderRectangle);
            }

            if (borderVisible)
            {
                // Draw column header border
                GDI.DrawBorder(graphics, columnHeaderPath, borderSize, columnBorder);
            }

            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            // Draw the header text.
            using (headerFont)
            {
                e.Graphics.DrawString(e.Header.Text, headerFont, new SolidBrush(headerText),
                    new Rectangle(e.Bounds.X + itemPadding, e.Bounds.Y + itemPadding, e.Bounds.Width - itemPadding * 2,
                        e.Bounds.Height - itemPadding * 2), stringFormat);
            }

            graphics.Dispose();
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            // We draw the current line of items (= item with subitems) on a temp bitmap, then draw the bitmap at once. This is to reduce flickering.
            Bitmap bitmap = new Bitmap(e.Item.Bounds.Width, e.Item.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            // always draw default background
            graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));

            if (e.State.HasFlag(ListViewItemStates.Selected))
            {
                // selected background
                graphics.FillRectangle(new SolidBrush(itemSelected), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));
            }
            else if (e.Bounds.Contains(MouseLocation) && controlState == ControlState.Hover)
            {
                // hover background
                graphics.FillRectangle(new SolidBrush(itemHover), new Rectangle(new Point(e.Bounds.X, 0), e.Bounds.Size));
            }

            // Draw separator
            graphics.DrawLine(new Pen(Style.BorderColor(0)), e.Bounds.Left, 0, e.Bounds.Right, 0);

            foreach (ListViewItem.ListViewSubItem subItem in e.Item.SubItems)
            {
                // Draw text
                graphics.DrawString(subItem.Text, Font, new SolidBrush(Color.Black),
                    new Rectangle(subItem.Bounds.X + itemPadding, itemPadding, subItem.Bounds.Width - 2 * itemPadding,
                        subItem.Bounds.Height - 2 * itemPadding),
                    GetStringFormat());
            }

            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Selected item background
                e.Graphics.FillRectangle(new SolidBrush(itemSelected), e.Bounds);

                if (drawFocusRectangle)
                {
                    // Draws the focus rectangle on selection
                    e.DrawFocusRectangle();
                }
            }
            else
            {
                // Unselected item background
                e.Graphics.FillRectangle(new SolidBrush(itemBackground), e.Bounds);
            }

            // Draw the item text for views other than the Details view
            if (View != View.Details)
            {
                e.DrawText();
            }

            e.Graphics.DrawImage((Image)bitmap.Clone(), new Point(0, e.Item.Bounds.Location.Y));
            graphics.Dispose();
            bitmap.Dispose();
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.Left;

            using (StringFormat sf = new StringFormat())
            {
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        flags = TextFormatFlags.HorizontalCenter;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        flags = TextFormatFlags.Right;
                        break;
                }

                // Draw the text and background for a subitem with a 
                // negative value. 
                double subItemValue;
                if (e.ColumnIndex > 0 && double.TryParse(
                        e.SubItem.Text, NumberStyles.Currency,
                        NumberFormatInfo.CurrentInfo, out subItemValue) &&
                    subItemValue < 0)
                {
                    // Unless the item is selected, draw the standard 
                    // background to make it stand out from the gradient.
                    if ((e.ItemState & ListViewItemStates.Selected) == 0)
                    {
                        e.DrawBackground();
                    }

                    // Draw the subitem text in red to highlight it. 
                    e.Graphics.DrawString(e.SubItem.Text, Font, Brushes.Red, e.Bounds, sf);

                    return;
                }

                // Draw normal text for a subitem with a nonnegative 
                // or nonnumerical value.
                e.DrawText(flags);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            controlState = ControlState.Down;
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        private static StringFormat GetStringFormat()
        {
            return new StringFormat
                {
                    FormatFlags = StringFormatFlags.LineLimit,
                    Trimming = StringTrimming.EllipsisCharacter,
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
        }

        #endregion
    }
}