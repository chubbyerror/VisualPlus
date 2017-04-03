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

    /// <summary>The visual CheckBox.</summary>
    [ToolboxBitmap(typeof(CheckBox)), Designer(VSDesignerBinding.VisualCheckBox)]
    public class VisualCheckBox : CheckBox
    {
        #region  ${0} Variables

        private const int spacing = 2;
        private bool animation = true;
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private Point boxLocation = new Point(0, 0);
        private Size boxSize = new Size(13, 13);
        private Color checkBoxColor = StylesManager.DefaultValue.Style.BackgroundColor(3);
        private GraphicsPath checkBoxPath;
        private Rectangle checkBoxRectangle;
        private Color checkMarkColor = StylesManager.MainColor;
        private Color checkMarkDisabled = StylesManager.DefaultValue.Style.TextDisabled;
        private ControlState controlState = ControlState.Normal;
        private VFXManager effectsManager;
        private VFXManager rippleEffectsManager;
        private Color textColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color textDisabled = StylesManager.DefaultValue.Style.TextDisabled;

        #endregion

        #region ${0} Properties

        public VisualCheckBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;

            // Setup effects animation
            effectsManager = new VFXManager
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

            CheckedChanged += (sender, args) =>
                {
                    effectsManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
                };

            Animation = true;
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
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumCheckBoxBorderRounding,
                    StylesManager.MaximumCheckBoxBorderRounding))
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

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color CheckBoxColor
        {
            get
            {
                return checkBoxColor;
            }

            set
            {
                checkBoxColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color CheckMarkColor
        {
            get
            {
                return checkMarkColor;
            }

            set
            {
                checkMarkColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
        public Color CheckMarkDisabled
        {
            get
            {
                return checkMarkDisabled;
            }

            set
            {
                checkMarkDisabled = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

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
        public Color TextDisabled
        {
            get
            {
                return textDisabled;
            }

            set
            {
                textDisabled = value;
                Invalidate();
            }
        }

        #endregion

        #region ${0} Events

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = Cursors.Hand;
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
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;

            Color textTemp;
            Color checkTemp;

            // Draw control state
            if (Enabled)
            {
                textTemp = textColor;
                checkTemp = checkMarkColor;
            }
            else
            {
                textTemp = textDisabled;
                checkTemp = checkMarkDisabled;
            }

            // Draw checkbox background
            graphics.FillPath(new SolidBrush(CheckBoxColor), checkBoxPath);

            if (Checked)
            {
                // Checkmark points
                var points = new[]
                    {
                        new PointF(checkBoxRectangle.X + 3, checkBoxRectangle.Y + 5), new PointF(checkBoxRectangle.X + 5, checkBoxRectangle.Y + 7),
                        new PointF(checkBoxRectangle.X + 9, checkBoxRectangle.Y + 3), new PointF(checkBoxRectangle.X + 9, checkBoxRectangle.Y + 4),
                        new PointF(checkBoxRectangle.X + 5, checkBoxRectangle.Y + 8), new PointF(checkBoxRectangle.X + 3, checkBoxRectangle.Y + 6),
                        new PointF(checkBoxRectangle.X + 3, checkBoxRectangle.Y + 7),
                        new PointF(checkBoxRectangle.X + 5, checkBoxRectangle.Y + 9), new PointF(checkBoxRectangle.X + 9, checkBoxRectangle.Y + 5)
                    };

                graphics.DrawLines(new Pen(checkTemp), points);
            }

            // Setup checkbox border
            if (BorderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, checkBoxPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, checkBoxPath, borderSize, borderColor);
                }
            }

            // Draw string
            StringFormat stringFormat = new StringFormat();

            // stringFormat.Alignment = StringAlignment.Center;
            // stringFormat.LineAlignment = StringAlignment.Center;
            Point textPoint = new Point(boxLocation.X + boxSize.Width + spacing, boxSize.Height / 2 - (int)Font.Size / 2);
            graphics.DrawString(Text, Font, new SolidBrush(textTemp), textPoint, stringFormat);
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

        private void UpdateLocationPoints()
        {
            // Update
            boxLocation = new Point(boxLocation.X, Height / 2 - boxSize.Height / 2);
            checkBoxRectangle = new Rectangle(boxLocation, boxSize);

            // Update paths
            checkBoxPath = GDI.GetBorderShape(checkBoxRectangle, borderShape, borderRounding);
        }

        #endregion
    }
}