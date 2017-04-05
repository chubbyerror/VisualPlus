namespace VisualPlus.Controls
{
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

    public enum ToggleTypes
    {
        /// <summary>Yes / No toggle.</summary>
        YesNo,

        /// <summary>On / Off toggle.</summary>
        OnOff,

        /// <summary>I / O toggle.</summary>
        IO
    }

    /// <summary>The visual Toggle.</summary>
    [ToolboxBitmap(typeof(Control)), Designer(VSDesignerBinding.VisualToggle), DefaultEvent("ToggledChanged"), Description("A toggle button allows the user to change a setting between two states.")]
    public class VisualToggle : Control
    {
        public class PillStyle
        {
            #region  ${0} Variables

            public bool Left;
            public bool Right;

            #endregion
        }

        #region  ${0} Variables

        private readonly Timer animationTimer = new Timer
            {
                Interval = 1
            };

        private Color backgroundColor = StylesManager.DefaultValue.Style.BackgroundColor(0);

        private Rectangle barSlider;
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color buttonColor = StylesManager.DefaultValue.Style.ButtonNormalColor;
        private Color controlDisabledColor = StylesManager.DefaultValue.Style.ControlDisabled;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Size sizeHandle = new Size(15, 20);
        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;
        private string textProcessor;
        private bool toggled;
        private int toggleLocation;
        private ToggleTypes toggleType = ToggleTypes.YesNo;
        private int xBar;

        #endregion

        #region ${0} Properties

        public VisualToggle()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;

            UpdateStyles();
            animationTimer.Tick += AnimationTimerTick;
        }

        public delegate void ToggledChangedEventHandler();

        public event ToggledChangedEventHandler ToggledChanged;

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextColor)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        [DefaultValue(false), Category(Localize.Category.Behavior), Description(Localize.Description.Toggled)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ToggleType)]
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

        #region ${0} Events

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
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            checked
            {
                // Bar slider
                barSlider = new Rectangle(8, 10, Width - 21, Height - 21);

                // Pill graphic path
                GraphicsPath pillPath = (GraphicsPath)Pill(
                    0,
                    (int)Math.Round(Height / 2.0 - sizeHandle.Height / 2.0),
                    Width - 1,
                    sizeHandle.Height - 5,
                    new PillStyle
                        {
                            Left = true,
                            Right = true
                        });

                // Button
                Rectangle buttonRectangle = new Rectangle(
                    barSlider.X + (int)Math.Round(barSlider.Width * (toggleLocation / 80.0)) - (int)Math.Round(sizeHandle.Width / 2.0),
                    barSlider.Y + (int)Math.Round(barSlider.Height / 2.0) - (int)Math.Round(sizeHandle.Height / 2.0 - 1.0),
                    sizeHandle.Width,
                    sizeHandle.Height - 5);

                // Set control state color
                Color controlTempColor = Enabled ? backgroundColor : controlDisabledColor;

                // Background color
                graphics.FillPath(new SolidBrush(controlTempColor), pillPath);

                // Draw pill border
                if (borderVisible)
                {
                    if (controlState == ControlState.Hover && borderHoverVisible)
                    {
                        GDI.DrawBorder(graphics, pillPath, borderSize, borderHoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, pillPath, borderSize, borderColor);
                    }
                }

                // Draw toggle
                DrawToggleType(graphics);

                // Button
                GraphicsPath buttonPath = GDI.DrawRoundedRectangle(buttonRectangle, 15);
                graphics.FillPath(new SolidBrush(buttonColor), buttonPath);

                // Button border
                GDI.DrawBorder(graphics, buttonPath, 1, StylesManager.DefaultValue.Style.BorderColor(0));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = 41;
            Height = 23;
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
                        if (Toggled)
                        {
                            textProcessor = "Yes";
                            xBar = barSlider.X + 7;
                        }
                        else
                        {
                            textProcessor = "No";
                            xBar = barSlider.X + 18;
                        }

                        break;
                    }

                case ToggleTypes.OnOff:
                    {
                        if (Toggled)
                        {
                            textProcessor = "On";
                            xBar = barSlider.X + 7;
                        }
                        else
                        {
                            textProcessor = "Off";
                            xBar = barSlider.X + 18;
                        }

                        break;
                    }

                case ToggleTypes.IO:
                    {
                        if (Toggled)
                        {
                            textProcessor = "I";
                            xBar = barSlider.X + 7;
                        }
                        else
                        {
                            textProcessor = "O";
                            xBar = barSlider.X + 18;
                        }

                        break;
                    }
            }

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            // Draw string
            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(
                textProcessor,
                new Font(Font.FontFamily, 7f, Font.Style),
                new SolidBrush(foreColor),
                xBar,
                barSlider.Y,
                stringFormat);
        }

        #endregion

        #region ${0} Methods

        public GraphicsPath Pill(Rectangle rectangle, PillStyle pillStyle)
        {
            GraphicsPath extractValue = new GraphicsPath();

            if (pillStyle.Left)
            {
                extractValue.AddArc(new Rectangle(rectangle.X, rectangle.Y, rectangle.Height, rectangle.Height), -270, 180);
            }
            else
            {
                extractValue.AddLine(rectangle.X, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y);
            }

            if (pillStyle.Right)
            {
                extractValue.AddArc(
                    new Rectangle(rectangle.X + rectangle.Width - rectangle.Height, rectangle.Y, rectangle.Height, rectangle.Height),
                    -90,
                    180);
            }
            else
            {
                extractValue.AddLine(rectangle.X + rectangle.Width, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
            }

            extractValue.CloseAllFigures();
            return extractValue;
        }

        public object Pill(int x, int y, int width, int height, PillStyle pillStyle)
        {
            return Pill(new Rectangle(x, y, width, height), pillStyle);
        }

        #endregion
    }
}