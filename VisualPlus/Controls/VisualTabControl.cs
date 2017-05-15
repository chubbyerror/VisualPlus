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

        private TabAlignment alignment = TabAlignment.Top;
        private bool arrowSelectorVisible = true;
        private int arrowSpacing = 10;
        private int arrowThickness = 5;
        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(3);
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private ControlState controlState = ControlState.Normal;
        private float gradientHoverAngle;
        private LinearGradientBrush gradientHoverBrush;
        private float[] gradientHoverPosition = { 0, 1 };
        private float gradientNormalAngle;
        private LinearGradientBrush gradientNormalBrush;
        private float[] gradientNormalPosition = { 0, 1 };
        private float gradientSelectedAngle;
        private LinearGradientBrush gradientSelectedBrush;
        private float[] gradientSelectedPosition = { 0, 1 };
        private Point hoverEndPoint;
        private Point hoverStartPoint;
        private Size itemSize = new Size(100, 25);
        private StringAlignment lineAlignment = StringAlignment.Near;
        private Point mouseLocation;
        private Point normalEndPoint;
        private Point normalStartPoint;
        private Point selectedEndPoint;
        private Point selectedStartPoint;
        private TabAlignment selectorAlignment = TabAlignment.Top;
        private TabAlignment selectorAlignment2 = TabAlignment.Bottom;
        private int selectorThickness = 4;
        private bool selectorVisible;
        private bool selectorVisible2;
        private Color separator = Settings.DefaultValue.Style.TabSelected;
        private int separatorSpacing = 2;
        private float separatorThickness = 2F;
        private bool separatorVisible;

        private Color[] tabHover =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TabSelected),
                Settings.DefaultValue.Style.TabSelected
            };

        private Color tabMenu = Settings.DefaultValue.Style.TabMenu;

        private Color[] tabNormal =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TabNormal),
                Settings.DefaultValue.Style.TabNormal
            };

        private Color[] tabSelected =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TabSelected),
                Settings.DefaultValue.Style.TabSelected
            };

        private Color tabSelector = Settings.DefaultValue.Style.StyleColor;
        private StringAlignment textAlignment = StringAlignment.Center;
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
            MinimumSize = new Size(144, 85);
            LineAlignment = StringAlignment.Center;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            ItemSize = itemSize;

            foreach (TabPage page in TabPages)
            {
                page.BackColor = backgroundColor;
                page.Font = Font;
            }
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Alignment)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
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

        [DefaultValue(Settings.DefaultValue.BorderThickness)]
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientHoverAngle
        {
            get
            {
                return gradientHoverAngle;
            }

            set
            {
                gradientHoverAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientHoverPosition
        {
            get
            {
                return gradientHoverPosition;
            }

            set
            {
                gradientHoverPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientNormalAngle
        {
            get
            {
                return gradientNormalAngle;
            }

            set
            {
                gradientNormalAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientNormalPosition
        {
            get
            {
                return gradientNormalPosition;
            }

            set
            {
                gradientNormalPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientSelectedAngle
        {
            get
            {
                return gradientSelectedAngle;
            }

            set
            {
                gradientSelectedAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientSelectedPosition
        {
            get
            {
                return gradientSelectedPosition;
            }

            set
            {
                gradientSelectedPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentSize)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Alignment)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Alignment)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentSize)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
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
        [Description(Localize.Description.ComponentVisible)]
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
        public Color[] TabHover
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
        public Color[] TabNormal
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
        public Color[] TabSelected
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

            UpdateLocationPoints();

            // Draw tab selector background body
            graphics.FillRectangle(new SolidBrush(tabMenu), new Rectangle(0, 0, Width, Height));

            // gradients
            gradientSelectedBrush = GDI.CreateGradientBrush(tabSelected, gradientSelectedPosition, gradientSelectedAngle, selectedStartPoint, selectedEndPoint);
            gradientNormalBrush = GDI.CreateGradientBrush(tabNormal, gradientNormalPosition, gradientNormalAngle, normalStartPoint, normalEndPoint);
            gradientHoverBrush = GDI.CreateGradientBrush(tabHover, gradientHoverPosition, gradientHoverAngle, hoverStartPoint, hoverEndPoint);

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

                // Draws the TabSelector
                Rectangle selectorRectangle = GDI.ApplyAnchor(selectorAlignment, GetTabRect(tabIndex), selectorThickness);
                Rectangle selectorRectangle2 = GDI.ApplyAnchor(SelectorAlignment2, GetTabRect(tabIndex), selectorThickness);

                if (tabIndex == SelectedIndex)
                {
                    // Draw selected tab
                    graphics.FillRectangle(gradientSelectedBrush, tabRect);

                    // Draw tab selector
                    if (selectorVisible)
                    {
                        graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle);
                    }

                    if (selectorVisible2)
                    {
                        graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle2);
                    }

                    // Draw border
                    if (borderVisible)
                    {
                        GraphicsPath borderPath = new GraphicsPath();
                        borderPath.AddRectangle(tabRect);
                        GDI.DrawBorderType(graphics, controlState, borderPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
                    }

                    if (arrowSelectorVisible)
                    {
                        DrawSelectionArrow(e, tabRect);
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
                    graphics.FillRectangle(gradientNormalBrush, tabRect);

                    if (controlState == ControlState.Hover && tabRect.Contains(mouseLocation))
                    {
                        Cursor = Cursors.Hand;

                        // Draw hover background
                        graphics.FillRectangle(gradientHoverBrush, tabRect);

                        // Draw tab selector
                        if (selectorVisible)
                        {
                            graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle);
                        }

                        if (selectorVisible2)
                        {
                            graphics.FillRectangle(new SolidBrush(tabSelector), selectorRectangle2);
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

        private void DrawSelectionArrow(PaintEventArgs e, Rectangle selectedRectangle)
        {
            var points = new Point[3];

            switch (Alignment)
            {
                case TabAlignment.Left:
                    {
                        points[0].X = selectedRectangle.Right - ArrowThickness;
                        points[0].Y = selectedRectangle.Y + selectedRectangle.Height / 2;

                        points[1].X = selectedRectangle.Right + ArrowSpacing;
                        points[1].Y = selectedRectangle.Top + ArrowSpacing;

                        points[2].X = selectedRectangle.Right + ArrowSpacing;
                        points[2].Y = selectedRectangle.Bottom - ArrowSpacing;
                        break;
                    }

                case TabAlignment.Top:
                    {
                        points[0].X = selectedRectangle.X + selectedRectangle.Width / 2;
                        points[0].Y = selectedRectangle.Bottom - ArrowThickness;

                        points[1].X = selectedRectangle.Left + ArrowSpacing;
                        points[1].Y = selectedRectangle.Bottom + ArrowSpacing;

                        points[2].X = selectedRectangle.Right - ArrowSpacing;
                        points[2].Y = selectedRectangle.Bottom + ArrowSpacing;
                        break;
                    }

                case TabAlignment.Bottom:
                    {
                        points[0].X = selectedRectangle.X + selectedRectangle.Width / 2;
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
                        points[0].Y = selectedRectangle.Y + selectedRectangle.Height / 2;

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

        private void UpdateLocationPoints()
        {
            selectedStartPoint = new Point(ItemSize.Width, 0);
            selectedEndPoint = new Point(ItemSize.Width, ItemSize.Height);

            normalStartPoint = new Point(ItemSize.Width, 0);
            normalEndPoint = new Point(ItemSize.Width, ItemSize.Height);

            hoverStartPoint = new Point(ClientRectangle.Width, 0);
            hoverEndPoint = new Point(ItemSize.Width, ItemSize.Height);
        }

        #endregion
    }
}