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
    using VisualPlus.Localization;

    #endregion

    [ToolboxBitmap(typeof(ContextMenuStrip))]
    public class VisualContextMenuStrip : ContextMenuStrip
    {
        #region Variables

        private static Color arrowColor = Settings.DefaultValue.Style.DropDownButtonColor;
        private static Color arrowDisabledColor = Settings.DefaultValue.Style.ControlDisabled;
        private static bool arrowVisible = Settings.DefaultValue.TextVisible;
        private static Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private static Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private static int borderSize = Settings.DefaultValue.BorderSize;
        private static bool borderVisible = Settings.DefaultValue.BorderVisible;
        private static Font contextMenuFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        private static Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private static Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;

        private ToolStripItemClickedEventArgs clickedEventArgs;

        #endregion

        #region Constructors

        public VisualContextMenuStrip()
        {
            Renderer = new VisualToolStripRender();
            BackColor = backgroundColor;
        }

        public delegate void ClickedEventHandler(object sender);

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
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

        [DefaultValue(Settings.DefaultValue.BorderSize)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderThickness)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderSize = value;
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
        [Description(Localize.Description.ComponentFont)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextColor)]
        public Color TextColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                foreColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        public event ClickedEventHandler Clicked;

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem != null && !(e.ClickedItem is ToolStripSeparator))
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

        #endregion

        #region Methods

        public sealed class VisualToolStripMenuItem : ToolStripMenuItem
        {
            #region Constructors

            public VisualToolStripMenuItem()
            {
                AutoSize = false;
                Size = new Size(160, 30);
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
                e.Graphics.Clear(Settings.DefaultValue.Style.BackgroundColor(0));
                Rectangle R = new Rectangle(0, e.Item.ContentRectangle.Y - 2, e.Item.ContentRectangle.Width + 4, e.Item.ContentRectangle.Height + 3);
                e.Graphics.FillRectangle(e.Item.Selected && e.Item.Enabled ? new SolidBrush(Color.FromArgb(130, backgroundColor)) : new SolidBrush(backgroundColor), R);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawLine(new Pen(Color.FromArgb(200, borderColor), borderSize), new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2), new Point(e.Item.Bounds.Right - 5, e.Item.Bounds.Height / 2));
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                base.OnRenderToolStripBackground(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.Clear(backgroundColor);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (borderVisible)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.High;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle hoverBorder = new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width, e.AffectedBounds.Height - 1);
                    GraphicsPath borderPath = new GraphicsPath();
                    borderPath.AddRectangle(hoverBorder);
                    borderPath.CloseAllFigures();

                    e.Graphics.SetClip(borderPath);
                    e.Graphics.DrawPath(new Pen(borderColor), borderPath);
                    e.Graphics.ResetClip();
                }
            }

            #endregion
        }

        #endregion
    }
}