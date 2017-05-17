namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual panel.</summary>
    [ToolboxBitmap(typeof(Panel))]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public sealed partial class VisualPanel : Panel
    {
        #region Variables

        public bool Expanded = true;

        #endregion

        #region Variables

        private static BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Color buttonColor = Settings.DefaultValue.Style.DropDownButtonColor;
        private Direction buttonDirection = Direction.Right;
        private Rectangle buttonRectangle;
        private Size buttonSize = new Size(12, 10);
        private int buttonSpacing = 6;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private bool expandButtonVisible = true;
        private Size originalSize;
        private int xValue;
        private int yValue;

        #endregion

        #region Constructors

        public VisualPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;

            UpdateStyles();

            originalSize = ClientRectangle.Size;
        }

        public delegate void ButtonClickedEventHandler();

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
                ExceptionHandler.ApplyContainerBackColorChange(this, backgroundColor);
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
        public Color ButtonColor
        {
            get
            {
                return buttonColor;
            }

            set
            {
                buttonColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Direction)]
        public Direction ButtonDirection
        {
            get
            {
                return buttonDirection;
            }

            set
            {
                buttonDirection = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public Size ButtonSize
        {
            get
            {
                return buttonSize;
            }

            set
            {
                buttonSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int ButtonSpacing
        {
            get
            {
                return buttonSpacing;
            }

            set
            {
                buttonSpacing = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ExpandButtonVisible
        {
            get
            {
                return expandButtonVisible;
            }

            set
            {
                expandButtonVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        public event ButtonClickedEventHandler ButtonClicked;

        protected override void OnControlAdded(ControlEventArgs e)
        {
            ExceptionHandler.SetControlBackColor(e.Control, backgroundColor, false);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            ExceptionHandler.SetControlBackColor(e.Control, backgroundColor, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnMouseClick(e);

            if (ExpandButtonVisible)
            {
                if (buttonDirection == Direction.Left)
                {
                    // Check if mouse in X position.
                    if (xValue > buttonRectangle.X && xValue < buttonSize.Width + ButtonSpacing)
                    {
                        // Determine the button middle separator by checking for the Y position.
                        if (yValue > buttonRectangle.Y && yValue < buttonSize.Height + ButtonSpacing)
                        {
                            if (Expanded)
                            {
                                Expand(false);
                            }
                            else
                            {
                                Expand(true);
                            }

                            ButtonClicked?.Invoke();
                        }
                    }
                    else
                    {
                        Focus();
                    }
                }
                else
                {
                    // Check if mouse in X position.
                    if (xValue > Width - buttonRectangle.X - buttonSize.Width && xValue < Width - buttonRectangle.X)
                    {
                        // Determine the button middle separator by checking for the Y position.
                        if (yValue > buttonRectangle.Y && yValue < buttonRectangle.Y + buttonRectangle.Height)
                        {
                            if (Expanded)
                            {
                                Expand(false);
                            }
                            else
                            {
                                Expand(true);
                            }

                            ButtonClicked?.Invoke();
                        }
                    }
                    else
                    {
                        Focus();
                    }
                }

                Invalidate();
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
            base.OnMouseLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            xValue = e.Location.X;
            yValue = e.Location.Y;
            Invalidate();

            // Cursor toggle
            if (ExpandButtonVisible)
            {
                if (buttonDirection == Direction.Left)
                {
                    if (e.X < buttonRectangle.X + buttonSize.Width && e.Y < buttonRectangle.Y + buttonSize.Height)
                    {
                        Cursor = Cursors.Hand;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    if (e.X > Width - buttonRectangle.X - buttonSize.Width && e.Y < buttonRectangle.Y + buttonSize.Height)
                    {
                        Cursor = Cursors.Hand;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            UpdateLocationPoints();

            // Draw background
            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            // Setup control border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            if (ExpandButtonVisible)
            {
                DrawExpanderArrow(e);
            }
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

        private void DrawExpanderArrow(PaintEventArgs e)
        {
            var points = new Point[3];
            if (buttonDirection == Direction.Left)
            {
                if (Expanded)
                {
                    points[0].X = buttonRectangle.X + ButtonSize.Width / 2;
                    points[0].Y = buttonRectangle.Y;

                    points[1].X = buttonRectangle.X;
                    points[1].Y = buttonRectangle.Y + ButtonSize.Height;

                    points[2].X = buttonRectangle.X + ButtonSize.Width;
                    points[2].Y = buttonRectangle.Y + ButtonSize.Height;
                }
                else
                {
                    points[0].X = buttonRectangle.X;
                    points[0].Y = buttonRectangle.Y;

                    points[1].X = buttonRectangle.X + ButtonSize.Width;
                    points[1].Y = buttonRectangle.Y;

                    points[2].X = buttonRectangle.X + ButtonSize.Width / 2;
                    points[2].Y = buttonRectangle.Y + ButtonSize.Height;
                }
            }
            else
            {
                if (Expanded)
                {
                    points[0].X = Width - buttonRectangle.X - ButtonSize.Width / 2;
                    points[0].Y = buttonRectangle.Y;

                    points[1].X = Width - buttonRectangle.X - buttonSize.Width;
                    points[1].Y = buttonRectangle.Y + ButtonSize.Height;

                    points[2].X = Width - buttonRectangle.X;
                    points[2].Y = buttonRectangle.Y + ButtonSize.Height;
                }
                else
                {
                    points[0].X = Width - buttonRectangle.X - buttonSize.Width;
                    points[0].Y = buttonRectangle.Y;

                    points[1].X = Width - buttonRectangle.X;
                    points[1].Y = buttonRectangle.Y;

                    points[2].X = Width - buttonRectangle.X - ButtonSize.Width / 2;
                    points[2].Y = buttonRectangle.Y + ButtonSize.Height;
                }
            }

            e.Graphics.FillPolygon(new SolidBrush(buttonColor), points);
        }

        private void Expand(bool contract)
        {
            int height;

            if (contract)
            {
                height = originalSize.Height;
                Expanded = true;
            }
            else
            {
                height = buttonRectangle.X + buttonRectangle.Height + ButtonSpacing;

                Expanded = false;
            }

            Size = new Size(ClientRectangle.Width, height);
        }

        private void UpdateLocationPoints()
        {
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
            buttonRectangle = new Rectangle(ButtonSpacing, ButtonSpacing, ButtonSize.Width, ButtonSize.Height);
        }

        #endregion
    }
}