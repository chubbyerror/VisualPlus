namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;

    using VisualPlus.Enums;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Styles;

    #endregion

    [TypeConverter(typeof(CheckMarkConverter))]
    [Description("The checkmark class.")]
    public class Checkmark : ICheckmark
    {
        #region Variables

        private readonly StyleManager _styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

        private bool autoSize;
        private char checkCharacter;
        private Font checkCharacterFont;
        private Point checkLocation;
        private CheckType checkType;
        private Gradient disabledGradient;
        private Bitmap disabledImage;
        private Gradient enabledGradient;
        private Bitmap enabledImage;
        private Size imageSize;
        private int shapeRounding;
        private Size shapeSize;
        private BorderType shapeType;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Checkmark" /> class.</summary>
        /// <param name="boundary">The boundary.</param>
        public Checkmark(Rectangle boundary)
        {
            enabledGradient = _styleManager.CheckmarkStyle.EnabledGradient;
            disabledGradient = _styleManager.CheckmarkStyle.DisabledGradient;

            autoSize = true;
            checkCharacter = '✔';
            checkCharacterFont = _styleManager.Font;
            checkType = CheckType.Character;

            shapeRounding = Settings.DefaultValue.Rounding.BoxRounding;
            shapeType = Settings.DefaultValue.BorderType;

            Bitmap bitmap = new Bitmap(Image.FromStream(new MemoryStream(Convert.FromBase64String(GetBase64CheckImage()))));

            disabledImage = bitmap.FilterGrayScale();
            enabledImage = bitmap;

            checkLocation = new Point();
            imageSize = boundary.Size;
            shapeSize = boundary.Size;
        }

        public enum CheckType
        {
            /// <summary>The character.</summary>
            Character,

            /// <summary>The image.</summary>
            Image,

            /// <summary>The shape.</summary>
            Shape
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.AutoSize)]
        public bool AutoSize
        {
            get
            {
                return autoSize;
            }

            set
            {
                autoSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Checkmark.Character)]
        public char Character
        {
            get
            {
                return checkCharacter;
            }

            set
            {
                checkCharacter = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Image)]
        public Bitmap DisabledImage
        {
            get
            {
                return disabledImage;
            }

            set
            {
                disabledImage = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Image)]
        public Bitmap EnabledImage
        {
            get
            {
                return enabledImage;
            }

            set
            {
                enabledImage = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Strings.Font)]
        public Font Font
        {
            get
            {
                return checkCharacterFont;
            }

            set
            {
                checkCharacterFont = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size ImageSize
        {
            get
            {
                return imageSize;
            }

            set
            {
                imageSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Point)]
        public Point Location
        {
            get
            {
                return checkLocation;
            }

            set
            {
                checkLocation = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Border.Rounding)]
        public int ShapeRounding
        {
            get
            {
                return shapeRounding;
            }

            set
            {
                shapeRounding = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size ShapeSize
        {
            get
            {
                return shapeSize;
            }

            set
            {
                shapeSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Type)]
        public BorderType ShapeType
        {
            get
            {
                return shapeType;
            }

            set
            {
                shapeType = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Checkmark.CheckType)]
        public CheckType Style
        {
            get
            {
                return checkType;
            }

            set
            {
                checkType = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
        public Gradient DisabledGradient
        {
            get
            {
                return disabledGradient;
            }

            set
            {
                disabledGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
        public Gradient EnabledGradient
        {
            get
            {
                return enabledGradient;
            }

            set
            {
                enabledGradient = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Draws the checkmark.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="checkmark">Checkmark type.</param>
        /// <param name="box">Shape type.</param>
        /// <param name="enabled">Control Enabled state.</param>
        /// <param name="textRendererHint">Text rendering hint.</param>
        public static void DrawCheckmark(Graphics graphics, Checkmark checkmark, Rectangle box, bool enabled, TextRenderingHint textRendererHint)
        {
            Gradient checkGradient = enabled ? checkmark.EnabledGradient : checkmark.DisabledGradient;
            Bitmap checkImage = enabled ? checkmark.EnabledImage : checkmark.DisabledImage;

            var boxGradientPoints = GDI.GetGradientPoints(box);
            LinearGradientBrush checkmarkBrush = Gradient.CreateGradientBrush(checkGradient.Colors, boxGradientPoints, checkGradient.Angle, checkGradient.Positions);

            Size characterSize = GDI.GetTextSize(graphics, checkmark.Character.ToString(), checkmark.Font);

            int stylesCount = checkmark.Style.Count();
            var autoLocations = new Point[stylesCount];
            autoLocations[0] = new Point((box.X + (box.Width / 2)) - (characterSize.Width / 2), (box.Y + (box.Height / 2)) - (characterSize.Height / 2));
            autoLocations[1] = new Point((box.X + (box.Width / 2)) - (checkmark.ImageSize.Width / 2), (box.Y + (box.Height / 2)) - (checkmark.ImageSize.Height / 2));
            autoLocations[2] = new Point((box.X + (box.Width / 2)) - (checkmark.ShapeSize.Width / 2), (box.Y + (box.Height / 2)) - (checkmark.ShapeSize.Height / 2));

            Point tempPoint;
            if (checkmark.AutoSize)
            {
                int styleIndex = checkmark.Style.GetIndexByValue(checkmark.Style.ToString());
                tempPoint = autoLocations[styleIndex];
            }
            else
            {
                tempPoint = checkmark.Location;
            }

            switch (checkmark.Style)
            {
                case CheckType.Character:
                    {
                        graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                        DrawCharacter(graphics, checkmark.Character, checkmark.Font, checkmarkBrush, tempPoint);
                        graphics.TextRenderingHint = textRendererHint;
                        break;
                    }

                case CheckType.Image:
                    {
                        Rectangle checkImageRectangle = new Rectangle(tempPoint, checkmark.ImageSize);
                        DrawImage(graphics, checkImage, checkImageRectangle);
                        break;
                    }

                case CheckType.Shape:
                    {
                        Rectangle shapeRectangle = new Rectangle(tempPoint, checkmark.ShapeSize);
                        GraphicsPath shapePath = Border.GetBorderShape(shapeRectangle, checkmark.ShapeType, checkmark.ShapeRounding);
                        DrawShape(graphics, checkmarkBrush, shapePath);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>Draws the checkmark character.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="checkMark">The checkmark character.</param>
        /// <param name="font">The font.</param>
        /// <param name="linearGradientBrush">The linear gradient brush.</param>
        /// <param name="location">The location.</param>
        private static void DrawCharacter(Graphics graphics, char checkMark, Font font, Brush linearGradientBrush, PointF location)
        {
            graphics.DrawString(checkMark.ToString(), font, linearGradientBrush, location);
        }

        /// <summary>Draws the checkmark image.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="image">The image.</param>
        /// <param name="imageRectangle">The image rectangle.</param>
        private static void DrawImage(Graphics graphics, Image image, Rectangle imageRectangle)
        {
            graphics.DrawImage(image, imageRectangle);
        }

        /// <summary>Draws the checkmark shape.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="linearGradientBrush">The linear Gradient Brush.</param>
        /// <param name="graphicsPath">The graphics path.</param>
        private static void DrawShape(Graphics graphics, Brush linearGradientBrush, GraphicsPath graphicsPath)
        {
            graphics.FillPath(linearGradientBrush, graphicsPath);
        }

        private static string GetBase64CheckImage()
        {
            return
                "iVBORw0KGgoAAAANSUhEUgAAABMAAAAQCAYAAAD0xERiAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEySURBVDhPY/hPRUBdw/79+/efVHz77bf/X37+wRAn2bDff/7+91l+83/YmtsYBpJs2ITjz/8rTbrwP2Dlrf9XXn5FkSPJsD13P/y3nHsVbNjyy28w5Ik27NWXX//TNt8DG1S19zFWNRiGvfzy8//ccy9RxEB4wvFnYIMMZl7+//brLwx5EEYx7MP33/9dF18Ha1py8RVcHBR7mlMvgsVXX8X0Hgwz/P379z8yLtz5AKxJdcpFcBj9+v3nf/CqW2Cx5E13UdSiYwzDvv36/d9/BUSzzvRL/0t2PQSzQd57+vEHilp0jGEYCJ9+8hnuGhiee+4Vhjp0jNUwEN566/1/m/mQZJC/48H/zz9+YVWHjHEaBsKgwAZ59eH771jl0TFew0D48osvWMWxYYKGEY///gcAqiuA6kEmfEMAAAAASUVORK5CYII=";
        }

        #endregion
    }

    public class CheckMarkConverter : ExpandableObjectConverter
    {
        #region Events

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (stringValue != null)
            {
                return new ObjectCheckMarkWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Checkmark checkmark;
            object result;

            result = null;
            checkmark = value as Checkmark;

            if ((checkmark != null) && (destinationType == typeof(string)))
            {
                result = "CheckMark Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(CheckMarkConverter))]
    public class ObjectCheckMarkWrapper
    {
        #region Constructors

        public ObjectCheckMarkWrapper()
        {
        }

        public ObjectCheckMarkWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}