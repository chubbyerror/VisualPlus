namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    using VisualPlus.Localization;

    #endregion

    [TypeConverter(typeof(GradientConverter))]
    [Description(Localize.Description.Border)]
    public class Gradient
    {
        #region Variables

        private float angle;
        private Color[] colors =
            {
                Color.Red,
                Color.Green,
                Color.Blue
            };

        private float[] positions = { 0, 1 / 2f, 1 };
        private Point endPoint;
        private Point startPoint;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The gradient angle.")]
        public float Angle
        {
            get
            {
                return angle;
            }

            set
            {
                angle = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The gradient colors.")]
        public Color[] Colors
        {
            get
            {
                return colors;
            }

            set
            {
                colors = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The color positioning of the gradient.")]
        public float[] Positions
        {
            get
            {
                return positions;
            }

            set
            {
                positions = value;
            }
        }

        #endregion
    }

    public class GradientConverter : ExpandableObjectConverter
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
                return new ObjectGradientWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Gradient gradient;
            object result;

            result = null;
            gradient = value as Gradient;

            if (gradient != null && destinationType == typeof(string))
            {
                // result = borderStyle.ToString();
                result = "Gradient Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(GradientConverter))]
    public class ObjectGradientWrapper
    {
        #region Constructors

        public ObjectGradientWrapper()
        {
        }

        public ObjectGradientWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}