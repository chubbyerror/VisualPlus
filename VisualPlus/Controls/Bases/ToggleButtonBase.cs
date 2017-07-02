namespace VisualPlus.Controls.Bases
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ToggleButtonBase : ControlBase
    {
        #region Variables

        private bool _checked;
        private bool animation;
        private Shape boxShape;
        private int boxSpacing = 2;
        private Checkmark checkMark;
        private Point mouseLocation;
        private VFXManager rippleEffectsManager;

        #endregion

        #region Constructors

        protected ToggleButtonBase()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            InitializeTheme();
            ConfigureAnimation();
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description(Localize.Description.Checkmark.Checked)]
        public event EventHandler CheckedChanged;

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
        public Shape Box
        {
            get
            {
                return boxShape;
            }

            set
            {
                boxShape = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Spacing)]
        public int BoxSpacing
        {
            get
            {
                return boxSpacing;
            }

            set
            {
                boxSpacing = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Checkmark.Checked)]
        public bool Checked
        {
            get
            {
                return _checked;
            }

            set
            {
                if (_checked != value)
                {
                    // Store new values
                    _checked = value;

                    // Generate events
                    OnCheckedChanged(EventArgs.Empty);

                    // Repaint
                    Invalidate();
                }
            }
        }

        [TypeConverter(typeof(CheckMarkConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Checkmark CheckMark
        {
            get
            {
                return checkMark;
            }

            set
            {
                checkMark = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        public void InitializeTheme()
        {
            if (StyleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = StyleManager.VisualStylesManager.BorderStyle;
                IControl controlStyle = StyleManager.VisualStylesManager.ControlStyle;

                animation = StyleManager.VisualStylesManager.Animation;

                boxShape.Border.Color = borderStyle.Color;
                boxShape.Border.HoverColor = borderStyle.HoverColor;
                boxShape.Border.HoverVisible = StyleManager.VisualStylesManager.BorderHoverVisible;
                boxShape.Border.Rounding = StyleManager.VisualStylesManager.BorderRounding;
                boxShape.Border.Type = StyleManager.VisualStylesManager.BorderType;
                boxShape.Border.Thickness = StyleManager.VisualStylesManager.BorderThickness;
                boxShape.Border.Visible = StyleManager.VisualStylesManager.BorderVisible;

                boxShape.EnabledGradient.Colors = controlStyle.ControlEnabled.Colors;
                boxShape.EnabledGradient.Positions = controlStyle.ControlEnabled.Positions;
                boxShape.DisabledGradient.Colors = controlStyle.ControlDisabled.Colors;
                boxShape.DisabledGradient.Positions = controlStyle.ControlDisabled.Positions;
                boxShape.HoverGradient.Colors = controlStyle.ControlHover.Colors;
                boxShape.HoverGradient.Positions = controlStyle.ControlHover.Positions;
                boxShape.PressedGradient.Colors = controlStyle.ControlPressed.Colors;
                boxShape.PressedGradient.Positions = controlStyle.ControlPressed.Positions;
            }
            else
            {
                animation = Settings.DefaultValue.Animation;
                boxShape = new Shape(ClientRectangle);
                checkMark = new Checkmark(ClientRectangle);
            }
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
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
                };
            MouseLeave += (sender, args) =>
                {
                    mouseLocation = new Point(-1, -1);
                    MouseState = MouseStates.Normal;
                };
            MouseDown += (sender, args) =>
                {
                    MouseState = MouseStates.Down;

                    if (animation && (args.Button == MouseButtons.Left) && GDI.IsMouseInBounds(mouseLocation, boxShape.Rectangle))
                    {
                        rippleEffectsManager.SecondaryIncrement = 0;
                        rippleEffectsManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                    }
                };
            MouseUp += (sender, args) =>
                {
                    MouseState = MouseStates.Hover;
                    rippleEffectsManager.SecondaryIncrement = 0.08;
                };
            MouseMove += (sender, args) =>
                {
                    mouseLocation = args.Location;
                    Cursor = GDI.IsMouseInBounds(mouseLocation, boxShape.Rectangle) ? Cursors.Hand : Cursors.Default;
                };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            if (StyleManager.LockedStyle)
            {
                InitializeTheme();
            }
            
            boxShape.Rectangle = new Rectangle(new Point(0, (ClientRectangle.Height / 2) - (boxShape.Rectangle.Height / 2)), boxShape.Rectangle.Size);
            GraphicsPath boxPath = Border.GetBorderShape(boxShape.Rectangle, boxShape.Border.Type, boxShape.Border.Rounding);

            Shape.DrawBackground(graphics, boxShape, ClientRectangle, Enabled);

            if (Checked)
            {
                Checkmark.DrawCheckmark(graphics, checkMark, boxShape, Enabled, TextRenderingHint);
            }

            Border.DrawBorderStyle(graphics, boxShape.Border, MouseState, boxPath);

            DrawText(graphics);
            DrawAnimation(graphics);
        }

        private void ConfigureAnimation()
        {
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
            effectsManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
        }

        private void DrawAnimation(Graphics graphics)
        {
            if (animation && rippleEffectsManager.IsAnimating())
            {
                for (var i = 0; i < rippleEffectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = rippleEffectsManager.GetProgress(i);

                    Point animationSource = new Point(boxShape.Rectangle.X + (boxShape.Rectangle.Width / 2), boxShape.Rectangle.Y + (boxShape.Rectangle.Height / 2));
                    SolidBrush animationBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), (bool)rippleEffectsManager.GetData(i)[0] ? Color.Black : checkMark.EnabledGradient.Colors[0]));

                    int height = boxShape.Rectangle.Height;
                    int size = rippleEffectsManager.GetDirection(i) == AnimationDirection.InOutIn ? (int)(height * (0.8d + (0.2d * animationValue))) : height;

                    GraphicsPath path = GDI.DrawRoundedRectangle(animationSource.X - (size / 2), animationSource.Y - (size / 2), size, size, size / 2);
                    graphics.FillPath(animationBrush, path);
                    animationBrush.Dispose();
                }
            }
        }

        private void DrawText(Graphics graphics)
        {
            StringFormat stringFormat = new StringFormat { LineAlignment = StringAlignment.Center };
            Point textPoint = new Point(boxShape.Rectangle.X + boxShape.Rectangle.Width + boxSpacing, ClientRectangle.Height / 2);
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textPoint, stringFormat);
        }

        #endregion
    }
}