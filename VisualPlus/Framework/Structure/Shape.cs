namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Delegates;
    using VisualPlus.Framework.Handlers;

    #endregion

    [Description("The shape.")]
    [TypeConverter(typeof(ShapeConverter))]
    public class Shape
    {
        #region Variables

        private Color color;
        private int rounding;
        private int thickness;
        private ShapeType type;
        private bool visible;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Shape" /> class.</summary>
        public Shape()
        {
            StyleManager styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

            color = styleManager.BorderStyle.Color;
            rounding = Settings.DefaultValue.Rounding.Default;
            thickness = Settings.DefaultValue.BorderThickness;
            type = ShapeType.Rounded;
            visible = true;
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the color has been changed.")]
        public event BorderColorChangedEventHandler ColorChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the border rounding has been changed.")]
        public event BorderRoundingChangedEventHandler RoundingChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the border thickness changed.")]
        public event BorderThicknessChangedEventHandler ThicknessChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the border type changed.")]
        public event BorderTypeChangedEventHandler TypeChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the border visible changed.")]
        public event BorderVisibleChangedEventHandler VisibleChanged;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
                ColorChanged?.Invoke();
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Border.Rounding)]
        public int Rounding
        {
            get
            {
                return rounding;
            }

            set
            {
                if (ExceptionManager.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    rounding = value;
                    RoundingChanged?.Invoke();
                }
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Border.Thickness)]
        public int Thickness
        {
            get
            {
                return thickness;
            }

            set
            {
                if (ExceptionManager.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    thickness = value;
                    ThicknessChanged?.Invoke();
                }
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Border.Shape)]
        public ShapeType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                TypeChanged?.Invoke();
            }
        }

        [NotifyParentProperty(true)]
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
                VisibleChanged?.Invoke();
            }
        }

        #endregion
    }

    public class ShapeConverter : ExpandableObjectConverter
    {
        #region Events

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (stringValue != null)
            {
                return new ObjectBorderShapeWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Shape shape;
            object result;

            result = null;
            shape = value as Shape;

            if (shape != null && destinationType == typeof(string))
            {
                // result = borderStyle.ToString();
                result = "Border Shape Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(ShapeConverter))]
    public class ObjectBorderShapeWrapper
    {
        #region Constructors

        public ObjectBorderShapeWrapper()
        {
        }

        public ObjectBorderShapeWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}