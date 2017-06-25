namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;

    using VisualPlus.Localization;

    #endregion

    [TypeConverter(typeof(CheckMarkConverter))]
    [Description("The checkmark class.")]
    public class Checkmark
    {
        #region Variables

        private char checkCharacter = '✔';

        private Font checkCharacterFont;

        private Point checkLocation = new Point(0, 0);

        private Border checkShape = new Border();

        private CheckType checkType;

        private Gradient disabledGradient = new Gradient();

        private Image disabledImage = Image.FromStream(new MemoryStream(Convert.FromBase64String(GetBase64CheckImage())));

        private Gradient enabledGradient = new Gradient();

        private Image enabledImage = Image.FromStream(new MemoryStream(Convert.FromBase64String(GetBase64CheckImage())));

        private Size imageSize = new Size(0, 0);

        private Size shapeSize = new Size(0, 0);

        #endregion

        #region Constructors

        public Checkmark()
        {
            enabledGradient.Colors = Settings.DefaultValue.Progress.Progress.Colors;
            enabledGradient.Positions = Settings.DefaultValue.Progress.Progress.Positions;
            disabledGradient.Colors = Settings.DefaultValue.Progress.ProgressDisabled.Colors;
            disabledGradient.Positions = Settings.DefaultValue.Progress.ProgressDisabled.Positions;

            checkCharacterFont = Settings.DefaultValue.DefaultFont;
            checkType = CheckType.Character;
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
        [Description(Localize.Description.Common.Image)]
        public Image DisabledImage
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

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Image)]
        public Image EnabledImage
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
        public Border Shape
        {
            get
            {
                return checkShape;
            }

            set
            {
                checkShape = value;
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

        #endregion

        #region Events

        /// <summary>Draws the checkmark character.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="checkMark">The checkmark character.</param>
        /// <param name="font">The font.</param>
        /// <param name="linearGradientBrush">The linear gradient brush.</param>
        /// <param name="location">The location.</param>
        public static void DrawCharacter(Graphics graphics, char checkMark, Font font, LinearGradientBrush linearGradientBrush, PointF location)
        {
            graphics.DrawString(checkMark.ToString(), font, linearGradientBrush, location);
        }

        /// <summary>Draws the checkmark image.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="image">The image.</param>
        /// <param name="imageRectangle">The image rectangle.</param>
        public static void DrawImage(Graphics graphics, Image image, Rectangle imageRectangle)
        {
            graphics.DrawImage(image, imageRectangle);
        }

        /// <summary>Draws the checkmark shape.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="linearGradientBrush">The linear Gradient Brush.</param>
        /// <param name="graphicsPath">The graphics Path.</param>
        public static void DrawShape(Graphics graphics, LinearGradientBrush linearGradientBrush, GraphicsPath graphicsPath)
        {
            graphics.FillPath(linearGradientBrush, graphicsPath);
        }

        public static string GetBase64CheckImage()
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