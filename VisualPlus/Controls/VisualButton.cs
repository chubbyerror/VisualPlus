namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    /// <summary>The visual Button.</summary>
    [ToolboxBitmap(typeof(Button)), Designer(VSDesignerBinding.VisualButton)]
    public class VisualButton : Button
    {
        #region  ${0} Variables

        private bool animation = true;

        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);

        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);

        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;

        private BorderShape borderShape = StylesManager.DefaultValue.BorderShape;

        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Color buttonDisabled = StylesManager.DefaultValue.Style.ControlDisabled;
        private Color buttonHover = ControlPaint.Light(StylesManager.DefaultValue.Style.ButtonNormalColor);
        private Color buttonNormal = StylesManager.DefaultValue.Style.ButtonNormalColor;
        private Color buttonPressed = ControlPaint.Light(StylesManager.DefaultValue.Style.ButtonDownColor);

        private GraphicsPath controlGraphicsPath;

        private ControlState controlState = ControlState.Normal;

        private VFXManager effectsManager;
        private VFXManager hoverEffectsManager;
        private Image icon;
        private GraphicsPath iconGraphicsPath;

        private Point iconPosition = new Point(4, 0);
        private Rectangle iconRectangle;
        private Size iconSize = new Size(24, 24);
        private Color textColor = StylesManager.DefaultValue.Style.ForeColor(0);

        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;
        private SizeF textSize;

        #endregion

        #region ${0} Properties

        public VisualButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = false;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(100, 25);

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

        [DefaultValue(StylesManager.DefaultValue.Animation), Category(Localize.Category.Behavior),
         Description(Localize.Description.Animation)]
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderColor)]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }

            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderHoverColor)]
        public Color BorderHoverColor
        {
            get
            {
                return borderHoverColor;
            }

            set
            {
                borderHoverColor = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
        public bool BorderHoverVisible
        {
            get
            {
                return borderHoverVisible;
            }

            set
            {
                borderHoverVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumRounding, StylesManager.MaximumRounding))
                {
                    borderRounding = value;
                }

                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
         Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderSize), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderSize)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumBorderSize, StylesManager.MaximumBorderSize))
                {
                    borderSize = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderVisible)]
        public bool BorderVisible
        {
            get
            {
                return borderVisible;
            }

            set
            {
                borderVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.HoverColor)]
        public Color HoverColor
        {
            get
            {
                return buttonHover;
            }

            set
            {
                buttonHover = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.Icon)]
        public Image Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
                if (AutoSize)
                {
                    Size = GetPreferredSize();
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description(Localize.Description.IconPosition)]
        public Point IconPosition
        {
            get
            {
                return iconPosition;
            }

            set
            {
                iconPosition = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout), Description(Localize.Description.IconSize)]
        public Size IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                iconSize = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Category(Localize.Category.Appearance), Description(Localize.Description.NormalColor)]
        public Color NormalColor
        {
            get
            {
                return buttonNormal;
            }

            set
            {
                buttonNormal = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.PressedColor)]
        public Color PressedColor
        {
            get
            {
                return buttonPressed;
            }

            set
            {
                buttonPressed = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextColor)]
        public Color TextColor
        {
            get
            {
                return textColor;
            }

            set
            {
                textColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
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

        #endregion

        #region ${0} Events

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
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Color tempColor = new Color();
            Color textTemp;

            // Draw control state
            if (Enabled)
            {
                // Text color
                textTemp = textColor;

                // Button back color
                switch (controlState)
                {
                    case ControlState.Normal:
                        {
                            tempColor = buttonNormal;
                            break;
                        }

                    case ControlState.Hover:
                        {
                            tempColor = buttonHover;
                            break;
                        }

                    case ControlState.Down:
                        {
                            tempColor = buttonPressed;
                            break;
                        }

                    default:
                        {
                            tempColor = buttonNormal;
                            break;
                        }
                }
            }
            else
            {
                tempColor = buttonDisabled;
                textTemp = textDisabledColor;
            }

            // Draw button background
            graphics.FillPath(new SolidBrush(tempColor), controlGraphicsPath);

            // Setup button border
            if (borderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderColor);
                }
            }

            // Draw icon border
            // graphics.DrawPath(new Pen(Color.Black), iconGraphicsPath);
            if (string.IsNullOrEmpty(Text))
            {
                // Center Icon
                iconRectangle.X += 2;
                iconRectangle.Y += 2;
            }

            if (Icon != null)
            {
                graphics.DrawImage(Icon, iconRectangle);
            }

            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            // Draw string
            graphics.DrawString(Text, Font, new SolidBrush(textTemp), ClientRectangle.Center(), stringFormat);

            // Ripple
            if (effectsManager.IsAnimating())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                for (var i = 0; i < effectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = effectsManager.GetProgress(i);
                    Point animationSource = effectsManager.GetSource(i);

                    using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(101 - animationValue * 100), Color.Black)))
                    {
                        var rippleSize = (int)(animationValue * Width * 2);
                        graphics.SetClip(controlGraphicsPath);
                        graphics.FillEllipse(rippleBrush,
                            new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    }
                }

                graphics.SmoothingMode = SmoothingMode.None;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }

        private void UpdateLocationPoints()
        {
            iconPosition = new Point(iconPosition.X, Height / 2 - iconSize.Height / 2);
            iconRectangle = new Rectangle(iconPosition, iconSize);

            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(iconRectangle);
            iconGraphicsPath.CloseAllFigures();

            GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion

        #region ${0} Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Provides extra space for proper padding for content
            var extra = 16;

            if (Icon != null)
            {
                // 24 is for icon size
                // 4 is for the space between icon & text
                extra += 24 + 4;
            }

            return new Size((int)Math.Ceiling(textSize.Width) + extra, 36);
        }

        #endregion
    }
}