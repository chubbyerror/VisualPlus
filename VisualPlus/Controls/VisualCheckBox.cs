namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(CheckBox))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual CheckBox")]
    [Designer(DesignManager.VisualCheckBox)]
    public sealed class VisualCheckBox : CheckBox
    {
        #region Variables

        private bool animation = true;
        private Image backgroundImage;
        private Gradient boxGradient = new Gradient();
        private LinearGradientBrush boxGradientBrush;
        private GraphicsPath boxPath;
        private Point boxPoint = new Point(0, 0);
        private Rectangle boxRectangle;
        private Shape boxShape = new Shape();
        private int boxSpacing = 2;
        private Gradient checkGradient;
        private LinearGradientBrush checkGradientBrush;
        private Image checkImage;
        private Rectangle checkImageRectangle;
        private Checkmark checkMark = new Checkmark();
        private GraphicsPath checkPath;
        private Rectangle checkRectangle;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Point mouseLocation = new Point(0, 0);
        private VFXManager rippleEffectsManager;
        private Color textColor;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;

        #endregion

        #region Constructors

        public VisualCheckBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            ForeColor = Settings.DefaultValue.Style.ForeColor(0);
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);
            Width = 132;
            UpdateStyles();
            Cursor = Cursors.Hand;

            checkMark.Style = CheckType.Character;
            checkMark.Location = new Point(-1, 5);
            checkMark.ImageSize = new Size(19, 16);
            checkMark.Shape.Shape = BorderShape.Rounded;
            checkMark.ShapeSize = new Size(8, 8);
            checkMark.Shape.Rounding = 2;
            boxShape.Size = new Size(14, 14);
            boxShape.ImageSize = new Size(19, 16);

            Color[] boxColor =
                {
                    ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3)),
                    Settings.DefaultValue.Style.BackgroundColor(3)
                };

            Color[] boxDisabledColor =
                {
                    ControlPaint.Light(Settings.DefaultValue.Style.TextDisabled),
                    Settings.DefaultValue.Style.TextDisabled
                };

            Color[] checkColor =
                {
                    ControlPaint.Light(Settings.DefaultValue.Style.StyleColor),
                    Settings.DefaultValue.Style.StyleColor
                };

            Color[] checkDisabledColor =
                {
                    ControlPaint.Light(Settings.DefaultValue.Style.TextDisabled),
                    Settings.DefaultValue.Style.TextDisabled
                };

            float[] gradientPosition = { 0, 1 };

            boxShape.EnabledGradient.Colors = boxColor;
            boxShape.EnabledGradient.Positions = gradientPosition;

            boxShape.HoverGradient.Colors = boxColor;
            boxShape.HoverGradient.Positions = gradientPosition;

            boxShape.DisabledGradient.Colors = boxDisabledColor;
            boxShape.DisabledGradient.Positions = gradientPosition;

            checkMark.EnabledGradient.Colors = checkColor;
            checkMark.EnabledGradient.Positions = gradientPosition;

            checkMark.DisabledGradient.Colors = checkDisabledColor;
            checkMark.DisabledGradient.Positions = gradientPosition;

            ConfigureControlState();
            ConfigureComponents();
            ConfigureAnimation();
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

        [TypeConverter(typeof(ShapeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
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

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSpacing)]
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
        [Category(Localize.Category.Appearance)]
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

        public new Color ForeColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                base.ForeColor = value;
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
                    mouseLocation = new Point(-1, -1);
                    controlState = ControlState.Normal;
                };
            MouseDown += (sender, args) =>
                {
                    controlState = ControlState.Down;

                    if (animation && (args.Button == MouseButtons.Left) && GDI.IsMouseInBounds(mouseLocation, boxRectangle))
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
                    mouseLocation = args.Location;
                    Cursor = GDI.IsMouseInBounds(mouseLocation, boxRectangle) ? Cursors.Hand : Cursors.Default;
                };
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;
            graphics.TextRenderingHint = textRendererHint;

            ConfigureControlState();
            ConfigureComponents();
            DrawCheckBoxState(graphics);
            DrawText(graphics);
            DrawAnimation(graphics);

            boxGradientBrush.Dispose();
            checkGradientBrush.Dispose();
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

            CheckedChanged += (sender, args) =>
                {
                    effectsManager.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
                };

            Animation = true;
        }

        private void ConfigureComponents()
        {
            boxPoint = new Point(0, (ClientRectangle.Height / 2) - (boxShape.Size.Height / 2));
            boxPath = GDI.GetBorderShape(boxRectangle, boxShape.Border.Shape, boxShape.Border.Rounding);
            boxRectangle = new Rectangle(boxPoint, boxShape.Size);

            checkPath = GDI.GetBorderShape(checkRectangle, checkMark.Shape.Shape, checkMark.Shape.Rounding);
            checkRectangle = new Rectangle(checkMark.Location, checkMark.ShapeSize);

            var boxGradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            boxGradientBrush = GDI.CreateGradientBrush(boxGradient.Colors, boxGradientPoints, boxGradient.Angle, boxGradient.Positions);
            checkGradientBrush = GDI.CreateGradientBrush(checkGradient.Colors, boxGradientPoints, checkGradient.Angle, checkGradient.Positions);
        }

        private void ConfigureControlState()
        {
            if (Enabled)
            {
                if (controlState == ControlState.Hover)
                {
                    boxGradient = boxShape.HoverGradient;
                }
                else
                {
                    boxGradient = boxShape.EnabledGradient;
                }

                backgroundImage = boxShape.EnabledImage;

                checkGradient = checkMark.EnabledGradient;
                checkImage = checkMark.EnabledImage;

                textColor = foreColor;
            }
            else
            {
                boxGradient = boxShape.DisabledGradient;
                backgroundImage = boxShape.DisabledImage;

                checkGradient = checkMark.DisabledGradient;
                checkImage = checkMark.DisabledImage;

                textColor = textDisabledColor;
            }
        }

        private void DrawAnimation(Graphics graphics)
        {
            if (animation && rippleEffectsManager.IsAnimating())
            {
                for (var i = 0; i < rippleEffectsManager.GetAnimationCount(); i++)
                {
                    double animationValue = rippleEffectsManager.GetProgress(i);

                    Point animationSource = new Point(boxRectangle.X + (boxRectangle.Width / 2), boxRectangle.Y + (boxRectangle.Height / 2));
                    SolidBrush animationBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), (bool)rippleEffectsManager.GetData(i)[0] ? Color.Black : checkMark.EnabledGradient.Colors[0]));

                    int height = boxRectangle.Height;
                    int size = rippleEffectsManager.GetDirection(i) == AnimationDirection.InOutIn ? (int)(height * (0.8d + (0.2d * animationValue))) : height;

                    GraphicsPath path = GDI.DrawRoundedRectangle(animationSource.X - (size / 2), animationSource.Y - (size / 2), size, size, size / 2);
                    graphics.FillPath(animationBrush, path);
                    animationBrush.Dispose();
                }
            }
        }

        private void DrawCheckBoxState(Graphics graphics)
        {
            if (boxShape.ImageVisible)
            {
                graphics.DrawImage(backgroundImage, new Rectangle(boxShape.ImageLocation, boxShape.ImageSize));
            }
            else
            {
                graphics.FillPath(boxGradientBrush, boxPath);
            }

            if (Checked)
            {
                graphics.SetClip(boxPath);

                switch (checkMark.Style)
                {
                    case CheckType.Character:
                        {
                            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                            Checkmark.DrawCharacter(graphics, checkMark.Character, checkMark.Font, checkGradientBrush, checkMark.Location);
                            graphics.TextRenderingHint = textRendererHint;
                            break;
                        }

                    case CheckType.Shape:
                        {
                            Checkmark.DrawShape(graphics, checkGradientBrush, checkPath);
                            break;
                        }

                    case CheckType.Image:
                        {
                            checkImageRectangle = new Rectangle(checkMark.Location, checkMark.ImageSize);
                            Checkmark.DrawImage(graphics, checkImage, checkImageRectangle);
                            break;
                        }
                }

                graphics.ResetClip();
            }

            if (boxShape.Border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, boxPath, boxShape.Border.Thickness, boxShape.Border.Color, boxShape.Border.HoverColor, boxShape.Border.HoverVisible);
            }
        }

        private void DrawText(Graphics graphics)
        {
            StringFormat stringFormat = new StringFormat { LineAlignment = StringAlignment.Center };
            Point textPoint = new Point(boxPoint.X + boxShape.Size.Width + boxSpacing, ClientRectangle.Height / 2);
            graphics.DrawString(Text, Font, new SolidBrush(textColor), textPoint, stringFormat);
        }

        #endregion
    }
}