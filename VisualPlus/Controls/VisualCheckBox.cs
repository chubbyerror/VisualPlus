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

    public enum CheckBoxType
    {
        /// <summary>The check mark.</summary>
        CheckMark,

        /// <summary>The filled.</summary>
        Filled
    }

    /// <summary>The visual CheckBox.</summary>
    [ToolboxBitmap(typeof(CheckBox)), Designer(VSDesignerBinding.VisualCheckBox)]
    public sealed class VisualCheckBox : CheckBox
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
        private Size boxSize = new Size(14, 14);
        private Color checkBoxColor = StylesManager.DefaultValue.Style.BackgroundColor(3);
        private Point checkBoxLocation = new Point(0, 0);
        private GraphicsPath checkBoxPath;
        private Rectangle checkBoxRectangle;
        private CheckBoxType checkBoxType = CheckBoxType.Filled;
        private Color checkMarkColor = StylesManager.MainColor;
        private Size checkMarkFillSize = new Size(8, 8);
        private Point checkMarkLocation = new Point(0, 0);
        private GraphicsPath checkMarkPath;
        private Rectangle checkMarkRectangle;
        private Color controlDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;
        private ControlState controlState = ControlState.Normal;
        private VFXManager effectsManager;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private VFXManager rippleEffectsManager;
        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;

        #endregion

        #region ${0} Properties

        public VisualCheckBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

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

        [Category(Localize.Category.Behavior), Description(Localize.Description.ComponentSize)]
        public Size CheckBoxSize
        {
            get
            {
                return boxSize;
            }

            set
            {
                boxSize = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior), Description(Localize.Description.ComponentNoName)]
        public CheckBoxType CheckBoxType
        {
            get
            {
                return checkBoxType;
            }

            set
            {
                checkBoxType = value;
                UpdateLocationPoints();
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

        [Category(Localize.Category.Behavior), Description(Localize.Description.ComponentSize)]
        public Size CheckMarkFillSize
        {
            get
            {
                return checkMarkFillSize;
            }

            set
            {
                checkMarkFillSize = value;
                UpdateLocationPoints();
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ControlDisabled)]
        public Color ControlDisabledColor
        {
            get
            {
                return controlDisabledColor;
            }

            set
            {
                controlDisabledColor = value;
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
                return foreColor;
            }

            set
            {
                foreColor = value;
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
            graphics.Clear(BackColor);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;
            Color controlCheckTemp = Enabled ? checkMarkColor : controlDisabledColor;

            // Draw checkbox background
            graphics.FillPath(new SolidBrush(CheckBoxColor), checkBoxPath);

            if (Checked)
            {
                if (checkBoxType == CheckBoxType.CheckMark)
                {
                    // Draw check mark
                    Point pt = new Point(-2, -1);
                    Rectangle layoutRectangle = new Rectangle(pt, new Size(checkBoxRectangle.Width, checkBoxRectangle.Height));

                    using (Font wing = new Font("Wingdings", 14f, FontStyle.Regular))
                    {
                        e.Graphics.DrawString("ü", wing, new SolidBrush(checkMarkColor), layoutRectangle);
                    }
                }
                else
                {
                    // Draw filled check mark
                    graphics.FillPath(new SolidBrush(controlCheckTemp), checkMarkPath);
                }
            }

            // Setup checkbox border
            if (BorderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, checkBoxPath, borderSize, borderColor, borderHoverColor, borderHoverVisible);
            }

            // Draw string
            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            // Point textPoint = new Point(checkBoxLocation.X + boxSize.Width + spacing, boxSize.Height / 2 - (int)Font.Size / 2);
            // Point textPoint = new Point(checkBoxLocation.X + boxSize.Width + spacing, ClientRectangle.Height / 2 - (int)Font.Size / 2);
            Point textPoint = new Point(checkBoxLocation.X + boxSize.Width + spacing, ClientRectangle.Height / 2);
            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textPoint, stringFormat);
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
            checkBoxLocation = new Point(checkBoxLocation.X, ClientRectangle.Height / 2 - boxSize.Height / 2);
            checkBoxRectangle = new Rectangle(checkBoxLocation, boxSize);

            checkMarkLocation = new Point(checkBoxLocation.X + boxSize.Width / 2 - checkMarkFillSize.Width / 2,
                checkBoxLocation.Y + boxSize.Height / 2 - checkMarkFillSize.Height / 2);

            checkMarkRectangle = new Rectangle(checkMarkLocation, checkMarkFillSize);

            // Update paths
            checkBoxPath = GDI.GetBorderShape(checkBoxRectangle, borderShape, borderRounding);
            checkMarkPath = GDI.GetBorderShape(checkMarkRectangle, borderShape, 1);
        }

        #endregion
    }
}