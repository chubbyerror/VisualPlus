namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;
    using VisualPlus.Managers;
    using VisualPlus.Properties;
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [Description("The Visual Button")]
    [Designer(ControlManager.FilterProperties.VisualButton)]
    public sealed class VisualButton : ButtonContentBase, IAnimate, IControlStates
    {
        #region Variables

        private bool animation;
        private VFXManager effectsManager;
        private VFXManager hoverEffectsManager;
        private TextImageRelation textImageRelation;
        private Point textPoint = new Point(0, 0);
        private VisualBitmap visualBitmap;

        #endregion

        #region Constructors

        public VisualButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            AutoSize = false;
            BackColor = Color.Transparent;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);
            MinimumSize = new Size(90, 25);
            DoubleBuffered = true;

            animation = true;

            visualBitmap = new VisualBitmap(Resources.Icon, new Size(24, 24))
                {
                    Visible = false
                };
            visualBitmap.Point = new Point(0, (Height / 2) - (visualBitmap.Size.Height / 2));

            textImageRelation = TextImageRelation.Overlay;

            animation = Settings.DefaultValue.Animation;
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

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient DisabledGradient
        {
            get
            {
                return ControlBrushCollection[3];
            }

            set
            {
                ControlBrushCollection[3] = value;
            }
        }

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient EnabledGradient
        {
            get
            {
                return ControlBrushCollection[0];
            }

            set
            {
                ControlBrushCollection[0] = value;
            }
        }

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient HoverGradient
        {
            get
            {
                return ControlBrushCollection[1];
            }

            set
            {
                ControlBrushCollection[1] = value;
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

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient PressedGradient
        {
            get
            {
                return ControlBrushCollection[2];
            }

            set
            {
                ControlBrushCollection[2] = value;
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

        public void ConfigureAnimation()
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

        public void DrawAnimation(Graphics graphics)
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
                        graphics.SetClip(ControlGraphicsPath);
                        graphics.FillEllipse(rippleBrush, new Rectangle(animationSource.X - (rippleSize / 2), animationSource.Y - (rippleSize / 2), rippleSize, rippleSize));
                    }
                }

                graphics.SmoothingMode = SmoothingMode.None;
            }
        }

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
            ConfigureComponents(graphics);

            DrawBackground(e.Graphics);
            VisualBitmap.DrawImage(graphics, visualBitmap.Border, visualBitmap.Point, visualBitmap.Image, visualBitmap.Size, visualBitmap.Visible);
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textPoint);
            DrawAnimation(graphics);
        }

        private void ConfigureComponents(Graphics graphics)
        {
            ControlGraphicsPath = Border.GetBorderShape(ClientRectangle, Border.Type, Border.Rounding);
            visualBitmap.Point = GDI.ApplyTextImageRelation(graphics, textImageRelation, new Rectangle(visualBitmap.Point, visualBitmap.Size), Text, Font, ClientRectangle, true);
            textPoint = GDI.ApplyTextImageRelation(graphics, textImageRelation, new Rectangle(visualBitmap.Point, visualBitmap.Size), Text, Font, ClientRectangle, false);
        }

        private void DrawBackground(Graphics graphics)
        {
            LinearGradientBrush controlGraphicsBrush = GDI.GetControlBrush(graphics, Enabled, MouseState, ControlBrushCollection, ClientRectangle);
            GDI.FillBackground(graphics, ControlGraphicsPath, controlGraphicsBrush);
            Border.DrawBorderStyle(graphics, Border, MouseState, ControlGraphicsPath);
        }

        #endregion
    }
}