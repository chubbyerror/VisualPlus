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

        private bool animation = true;

        private Shape buttonShape = new Shape();
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private VFXManager effectsManager;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private LinearGradientBrush gradientBrush;
        private VFXManager hoverEffectsManager;
        private GraphicsPath iconGraphicsPath;
        private Point imagePoint = new Point(0, 0);
        private Rectangle imageRectangle;
        private Size imageSize = new Size(24, 24);
        private bool imageVisible;
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
            BackColor = Color.Transparent;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);

            DefaultGradient();
            ConfigureAnimation();
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

        [TypeConverter(typeof(ShapeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Shape Button
        {
            get
            {
                return buttonShape;
            }

            set
            {
                buttonShape = value;
                Invalidate();
            }
        }

        public new Color ForeColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                base.ForeColor = value;
                foreColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.IconSize)]
        public Size ImageSize
        {
            get
            {
                return imageSize;
            }

            set
            {
                imageSize = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ImageVisible
        {
            get
            {
                return imageVisible;
            }

            set
            {
                imageVisible = value;
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
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            textPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, imageRectangle, Text, Font, ClientRectangle, false);
            textBoxRectangle.Location = textPoint;
            imagePoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, imageRectangle, Text, Font, ClientRectangle, true);
            imageRectangle = new Rectangle(imagePoint, imageSize);

            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(imageRectangle);
            iconGraphicsPath.CloseAllFigures();

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, buttonShape.Border.Shape, buttonShape.Border.Rounding);

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
                            controlTempColor = buttonShape.EnabledGradient.Colors;
                            gradientAngle = buttonShape.EnabledGradient.Angle;
                            gradientPositions = buttonShape.EnabledGradient.Positions;
                            break;
                        }

                    case ControlState.Hover:
                        {
                            controlTempColor = buttonShape.HoverGradient.Colors;
                            gradientAngle = buttonShape.HoverGradient.Angle;
                            gradientPositions = buttonShape.HoverGradient.Positions;
                            break;
                        }

                    case ControlState.Down:
                        {
                            controlTempColor = buttonShape.PressedGradient.Colors;
                            gradientAngle = buttonShape.PressedGradient.Angle;
                            gradientPositions = buttonShape.PressedGradient.Positions;
                            break;
                        }

                    default:
                        {
                            controlTempColor = buttonShape.EnabledGradient.Colors;
                            gradientAngle = buttonShape.EnabledGradient.Angle;
                            gradientPositions = buttonShape.EnabledGradient.Positions;
                            break;
                        }
                }
            }
            else
            {
                controlTempColor = buttonShape.DisabledGradient.Colors;
                gradientAngle = buttonShape.DisabledGradient.Angle;
                gradientPositions = buttonShape.DisabledGradient.Positions;
            }

            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            gradientBrush = GDI.CreateGradientBrush(controlTempColor, gradientPoints, gradientAngle, gradientPositions);
            graphics.FillPath(gradientBrush, controlGraphicsPath);

            if (buttonShape.Border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, buttonShape.Border.Thickness, buttonShape.Border.Color, buttonShape.Border.HoverColor, buttonShape.Border.HoverVisible);
            }

            DrawImage(graphics);

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle);

            DrawAnimation(graphics);
        }

        private void ConfigureAnimation()
        {
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

        private void DefaultGradient()
        {
            Color[] buttonDisabled =
                {
                    Settings.DefaultValue.Style.ControlDisabled,
                    ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                    Settings.DefaultValue.Style.ControlDisabled
                };

            Color[] buttonHover =
                {
                    Settings.DefaultValue.Style.ButtonHoverColor,
                    ControlPaint.Light(Settings.DefaultValue.Style.ButtonHoverColor),
                    Settings.DefaultValue.Style.ButtonHoverColor
                };

            Color[] buttonNormal =
                {
                    Settings.DefaultValue.Style.ButtonNormalColor,
                    ControlPaint.Light(Settings.DefaultValue.Style.ButtonNormalColor),
                    Settings.DefaultValue.Style.ButtonNormalColor
                };

            Color[] buttonPressed =
                {
                    Settings.DefaultValue.Style.ButtonDownColor,
                    ControlPaint.Light(Settings.DefaultValue.Style.ButtonDownColor),
                    Settings.DefaultValue.Style.ButtonDownColor
                };

            float[] gradientPosition = { 0, 1 / 2f, 1 };

            buttonShape.EnabledGradient.Colors = buttonNormal;
            buttonShape.EnabledGradient.Positions = gradientPosition;

            buttonShape.DisabledGradient.Colors = buttonDisabled;
            buttonShape.DisabledGradient.Positions = gradientPosition;

            buttonShape.HoverGradient.Colors = buttonHover;
            buttonShape.HoverGradient.Positions = gradientPosition;

            buttonShape.PressedGradient.Colors = buttonPressed;
            buttonShape.PressedGradient.Positions = gradientPosition;
        }

        private void DrawAnimation(Graphics graphics)
        {
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

        private void DrawImage(Graphics graphics)
        {
            if (Image != null)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    // Center Image
                    imageRectangle.X += 2;
                    imageRectangle.Y += 2;
                }

                imageRectangle.Location = imagePoint;

                if (imageVisible)
                {
                    graphics.DrawImage(Image, imageRectangle);
                }
            }
        }

        #endregion
    }
}