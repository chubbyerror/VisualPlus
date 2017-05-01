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
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private ControlState controlState = ControlState.Normal;
        private StringAlignment lineAlignment = StringAlignment.Near;
        private Point mouseLocation;
        private TabAlignment selectorAlignment = TabAlignment.Top;
        private int selectorThickness = 4;
        private bool selectorVisible;
        private Color separator = Settings.DefaultValue.Style.TabSelected;
        private int separatorSpacing = 2;
        private float separatorThickness = 2F;
        private bool separatorVisible;

        private Color[] tabHover =
        {
            Settings.DefaultValue.Style.TabSelected,
            ControlPaint.Light(Settings.DefaultValue.Style.TabSelected),
            Settings.DefaultValue.Style.TabSelected
        };

        private Color tabMenu = Settings.DefaultValue.Style.TabMenu;

        private Color[] tabNormal =
        {
            Settings.DefaultValue.Style.TabNormal,
            ControlPaint.Light(Settings.DefaultValue.Style.TabNormal),
            Settings.DefaultValue.Style.TabNormal
        };

        private Point selectedStartPoint;
        private Point selectedEndPoint;
        private Point normalStartPoint;
        private Point normalEndPoint;
        private Point hoverStartPoint;
        private Point hoverEndPoint;

        private Color[] tabSelected =
        {
            Settings.DefaultValue.Style.TabSelected,
            ControlPaint.Light(Settings.DefaultValue.Style.TabSelected),
            Settings.DefaultValue.Style.TabSelected
        };

        private float gradientHoverAngle;
        private LinearGradientBrush gradientHoverBrush;
        private float[] gradientHoverPosition = { 0, 1 / 2f, 1 };
        private float gradientNormalAngle;
        private LinearGradientBrush gradientNormalBrush;
        private float[] gradientNormalPosition = { 0, 1 / 2f, 1 };
        private float gradientSelectedAngle;
        private LinearGradientBrush gradientSelectedBrush;
        private float[] gradientSelectedPosition = { 0, 1 / 2f, 1 };
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
            ItemSize = new Size(100, 30);
            MinimumSize = new Size(144, 85);
            LineAlignment = StringAlignment.Center;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);

            foreach (TabPage page in TabPages)
            {
                page.BackColor = backgroundColor;
                page.Font = Font;
            }
        }

        #endregion

        #region Properties

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
                Rectangle tabHighlighter = GDI.ApplyAnchor(SelectorAlignment, GetTabRect(tabIndex), selectorThickness);

                if (tabIndex == SelectedIndex)
                {
                    // Draw selected tab
                    graphics.FillRectangle(gradientSelectedBrush, tabRect);

                    // Draw tab selector
                    if (selectorVisible)
                    {
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
                    graphics.FillRectangle(gradientNormalBrush, tabRect);

                    if (controlState == ControlState.Hover && tabRect.Contains(mouseLocation))
                    {
                        Cursor = Cursors.Hand;

                        // Draw hover background
                        graphics.FillRectangle(gradientHoverBrush, tabRect);

                        // Draw tab selector
                        if (selectorVisible)
                        {
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
                    {
                        throw new ArgumentOutOfRangeException();
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