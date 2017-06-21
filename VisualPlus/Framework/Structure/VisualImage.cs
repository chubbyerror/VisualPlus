namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;

    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    [TypeConverter(typeof(VisualImageConverter))]
    [Description("Visual Image.")]
    public class VisualImage
    {
        #region Variables

        private Border border = new Border();
        private Image image;
        private Point imagePoint = new Point(0, 0);
        private Size imageSize;
        private bool visible = true;

        #endregion

        #region Constructors

        public VisualImage(Image _image, Size _size)
        {
            border.Visible = false;
            border.HoverVisible = false;

            image = _image;
            imageSize = _size;
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Border Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Image)]
        public Image Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Point)]
        public Point Point
        {
            get
            {
                return imagePoint;
            }

            set
            {
                imagePoint = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size Size
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

        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Visible)]
        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        #endregion

        #region Events

        public static void DrawImage(Graphics graphics, Border _border, Point _imagePoint, Image _image, Size _imageSize, bool _visible)
        {
            Rectangle imageRectangle = new Rectangle(_imagePoint, _imageSize);
            GraphicsPath imagePath = new GraphicsPath();
            imagePath.AddRectangle(imageRectangle);

            if (_border.Visible)
            {
                GDI.DrawBorder(graphics, imagePath, _border.Thickness, _border.Color);
            }

            if (_visible)
            {
                graphics.DrawImage(_image, imageRectangle);
            }
        }

        public Size GetOriginalSize(Image _image)
        {
            return _image.Size;
        }

        #endregion
    }

    public class VisualImageConverter : ExpandableObjectConverter
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
                return new ObjectVisualImageWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;
            Expander expander = value as Expander;

            if ((expander != null) && (destinationType == typeof(string)))
            {
                // result = borderStyle.ToString();
                result = "Visual Image Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(VisualImageConverter))]
    public class ObjectVisualImageWrapper
    {
        #region Constructors

        public ObjectVisualImageWrapper()
        {
        }

        public ObjectVisualImageWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}