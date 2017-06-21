namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    using VisualPlus.Localization;
    using VisualPlus.Properties;

    #endregion

    [TypeConverter(typeof(ShapeConverter))]
    public class Shape
    {
        #region Variables

        private Border border = new Border();
        private Gradient disabledGradient = new Gradient();
        private Image disabledImage = Resources.Icon;
        private Gradient enabledGradient = new Gradient();
        private Image enabledImage = Resources.Icon;
        private Gradient hoverGradient = new Gradient();
        private Point imagePoint = new Point(0, 0);
        private Size imageSize = new Size(0, 0);
        private bool imageVisible;
        private Gradient pressedGradient = new Gradient();
        private Size size = new Size(25, 25);

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

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Gradient)]
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
        [Description(Localize.Description.Common.Gradient)]
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
        [Description(Localize.Description.Common.Gradient)]
        public Gradient HoverGradient
        {
            get
            {
                return hoverGradient;
            }

            set
            {
                hoverGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Point)]
        public Point ImageLocation
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
        [Description(Localize.Description.Common.Visible)]
        public bool ImageVisible
        {
            get
            {
                return imageVisible;
            }

            set
            {
                imageVisible = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Gradient)]
        public Gradient PressedGradient
        {
            get
            {
                return pressedGradient;
            }

            set
            {
                pressedGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        #endregion
    }

    public class ShapeConverter : ExpandableObjectConverter
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
                return new ObjectShapeWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Shape shape;
            object result;

            result = null;
            shape = value as Shape;

            if ((shape != null) && (destinationType == typeof(string)))
            {
                result = "Shape Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(ShapeConverter))]
    public class ObjectShapeWrapper
    {
        #region Constructors

        public ObjectShapeWrapper()
        {
        }

        public ObjectShapeWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}