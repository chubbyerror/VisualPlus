namespace VisualPlus.Toolkit.Controls
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
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Control))]
    [DefaultEvent("Click")]
    [DefaultProperty("ShapeForm")]
    [Description("The Visual Shape")]
    public sealed class VisualShape : VisualControlBase
    {
        #region Variables

        private Drag _drag;

        private bool animation;
        private Gradient background;
        private GraphicsPath controlGraphicsPath;
        private VFXManager effectsManager;
        private VFXManager hoverEffectsManager;
        private ShapeType shapeType;

        #endregion

        #region Constructors

        public VisualShape()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            UpdateStyles();
            shapeType = ShapeType.Rectangle;
            BackColor = Color.Transparent;
            Size = new Size(100, 100);

            animation = Settings.DefaultValue.Animation;
            background = StyleManager.ControlStatesStyle.ControlEnabled;

            _drag = new Drag(this, Settings.DefaultValue.Moveable);

            ConfigureAnimation();
        }

        public enum ShapeType
        {
            /// <summary>The circle.</summary>
            Circle,

            /// <summary>The rectangle.</summary>
            Rectangle,

            /// <summary>The triangle.</summary>
            Triangle
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
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

        [TypeConverter(typeof(DragConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Drag Drag
        {
            get
            {
                return _drag;
            }

            set
            {
                _drag = value;
            }
        }

        [Category(Localize.PropertiesCategory.Behavior)]
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

        protected override void OnMouseEnter(EventArgs e)
        {
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            ConfigureComponents(graphics);

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
            LinearGradientBrush gradientBrush = Gradient.CreateGradientBrush(background.Colors, gradientPoints, background.Angle, background.Positions);
            controlGraphicsPath = new GraphicsPath();

            switch (shapeType)
            {
                case ShapeType.Circle:
                    {
                        Rectangle circleRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);

                        graphics.FillEllipse(gradientBrush, circleRectangle);
                        controlGraphicsPath.AddEllipse(circleRectangle);

                        break;
                    }

                case ShapeType.Rectangle:
                    {
                        controlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
                        graphics.FillPath(gradientBrush, controlGraphicsPath);

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

                        break;
                    }
            }

            Border.DrawBorderStyle(graphics, ControlBorder, MouseState, controlGraphicsPath);
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

        // TODO: Add rotation
    }
}