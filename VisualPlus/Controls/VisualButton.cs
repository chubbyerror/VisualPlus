namespace VisualPlus.Controls
{
    #region Namespace

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
    using VisualPlus.Styles;

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

        private readonly MouseState mouseState;

        private bool animation;
        private Shape buttonShape = new Shape();
        private GraphicsPath controlGraphicsPath;
        private VFXManager effectsManager;
        private Color foreColor;
        private LinearGradientBrush gradientBrush;
        private VFXManager hoverEffectsManager;
        private GraphicsPath iconGraphicsPath;
        private Point imagePoint = new Point(0, 0);
        private Rectangle imageRectangle;
        private Size imageSize = new Size(24, 24);
        private bool imageVisible;
        private bool moveable = Settings.DefaultValue.Moveable;
        private StyleManager styleManager = new StyleManager();
        private Rectangle textBoxRectangle;
        private Color textDisabledColor;
        private TextImageRelation textImageRelation = TextImageRelation.Overlay;
        private Point textPoint = new Point(0, 0);
        private TextRenderingHint textRendererHint;

        #endregion

        #region Constructors

        public VisualButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            mouseState = new MouseState(this);

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = false;
            BackColor = Color.Transparent;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);

            DefaultGradient();
            ConfigureStyleManager();
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

        [Category(Localize.Category.Appearance)]
        public MouseStates MouseState
        {
            get
            {
                return mouseState.State;
            }

            set
            {
                mouseState.State = value;
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

        [TypeConverter(typeof(VisualStyleManagerConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public StyleManager StyleManager
        {
            get
            {
                return styleManager;
            }

            set
            {
                styleManager = value;
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

            mouseState.State = MouseStates.Normal;
            MouseEnter += (sender, args) =>
                {
                    mouseState.State = MouseStates.Hover;
                    hoverEffectsManager.StartNewAnimation(AnimationDirection.In);
                    Invalidate();
                };
            MouseLeave += (sender, args) =>
                {
                    mouseState.State = MouseStates.Normal;
                    hoverEffectsManager.StartNewAnimation(AnimationDirection.Out);
                    Invalidate();
                };
            MouseDown += (sender, args) =>
                {
                    if (args.Button == MouseButtons.Left)
                    {
                        mouseState.State = MouseStates.Down;
                        effectsManager.StartNewAnimation(AnimationDirection.In, args.Location);
                        Invalidate();
                    }
                };
            MouseUp += (sender, args) =>
                {
                    mouseState.State = MouseStates.Hover;
                    Invalidate();
                };
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            ConfigureComponents(graphics);

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            DrawBackground(graphics);
            DrawImage(graphics);

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle);

            DrawAnimation(graphics);

            Text = MouseState.ToString();
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

        private void ConfigureComponents(Graphics graphics)
        {
            textPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, imageRectangle, Text, Font, ClientRectangle, false);
            textBoxRectangle.Location = textPoint;
            imagePoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, imageRectangle, Text, Font, ClientRectangle, true);
            imageRectangle = new Rectangle(imagePoint, imageSize);
            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(imageRectangle);
            iconGraphicsPath.CloseAllFigures();
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, buttonShape.Border.Shape, buttonShape.Border.Rounding);
        }

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.BorderStyle;
                IControl controlStyle = styleManager.VisualStylesManager.ControlStyle;
                IFont fontStyle = styleManager.VisualStylesManager.FontStyle;

                animation = styleManager.VisualStylesManager.Animation;
                buttonShape.Border.Color = borderStyle.Color;
                buttonShape.Border.HoverColor = borderStyle.HoverColor;
                buttonShape.Border.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                buttonShape.Border.Rounding = styleManager.VisualStylesManager.BorderRounding;
                buttonShape.Border.Shape = styleManager.VisualStylesManager.BorderShape;
                buttonShape.Border.Thickness = styleManager.VisualStylesManager.BorderThickness;
                buttonShape.Border.Visible = styleManager.VisualStylesManager.BorderVisible;
                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;

                buttonShape.EnabledGradient.Colors = controlStyle.ControlEnabled.Colors;
                buttonShape.EnabledGradient.Positions = controlStyle.ControlEnabled.Positions;
                buttonShape.DisabledGradient.Colors = controlStyle.ControlDisabled.Colors;
                buttonShape.DisabledGradient.Positions = controlStyle.ControlDisabled.Positions;
                buttonShape.HoverGradient.Colors = controlStyle.ControlHover.Colors;
                buttonShape.HoverGradient.Positions = controlStyle.ControlHover.Positions;
                buttonShape.PressedGradient.Colors = controlStyle.ControlPressed.Colors;
                buttonShape.PressedGradient.Positions = controlStyle.ControlPressed.Positions;
            }
            else
            {
                // Load default settings
                animation = Settings.DefaultValue.Animation;
                buttonShape.Border.HoverVisible = Settings.DefaultValue.BorderHoverVisible;
                buttonShape.Border.Rounding = Settings.DefaultValue.BorderRounding;
                buttonShape.Border.Shape = Settings.DefaultValue.BorderShape;
                buttonShape.Border.Thickness = Settings.DefaultValue.BorderThickness;
                buttonShape.Border.Visible = Settings.DefaultValue.BorderVisible;
                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
        }

        private void DefaultGradient()
        {
            buttonShape.EnabledGradient.Colors = Settings.DefaultValue.Control.ControlEnabled.Colors;
            buttonShape.EnabledGradient.Positions = Settings.DefaultValue.Control.ControlEnabled.Positions;

            buttonShape.DisabledGradient.Colors = Settings.DefaultValue.Control.ControlDisabled.Colors;
            buttonShape.DisabledGradient.Positions = Settings.DefaultValue.Control.ControlDisabled.Positions;

            buttonShape.HoverGradient.Colors = Settings.DefaultValue.Control.ControlHover.Colors;
            buttonShape.HoverGradient.Positions = Settings.DefaultValue.Control.ControlHover.Positions;

            buttonShape.PressedGradient.Colors = Settings.DefaultValue.Control.ControlPressed.Colors;
            buttonShape.PressedGradient.Positions = Settings.DefaultValue.Control.ControlPressed.Positions;
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

        private void DrawBackground(Graphics graphics)
        {
            Color[] controlTempColor;
            float gradientAngle;
            float[] gradientPositions;
            foreColor = Enabled ? foreColor : textDisabledColor;

            if (Enabled)
            {
                switch (mouseState.State)
                {
                    case MouseStates.Normal:
                        {
                            controlTempColor = buttonShape.EnabledGradient.Colors;
                            gradientAngle = buttonShape.EnabledGradient.Angle;
                            gradientPositions = buttonShape.EnabledGradient.Positions;
                            break;
                        }

                    case MouseStates.Hover:
                        {
                            controlTempColor = buttonShape.HoverGradient.Colors;
                            gradientAngle = buttonShape.HoverGradient.Angle;
                            gradientPositions = buttonShape.HoverGradient.Positions;
                            break;
                        }

                    case MouseStates.Down:
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
                GDI.DrawBorderType(graphics, mouseState.State, controlGraphicsPath, buttonShape.Border.Thickness, buttonShape.Border.Color, buttonShape.Border.HoverColor, buttonShape.Border.HoverVisible);
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