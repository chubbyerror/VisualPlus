namespace VisualPlus.Controls.Bases
{
    #region Namespace

    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public abstract class ContainerBase : ControlBase
    {
        #region Variables

        private Point _lastPosition;
        private bool _movable;
        private Color _backgroundColor;

        #endregion

        #region Constructors

        protected ContainerBase()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            _movable = Settings.DefaultValue.Moveable;
            InitializeTheme();
        }

        public delegate void BackgroundChangedEventHandler();

        public delegate void ControlMovedEventHandler();

        [Category(Localize.EventsCategory.Appearance)]
        public event BackgroundChangedEventHandler BackgroundChanged;

        [Category(Localize.EventsCategory.Behavior)]
        public event ControlMovedEventHandler ControlMoved;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color Background
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
                OnBackgroundChanged();
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return ControlBorder;
            }

            set
            {
                ControlBorder = value;
                Invalidate();
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
                Cursor = _movable ? Cursors.Hand : Cursors.Default;
            }
        }

        #endregion

        #region Events

        /// <summary>Fires the OnBackgroundChanged event.</summary>
        protected virtual void OnBackgroundChanged()
        {
            ExceptionManager.ApplyContainerBackColorChange(this, _backgroundColor);
            BackgroundChanged?.Invoke();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            ExceptionManager.SetControlBackColor(e.Control, _backgroundColor, false);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            ExceptionManager.SetControlBackColor(e.Control, _backgroundColor, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_movable)
            {
                _lastPosition = e.Location;
                Cursor = Cursors.SizeAll;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_movable && (e.Button == MouseButtons.Left))
            {
                Left += e.Location.X - _lastPosition.X;
                Top += e.Location.Y - _lastPosition.Y;

                ControlMoved?.Invoke();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_movable)
            {
                Cursor = Cursors.Hand;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (StyleManager.LockedStyle)
            {
                InitializeTheme();
            }
        }

        private void InitializeTheme()
        {
            if (StyleManager.VisualStylesManager != null)
            {
                IControl controlStyle = StyleManager.VisualStylesManager.VisualStylesInterface.ControlStyle;
                _backgroundColor = controlStyle.Background(0);
            }
            else
            {
                _backgroundColor = Settings.DefaultValue.Control.Background(0);
            }
        }

        #endregion
    }
}