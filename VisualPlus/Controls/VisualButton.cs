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
    using VisualPlus.Properties;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [Description("The Visual Button")]
    [Designer(ControlManager.FilterProperties.VisualButton)]
    public sealed class VisualButton : Button
    {
        #region Variables

        private readonly MouseState mouseState;
        private bool animation;
        private Shape buttonShape;
        private GraphicsPath controlGraphicsPath;
        private VFXManager effectsManager;
        private Color foreColor;
        private VFXManager hoverEffectsManager;
        private bool moveable = Settings.DefaultValue.Moveable;
        private StyleManager styleManager = new StyleManager();
       // private Rectangle textBoxRectangle;
        private Color textDisabledColor;
        private TextImageRelation textImageRelation = TextImageRelation.Overlay;
        private Point textPoint = new Point(0, 0);
        private TextRenderingHint textRendererHint;
        private VisualBitmap vsImage;

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

            vsImage = new VisualBitmap(Resources.Icon, new Size(24, 24))
                {
                    Visible = false
                };

            vsImage.Point = new Point(0, (Height / 2) - (vsImage.Size.Height / 2));

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = false;
            BackColor = Color.Transparent;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);
            MinimumSize = new Size(90, 25);

            ConfigureStyleManager();
            ConfigureAnimation();
        }

        public delegate void ControlMovedEventHandler();

        public event ControlMovedEventHandler ControlMoved;

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Common.Animation)]
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

        [TypeConverter(typeof(VisualBitmapConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public new VisualBitmap Image
        {
            get
            {
                return vsImage;
            }

            set
            {
                vsImage = value;
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
        [Description(Localize.Description.Common.Toggle)]
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

        [TypeConverter(typeof(StyleManagerConverter))]
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
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Common.TextImageRelation)]
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
        [Description(Localize.Description.Strings.TextRenderingHint)]
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
            VisualBitmap.DrawImage(graphics, vsImage.Border, vsImage.Point, vsImage.Image, vsImage.Size, vsImage.Visible);

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textPoint);

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

        private void ConfigureComponents(Graphics graphics)
        {
            vsImage.Point = GDI.ApplyTextImageRelation(graphics, textImageRelation, new Rectangle(vsImage.Point, vsImage.Size), Text, Font, ClientRectangle, true);
            textPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, new Rectangle(vsImage.Point, vsImage.Size), Text, Font, ClientRectangle, false);
            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, buttonShape.Border.Type, buttonShape.Border.Rounding);
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
                buttonShape.Border.Type = styleManager.VisualStylesManager.BorderType;
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

                buttonShape = new Shape(ClientRectangle);

                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = Settings.DefaultValue.DefaultFont;
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
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
            Gradient tempGradient;

            foreColor = Enabled ? foreColor : textDisabledColor;

            if (Enabled)
            {
                switch (mouseState.State)
                {
                    case MouseStates.Normal:
                        {
                            tempGradient = buttonShape.EnabledGradient;
                            break;
                        }

                    case MouseStates.Hover:
                        {
                            tempGradient = buttonShape.HoverGradient;
                            break;
                        }

                    case MouseStates.Down:
                        {
                            tempGradient = buttonShape.PressedGradient;
                            break;
                        }

                    default:
                        {
                            tempGradient = buttonShape.EnabledGradient;
                            break;
                        }
                }
            }
            else
            {
                tempGradient = buttonShape.DisabledGradient;
            }

            var gradientPoints = GDI.GetGradientPoints(ClientRectangle);
            LinearGradientBrush gradientBrush = Gradient.CreateGradientBrush(tempGradient.Colors, gradientPoints, tempGradient.Angle, tempGradient.Positions);

            graphics.FillPath(gradientBrush, controlGraphicsPath);

            Border.DrawBorderStyle(graphics, buttonShape.Border, mouseState.State, controlGraphicsPath);
        }

        #endregion
    }
}