namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Components.Symbols;
    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(CheckBox))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual Checkbox")]
    [Designer(VSDesignerBinding.VisualCheckBox)]
    public sealed class VisualCheckBox : CheckBox
    {
        #region Variables

        private const int Spacing = 2;
        private bool animation = true;

        private Border border = new Border();

        private Color[] boxColor =
            {
                Settings.DefaultValue.Style.BackgroundColor(3),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3))
            };

        private Color[] boxDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled
            };

        private Gradient boxDisabledGradient = new Gradient();
        private Gradient boxGradient = new Gradient();

        private Size boxSize = new Size(14, 14);

        private GraphicsPath checkBoxPath;
        private Point checkBoxPoint = new Point(0, 0);
        private Rectangle checkBoxRectangle;
        private CheckBoxType checkBoxType = CheckBoxType.Check;
        private string checkCharacter = Checkmark.CheckChar.ToString();
        private Font checkCharacterFont = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Bold);

        private Color[] checkDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.TextDisabled),
                Settings.DefaultValue.Style.TextDisabled
            };

        private Gradient checkDisabledGradient = new Gradient();
        private Gradient checkGradient = new Gradient();
        private Image checkImage = Checkmark.CheckImage;
        private Rectangle checkImageRectangle;
        private Size checkImageSize = new Size(19, 16);

        private Color[] checkMarkColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.StyleColor),
                Settings.DefaultValue.Style.StyleColor
            };

        private Size checkMarkFillSize = new Size(8, 8);
        private GraphicsPath checkPath;
        private Point checkPoint = new Point(-1, 5);
        private Rectangle checkRectangle;

        private ControlState controlState = ControlState.Normal;
        private VFXManager effectsManager;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);

        private VFXManager rippleEffectsManager;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

        #endregion

        #region Constructors

        public VisualCheckBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Cursor = Cursors.Hand;

            Size = new Size(140, 24);

            float[] gradientPosition = { 0, 1 };

            Box.Colors = boxColor;
            Box.Positions = gradientPosition;

            boxDisabledGradient.Colors = boxDisabledColor;
            boxDisabledGradient.Positions = gradientPosition;

            checkGradient.Colors = checkMarkColor;
            checkGradient.Positions = gradientPosition;

            checkDisabledGradient.Colors = checkDisabledColor;
            checkDisabledGradient.Positions = gradientPosition;

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

        public enum CheckBoxType
        {
            /// <summary>The check mark.</summary>
            Check,

            /// <summary>The filled.</summary>
            Filled,

            /// <summary>The image</summary>
            Image
        }

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Animation)]
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
        public Gradient Box
        {
            get
            {
                return boxGradient;
            }

            set
            {
                boxGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border BoxBorder
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

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient BoxDisabled
        {
            get
            {
                return boxDisabledGradient;
            }

            set
            {
                boxDisabledGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentSize)]
        public Size BoxSize
        {
            get
            {
                return boxSize;
            }

            set
            {
                boxSize = value;
                UpdateCheckPoint();
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentNoName)]
        public CheckBoxType BoxType
        {
            get
            {
                return checkBoxType;
            }

            set
            {
                checkBoxType = value;
                UpdateCheckPoint();
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font CheckCharacterFont
        {
            get
            {
                return checkCharacterFont;
            }

            set
            {
                checkCharacterFont = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient CheckDisabled
        {
            get
            {
                return checkDisabledGradient;
            }

            set
            {
                checkDisabledGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Icon)]
        public Image CheckImage
        {
            get
            {
                return checkImage;
            }

            set
            {
                checkImage = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.IconSize)]
        public Size CheckImageSize
        {
            get
            {
                return checkImageSize;
            }

            set
            {
                checkImageSize = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient CheckMark
        {
            get
            {
                return checkGradient;
            }

            set
            {
                checkGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description("Checkmark character string")]
        public string CheckmarkCharacter
        {
            get
            {
                return checkCharacter;
            }

            set
            {
                checkCharacter = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentSize)]
        public Size CheckMarkFillSize
        {
            get
            {
                return checkMarkFillSize;
            }

            set
            {
                checkMarkFillSize = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentLocation)]
        public Point CheckPoint
        {
            get
            {
                return checkPoint;
            }

            set
            {
                checkPoint = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextRenderingHint)]
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

            controlState = ControlState.Normal;
            MouseEnter += (sender, args) =>
                {
                    controlState = ControlState.Hover;
                };
            MouseLeave += (sender, args) =>
                {
                    MouseLocation = new Point(-1, -1);
                    controlState = ControlState.Normal;
                };
            MouseDown += (sender, args) =>
                {
                    controlState = ControlState.Down;

                    if (animation && args.Button == MouseButtons.Left && IsMouseInCheckArea())
                    {
                        rippleEffectsManager.SecondaryIncrement = 0;
                        rippleEffectsManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                    }
                };
            MouseUp += (sender, args) =>
                {
                    controlState = ControlState.Hover;
                    rippleEffectsManager.SecondaryIncrement = 0.08;
                };
            MouseMove += (sender, args) =>
                {
                    MouseLocation = args.Location;
                    Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
                };
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
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
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;
            graphics.TextRenderingHint = textRendererHint;

            Color textColor = Enabled ? foreColor : textDisabledColor;
            Gradient checkMark;
            Gradient box;

            if (Enabled)
            {
                box = boxGradient;
                checkMark = checkGradient;
            }
            else
            {
                box = boxDisabledGradient;
                checkMark = checkDisabledGradient;
            }

            checkBoxPoint = new Point(0, ClientRectangle.Height / 2 - boxSize.Height / 2);
            checkBoxRectangle = new Rectangle(checkBoxPoint, boxSize);

            checkRectangle = new Rectangle(checkPoint, checkMarkFillSize);
            checkBoxPath = GDI.GetBorderShape(checkBoxRectangle, border.Shape, border.Rounding);
            checkPath = GDI.GetBorderShape(checkRectangle, border.Shape, 1);

            var boxGradientPoints = new Point[2] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush boxGradientBrush = GDI.CreateGradientBrush(box.Colors, boxGradientPoints, box.Angle, box.Positions);

            // Draw checkbox background
            graphics.FillPath(boxGradientBrush, checkBoxPath);

            // draw ripple animation
            if (animation && rippleEffectsManager.IsAnimating())
            {
                for (var i = 0; i < rippleEffectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = rippleEffectsManager.GetProgress(i);

                    Point animationSource = new Point(checkBoxRectangle.X + checkBoxRectangle.Width / 2, checkBoxRectangle.Y + checkBoxRectangle.Height / 2);
                    SolidBrush animationBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), (bool)rippleEffectsManager.GetData(i)[0] ? Color.Black : checkMarkColor[0]));

                    int height = checkBoxRectangle.Height;
                    int size = rippleEffectsManager.GetDirection(i) == AnimationDirection.InOutIn ? (int)(height * (0.8d + 0.2d * animationValue)) : height;

                    using (GraphicsPath path = GDI.DrawRoundedRectangle(
                        animationSource.X - size / 2,
                        animationSource.Y - size / 2,
                        size,
                        size,
                        size / 2))
                    {
                        graphics.FillPath(animationBrush, path);
                    }

                    animationBrush.Dispose();
                }
            }

            LinearGradientBrush checkGradientBrush = GDI.CreateGradientBrush(checkMark.Colors, boxGradientPoints, checkMark.Angle, checkMark.Positions);

            if (Checked)
            {
                switch (checkBoxType)
                {
                    case CheckBoxType.Check:
                        {
                            DrawCheckMark(graphics, checkGradientBrush);
                            break;
                        }

                    case CheckBoxType.Filled:
                        {
                            graphics.FillPath(checkGradientBrush, checkPath);
                            break;
                        }

                    case CheckBoxType.Image:
                        {
                            DrawCheckImage(graphics);
                            break;
                        }
                }
            }

            // Setup checkbox border
            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, checkBoxPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }

            // Draw string
            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            Point textPoint = new Point(checkBoxPoint.X + boxSize.Width + Spacing, ClientRectangle.Height / 2);
            graphics.DrawString(Text, Font, new SolidBrush(textColor), textPoint, stringFormat);
        }

        private void DrawCheckImage(Graphics graphics)
        {
            if (checkImage != null)
            {
                checkImageRectangle = new Rectangle(checkPoint, checkImageSize);

                // Draw checkmark inside clip
                graphics.SetClip(checkBoxPath);

                // Draw icon
                graphics.DrawImage(checkImage, checkImageRectangle);

                graphics.ResetClip();
            }
        }

        private void DrawCheckMark(Graphics graphics, Brush controlCheckTemp)
        {
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.SetClip(checkBoxPath);
            Checkmark.DrawCheckMark(graphics, checkCharacterFont, controlCheckTemp, checkPoint, Checkmark.CheckChar);
            graphics.ResetClip();
            graphics.TextRenderingHint = textRendererHint;
        }

        private bool IsMouseInCheckArea()
        {
            return checkBoxRectangle.Contains(MouseLocation);
        }

        private void UpdateCheckPoint()
        {
            switch (checkBoxType)
            {
                case CheckBoxType.Check:
                    {
                        checkPoint = new Point(-1, 2);
                        break;
                    }

                case CheckBoxType.Filled:
                    {
                        checkPoint = new Point(checkBoxPoint.X + boxSize.Width / 2 - checkMarkFillSize.Width / 2, checkBoxPoint.Y + boxSize.Height / 2 - checkMarkFillSize.Height / 2);
                        break;
                    }

                case CheckBoxType.Image:
                    {
                        checkPoint = new Point(-3, 0);
                        break;
                    }
            }
        }

        #endregion
    }
}