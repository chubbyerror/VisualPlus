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
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RadioButton))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual RadioButton")]
    [Designer(ControlManager.FilterProperties.VisualRadioButton)]
    public sealed class VisualRadioButton : RadioButton
    {
        #region Variables

        private readonly MouseState mouseState;
        private bool animation = true;
        private Gradient boxGradient = new Gradient();
        private LinearGradientBrush boxGradientBrush;
        private GraphicsPath boxPath;
        private Point boxPoint = new Point(0, 0);
        private Shape boxShape;
        private int boxSpacing = 2;
        private Checkmark checkMark;
        private Color foreColor;
        private Point mouseLocation = new Point(0, 0);
        private VFXManager rippleEffectsManager;
        private StyleManager styleManager = new StyleManager();
        private Color textDisabledColor;
        private TextRenderingHint textRendererHint;

        #endregion

        #region Constructors

        public VisualRadioButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            UpdateStyles();

            Width = 132;
            Cursor = Cursors.Hand;

            mouseState = new MouseState(this);

            ConfigureStyleManager();
            ConfigureControlState();
            ConfigureComponents();
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
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
        [Description(Localize.Description.Strings.TextRenderingHint)]
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

            mouseState.State = MouseStates.Normal;
            MouseEnter += (sender, args) =>
                {
                    mouseState.State = MouseStates.Hover;
                };
            MouseLeave += (sender, args) =>
                {
                    mouseLocation = new Point(-1, -1);
                    mouseState.State = MouseStates.Normal;
                };
            MouseDown += (sender, args) =>
                {
                    mouseState.State = MouseStates.Down;

                    if (animation && (args.Button == MouseButtons.Left) && GDI.IsMouseInBounds(mouseLocation, boxShape.Rectangle))
                    {
                        rippleEffectsManager.SecondaryIncrement = 0;
                        rippleEffectsManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                    }
                };
            MouseUp += (sender, args) =>
                {
                    mouseState.State = MouseStates.Hover;
                    rippleEffectsManager.SecondaryIncrement = 0.08;
                };
            MouseMove += (sender, args) =>
                {
                    mouseLocation = args.Location;
                    Cursor = GDI.IsMouseInBounds(mouseLocation, boxShape.Rectangle) ? Cursors.Hand : Cursors.Default;
                };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Checked)
            {
                Checked = true;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseState.State = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            if (styleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            ConfigureControlState();
            ConfigureComponents();

            Shape.DrawBackground(graphics, boxShape, boxGradientBrush, boxPath);

            if (Checked)
            {
                Checkmark.DrawCheckmark(graphics, checkMark, boxShape, Enabled, textRendererHint);
            }

            Border.DrawBorderStyle(graphics, boxShape.Border, mouseState.State, boxPath);

            DrawText(graphics);
            DrawAnimation(graphics);

            boxGradientBrush.Dispose();
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
            // boxShape.Location = new Point(0, (ClientRectangle.Height / 2) - (boxShape.Size.Height / 2));
            boxShape.Rectangle = new Rectangle(new Point(0, (ClientRectangle.Height / 2) - (boxShape.Rectangle.Height / 2)), boxShape.Rectangle.Size);

            // boxRectangle = new Rectangle(boxShape.Location, boxShape.Size);
            boxPath = Border.GetBorderShape(boxShape.Rectangle, boxShape.Border.Type, boxShape.Border.Rounding);

            var boxGradientPoints = GDI.GetGradientPoints(ClientRectangle);
            boxGradientBrush = Gradient.CreateGradientBrush(boxGradient.Colors, boxGradientPoints, boxGradient.Angle, boxGradient.Positions);
        }

        private void ConfigureControlState()
        {
            foreColor = Enabled ? foreColor : textDisabledColor;
            boxGradient = Enabled ? boxShape.EnabledGradient : boxShape.DisabledGradient;
        }

        private void ConfigureStyleManager()
        {
            if (styleManager.VisualStylesManager != null)
            {
                // Load style manager settings 
                IBorder borderStyle = styleManager.VisualStylesManager.BorderStyle;
                ICheckmark checkmarkStyle = StyleManager.VisualStylesManager.CheckmarkStyle;
                IControl controlStyle = styleManager.VisualStylesManager.ControlStyle;
                IFont fontStyle = styleManager.VisualStylesManager.FontStyle;

                animation = styleManager.VisualStylesManager.Animation;
                boxShape.Border.Color = borderStyle.Color;
                boxShape.Border.HoverColor = borderStyle.HoverColor;
                boxShape.Border.HoverVisible = styleManager.VisualStylesManager.BorderHoverVisible;
                boxShape.Border.Rounding = styleManager.VisualStylesManager.BorderRounding;
                boxShape.Border.Type = styleManager.VisualStylesManager.BorderType;
                boxShape.Border.Thickness = styleManager.VisualStylesManager.BorderThickness;
                boxShape.Border.Visible = styleManager.VisualStylesManager.BorderVisible;
                textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;
                Font = new Font(fontStyle.FontFamily, fontStyle.FontSize, fontStyle.FontStyle);
                foreColor = fontStyle.ForeColor;
                textDisabledColor = fontStyle.ForeColorDisabled;

                boxShape.EnabledGradient.Colors = controlStyle.BoxEnabled.Colors;
                boxShape.EnabledGradient.Positions = controlStyle.BoxEnabled.Positions;
                boxShape.DisabledGradient.Colors = controlStyle.BoxDisabled.Colors;
                boxShape.DisabledGradient.Positions = controlStyle.BoxDisabled.Positions;

                checkMark.EnabledGradient = checkmarkStyle.EnabledGradient;
                checkMark.DisabledGradient = checkmarkStyle.DisabledGradient;
            }
            else
            {
                // Load default settings
                animation = Settings.DefaultValue.Animation;

                boxShape = new Shape(new Rectangle(new Point(0, 0), new Size(14, 14)))
                    {
                        Border = new Border
                            {
                                Rounding = Settings.DefaultValue.Rounding.RoundedRectangle
                            },
                        Image =
                            {
                                Size = new Size(19, 16)
                            }
                    };

                checkMark = new Checkmark(ClientRectangle)
                    {
                        Style = Checkmark.CheckType.Shape,
                        Location = new Point(3, 8),
                        ImageSize = new Size(19, 16),
                        ShapeSize = new Size(8, 8)
                    };

                textRendererHint = Settings.DefaultValue.TextRenderingHint;
                Font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
                foreColor = Settings.DefaultValue.Font.ForeColor;
                textDisabledColor = Settings.DefaultValue.Font.ForeColorDisabled;
            }
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
            Point textPoint = new Point(boxPoint.X + boxShape.Rectangle.Width + boxSpacing, ClientRectangle.Height / 2);
            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textPoint, stringFormat);
        }

        #endregion
    }
}