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
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [Description("The Visual Button")]
    [Designer(DesignManager.VisualButton)]
    public sealed class VisualButton : Button
    {
        #region Variables

        private readonly Color[] buttonDisabled =
            {
                Settings.DefaultValue.Style.ControlDisabled,
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled
            };

        private readonly Color[] buttonHover =
            {
                Settings.DefaultValue.Style.ButtonHoverColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ButtonHoverColor),
                Settings.DefaultValue.Style.ButtonHoverColor
            };

        private readonly Color[] buttonNormal =
            {
                Settings.DefaultValue.Style.ButtonNormalColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ButtonNormalColor),
                Settings.DefaultValue.Style.ButtonNormalColor
            };

        private readonly Color[] buttonPressed =
            {
                Settings.DefaultValue.Style.ButtonDownColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ButtonDownColor),
                Settings.DefaultValue.Style.ButtonDownColor
            };

        private bool animation = true;
        private Border border = new Border();
        private Gradient buttonDisabledGradient = new Gradient();
        private Gradient buttonHoverGradient = new Gradient();
        private Gradient buttonNormalGradient = new Gradient();
        private Gradient buttonPressedGradient = new Gradient();
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private VFXManager effectsManager;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private LinearGradientBrush gradientBrush;
        private VFXManager hoverEffectsManager;
        private Image icon;
        private bool iconBorder;
        private GraphicsPath iconGraphicsPath;
        private Point iconPoint = new Point(0, 0);
        private Rectangle iconRectangle;
        private Size iconSize = new Size(24, 24);
        private bool moveable = Settings.DefaultValue.Moveable;
        private Rectangle textBoxRectangle;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextImageRelation textImageRelation = TextImageRelation.Overlay;
        private Point textPoint = new Point(0, 0);
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private VisualStylesManager visualStylesManager;

        #endregion

        #region Constructors

        public VisualButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = false;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);
            BackColor = Color.Transparent;

            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);

            float[] gradientPosition = { 0, 1 / 2f, 1 };

            buttonNormalGradient.Colors = buttonNormal;
            buttonNormalGradient.Positions = gradientPosition;

            buttonHoverGradient.Colors = buttonHover;
            buttonHoverGradient.Positions = gradientPosition;

            buttonPressedGradient.Colors = buttonPressed;
            buttonPressedGradient.Positions = gradientPosition;

            buttonDisabledGradient.Colors = buttonDisabled;
            buttonDisabledGradient.Positions = gradientPosition;

            // Setup effects animation
            effectsManager = new VFXManager(false)
                {
                    Increment = 0.03,
                    EffectType = EffectType.EaseOut
                };
            hoverEffectsManager = new VFXManager
                {
                    Increment = 0.07,
                    EffectType = EffectType.Linear
                };

            hoverEffectsManager.OnAnimationProgress += sender => Invalidate();
            effectsManager.OnAnimationProgress += sender => Invalidate();
        }

        public delegate void ControlMovedEventHandler();

        public event ControlMovedEventHandler ControlMoved;

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
                return border;
            }

            set
            {
                border = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
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
        [Category(Localize.Category.Appearance)]
        public Gradient ButtonHover
        {
            get
            {
                return buttonHoverGradient;
            }

            set
            {
                buttonHoverGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient ButtonNormal
        {
            get
            {
                return buttonNormalGradient;
            }

            set
            {
                buttonNormalGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient ButtonPressed
        {
            get
            {
                return buttonPressedGradient;
            }

            set
            {
                buttonPressedGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Icon)]
        public Image Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderVisible)]
        public bool IconBorder
        {
            get
            {
                return iconBorder;
            }

            set
            {
                iconBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.IconSize)]
        public Size IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                iconSize = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.Moveable)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Moveable)]
        public bool Moveable
        {
            get
            {
                return moveable;
            }

            set
            {
                moveable = value;
            }
        }

        [Category(Localize.Category.Design)]
        [Description(Localize.Description.Style)]
        public VisualStylesManager StyleManager
        {
            get
            {
                return visualStylesManager;
            }

            set
            {
                visualStylesManager = value;
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.TextImageRelation)]
        public new TextImageRelation TextImageRelation
        {
            get
            {
                return textImageRelation;
            }

            set
            {
                textImageRelation = value;
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
                    hoverEffectsManager.StartNewAnimation(AnimationDirection.In);
                    Invalidate();
                };
            MouseLeave += (sender, args) =>
                {
                    controlState = ControlState.Normal;
                    hoverEffectsManager.StartNewAnimation(AnimationDirection.Out);
                    Invalidate();
                };
            MouseDown += (sender, args) =>
                {
                    if (args.Button == MouseButtons.Left)
                    {
                        controlState = ControlState.Down;

                        effectsManager.StartNewAnimation(AnimationDirection.In, args.Location);
                        Invalidate();
                    }
                };
            MouseUp += (sender, args) =>
                {
                    controlState = ControlState.Hover;

                    Invalidate();
                };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            controlState = ControlState.Down;
            Invalidate();
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

            this.ToggleMove(moveable);
            if (moveable)
            {
                ControlMoved?.Invoke();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            textPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, iconRectangle, Text, Font, ClientRectangle, false);
            textBoxRectangle.Location = textPoint;
            iconPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, iconRectangle, Text, Font, ClientRectangle, true);
            iconRectangle = new Rectangle(iconPoint, iconSize);

            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(iconRectangle);
            iconGraphicsPath.CloseAllFigures();

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);

            foreColor = Enabled ? foreColor : textDisabledColor;

            Color[] controlTempColor;
            float gradientAngle;
            float[] gradientPositions;

            if (Enabled)
            {
                switch (controlState)
                {
                    case ControlState.Normal:
                        {
                            controlTempColor = buttonNormalGradient.Colors;
                            gradientAngle = buttonNormalGradient.Angle;
                            gradientPositions = buttonNormalGradient.Positions;
                            break;
                        }

                    case ControlState.Hover:
                        {
                            controlTempColor = buttonHoverGradient.Colors;
                            gradientAngle = buttonHoverGradient.Angle;
                            gradientPositions = buttonHoverGradient.Positions;
                            break;
                        }

                    case ControlState.Down:
                        {
                            controlTempColor = buttonPressedGradient.Colors;
                            gradientAngle = buttonPressedGradient.Angle;
                            gradientPositions = buttonPressedGradient.Positions;
                            break;
                        }

                    default:
                        {
                            controlTempColor = buttonNormalGradient.Colors;
                            gradientAngle = buttonNormalGradient.Angle;
                            gradientPositions = buttonNormalGradient.Positions;
                            break;
                        }
                }
            }
            else
            {
                controlTempColor = buttonDisabledGradient.Colors;
                gradientAngle = buttonDisabledGradient.Angle;
                gradientPositions = buttonDisabledGradient.Positions;
            }

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            gradientBrush = GDI.CreateGradientBrush(controlTempColor, gradientPoints, gradientAngle, gradientPositions);

            // Draw button background
            graphics.FillPath(gradientBrush, controlGraphicsPath);

            // Setup button border
            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }

            if (string.IsNullOrEmpty(Text))
            {
                // Center Icon
                iconRectangle.X += 2;
                iconRectangle.Y += 2;
            }

            if (Icon != null)
            {
                // Update point
                iconRectangle.Location = iconPoint;

                // Draw icon border
                if (iconBorder)
                {
                    graphics.DrawPath(new Pen(border.Color), iconGraphicsPath);
                }

                // Draw icon
                graphics.DrawImage(Icon, iconRectangle);
            }

            // Draw string
            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle);

            // Ripple
            if (effectsManager.IsAnimating() && animation)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                for (var i = 0; i < effectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = effectsManager.GetProgress(i);
                    Point animationSource = effectsManager.GetSource(i);

                    using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(101 - (animationValue * 100)), Color.Black)))
                    {
                        var rippleSize = (int)(animationValue * Width * 2);
                        graphics.SetClip(controlGraphicsPath);
                        graphics.FillEllipse(rippleBrush, new Rectangle(animationSource.X - (rippleSize / 2), animationSource.Y - (rippleSize / 2), rippleSize, rippleSize));
                    }
                }

                graphics.SmoothingMode = SmoothingMode.None;
            }
        }

        #endregion
    }
}