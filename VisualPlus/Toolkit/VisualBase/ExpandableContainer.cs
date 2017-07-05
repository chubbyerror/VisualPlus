namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ExpandableContainer : ContainerDrag
    {
        #region Variables

        private Expandable _expandable;

        #endregion

        #region Constructors

        public delegate void ToggleChangedEventHandler();

        [Category(Localize.EventsCategory.Layout)]
        [Description(Localize.EventDescription.ToggleExpanderChanged)]
        public event ToggleChangedEventHandler ToggleExpanderChanged;

        #endregion

        #region Properties

        [TypeConverter(typeof(ExpandableConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Expandable Expander
        {
            get
            {
                return _expandable;
            }

            set
            {
                _expandable = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_expandable.MouseOnButton)
            {
                _expandable.Expanded = !_expandable.Expanded;
                ToggleExpanderChanged?.Invoke();
            }
            else
            {
                Focus();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_expandable.Visible)
            {
                _expandable.GetMouseOnButton(e.Location);
                Cursor = _expandable.MouseOnButton ? _expandable.Cursor : Cursors.Default;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _expandable?.UpdateOriginal(Size);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _expandable?.UpdateOriginal(Size);
        }

        #endregion
    }
}