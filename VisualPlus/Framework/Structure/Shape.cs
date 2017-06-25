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

        // iShape < -- >
        private Gradient disabledGradient;
        private Gradient enabledGradient;
        private Gradient hoverGradient;
        private Gradient pressedGradient;

        private Size size;
        private Point point;
        private VisualBitmap visualBitmap;

        #endregion

        #region Constructors

        public Shape()
        {
            disabledGradient = Settings.DefaultValue.Control.ControlDisabled;
            enabledGradient = Settings.DefaultValue.Control.ControlEnabled;
            hoverGradient = Settings.DefaultValue.Control.ControlHover;
            pressedGradient = Settings.DefaultValue.Control.ControlPressed;

            visualBitmap = new VisualBitmap(Resources.Icon, new Size(25, 25));
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

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
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
        public VisualBitmap Image
        {
            get
            {
                return visualBitmap;
            }

            set
            {
                visualBitmap = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
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
        [Description(Localize.Description.Common.Point)]
        public Point Location
        {
            get
            {
                return point;
            }

            set
            {
                point = value;
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