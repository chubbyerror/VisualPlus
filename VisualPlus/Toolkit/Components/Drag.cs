namespace VisualPlus.Toolkit.Components
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using VisualPlus.Delegates;
    using VisualPlus.EventArgs;

    #endregion

    [Description("Drag component.")]
    [TypeConverter(typeof(DragConverter))]
    public class Drag
    {
        #region Variables

        private Control _control;
        private Cursor _cursorMove;
        private bool _enabled;
        private Point _lastPosition;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Drag" /> class.</summary>
        /// <param name="control">The control to attach.</param>
        /// <param name="enabled">Dragging enabled state.</param>
        public Drag(Control control, bool enabled)
        {
            _cursorMove = Cursors.SizeAll;
            _control = control;
            _enabled = enabled;

            AttachEvents();
        }

        [Category(Localize.EventsCategory.DragDrop)]
        [Description(Localize.EventDescription.ControlDragChanged)]
        public event ControlDragEventHandler ControlDrag;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Cursor CursorMove
        {
            get
            {
                return _cursorMove;
            }

            set
            {
                _cursorMove = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
            }
        }

        [Description("The current drag state of the control.")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDragging { get; private set; }

        #endregion

        #region Events

        /// <summary>Attach the extension events to the control.</summary>
        public void AttachEvents()
        {
            _control.MouseDown += ControlMouseDown;
            _control.MouseMove += ControlMouseMove;
            _control.MouseUp += ControlMouseUp;
        }

        /// <summary>Detach the extension events to the control.</summary>
        public void DetachEvents()
        {
            _control.MouseDown -= ControlMouseDown;
            _control.MouseMove -= ControlMouseMove;
            _control.MouseUp -= ControlMouseUp;
        }

        /// <summary>Control mouse down event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void ControlMouseDown(object sender, MouseEventArgs e)
        {
            if (_enabled)
            {
                _lastPosition = e.Location;
                _control.Cursor = CursorMove;
            }
        }

        /// <summary>Control mouse move event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void ControlMouseMove(object sender, MouseEventArgs e)
        {
            if (_enabled && (e.Button == MouseButtons.Left))
            {
                _control.Left += e.Location.X - _lastPosition.X;
                _control.Top += e.Location.Y - _lastPosition.Y;
                _control.Cursor = _cursorMove;
                IsDragging = true;
                ControlDrag?.Invoke(new DragControlEventArgs(e.Location));
            }
        }

        /// <summary>Control mouse up event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void ControlMouseUp(object sender, MouseEventArgs e)
        {
            if (_enabled)
            {
                _control.Cursor = Cursors.Default;
            }
        }

        #endregion
    }

    public class DragConverter : ExpandableObjectConverter
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
                return new ObjectDragWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;
            Drag drag = value as Drag;

            if ((drag != null) && (destinationType == typeof(string)))
            {
                // result = borderStyle.ToString();
                result = "Drag Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(DragConverter))]
    public class ObjectDragWrapper
    {
        #region Constructors

        public ObjectDragWrapper()
        {
        }

        public ObjectDragWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}