namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Toolkit.Delegates;
    using VisualPlus.Toolkit.EventArgs;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ContainerDrag : ContainerBase
    {
        #region Variables

        private Cursor _cursorMove;
        private bool _movable;

        #endregion

        #region Constructors

        protected ContainerDrag()
        {
            _cursorMove = Cursors.SizeAll;
            _movable = Settings.DefaultValue.Moveable;
        }

        [Category(Localize.EventsCategory.DragDrop)]
        [Description(Localize.EventDescription.ControlDragChanged)]
        public event ControlDragEventHandler ControlDrag;

        #endregion

        #region Properties

        [DefaultValue(typeof(Cursor), "Hand")]
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Cursor)]
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

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
        public bool Movable
        {
            get
            {
                return _movable;
            }

            set
            {
                _movable = value;
            }
        }

        #endregion

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_movable)
            {
                LastPosition = e.Location;
                Cursor = _cursorMove;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_movable && (e.Button == MouseButtons.Left))
            {
                Left += e.Location.X - LastPosition.X;
                Top += e.Location.Y - LastPosition.Y;

                ControlDrag?.Invoke(new ControlDragEventArgs(e.Location));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_movable)
            {
                Cursor = DefaultCursor;
            }
        }

        /// <summary>Toggle the control move event cursors.</summary>
        /// <param name="movable">Movable toggle.</param>
        /// <param name="cursorMove">The move cursor.</param>
        /// <param name="defaultCursor">The default cursor.</param>
        /// <returns>The new cursor type.</returns>
        private static Cursor ToggleMoveCursor(bool movable, Cursor cursorMove, Cursor defaultCursor)
        {
            return movable ? cursorMove : defaultCursor;
        }

        #endregion
    }
}