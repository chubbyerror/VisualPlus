namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    public enum ShapeType
    {
        /// <summary>The circle.</summary>
        Circle,

        /// <summary>The rectangle.</summary>
        Rectangle,

        /// <summary>The triangle.</summary>
        Triangle
    }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Control))]
    [DefaultEvent("Click")]
    [DefaultProperty("ShapeForm")]
    [Description("The Visual Shape")]
    public sealed class VisualShape : Control
    {
        // TODO: Add rotation
        #region Variables

        private readonly MouseState mouseState;

        private bool animation;
        private Gradient background = new Gradient();
        private Border border = new Border();
        private GraphicsPath controlGraphicsPath;
        private VFXManager effectsManager;
        private VFXManager hoverEffectsManager;
        private ShapeType shapeType;
        private StyleManager styleManager = new StyleManager();

        #endregion

        #region Constructors

        public VisualShape()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();
            shapeType = ShapeType.Rectangle;
            BackColor = Color.Transparent;
            Size = new Size(100, 100);

            mouseState = new MouseState(this);

            DefaultGradient();
            ConfigureStyleManager();
            ConfigureAnimation();
        }

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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Background
        {
            get
            {
                return background;
            }

            set
            {
                background = value;
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

        [Category(Localize.Category.Behavior)]
        [Description("The type of shape.")]
        public ShapeType ShapeForm
        {
            get
            {
                return shapeType;
            }

            set
            {
                shapeType = value;
                Invalidate();
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

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseState.State = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            ConfigureComponents(graphics);

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            DrawBackground(graphics);
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
            var gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush gradientBrush = GDI.CreateGradientBrush(background.Colors, gradientPoints, background.Angle, background.Positions);
            controlGraphicsPath = new GraphicsPath();

            switch (shapeType)
            {
                case ShapeType.Circle:
                    {
                        Rectangle circleRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);

                        graphics.FillEllipse(gradientBrush, circleRectangle);
                        controlGraphicsPath.AddEllipse(circleRectangle);

                        if (border.Visible)
                        {
                            GDI.DrawBorderType(graphics, mouseState.State, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
                        }

                        break;
                    }

                case ShapeType.Rectangle:
                    {
                        controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Type, border.Rounding);
                        graphics.FillPath(gradientBrush, controlGraphicsPath);

                        if (border.Visible)
                        {
                            GDI.DrawBorderType(graphics, mouseState.State, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
                        }

                        break;
                    }

                case ShapeType.Triangle:
                    {
                        Rectangle triangleRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);

                        var points = new Point[3];

                        points[0].X = triangleRectangle.X + (triangleRectangle.Width / 2);
                        points[0].Y = triangleRectangle.Y;

                        points[1].X = triangleRectangle.X;
                        points[1].Y = triangleRectangle.Y + triangleRectangle.Height;

                        points[2].X = triangleRectangle.X + triangleRectangle.Width;
                        points[2].Y = triangleRectangle.Y + triangleRectangle.Height;

                        graphics.FillPolygon(gradientBrush, points);

                        controlGraphicsPath.AddPolygon(points);

                        if (border.Visible)
                        {
                            GDI.DrawBorderType(graphics, mouseState.State, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
                        }

                        break;
                    }
            }
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
                border.Color = borderStyle.Color;
                border.HoverColor = borderStyle.HoverColor;
                border.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                border.Rounding = styleManager.VisualStylesManager.BorderRounding;
                border.Type = styleManager.VisualStylesManager.BorderType;
                border.Visible = styleManager.VisualStylesManager.BorderVisible;

                background.Colors = controlStyle.ControlEnabled.Colors;
                background.Positions = controlStyle.ControlEnabled.Positions;
            }
            else
            {
                // Load default settings
                animation = Settings.DefaultValue.Animation;

                border = new Border();
            }
        }

        private void DefaultGradient()
        {
            background.Colors = Settings.DefaultValue.Control.ControlEnabled.Colors;
            background.Positions = Settings.DefaultValue.Control.ControlEnabled.Positions;
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
        }

        #endregion
    }
}