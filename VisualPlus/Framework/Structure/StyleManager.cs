namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Globalization;

    using VisualPlus.Controls;

    #endregion

    [TypeConverter(typeof(VisualStyleManagerConverter))]
    [Description("The Visual Style Manager type.")]
    public class StyleManager
    {
        #region Variables

        private bool lockedStyle;
        private VisualStylesManager visualStylesManager;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The lock the control to the style to prevent changes.")]
        public bool LockedStyle
        {
            get
            {
                return lockedStyle;
            }

            set
            {
                lockedStyle = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The Visual Style Manager to hook into.")]
        public VisualStylesManager VisualStylesManager
        {
            get
            {
                return visualStylesManager;
            }

            set
            {
                visualStylesManager = value;
            }
        }

        #endregion
    }

    public class VisualStyleManagerConverter : ExpandableObjectConverter
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
                return new ObjectVisualStyleManagerWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;
            VisualStylesManager visualStyleManager = value as VisualStylesManager;

            if ((visualStyleManager != null) && (destinationType == typeof(string)))
            {
                // result = borderStyle.ToString();
                result = "StyleManager Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(VisualStyleManagerConverter))]
    public class ObjectVisualStyleManagerWrapper
    {
        #region Constructors

        public ObjectVisualStyleManagerWrapper()
        {
        }

        public ObjectVisualStyleManagerWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}