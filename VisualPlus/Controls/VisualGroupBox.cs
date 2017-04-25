namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual GroupBox.</summary>
    [ToolboxBitmap(typeof(GroupBox))]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public sealed partial class VisualGroupBox : GroupBox
    {
        #region Variables

        private static BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color groupBoxColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color titleBoxColor = Settings.DefaultValue.Style.BackgroundColor(1);
        private GraphicsPath titleBoxPath;
        private Rectangle titleBoxRectangle;
        private bool titleBoxVisible = Settings.DefaultValue.TitleBoxVisible;

        #endregion

        #region Constructors

        public VisualGroupBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            ForeColor = Settings.DefaultValue.Style.ForeColor(0);
            Size = new Size(220, 180);
            MinimumSize = new Size(136, 50);
            Padding = new Padding(5, 28, 5, 5);
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            UpdateStyles();
        }

        #endregion

        #region Properties

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

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    borderRounding = value;
                }

                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
                controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color GroupBoxColor
        {
            get
            {
                return groupBoxColor;
            }

            set
            {
                groupBoxColor = value;
                ExceptionHandler.ContainerBackColorFix(this, groupBoxColor);
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
        public Color TitleBoxColor
        {
            get
            {
                return titleBoxColor;
            }

            set
            {
                titleBoxColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TitleBoxVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.TitleBoxVisible)]
        public bool TitleBoxVisible
        {
            get
            {
                return titleBoxVisible;
            }

            set
            {
                titleBoxVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            ExceptionHandler.ContainerBackColorFix(this, groupBoxColor);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            UpdateLocationPoints();

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            // Draw the body of the GroupBoxColor
            graphics.FillPath(new SolidBrush(groupBoxColor), controlGraphicsPath);

            // Setup group box border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            // Draw the title box
            if (titleBoxVisible)
            {
                // Draw the background of the title box
                graphics.FillPath(new SolidBrush(titleBoxColor), titleBoxPath);

                // Setup title boxborder
                if (borderVisible)
                {
                    if (controlState == ControlState.Hover && borderHoverVisible)
                    {
                        GDI.DrawBorder(graphics, titleBoxPath, borderThickness, borderHoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, titleBoxPath, borderThickness, borderColor);
                    }
                }
            }

            // Draw the specified string from 'Text' property inside the title box
            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), titleBoxRectangle, stringFormat);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private void UpdateLocationPoints()
        {
            titleBoxRectangle = new Rectangle(0, 0, Width - 1, 25);

            // Determine type of border rounding to draw
            if (borderShape == BorderShape.Rounded)
            {
                titleBoxPath = GDI.DrawRoundedRectangle(titleBoxRectangle, borderRounding);
            }
            else
            {
                titleBoxPath = GDI.DrawRoundedRectangle(titleBoxRectangle, 1);
            }

            // Update paths
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion
    }
}