namespace VisualPlus.Toolkit.VisualBase
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
    using VisualPlus.Framework.Interface;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ToggleButtonBase : VisualControlBase, IAnimate, IControlStates
    {
        #region Variables

        private bool animation;

        private Rectangle box;
        private int boxSpacing = 2;
        private Checkmark checkMark;
        private Point mouseLocation;
        private VFXManager rippleEffectsManager;

        #endregion

        #region Constructors

        protected ToggleButtonBase()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            box = new Rectangle(0, 0, 14, 14);

            animation = Settings.DefaultValue.Animation;
            checkMark = new Checkmark(ClientRectangle);
            ConfigureAnimation();
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the toggle has been changed on the control.")]
        public event EventHandler ToggleChanged;

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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool Toggle { get; set; }

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
                };
            MouseLeave += (sender, args) =>
                {
                    mouseLocation = new Point(-1, -1);
                    MouseState = MouseStates.Normal;
                };
            MouseDown += (sender, args) =>
                {
                    MouseState = MouseStates.Down;

                    if (animation && (args.Button == MouseButtons.Left) && GDI.IsMouseInBounds(mouseLocation, box))
                    {
                        rippleEffectsManager.SecondaryIncrement = 0;
                        rippleEffectsManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Toggle });
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
                    Cursor = GDI.IsMouseInBounds(mouseLocation, box) ? Cursors.Hand : Cursors.Default;
                };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            MouseState = MouseStates.Down;
            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            box = new Rectangle(new Point(0, (ClientRectangle.Height / 2) - (box.Height / 2)), box.Size);
            GraphicsPath boxPath = Border.GetBorderShape(box, Border.Type, Border.Rounding);
            LinearGradientBrush controlGraphicsBrush = GDI.GetControlBrush(graphics, Enabled, MouseState, ControlBrushCollection, ClientRectangle);
            GDI.FillBackground(graphics, boxPath, controlGraphicsBrush);

            if (Toggle)
            {
                graphics.SetClip(boxPath);
                Checkmark.DrawCheckmark(graphics, checkMark, box, Enabled, TextRenderingHint);
                graphics.ResetClip();
            }

            Border.DrawBorderStyle(graphics, Border, MouseState, boxPath);

            DrawText(graphics);
            DrawAnimation(graphics);
        }

        protected virtual void OnToggleChanged(EventArgs e)
        {
            ToggleChanged?.Invoke(this, e);
        }

        public void ConfigureAnimation()
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
            effectsManager.StartNewAnimation(Toggle ? AnimationDirection.In : AnimationDirection.Out);
        }

        public void DrawAnimation(Graphics graphics)
        {
            if (animation && rippleEffectsManager.IsAnimating())
            {
                for (var i = 0; i < rippleEffectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = rippleEffectsManager.GetProgress(i);

                    Point animationSource = new Point(box.X + (box.Width / 2), box.Y + (box.Height / 2));
                    SolidBrush animationBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), (bool)rippleEffectsManager.GetData(i)[0] ? Color.Black : checkMark.EnabledGradient.Colors[0]));

                    int height = box.Height;
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
            Point textPoint = new Point(box.X + box.Width + boxSpacing, ClientRectangle.Height / 2);
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textPoint, stringFormat);
        }

        #endregion
    }
}