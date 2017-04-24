namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual Toggle.</summary>
    [ToolboxBitmap(typeof(Control))]
    [Designer(VSDesignerBinding.VisualToggle)]
    [DefaultEvent("ToggledChanged")]
    [Description("A toggle button allows the user to change a setting between two states.")]
    public sealed class VisualToggle : Control
    {
        #region Variables

        private readonly Timer animationTimer = new Timer
            {
                Interval = 1
            };

        private Color backgroundColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderSize;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Color buttonColor = Settings.DefaultValue.Style.ButtonNormalColor;
        private Rectangle buttonRectangle;
        private int buttonRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape buttonShape = Settings.DefaultValue.BorderShape;
        private Size buttonSize = new Size(20, 16);
        private Color controlDisabledColor = Settings.DefaultValue.Style.ControlDisabled;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Point endPoint;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Point startPoint;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private string textProcessor;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private bool toggled;
        private int toggleLocation;
        private ToggleTypes toggleType = ToggleTypes.YesNo;

        #endregion

        #region Constructors

        public VisualToggle()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            BackColor = Color.Transparent;
            Size = new Size(50, 20);

            // barSize = new Size(ClientRectangle.Width, ClientRectangle.Height);
            animationTimer.Tick += AnimationTimerTick;
        }

        public delegate void ToggledChangedEventHandler();

        public enum ToggleTypes
        {
            /// <summary>Yes / No toggle.</summary>
            YesNo,

            /// <summary>On / Off toggle.</summary>
            OnOff,

            /// <summary>I / O toggle.</summary>
            IO
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
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderSize)]
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

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
        public int ButtonRounding
        {
            get
            {
                return buttonRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    buttonRounding = value;
                }

                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public BorderShape ButtonShape
        {
            get
            {
                return buttonShape;
            }

            set
            {
                buttonShape = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ButtonSize)]
        public Size ButtonSize
        {
            get
            {
                return buttonSize;
            }

            set
            {
                buttonSize = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
        public Color ControlDisabledColor
        {
            get
            {
                return controlDisabledColor;
            }

            set
            {
                controlDisabledColor = value;
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

        [DefaultValue(false)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Toggled)]
        public bool Toggled
        {
            get
            {
                return toggled;
            }

            set
            {
                toggled = value;
                Invalidate();

                ToggledChanged?.Invoke();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ToggleType)]
        public ToggleTypes Type
        {
            get
            {
                return toggleType;
            }

            set
            {
                toggleType = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        public event ToggledChangedEventHandler ToggledChanged;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            animationTimer.Start();
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
            Toggled = !Toggled;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            // Set control state color
            Color controlTempColor = Enabled ? backgroundColor : controlDisabledColor;

            // Background color
            graphics.FillPath(new SolidBrush(controlTempColor), controlGraphicsPath);

            // Draw pill border
            if (borderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            // Determines button state to draw
            Point buttonPoint = toggled ? endPoint : startPoint;
            buttonRectangle = new Rectangle(buttonPoint, buttonSize);

            // Draw toggle
            DrawToggleType(graphics);

            // Draw button
            GraphicsPath buttonPath = GDI.GetBorderShape(buttonRectangle, buttonShape, buttonRounding);
            graphics.FillPath(new SolidBrush(buttonColor), buttonPath);

            // Button border
            GDI.DrawBorder(graphics, buttonPath, 1, Settings.DefaultValue.Style.BorderColor(0));
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

        private void AnimationTimerTick(object sender, EventArgs e)
        {
            // Create a slide animation when toggled.
            if (toggled)
            {
                if (toggleLocation >= 100)
                {
                    return;
                }

                toggleLocation += 10;
                Invalidate(false);
            }
            else if (toggleLocation > 0)
            {
                toggleLocation -= 10;
                Invalidate(false);
            }
        }

        private void DrawToggleType(Graphics graphics)
        {
            // Determines the type of toggle to draw.
            switch (toggleType)
            {
                case ToggleTypes.YesNo:
                    {
                        textProcessor = Toggled ? "Yes" : "No";

                        break;
                    }

                case ToggleTypes.OnOff:
                    {
                        textProcessor = Toggled ? "On" : "Off";

                        break;
                    }

                case ToggleTypes.IO:
                    {
                        textProcessor = Toggled ? "I" : "O";

                        break;
                    }
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            // Draw string
            Rectangle textboxRectangle;

            if (toggled)
            {
                textboxRectangle = new Rectangle(5, 0, Width - 1, Height - 1);
            }
            else
            {
                textboxRectangle = new Rectangle(Width - (int)Font.SizeInPoints - 5 * 2, 0, Width - 1, Height - 1);
            }

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            // Draw the string
            graphics.DrawString(
                textProcessor,
                new Font(Font.FontFamily, 7f, Font.Style),
                new SolidBrush(foreColor),
                textboxRectangle,
                stringFormat);
        }

        private void UpdateLocationPoints()
        {
            // Update button location points
            startPoint = new Point(0 + 2, ClientRectangle.Height / 2 - buttonSize.Height / 2);
            endPoint = new Point(ClientRectangle.Width - buttonSize.Width - 2, ClientRectangle.Height / 2 - buttonSize.Height / 2);

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion

        #region Methods ${0}

        public class PillStyle
        {
            #region Variables

            public bool Left;
            public bool Right;

            #endregion
        }

        #endregion
    }
}