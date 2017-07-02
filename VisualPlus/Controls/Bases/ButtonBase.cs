namespace VisualPlus.Controls.Bases
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Properties;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ButtonBase : ControlBase
    {
        #region Variables

        private bool animation;
        private Shape buttonShape;
        private GraphicsPath controlGraphicsPath;
        private VFXManager effectsManager;
        private VFXManager hoverEffectsManager;
        private TextImageRelation textImageRelation;
        private Point textPoint = new Point(0, 0);
        private VisualBitmap visualBitmap;

        #endregion

        #region Constructors

        protected ButtonBase()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            animation = true;

            visualBitmap = new VisualBitmap(Resources.Icon, new Size(24, 24))
                {
                    Visible = false
                };
            visualBitmap.Point = new Point(0, (Height / 2) - (visualBitmap.Size.Height / 2));

            textImageRelation = TextImageRelation.Overlay;

            InitializeTheme();
            ConfigureAnimation();
        }

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.PropertiesCategory.Behavior)]
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
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [TypeConverter(typeof(VisualBitmapConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public VisualBitmap Image
        {
            get
            {
                return visualBitmap;
            }

            set
            {
                visualBitmap = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.TextImageRelation)]
        public TextImageRelation TextImageRelation
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

        #endregion

        #region Events

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode)
            {
                return;
            }

            MouseState = MouseStates.Normal;
            MouseEnter += (sender, args) =>
                {
                    MouseState = MouseStates.Hover;
                    hoverEffectsManager.StartNewAnimation(AnimationDirection.In);
                    Invalidate();
                };
            MouseLeave += (sender, args) =>
                {
                    MouseState = MouseStates.Normal;
                    hoverEffectsManager.StartNewAnimation(AnimationDirection.Out);
                    Invalidate();
                };
            MouseDown += (sender, args) =>
                {
                    if (args.Button == MouseButtons.Left)
                    {
                        MouseState = MouseStates.Down;
                        effectsManager.StartNewAnimation(AnimationDirection.In, args.Location);
                        Invalidate();
                    }
                };
            MouseUp += (sender, args) =>
                {
                    MouseState = MouseStates.Hover;
                    Invalidate();
                };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;

            InitializeTheme();

            ConfigureComponents(graphics);

            DrawBackground(e.Graphics);
            VisualBitmap.DrawImage(graphics, visualBitmap.Border, visualBitmap.Point, visualBitmap.Image, visualBitmap.Size, visualBitmap.Visible);
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textPoint);
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
            visualBitmap.Point = GDI.ApplyTextImageRelation(graphics, textImageRelation, new Rectangle(visualBitmap.Point, visualBitmap.Size), Text, Font, ClientRectangle, true);
            textPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, new Rectangle(visualBitmap.Point, visualBitmap.Size), Text, Font, ClientRectangle, false);
            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, buttonShape.Border.Type, buttonShape.Border.Rounding);
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

            if (Enabled)
            {
                switch (MouseState)
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

            Shape.DrawBackground(graphics, buttonShape, gradientBrush, controlGraphicsPath);
            Border.DrawBorderStyle(graphics, buttonShape.Border, MouseState, controlGraphicsPath);
        }

        private void InitializeTheme()
        {
            if (StyleManager.LockedStyle)
            {
                if (StyleManager.VisualStylesManager != null)
                {
                    // Load style manager settings 
                    IBorder borderStyle = StyleManager.VisualStylesManager.BorderStyle;
                    IControl controlStyle = StyleManager.VisualStylesManager.ControlStyle;

                    animation = StyleManager.VisualStylesManager.Animation;
                    buttonShape.Border.Color = borderStyle.Color;
                    buttonShape.Border.HoverColor = borderStyle.HoverColor;
                    buttonShape.Border.HoverVisible = StyleManager.VisualStylesManager.BorderHoverVisible;
                    buttonShape.Border.Rounding = StyleManager.VisualStylesManager.BorderRounding;
                    buttonShape.Border.Type = StyleManager.VisualStylesManager.BorderType;
                    buttonShape.Border.Thickness = StyleManager.VisualStylesManager.BorderThickness;
                    buttonShape.Border.Visible = StyleManager.VisualStylesManager.BorderVisible;

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
                }
            }
        }

        #endregion
    }
}