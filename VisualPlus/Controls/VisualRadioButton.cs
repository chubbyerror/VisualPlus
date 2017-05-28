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
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RadioButton))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual RadioButton")]
    [Designer(DesignManager.VisualRadioButton)]
    public sealed class VisualRadioButton : RadioButton
    {
        #region Variables

        private const int Spacing = 2;

        private readonly Color[] boxColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3)),
                Settings.DefaultValue.Style.BackgroundColor(3)
            };

        private readonly Color[] boxDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TextDisabled),
                Settings.DefaultValue.Style.TextDisabled
            };

        private readonly Color[] checkColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.StyleColor),
                Settings.DefaultValue.Style.StyleColor
            };

        private readonly Color[] checkDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TextDisabled),
                Settings.DefaultValue.Style.TextDisabled
            };

        private bool animation = true;
        private Border boxBorder = new Border();
        private Gradient boxDisabledGradient = new Gradient();
        private Gradient boxGradient = new Gradient();
        private GraphicsPath boxGraphicsPath;
        private Point boxLocation = new Point(2, 2);
        private Rectangle boxRectangle;
        private Size boxSize = new Size(10, 10);
        private Gradient checkDisabledGradient = new Gradient();
        private Gradient checkGradient = new Gradient();
        private Point checkLocation = new Point(0, 0);
        private Size checkSize = new Size(6, 6);
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private VFXManager rippleEffectsManager;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

        #endregion

        #region Constructors

        public VisualRadioButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Width = 132;
            UpdateStyles();
            Cursor = Cursors.Hand;

            // Setup effects animation
            VFXManager effectsManager = new VFXManager
                {
                    Increment = 0.05,
                    EffectType = EffectType.EaseInOut
                };
            rippleEffectsManager = new VFXManager(false)
                {
                    Increment = 0.10,
                    SecondaryIncrement = 0.08,
                    EffectType = EffectType.Linear
                };

            effectsManager.OnAnimationProgress += sender => Invalidate();
            rippleEffectsManager.OnAnimationProgress += sender => Invalidate();

            CheckedChanged += (sender, args) =>
                {
                    effectsManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
                };

            Animation = true;

            float[] gradientPosition = { 0, 1 };

            boxGradient.Colors = boxColor;
            boxGradient.Positions = gradientPosition;

            boxDisabledGradient.Colors = boxDisabledColor;
            boxDisabledGradient.Positions = gradientPosition;

            checkGradient.Colors = checkColor;
            checkGradient.Positions = gradientPosition;

            checkDisabledGradient.Colors = checkDisabledColor;
            checkDisabledGradient.Positions = gradientPosition;
        }

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Animation)]
        public bool Animation
        {
            get
            {
                return animation;
            }

            set
            {
                animation = value;
                AutoSize = AutoSize; // Make AutoSize directly set the bounds.

                if (value)
                {
                    Margin = new Padding(0);
                }

                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Border
        {
            get
            {
                return boxBorder;
            }

            set
            {
                boxBorder = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Box
        {
            get
            {
                return boxGradient;
            }

            set
            {
                boxGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient BoxDisabled
        {
            get
            {
                return boxDisabledGradient;
            }

            set
            {
                boxDisabledGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentSize)]
        public Size BoxSize
        {
            get
            {
                return boxSize;
            }

            set
            {
                boxSize = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Check
        {
            get
            {
                return checkGradient;
            }

            set
            {
                checkGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient CheckDisabled
        {
            get
            {
                return checkDisabledGradient;
            }

            set
            {
                checkDisabledGradient = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

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

        #endregion

        #region Events

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (DesignMode)
            {
                return;
            }

            controlState = ControlState.Normal;
            MouseEnter += (sender, args) =>
                {
                    controlState = ControlState.Hover;
                };
            MouseLeave += (sender, args) =>
                {
                    MouseLocation = new Point(-1, -1);
                    controlState = ControlState.Normal;
                };
            MouseDown += (sender, args) =>
                {
                    controlState = ControlState.Down;

                    if (animation && (args.Button == MouseButtons.Left) && GDI.IsMouseInBounds(MouseLocation, boxRectangle))
                    {
                        rippleEffectsManager.SecondaryIncrement = 0;
                        rippleEffectsManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                    }
                };
            MouseUp += (sender, args) =>
                {
                    controlState = ControlState.Hover;
                    rippleEffectsManager.SecondaryIncrement = 0.08;
                };
            MouseMove += (sender, args) =>
                {
                    MouseLocation = args.Location;
                    Cursor = GDI.IsMouseInBounds(MouseLocation, boxRectangle) ? Cursors.Hand : Cursors.Default;
                };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Checked)
            {
                Checked = true;
            }

            base.OnMouseDown(e);
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
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;
            graphics.TextRenderingHint = textRendererHint;

            boxGraphicsPath = new GraphicsPath();
            boxGraphicsPath.AddEllipse(boxLocation.X, boxLocation.Y, boxSize.Width, boxSize.Height);
            boxGraphicsPath.CloseAllFigures();

            // Centers the check location according to the checkbox
            boxRectangle = new Rectangle(boxLocation.X, boxLocation.Y, boxSize.Width, boxSize.Height);

            Rectangle check = new Rectangle(boxRectangle.X, boxRectangle.Y, checkSize.Width / 2, checkSize.Height / 2);
            check = check.AlignCenterX(boxRectangle);
            check = check.AlignCenterY(boxRectangle);
            checkLocation = new Point(check.X, check.Y);

            foreColor = Enabled ? foreColor : textDisabledColor;
            Gradient checkTemp = Enabled ? checkGradient : checkDisabledGradient;
            Gradient boxTemp = Enabled ? boxGradient : boxDisabledGradient;
            var gradientPoints = new[] { new Point { X = boxRectangle.Width, Y = 0 }, new Point { X = boxRectangle.Width, Y = boxRectangle.Height } };

            LinearGradientBrush boxGradientBrush = GDI.CreateGradientBrush(boxTemp.Colors, gradientPoints, boxTemp.Angle, boxTemp.Positions);
            graphics.FillPath(boxGradientBrush, boxGraphicsPath);

            if (boxBorder.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, boxGraphicsPath, boxBorder.Thickness, boxBorder.Color, boxBorder.HoverColor, boxBorder.HoverVisible);
            }

            if (animation && rippleEffectsManager.IsAnimating())
            {
                for (var i = 0; i < rippleEffectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = rippleEffectsManager.GetProgress(i);

                    Point animationSource = new Point(boxRectangle.X + (boxRectangle.Width / 2), boxRectangle.Y + (boxRectangle.Height / 2));
                    SolidBrush animationBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), (bool)rippleEffectsManager.GetData(i)[0] ? Color.Black : checkColor[0]));

                    int height = boxRectangle.Height;
                    int size = rippleEffectsManager.GetDirection(i) == AnimationDirection.InOutIn ? (int)(height * (0.8d + (0.2d * animationValue))) : height;

                    using (GraphicsPath path = GDI.DrawRoundedRectangle(
                        animationSource.X - (size / 2),
                        animationSource.Y - (size / 2),
                        size,
                        size,
                        size / 2))
                    {
                        graphics.FillPath(animationBrush, path);
                    }

                    animationBrush.Dispose();
                }
            }

            // Draw an ellipse inside the body
            if (Checked)
            {
                LinearGradientBrush checkGradientBrush = GDI.CreateGradientBrush(checkTemp.Colors, gradientPoints, checkTemp.Angle, checkTemp.Positions);
                graphics.FillEllipse(checkGradientBrush, new Rectangle(checkLocation, checkSize));
            }

            // Draw the string specified in 'Text' property
            Point textPoint = new Point(boxLocation.X + boxSize.Width + Spacing, (boxSize.Height / 2) - ((int)Font.Size / 2));

            StringFormat stringFormat = new StringFormat();

            // stringFormat.Alignment = StringAlignment.Center;
            // stringFormat.LineAlignment = StringAlignment.Center;
            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textPoint, stringFormat);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion
    }
}