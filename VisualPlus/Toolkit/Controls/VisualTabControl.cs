namespace VisualPlus.Toolkit.Controls
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
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TabControl))]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("TabPages")]
    [Description("The Visual TabControl")]
    public sealed class VisualTabControl : TabControl
    {
        #region Variables

        private TabAlignment alignment = TabAlignment.Top;
        private bool arrowSelectorVisible = true;
        private int arrowSpacing = 10;
        private int arrowThickness = 5;
        private Color backgroundColor = Settings.DefaultValue.Control.Background(3);
        private Gradient hover = new Gradient();
        private Size itemSize = new Size(100, 25);
        private StringAlignment lineAlignment = StringAlignment.Near;
        private Point mouseLocation;

        private MouseStates mouseState;
        private Gradient normal = new Gradient();
        private Gradient selected = new Gradient();
        private TabAlignment selectorAlignment = TabAlignment.Top;
        private TabAlignment selectorAlignment2 = TabAlignment.Bottom;
        private int selectorThickness = 4;
        private bool selectorVisible;
        private bool selectorVisible2;
        private Color separator = Settings.DefaultValue.Control.Line;
        private int separatorSpacing = 2;
        private float separatorThickness = 2F;
        private bool separatorVisible;
        private Color tabMenu = Color.FromArgb(55, 61, 73);
        private Border tabPageBorder;
        private Rectangle tabPageRectangle;
        private Color tabSelector = Color.Green;
        private StringAlignment textAlignment = StringAlignment.Center;
        private Color textNormal = Color.FromArgb(174, 181, 187);
        private Rectangle textRectangle;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color textSelected = Color.FromArgb(217, 220, 227);

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
            MinimumSize = new Size(144, 85);
            LineAlignment = StringAlignment.Center;
            Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
            ItemSize = itemSize;

            tabPageBorder = new Border();

            float[] gradientPosition = { 0, 1 };

            Color[] tabHover =
                {
                    ControlPaint.Light(Settings.DefaultValue.Tab.TabSelected),
                    Settings.DefaultValue.Tab.TabSelected
                };

            Color[] tabNormal =
                {
                    ControlPaint.Light(Settings.DefaultValue.Tab.TabEnabled),
                    Settings.DefaultValue.Tab.TabEnabled
                };

            Color[] tabSelected =
                {
                    ControlPaint.Light(Settings.DefaultValue.Tab.TabSelected),
                    Settings.DefaultValue.Tab.TabSelected
                };

            normal.Colors = tabNormal;
            normal.Positions = gradientPosition;

            hover.Colors = tabHover;
            hover.Positions = gradientPosition;

            selected.Colors = tabSelected;
            selected.Positions = gradientPosition;

            foreach (TabPage page in TabPages)
            {
                page.BackColor = backgroundColor;
                page.Font = Font;
            }
        }

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Alignment)]
        public new TabAlignment Alignment
        {
            get
            {
                return alignment;
            }

            set
            {
                alignment = value;
                base.Alignment = alignment;

                // Resize tabs
                switch (alignment)
                {
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                        {
                            if (itemSize.Width < itemSize.Height)
                            {
                                ItemSize = new Size(itemSize.Height, itemSize.Width);
                            }

                            break;
                        }

                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        {
                            if (itemSize.Width > itemSize.Height)
                            {
                                ItemSize = new Size(itemSize.Height, itemSize.Width);
                            }

                            break;
                        }
                }

                UpdateArrowLocation();
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
        public bool ArrowSelectorVisible
        {
            get
            {
                return arrowSelectorVisible;
            }

            set
            {
                arrowSelectorVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Spacing)]
        public int ArrowSpacing
        {
            get
            {
                return arrowSpacing;
            }

            set
            {
                arrowSpacing = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
        public int ArrowThickness
        {
            get
            {
                return arrowThickness;
            }

            set
            {
                arrowThickness = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient HoverGradient
        {
            get
            {
                return hover;
            }

            set
            {
                hover = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Size)]
        public new Size ItemSize
        {
            get
            {
                return itemSize;
            }

            set
            {
                itemSize = value;
                base.ItemSize = itemSize;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient NormalGradient
        {
            get
            {
                return normal;
            }

            set
            {
                normal = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient SelectedGradient
        {
            get
            {
                return selected;
            }

            set
            {
                selected = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Alignment)]
        public TabAlignment SelectorAlignment
        {
            get
            {
                return selectorAlignment;
            }

            set
            {
                selectorAlignment = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Alignment)]
        public TabAlignment SelectorAlignment2
        {
            get
            {
                return selectorAlignment2;
            }

            set
            {
                selectorAlignment2 = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Size)]
        public int SelectorThickness
        {
            get
            {
                return selectorThickness;
            }

            set
            {
                selectorThickness = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
        public bool SelectorVisible2
        {
            get
            {
                return selectorVisible2;
            }

            set
            {
                selectorVisible2 = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Spacing)]
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

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
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

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Behavior)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.MouseState)]
        public MouseStates State
        {
            get
            {
                return mouseState;
            }

            set
            {
                mouseState = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border TabPageBorder
        {
            get
            {
                return tabPageBorder;
            }

            set
            {
                tabPageBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
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
            State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            State = MouseStates.Normal;
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
            Graphics graphics = GDI.Initialize(e, CompositingMode.SourceOver, CompositingQuality.Default, InterpolationMode.Default, PixelOffsetMode.Default, SmoothingMode.HighQuality, textRendererHint);
            graphics.Clear(Parent.BackColor);

            // Draw tab selector background body
            graphics.FillRectangle(new SolidBrush(tabMenu), new Rectangle(0, 0, Width, Height));

            for (var tabIndex = 0; tabIndex <= TabCount - 1; tabIndex++)
            {
                ConfigureAlignmentStyle(tabIndex);

                var gradientPoints = new[] { new Point { X = GetTabRect(tabIndex).Width, Y = 0 }, new Point { X = GetTabRect(tabIndex).Width, Y = GetTabRect(tabIndex).Height } };

                LinearGradientBrush normalBrush = Gradient.CreateGradientBrush(normal.Colors, gradientPoints, normal.Angle, normal.Positions);
                LinearGradientBrush hoverBrush = Gradient.CreateGradientBrush(hover.Colors, gradientPoints, hover.Angle, hover.Positions);
                LinearGradientBrush selectedBrush = Gradient.CreateGradientBrush(selected.Colors, gradientPoints, selected.Angle, selected.Positions);

                // Draws the TabSelector
                Rectangle selectorRectangle = GDI.ApplyAnchor(selectorAlignment, GetTabRect(tabIndex), selectorThickness);
                Rectangle selectorRectangle2 = GDI.ApplyAnchor(SelectorAlignment2, GetTabRect(tabIndex), selectorThickness);

                StringFormat stringFormat = new StringFormat
                    {
                        Alignment = textAlignment,
                        LineAlignment = lineAlignment
                    };

                if (tabIndex == SelectedIndex)
                {
                    // Draw selected tab
                    graphics.FillRectangle(selectedBrush, tabPageRectangle);

                    // Draw tab selector
                    if (selectorVisible)
                    {
                        graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle);
                    }

                    if (selectorVisible2)
                    {
                        graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle2);
                    }

                    GraphicsPath borderPath = new GraphicsPath();
                    borderPath.AddRectangle(tabPageRectangle);
                    Border.DrawBorderStyle(graphics, tabPageBorder, State, borderPath);

                    if (arrowSelectorVisible)
                    {
                        DrawSelectionArrow(e, tabPageRectangle);
                    }

                    // Draw selected tab text
                    graphics.DrawString(
                        TabPages[tabIndex].Text,
                        Font,
                        new SolidBrush(textSelected),
                        textRectangle,
                        stringFormat);

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabPageRectangle.X + 12, tabPageRectangle.Y + 11, ImageList.Images[tabIndex].Size.Height, ImageList.Images[tabIndex].Size.Width);
                    }
                }
                else
                {
                    // Draw other TabPages
                    graphics.FillRectangle(normalBrush, tabPageRectangle);

                    if ((State == MouseStates.Hover) && tabPageRectangle.Contains(mouseLocation))
                    {
                        Cursor = Cursors.Hand;

                        // Draw hover background
                        graphics.FillRectangle(hoverBrush, tabPageRectangle);

                        // Draw tab selector
                        if (selectorVisible)
                        {
                            graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle);
                        }

                        if (selectorVisible2)
                        {
                            graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle2);
                        }

                        GraphicsPath borderPath = new GraphicsPath();
                        borderPath.AddRectangle(tabPageRectangle);
                        Border.DrawBorderStyle(graphics, tabPageBorder, State, borderPath);
                    }

                    graphics.DrawString(
                        TabPages[tabIndex].Text,
                        Font,
                        new SolidBrush(textNormal),
                        textRectangle,
                        stringFormat);

                    // Draw image list
                    if (ImageList != null)
                    {
                        graphics.DrawImage(ImageList.Images[tabIndex], tabPageRectangle.X + 12, tabPageRectangle.Y + 11, ImageList.Images[tabIndex].Size.Height, ImageList.Images[tabIndex].Size.Width);
                    }
                }
            }

            DrawSeparator(e);
        }

        private void ConfigureAlignmentStyle(int tabIndex)
        {
            if ((Alignment == TabAlignment.Top) && (Alignment == TabAlignment.Bottom))
            {
                // Top - Bottom
                tabPageRectangle = new Rectangle(
                    new Point(
                        GetTabRect(tabIndex).Location.X,
                        GetTabRect(tabIndex).Location.Y),
                    new Size(
                        GetTabRect(tabIndex).Width,
                        GetTabRect(tabIndex).Height));

                textRectangle = new Rectangle(tabPageRectangle.Left, tabPageRectangle.Top, tabPageRectangle.Width, tabPageRectangle.Height);
            }
            else
            {
                // Left - Right
                tabPageRectangle = new Rectangle(
                    new Point(
                        GetTabRect(tabIndex).Location.X,
                        GetTabRect(tabIndex).Location.Y),
                    new Size(
                        GetTabRect(tabIndex).Width,
                        GetTabRect(tabIndex).Height));

                textRectangle = new Rectangle(tabPageRectangle.Left, tabPageRectangle.Top, tabPageRectangle.Width, tabPageRectangle.Height);
            }
        }

        private void DrawSelectionArrow(PaintEventArgs e, Rectangle selectedRectangle)
        {
            var points = new Point[3];

            switch (Alignment)
            {
                case TabAlignment.Left:
                    {
                        points[0].X = selectedRectangle.Right - ArrowThickness;
                        points[0].Y = selectedRectangle.Y + (selectedRectangle.Height / 2);

                        points[1].X = selectedRectangle.Right + ArrowSpacing;
                        points[1].Y = selectedRectangle.Top + ArrowSpacing;

                        points[2].X = selectedRectangle.Right + ArrowSpacing;
                        points[2].Y = selectedRectangle.Bottom - ArrowSpacing;
                        break;
                    }

                case TabAlignment.Top:
                    {
                        points[0].X = selectedRectangle.X + (selectedRectangle.Width / 2);
                        points[0].Y = selectedRectangle.Bottom - ArrowThickness;

                        points[1].X = selectedRectangle.Left + ArrowSpacing;
                        points[1].Y = selectedRectangle.Bottom + ArrowSpacing;

                        points[2].X = selectedRectangle.Right - ArrowSpacing;
                        points[2].Y = selectedRectangle.Bottom + ArrowSpacing;
                        break;
                    }

                case TabAlignment.Bottom:
                    {
                        points[0].X = selectedRectangle.X + (selectedRectangle.Width / 2);
                        points[0].Y = selectedRectangle.Top + ArrowThickness;

                        points[1].X = selectedRectangle.Left + ArrowSpacing;
                        points[1].Y = selectedRectangle.Top - ArrowSpacing;

                        points[2].X = selectedRectangle.Right - ArrowSpacing;
                        points[2].Y = selectedRectangle.Top - ArrowSpacing;
                        break;
                    }

                case TabAlignment.Right:
                    {
                        points[0].X = selectedRectangle.Left + ArrowThickness;
                        points[0].Y = selectedRectangle.Y + (selectedRectangle.Height / 2);

                        points[1].X = selectedRectangle.Left - ArrowSpacing;
                        points[1].Y = selectedRectangle.Top + ArrowSpacing;

                        points[2].X = selectedRectangle.Left - ArrowSpacing;
                        points[2].Y = selectedRectangle.Bottom - ArrowSpacing;
                        break;
                    }
            }

            e.Graphics.FillPolygon(new SolidBrush(backgroundColor), points);
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
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }
        }

        private void UpdateArrowLocation()
        {
            switch (alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    {
                        arrowThickness = 5;
                        arrowSpacing = 10;
                        break;
                    }

                case TabAlignment.Left:
                case TabAlignment.Right:
                    {
                        arrowThickness = 10;
                        arrowSpacing = 3;
                        break;
                    }
            }
        }

        #endregion
    }
}