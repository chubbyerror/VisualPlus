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

    [TypeConverter(typeof(VisualBitmapConverter))]
    public class VisualBitmap
    {
        #region Variables

        private Border border;
        private Bitmap image;
        private Point imagePoint;
        private Size imageSize;
        private bool visible;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="VisualBitmap" /> class.</summary>
        /// <param name="bitmap">The image bitmap.</param>
        /// <param name="bitmapSize">The size of the bitmap.</param>
        public VisualBitmap(Bitmap bitmap, Size bitmapSize)
        {
            border = new Border
                {
                    Visible = false,
                    HoverVisible = false
                };

            imagePoint = new Point();
            visible = false;

            image = bitmap;
            imageSize = bitmapSize;
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
        public Bitmap Image
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

        /// <summary>Draws the bitmap image.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="_border">The border.</param>
        /// <param name="_imagePoint">The location.</param>
        /// <param name="_image">The image.</param>
        /// <param name="_imageSize">The size.</param>
        /// <param name="_visible">The visibility.</param>
        public static void DrawImage(Graphics graphics, Border _border, Point _imagePoint, Bitmap _image, Size _imageSize, bool _visible)
        {
            using (GraphicsPath imagePath = new GraphicsPath())
            {
                imagePath.AddRectangle(new Rectangle(_imagePoint, _imageSize));

                if (_border.Visible)
                {
                    GDI.DrawBorder(graphics, imagePath, _border.Thickness, _border.Color);
                }
            }

            if (_visible)
            {
                graphics.DrawImage(_image, new Rectangle(_imagePoint, _imageSize));
            }
        }

        /// <summary>Returns the size of the image.</summary>
        /// <param name="_image">The image.</param>
        /// <returns>The size.</returns>
        public Size GetOriginalSize(Image _image)
        {
            return _image.Size;
        }

        #endregion
    }

    public class VisualBitmapConverter : ExpandableObjectConverter
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
                return new ObjectVisualBitmapWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            VisualBitmap visualBitmap;
            object result;

            result = null;
            visualBitmap = value as VisualBitmap;

            if ((visualBitmap != null) && (destinationType == typeof(string)))
            {
                result = "Image Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(VisualBitmapConverter))]
    public class ObjectVisualBitmapWrapper
    {
        #region Constructors

        public ObjectVisualBitmapWrapper()
        {
        }

        public ObjectVisualBitmapWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}