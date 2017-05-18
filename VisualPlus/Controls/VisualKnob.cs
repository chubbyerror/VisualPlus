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

    /// <summary>The visual CircleProgressBar.</summary>
    [ToolboxBitmap(typeof(TrackBar))]
    public sealed class VisualKnob : Control
    {
        #region Variables

        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private ControlState controlState = ControlState.Normal;
        private float currentValue;
        private float drawRatio;
        private RectangleF drawRect;
        private Color indicatorColor = Settings.DefaultValue.Style.ProgressColor;
        private float indicatorOffset = 10F;
        private PointF indicatorPosition;
        private RectangleF indicatorRectangleF;
        private PointF knobCenter;
        private Color knobColor = Settings.DefaultValue.Style.BackgroundColor(0);
        private float maxValue = 1.0F;
        private float minValue;
        private bool rotating;
        private Color scaleColor = Color.FromArgb(204, 208, 214);
        private RectangleF scaleRectangleF;
        private float stepValue = 0.1F;
        private KnobStyle style = KnobStyle.Circle;

        #endregion

        #region Constructors

        public VisualKnob()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Size = new Size(76, 76);
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            KeyDown += OnKeyDown;
            ConfigureDimensions();
        }

        public delegate void ValueChangedEventHandler(object sender, KnobEventArgs e);

        public enum KnobStyle
        {
            /// <summary>The circle.</summary>
            Circle = 0
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
        public Color IndicatorColor
        {
            get
            {
                return indicatorColor;
            }

            set
            {
                indicatorColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description("The offset indicator from border.")]
        public float IndicatorOffset
        {
            get
            {
                return indicatorOffset;
            }

            set
            {
                indicatorOffset = value;
                ConfigureDimensions();
                Invalidate();
            }
        }

        [Browsable(false)]
        public PointF KnobCenter
        {
            get
            {
                return knobCenter;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color KnobColor
        {
            get
            {
                return knobColor;
            }

            set
            {
                knobColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description("Maximum value.")]
        public float MaxValue
        {
            get
            {
                return maxValue;
            }

            set
            {
                maxValue = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description("Minimum value.")]
        public float MinValue
        {
            get
            {
                return minValue;
            }

            set
            {
                minValue = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ScaleColor
        {
            get
            {
                return scaleColor;
            }

            set
            {
                scaleColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description("The value in steps.")]
        public float StepValue
        {
            get
            {
                return stepValue;
            }

            set
            {
                stepValue = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description("The knob style.")]
        public KnobStyle Style
        {
            get
            {
                return style;
            }

            set
            {
                style = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description("The current value.")]
        public float Value
        {
            get
            {
                return currentValue;
            }

            set
            {
                if (value != currentValue)
                {
                    currentValue = value;
                    indicatorPosition = GetPositionFromValue(currentValue);
                    Invalidate();

                    KnobEventArgs e = new KnobEventArgs();
                    e.Value = currentValue;
                    OnKnobChangeValue(e);
                }
            }
        }

        #endregion

        #region Events

        public PointF GetPositionFromValue(float value)
        {
            PointF valuePosition = new PointF(0.0F, 0.0F);

            if (MaxValue - MinValue == 0)
            {
                return valuePosition;
            }

            float degree = 270F * value / (MaxValue - MinValue);
            degree = (degree + 135F) * (float)Math.PI / 180F;

            valuePosition.X = (int)(Math.Cos(degree) * (indicatorRectangleF.Width * 0.5F - indicatorOffset) + indicatorRectangleF.X + indicatorRectangleF.Width * 0.5F);
            valuePosition.Y = (int)(Math.Sin(degree) * (indicatorRectangleF.Width * 0.5F - indicatorOffset) + indicatorRectangleF.Y + indicatorRectangleF.Height * 0.5F);

            return valuePosition;
        }

        public float GetValueFromPosition(PointF position)
        {
            float degree;
            var v = 0.0F;

            PointF center = KnobCenter;

            if (position.X <= center.X)
            {
                degree = (center.Y - position.Y) / (center.X - position.X);
                degree = (float)Math.Atan(degree);
                degree = (float)(degree * (180F / Math.PI) + 45F);
                v = degree * (MaxValue - MinValue) / 270F;
            }
            else
            {
                if (position.X > center.X)
                {
                    degree = (position.Y - center.Y) / (position.X - center.X);
                    degree = (float)Math.Atan(degree);
                    degree = (float)(225F + degree * (180F / Math.PI));
                    v = degree * (MaxValue - MinValue) / 270F;
                }
            }

            if (v > MaxValue)
            {
                v = MaxValue;
            }

            if (v < MinValue)
            {
                v = MinValue;
            }

            return v;
        }

        public Color StepColor(Color color, int inputAlpha)
        {
            if (inputAlpha == 100)
            {
                return color;
            }

            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            float background;

            int alpha = Math.Min(inputAlpha, 200);
            alpha = Math.Max(alpha, 0);
            double doubleAlpha = (alpha - 100.0) / 100.0;

            if (doubleAlpha > 100)
            {
                // Blend with white
                background = 255.0F;

                // 0 = transparent fg; 1 = opaque fg
                doubleAlpha = 1.0F - doubleAlpha;
            }
            else
            {
                // Blend with black
                background = 0.0F;

                // 0 = transparent fg; 1 = opaque fg
                doubleAlpha = 1.0F + doubleAlpha;
            }

            r = (byte)ColorHelper.BlendColor(r, background, doubleAlpha);
            g = (byte)ColorHelper.BlendColor(g, background, doubleAlpha);
            b = (byte)ColorHelper.BlendColor(b, background, doubleAlpha);

            return Color.FromArgb(a, r, g, b);
        }

        public event ValueChangedEventHandler ValueChanged;

        protected void ConfigureDimensions()
        {
            // Rectangle
            float x = 0;
            float y = 0;
            float w = Size.Width;
            float h = Size.Height;

            // Calculate ratio
            drawRatio = Math.Min(w, h) / 200;
            if (drawRatio == 0.0)
            {
                drawRatio = 1;
            }

            // Draw rectangle
            drawRect.X = x;
            drawRect.Y = y;
            drawRect.Width = w - 2;
            drawRect.Height = h - 2;

            if (w < h)
            {
                drawRect.Height = w;
            }
            else if (w > h)
            {
                drawRect.Width = h;
            }

            if (drawRect.Width < 10)
            {
                drawRect.Width = 10;
            }

            if (drawRect.Height < 10)
            {
                drawRect.Height = 10;
            }

            scaleRectangleF = drawRect;
            indicatorRectangleF = drawRect;
            indicatorRectangleF.Inflate(-20 * drawRatio, -20 * drawRatio);

            knobCenter.X = indicatorRectangleF.Left + indicatorRectangleF.Width * 0.5F;
            knobCenter.Y = indicatorRectangleF.Top + indicatorRectangleF.Height * 0.5F;

            indicatorPosition = GetPositionFromValue(Value);
        }

        protected override void OnClick(EventArgs e)
        {
            Focus();
            Invalidate();
            base.OnClick(e);
        }

        protected void OnKnobChangeValue(KnobEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
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
            Graphics graphics = GDI.Initialize(e, CompositingMode.SourceOver, CompositingQuality.Default, InterpolationMode.Default, PixelOffsetMode.Default, SmoothingMode.AntiAlias, TextRenderingHint.SystemDefault);
            RectangleF rectangleF = new RectangleF(0, 0, Width - borderThickness, Height - borderThickness);

            DrawScale(graphics, rectangleF);
            DrawKnob(graphics, rectangleF);
            DrawKnobIndicator(graphics, indicatorRectangleF, indicatorPosition);

            // BUG: Not showing during runtime
            // if (textVisible)
            // {
            // string value = currentValue.ToString("0.00");
            // Size textAreaSize = GDI.GetTextSize(e.Graphics, value, Font);
            // graphics.DrawString(value, Font, new SolidBrush(Color.Black), Width / 2 - textAreaSize.Width / 2, Height / 2 - textAreaSize.Height / 2);
            // }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ConfigureDimensions();
            Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            var commandResult = true;

            // Specified WM_KEYDOWN enumeration value.
            const int WM_KEYDOWN = 0x0100;

            // Specified WM_SYSKEYDOWN enumeration value.
            const int WM_SYSKEYDOWN = 0x0104;

            float value = Value;

            if (message.Msg == WM_KEYDOWN || message.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        {
                            value += StepValue;
                            if (value <= MaxValue)
                            {
                                Value = value;
                            }

                            break;
                        }

                    case Keys.Down:
                        {
                            value -= StepValue;
                            if (value >= MinValue)
                            {
                                Value = value;
                            }

                            break;
                        }

                    case Keys.PageUp:
                        {
                            if (value < MaxValue)
                            {
                                value += StepValue * 10;
                                Value = value;
                            }

                            break;
                        }

                    case Keys.PageDown:
                        {
                            if (value > MinValue)
                            {
                                value -= StepValue * 10;
                                Value = value;
                            }

                            break;
                        }

                    case Keys.Home:
                        {
                            Value = MinValue;
                            break;
                        }

                    case Keys.End:
                        {
                            Value = MaxValue;
                            break;
                        }

                    default:
                        {
                            commandResult = base.ProcessCmdKey(ref message, keyData);
                            break;
                        }
                }
            }

            return commandResult;
        }

        private void DrawKnob(Graphics graphics, RectangleF rectangleF)
        {
            Color knobColor2 = StepColor(knobColor, 60);

            Size knobSize = new Size((int)rectangleF.Width - 14, (int)rectangleF.Height - 14);
            Point knobPoint = new Point((int)rectangleF.X + (int)rectangleF.Width / 2 - knobSize.Width / 2, (int)rectangleF.Y + (int)rectangleF.Height / 2 - knobSize.Height / 2);
            RectangleF knobRectangleF = new RectangleF(knobPoint, knobSize);

            Size plateSize = new Size(knobSize.Width - 10, knobSize.Height - 10);
            Point platePoint = new Point((int)rectangleF.X + (int)rectangleF.Width / 2 - plateSize.Width / 2, (int)rectangleF.Y + (int)rectangleF.Height / 2 - plateSize.Height / 2);
            RectangleF plateRectangleF = new RectangleF(platePoint, plateSize);

            if (currentValue == 0)
            {
                currentValue = 0.01F;
            }

            LinearGradientBrush gradientBrush = new LinearGradientBrush(knobRectangleF, knobColor, knobColor2, currentValue * 180);

            graphics.FillEllipse(gradientBrush, knobRectangleF);

            graphics.FillEllipse(new SolidBrush(Color.FromArgb(219, 223, 228)), plateRectangleF);

            if (borderVisible)
            {
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddEllipse(knobRectangleF);

                GDI.DrawBorder(graphics, borderPath, borderThickness, borderColor);
            }

            gradientBrush.Dispose();
        }

        private void DrawKnobIndicator(Graphics graphics, RectangleF rectangleF, PointF positionF)
        {
            RectangleF indicatorRectangleF = rectangleF;
            indicatorRectangleF.X = positionF.X - 4;
            indicatorRectangleF.Y = positionF.Y - 4;
            indicatorRectangleF.Width = 8;
            indicatorRectangleF.Height = 8;

            Color indicatorColor = this.indicatorColor;
            Color indicatorColor2 = StepColor(indicatorColor, 60);

            LinearGradientBrush gradientBrush = new LinearGradientBrush(indicatorRectangleF, indicatorColor2, indicatorColor, 45);
            graphics.FillEllipse(gradientBrush, indicatorRectangleF);
            gradientBrush.Dispose();
        }

        private void DrawScale(Graphics graphics, RectangleF rectangleF)
        {
            Color scaleColor2 = StepColor(scaleColor, 60);

            Size scaleSize = new Size((int)rectangleF.Width, (int)rectangleF.Height);
            Point scalePoint = new Point((int)rectangleF.X, (int)rectangleF.Y);
            RectangleF scaleRectangleF = new RectangleF(scalePoint, scaleSize);

            LinearGradientBrush gradientBrush = new LinearGradientBrush(scaleRectangleF, scaleColor2, scaleColor, 90);
            graphics.FillEllipse(gradientBrush, scaleRectangleF);

            if (borderVisible)
            {
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddEllipse(scaleRectangleF);

                GDI.DrawBorderType(graphics, controlState, borderPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
            }

            gradientBrush.Dispose();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            float value = Value;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        value = Value + StepValue;
                        break;
                    }

                case Keys.Down:
                    {
                        value = Value - StepValue;
                        break;
                    }
            }

            if (value < MinValue)
            {
                value = MinValue;
            }

            if (value > MaxValue)
            {
                value = MaxValue;
            }

            Value = value;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (indicatorRectangleF.Contains(e.Location) == false)
            {
                return;
            }

            rotating = true;

            Focus();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (rotating == false)
            {
                return;
            }

            float value = GetValueFromPosition(e.Location);
            if (value != Value)
            {
                Value = value;
                Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            rotating = false;

            if (indicatorRectangleF.Contains(e.Location) == false)
            {
                return;
            }

            float value = GetValueFromPosition(e.Location);
            if (value != Value)
            {
                Value = value;
                Invalidate();
            }
        }

        #endregion

        #region Methods

        public class KnobEventArgs : EventArgs
        {
            #region Variables

            private float value;

            #endregion

            #region Properties

            public float Value
            {
                get
                {
                    return value;
                }

                set
                {
                    this.value = value;
                }
            }

            #endregion
        }

        #endregion
    }
}