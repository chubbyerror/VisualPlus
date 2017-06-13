namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Forms;

    using VisualPlus.Controls;

    #endregion

    public enum MouseStates
    {
        /// <summary>Normal state.</summary>
        Normal,

        /// <summary>Hover state.</summary>
        Hover,

        /// <summary>Down state.</summary>
        Down
    }

    [Description("The state of the mouse on the control.")]
    public class MouseState
    {
        #region Variables

        private MouseStates mouseState = MouseStates.Normal;

        #endregion

        #region Constructors

        public MouseState(Control control)
        {
            control.MouseDown += OnMouseDown;
            control.MouseEnter += OnMouseEnter;
            control.MouseLeave += OnMouseLeave;
            control.MouseUp += OnMouseUp;

            // Specific controls might need to ignore some events
            if (!(control is VisualCheckBox))
            {
                // Add here
            }
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The state of the control.")]
        public MouseStates ControlState
        {
            get
            {
                return mouseState;
            }

            set
            {
                mouseState = value;
            }
        }

        #endregion

        #region Events

        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            mouseState = MouseStates.Down;
        }

        protected virtual void OnMouseEnter(object sender, EventArgs e)
        {
            mouseState = MouseStates.Hover;
        }

        protected virtual void OnMouseLeave(object sender, EventArgs e)
        {
            mouseState = MouseStates.Normal;
        }

        protected virtual void OnMouseUp(object sender, MouseEventArgs e)
        {
            mouseState = MouseStates.Hover;
        }

        #endregion
    }

    //public class MouseStateConverter : ExpandableObjectConverter
    //{
    //    #region Events

    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    //    {
    //        var stringValue = value as string;

    //        if (stringValue != null)
    //        {
    //            return new ObjectMouseStateWrapper(stringValue);
    //        }

    //        return base.ConvertFrom(context, culture, value);
    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    //    {
    //        MouseState mouseState;
    //        object result;

    //        result = null;
    //        mouseState = value as MouseState;

    //        if (mouseState != null && destinationType == typeof(string))
    //        {
    //            // result = borderStyle.ToString();
    //            result = "MouseState Settings";
    //        }

    //        return result ?? base.ConvertTo(context, culture, value, destinationType);
    //    }

    //    #endregion
    //}

    //[TypeConverter(typeof(MouseStateConverter))]
    //public class ObjectMouseStateWrapper
    //{
    //    #region Constructors

    //    public ObjectMouseStateWrapper()
    //    {
    //    }

    //    public ObjectMouseStateWrapper(string value)
    //    {
    //        Value = value;
    //    }

    //    #endregion

    //    #region Properties

    //    public object Value { get; set; }

    //    #endregion
    //}
}