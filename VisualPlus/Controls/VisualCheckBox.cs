namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.IO;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual CheckBox.</summary>
    [ToolboxBitmap(typeof(CheckBox))]
    [Designer(VSDesignerBinding.VisualCheckBox)]
    public sealed class VisualCheckBox : CheckBox
    {
        #region Variables

        private const int Spacing = 2;
        private bool animation = true;
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Size boxSize = new Size(14, 14);

        private Color[] checkBoxColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3)),
                Settings.DefaultValue.Style.BackgroundColor(3),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(3))
            };

        private GraphicsPath checkBoxPath;
        private Point checkBoxPoint = new Point(0, 0);
        private Rectangle checkBoxRectangle;
        private CheckBoxType checkBoxType = CheckBoxType.Check;
        private string checkCharacter = "✔";
        private Font checkCharacterFont = new Font(Settings.DefaultValue.Style.FontFamily, 8.25F, FontStyle.Bold);
        private Image checkImage = Image.FromStream(new MemoryStream(Convert.FromBase64String(GetCheckMarkPNG())));
        private Rectangle checkImageRectangle;
        private Size checkImageSize = new Size(19, 16);

        private Color[] checkMarkColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.StyleColor),
                Settings.DefaultValue.Style.StyleColor
            };

        private Size checkMarkFillSize = new Size(8, 8);
        private GraphicsPath checkPath;
        private Point checkPoint = new Point(-1, 2);
        private Rectangle checkRectangle;

        private Color[] controlDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled
            };

        private ControlState controlState = ControlState.Normal;
        private VFXManager effectsManager;
        private Point endCheckPoint;
        private Point endPoint;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private float gradientAngle;
        private LinearGradientBrush gradientBrush;
        private float gradientCheckAngle;
        private LinearGradientBrush gradientCheckBrush;
        private float[] gradientCheckPosition = { 0, 1 };
        private float[] gradientPosition = { 0, 1 / 2f, 1 };
        private VFXManager rippleEffectsManager;

        private Point startCheckPoint;
        private Point startPoint;
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderColor)]
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderHoverColor)]
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

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderHoverVisible)]
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

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumCheckBoxBorderRounding, Settings.MaximumCheckBoxBorderRounding))
                {
                    borderRounding = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderThickness)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderThickness)]
        public int BorderThickness
        {
            get
            {
                return borderThickness;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderThickness = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderVisible)]
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
        [Description(Localize.Description.ComponentColor)]
        public Color[] CheckBoxColor
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentSize)]
        public Size CheckBoxSize
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] CheckMarkColor
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ControlDisabled)]
        public Color[] ControlDisabledColor
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

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientAngle
        {
            get
            {
                return gradientAngle;
            }

            set
            {
                gradientAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientCheckAngle
        {
            get
            {
                return gradientCheckAngle;
            }

            set
            {
                gradientCheckAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientCheckPosition
        {
            get
            {
                return gradientCheckPosition;
            }

            set
            {
                gradientCheckPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientPosition
        {
            get
            {
                return gradientPosition;
            }

            set
            {
                gradientPosition = value;
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

        public static string GetCheckMarkPNG()
        {
            return
                "iVBORw0KGgoAAAANSUhEUgAAABMAAAAQCAYAAAD0xERiAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEySURBVDhPY/hPRUBdw/79+/efVHz77bf/X37+wRAn2bDff/7+91l+83/YmtsYBpJs2ITjz/8rTbrwP2Dlrf9XXn5FkSPJsD13P/y3nHsVbNjyy28w5Ik27NWXX//TNt8DG1S19zFWNRiGvfzy8//ccy9RxEB4wvFnYIMMZl7+//brLwx5EEYx7MP33/9dF18Ha1py8RVcHBR7mlMvgsVXX8X0Hgwz/P379z8yLtz5AKxJdcpFcBj9+v3nf/CqW2Cx5E13UdSiYwzDvv36/d9/BUSzzvRL/0t2PQSzQd57+vEHilp0jGEYCJ9+8hnuGhiee+4Vhjp0jNUwEN566/1/m/mQZJC/48H/zz9+YVWHjHEaBsKgwAZ59eH771jl0TFew0D48osvWMWxYYKGEY///gcAqiuA6kEmfEMAAAAASUVORK5CYII=";
        }

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

            // Set control state color
            Color textColor = Enabled ? foreColor : textDisabledColor;
            var controlCheckTemp = Enabled ? checkMarkColor : controlDisabledColor;

            // Update
            checkBoxPoint = new Point(0, ClientRectangle.Height / 2 - boxSize.Height / 2);
            checkBoxRectangle = new Rectangle(checkBoxPoint, boxSize);

            checkRectangle = new Rectangle(checkPoint, checkMarkFillSize);
            checkBoxPath = GDI.GetBorderShape(checkBoxRectangle, borderShape, borderRounding);
            checkPath = GDI.GetBorderShape(checkRectangle, borderShape, 1);

            // Gradient points
            startPoint = new Point(checkBoxRectangle.Width, 0);
            endPoint = new Point(checkBoxRectangle.Width, checkBoxRectangle.Height);
            startCheckPoint = new Point(checkBoxRectangle.Width, 0);
            endCheckPoint = new Point(checkBoxRectangle.Width, checkBoxRectangle.Height);

            gradientBrush = GDI.CreateGradientBrush(checkBoxColor, gradientPosition, gradientAngle, startPoint, endPoint);

            // Draw checkbox background
            graphics.FillPath(gradientBrush, checkBoxPath);

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

            gradientCheckBrush = GDI.CreateGradientBrush(controlCheckTemp, gradientCheckPosition, gradientCheckAngle, startCheckPoint, endCheckPoint);
            if (Checked)
            {
                switch (checkBoxType)
                {
                    case CheckBoxType.Check:
                        {
                            DrawCheckMark(graphics, gradientCheckBrush);
                            break;
                        }

                    case CheckBoxType.Filled:
                        {
                            graphics.FillPath(gradientCheckBrush, checkPath);
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
            if (BorderVisible)
            {
                GDI.DrawBorderType(graphics, controlState, checkBoxPath, borderThickness, borderColor, borderHoverColor, borderHoverVisible);
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

            // Draw checkmark inside clip
            graphics.SetClip(checkBoxPath);

            // Draw check mark
            graphics.DrawString(checkCharacter, checkCharacterFont, controlCheckTemp, checkPoint);

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