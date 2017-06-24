namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    [TypeConverter(typeof(BorderConverter))]
    public class Border : IBorder
    {
        #region Variables

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
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color HoverColor
        {
            get
            {
                return hoverColor;
            }

            set
            {
                hoverColor = value;
            }
        }

        #endregion

        #region Variables

        private Color color = Settings.DefaultValue.Border.Color;
        private Color hoverColor = Settings.DefaultValue.Border.HoverColor;
        private bool hoverVisible = true;
        private int rounding = Settings.DefaultValue.Rounding.Default;
        private BorderType type = Settings.DefaultValue.BorderShape;
        private int thickness = Settings.DefaultValue.BorderThickness;
        private bool visible = true;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Visible)]
        public bool HoverVisible
        {
            get
            {
                return hoverVisible;
            }

            set
            {
                hoverVisible = value;
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
                }
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Border.Shape)]
        public BorderType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
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
                }
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
            }
        }

        #endregion
    }

    public class BorderConverter : ExpandableObjectConverter
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
                return new ObjectBorderWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Border border;
            object result;

            result = null;
            border = value as Border;

            if (border != null && destinationType == typeof(string))
            {
                // result = borderStyle.ToString();
                result = "Border Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(BorderConverter))]
    public class ObjectBorderWrapper
    {
        #region Constructors

        public ObjectBorderWrapper()
        {
        }

        public ObjectBorderWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}