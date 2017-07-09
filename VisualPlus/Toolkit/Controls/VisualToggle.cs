namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;
    using VisualPlus.Handlers;
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Control))]
    [DefaultEvent("ToggledChanged")]
    [DefaultProperty("Toggled")]
    [Description("The Visual Toggle")]
    [Designer(ControlManager.FilterProperties.VisualToggle)]
    public sealed class VisualToggle : VisualControlBase
    {
        #region Variables

        private readonly Timer animationTimer = new Timer
            {
                Interval = 1
            };

        private Gradient backgroundDisabledGradient;
        private Gradient backgroundEnabledGradient;
        private Border buttonBorder;
        private Gradient buttonDisabledGradient;
        private Gradient buttonGradient;
        private Rectangle buttonRectangle;
        private Size buttonSize;
        private GraphicsPath controlGraphicsPath;
        private Point endPoint;
        private Point startPoint;
        private string textProcessor;
        private bool toggled;
        private int toggleLocation;
        private ToggleTypes toggleType;

        #endregion

        #region Constructors

        public VisualToggle()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Size = new Size(50, 25);
            Font = StyleManager.Font;
            animationTimer.Tick += AnimationTimerTick;

            toggleType = ToggleTypes.YesNo;
            buttonSize = new Size(20, 20);

            ControlBorder = new Border
                {
                    Rounding = Settings.DefaultValue.Rounding.ToggleBorder
                };

            buttonBorder = new Border
                {
                    Rounding = Settings.DefaultValue.Rounding.ToggleButton
                };

            backgroundEnabledGradient = StyleManager.ProgressStyle.Background;
            backgroundDisabledGradient = StyleManager.ControlStatesStyle.ControlDisabled;
            buttonGradient = StyleManager.ControlStatesStyle.ControlEnabled;
            buttonDisabledGradient = StyleManager.ControlStatesStyle.ControlDisabled;
        }

        public delegate void ToggledChangedEventHandler();

        public event ToggledChangedEventHandler ToggledChanged;

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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient BackgroundDisabled
        {
            get
            {
                return backgroundDisabledGradient;
            }

            set
            {
                backgroundDisabledGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient BackgroundEnabled
        {
            get
            {
                return backgroundEnabledGradient;
            }

            set
            {
                backgroundEnabledGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return ControlBorder;
            }

            set
            {
                ControlBorder = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border ButtonBorder
        {
            get
            {
                return buttonBorder;
            }

            set
            {
                buttonBorder = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient ButtonDisabled
        {
            get
            {
                return buttonDisabledGradient;
            }

            set
            {
                buttonDisabledGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient ButtonGradient
        {
            get
            {
                return buttonGradient;
            }

            set
            {
                buttonGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
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

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
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

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            animationTimer.Start();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Toggled = !Toggled;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);

            // Update button location points
            startPoint = new Point(0 + 2, (ClientRectangle.Height / 2) - (buttonSize.Height / 2));
            endPoint = new Point(ClientRectangle.Width - buttonSize.Width - 2, (ClientRectangle.Height / 2) - (buttonSize.Height / 2));

            Gradient buttonTemp = Enabled ? buttonGradient : buttonDisabledGradient;
            Gradient backTemp = Enabled ? backgroundEnabledGradient : backgroundDisabledGradient;

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush buttonGradientBrush = Gradient.CreateGradientBrush(buttonTemp.Colors, gradientPoints, buttonTemp.Angle, buttonTemp.Positions);
            LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backTemp.Colors, gradientPoints, backTemp.Angle, backTemp.Positions);

            graphics.FillPath(backgroundGradientBrush, controlGraphicsPath);

            Border.DrawBorderStyle(graphics, ControlBorder, MouseState, controlGraphicsPath);

            // Determines button state to draw
            Point buttonPoint = toggled ? endPoint : startPoint;
            buttonRectangle = new Rectangle(buttonPoint, buttonSize);

            DrawToggleType(graphics);

            GraphicsPath buttonPath = Border.GetBorderShape(buttonRectangle, buttonBorder.Type, buttonBorder.Rounding);
            graphics.FillPath(buttonGradientBrush, buttonPath);

            Border.DrawBorderStyle(graphics, buttonBorder, MouseState, buttonPath);
        }

        /// <summary>Create a slide animation when toggled.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void AnimationTimerTick(object sender, EventArgs e)
        {
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

            // Draw string
            Rectangle textBoxRectangle;

            const int XOff = 5;
            const int XOn = 7;

            if (toggled)
            {
                textBoxRectangle = new Rectangle(XOff, 0, Width - 1, Height - 1);
            }
            else
            {
                textBoxRectangle = new Rectangle(Width - (int)Font.SizeInPoints - (XOn * 2), 0, Width - 1, Height - 1);
            }

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(
                textProcessor,
                new Font(Font.FontFamily, 7f, Font.Style),
                new SolidBrush(ForeColor),
                textBoxRectangle,
                stringFormat);
        }

        #endregion
    }
}