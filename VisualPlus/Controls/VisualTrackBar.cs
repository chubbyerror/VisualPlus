namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Styles;
    using VisualPlus.Localization;

    public enum ValueDivisor
    {
        /// <summary>The by 1.</summary>
        By1 = 1,

        /// <summary>The by 10.</summary>
        By10 = 10,

        /// <summary>The by 100.</summary>
        By100 = 100,

        /// <summary>The by 1000.</summary>
        By1000 = 1000
    }

    /// <summary>The visual Trackbar.</summary>
    [DefaultEvent("ValueChanged"), ToolboxBitmap(typeof(TrackBar)), Designer(VSDesignerBinding.VisualTrackBar)]
    public class VisualTrackBar : TrackBar
    {
        #region  ${0} Variables

        private static readonly IStyle Style = new Visual();
        private static ControlState controlState = ControlState.Normal;
        private static Color hatchBackColor = Style.HatchColor;
        private static Color progressColor1 = Style.ProgressColor;
        private static Orientation trackBarType = Orientation.Horizontal;
        private Color backgroundColor1 = Style.BackgroundColor(0);
        private Color borderColor = Style.BorderColor(0);
        private Color borderHoverColor = Style.BorderColor(1);
        private bool borderHoverVisible = true;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = true;

        private Color buttonColor1 = Style.ButtonNormalColor;
        private bool cap;

        private ValueDivisor dividedValue = ValueDivisor.By1;
        private Color hatchForeColor = Color.FromArgb(40, hatchBackColor);
        private float hatchSize = StylesManager.DefaultValue.HatchSize;

        private HatchStyle hatchStyle = HatchStyle.DarkDownwardDiagonal;

        private bool hatchVisible;

        private Rectangle pipeRectangle;

        private Color progressColor2 = ControlPaint.Light(progressColor1);
        private int progressDrawer;
        private GraphicsPath progressPath = new GraphicsPath();
        private BrushType progressStyle = BrushType.Gradient;
        private Rectangle progressValueRectangle;

        private GraphicsPath trackBarHandle;
        private Rectangle trackBarHandleRectangle;

        private bool valueVisible;

        #endregion

        #region ${0} Properties

        public VisualTrackBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            UpdateStyles();
            AutoSize = false;
            hatchVisible = true;
            Size = new Size(80, 22);
            MinimumSize = new Size(37, 22);
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor1
        {
            get
            {
                return backgroundColor1;
            }

            set
            {
                backgroundColor1 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderHoverColor)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
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

        [DefaultValue(StylesManager.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumRounding, StylesManager.MaximumRounding))
                {
                    borderRounding = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
         Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
                Invalidate();
            }
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
        public Color ButtonColor1
        {
            get
            {
                return buttonColor1;
            }

            set
            {
                buttonColor1 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color HatchBackColor
        {
            get
            {
                return hatchBackColor;
            }

            set
            {
                hatchBackColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color HatchForeColor
        {
            get
            {
                return hatchForeColor;
            }

            set
            {
                hatchForeColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), DefaultValue(StylesManager.DefaultValue.HatchSize), Description(Localize.Description.HatchSize)]
        public float HatchSize
        {
            get
            {
                return hatchSize;
            }

            set
            {
                hatchSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.HatchStyle)]
        public HatchStyle HatchStyle
        {
            get
            {
                return hatchStyle;
            }

            set
            {
                hatchStyle = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.HatchVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.ComponentVisible)]
        public bool HatchVisible
        {
            get
            {
                return hatchVisible;
            }

            set
            {
                hatchVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(false), Category(Localize.Category.Behavior)]
        public bool JumpToMouse { get; set; } = false;

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color ProgressColor1
        {
            get
            {
                return progressColor1;
            }

            set
            {
                progressColor1 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color ProgressColor2
        {
            get
            {
                return progressColor2;
            }

            set
            {
                progressColor2 = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ProgressStyle)]
        public BrushType ProgressStyle
        {
            get
            {
                return progressStyle;
            }

            set
            {
                progressStyle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.TrackBarType)]
        public Orientation TrackBarType
        {
            get
            {
                return trackBarType;
            }

            set
            {
                trackBarType = value;

                if (trackBarType == Orientation.Horizontal)
                {
                    if (Width < Height)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }
                }
                else
                {
                    // Vertical
                    if (Width > Height)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior), Description(Localize.Description.ValueDivisor)]
        public ValueDivisor ValueDivision
        {
            get
            {
                return dividedValue;
            }

            set
            {
                dividedValue = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets the value to set.</summary>
        [Browsable(false)]
        public float ValueToSet
        {
            get
            {
                return (float)(Value / (double)dividedValue);
            }

            set
            {
                Value = (int)Math.Round(value * (float)dividedValue);
            }
        }

        [DefaultValue(false), Category(Localize.Category.Behavior)]
        public bool ValueVisible
        {
            get
            {
                return valueVisible;
            }

            set
            {
                valueVisible = value;
                if (valueVisible)
                {
                    Height = 40;
                }
                else
                {
                    Height = 22;
                }

                Invalidate();
            }
        }

        #endregion

        #region ${0} Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Jump to mouse position
            if (e.Button == MouseButtons.Left)
            {
                if (trackBarType == Orientation.Vertical)
                {
                    progressDrawer = (int)Math.Round((Value - Minimum) / (double)(Maximum - Minimum) * (Height + 11));
                    trackBarHandleRectangle = new Rectangle(0, progressDrawer, 20, 10);

                    if (JumpToMouse)
                    {
                        Value = Minimum + (int)Math.Round((Maximum - Minimum) * (e.Y / (double)Height));
                    }
                }
                else
                {
                    progressDrawer = (int)Math.Round((Value - Minimum) / (double)(Maximum - Minimum) * (Width - 11));

                    trackBarHandleRectangle = new Rectangle(progressDrawer, 0, 10, 20);
                    cap = trackBarHandleRectangle.Contains(e.Location);

                    if (JumpToMouse)
                    {
                        Value = Minimum + (int)Math.Round((Maximum - Minimum) * (e.X / (double)Width));
                    }
                }
            }

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            OnEnter(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            if (trackBarType == Orientation.Vertical)
            {
                Cursor = Cursors.SizeNS;
            }
            else
            {
                Cursor = Cursors.SizeWE;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            OnLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (trackBarType == Orientation.Vertical)
            {
                if (cap && e.Y > -1 && e.Y < Height + 1)
                {
                    Value = Minimum + (int)Math.Round((Maximum - Minimum) * (e.Y / (double)Height));
                }
            }
            else
            {
                if (cap && e.X > -1 && e.X < Width + 1)
                {
                    Value = Minimum + (int)Math.Round((Maximum - Minimum) * (e.X / (double)Width));
                }
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            cap = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (trackBarType == Orientation.Vertical)
            {
                pipeRectangle = new Rectangle(6, 0, 8, Height); // TODO

                // Determine border shape of total progress path
                progressPath = GDI.GetBorderShape(pipeRectangle, borderShape, borderRounding);

                try
                {
                    progressDrawer = (int)Math.Round(checked((Value - Minimum) / (double)(Maximum - Minimum)) * checked(Height - 11));
                }
                catch (Exception)
                {
                    // ignored
                }

                // Trackbar rectangle
                trackBarHandleRectangle = new Rectangle(0, pipeRectangle.Height - trackBarHandleRectangle.Height - 1, 20, 10);
                progressValueRectangle = new Rectangle(6, pipeRectangle.Height - trackBarHandleRectangle.Height - 1, 8, pipeRectangle.Height - 1);
            }
            else
            {
                pipeRectangle = new Rectangle(1, 6, Width - 3, 8);

                // Determine border shape of total progress path
                progressPath = GDI.GetBorderShape(pipeRectangle, borderShape, borderRounding);

                try
                {
                    progressDrawer = (int)Math.Round(checked((Value - Minimum) / (double)(Maximum - Minimum)) * checked(Width - 11));
                }
                catch (Exception)
                {
                    // ignored
                }

                // Trackbar rectangle
                trackBarHandleRectangle = new Rectangle(progressDrawer, 0, 10, 20);
                progressValueRectangle = new Rectangle(1, 6, trackBarHandleRectangle.X + trackBarHandleRectangle.Width - 2, 8);
            }

            // Current progress path
            GraphicsPath progressValue = GDI.GetBorderShape(progressValueRectangle, borderShape, borderRounding);

            graphics.SetClip(progressPath);

            // Handle
            trackBarHandle = GDI.DrawRoundedRectangle(trackBarHandleRectangle, 3);

            // Draw background color
            graphics.FillPath(new SolidBrush(backgroundColor1), progressPath);

            // Draw progress slide
            if (progressStyle == BrushType.Gradient)
            {
                GDI.FillBackground(graphics, progressValueRectangle, progressValue, progressColor1, progressColor2, 90, true);
            }
            else
            {
                // Solid color
                graphics.FillRectangle(new SolidBrush(progressColor1), progressValueRectangle);
            }

            // Draw hatch overlay
            if (hatchVisible)
            {
                HatchBrush hatchBrush = new HatchBrush(hatchStyle, hatchForeColor, hatchBackColor);
                using (TextureBrush textureBrush = GDI.DrawTextureUsingHatch(hatchBrush))
                {
                    textureBrush.ScaleTransform(hatchSize, hatchSize);
                    graphics.FillRectangle(textureBrush, progressValueRectangle);
                }
            }

            graphics.ResetClip();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw border
            if (borderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, progressPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, progressPath, borderSize, borderColor);
                }
            }

            // Draw TrackBar handle
            graphics.FillPath(new SolidBrush(buttonColor1), trackBarHandle);

            // Draw handle border
            if (controlState == ControlState.Hover && borderHoverVisible)
            {
                GDI.DrawBorder(graphics, trackBarHandle, 1, borderHoverColor);
            }
            else
            {
                GDI.DrawBorder(graphics, trackBarHandle, 1, borderColor);
            }

            // Draw value string
            if (valueVisible)
            {
                StringFormat stringFormat = new StringFormat();

                // stringFormat.Alignment = StringAlignment.Center;
                // stringFormat.LineAlignment = StringAlignment.Center;
                graphics.DrawString(Convert.ToString(ValueToSet), Font, new SolidBrush(ForeColor), 0, 25, stringFormat);
            }

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.ResetClip();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // TODO: Refactor sizing
            if (trackBarType == Orientation.Vertical)
            {
                // Width = 20;
            }
            else
            {
                if (valueVisible)
                {
                    // Height = 40;
                }
            }
        }

        #endregion

        #region ${0} Methods

        public new void SetRange(int minimumValue, int maximumValue)
        {
            Minimum = minimumValue;

            if (Minimum > Value)
            {
                Value = Minimum;
            }

            Maximum = maximumValue;

            if (Maximum < Value)
            {
                Value = Maximum;
            }

            if (Maximum < Minimum)
            {
                Minimum = Maximum;
            }

            Invalidate();
        }

        #endregion
    }
}