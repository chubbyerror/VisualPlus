namespace VisualPlus.Controls.Bases
{
    #region Namespace

    using System.ComponentModel;
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
    public abstract class ContainerBase : ControlBase
    {
        #region Variables

        private Color backgroundColor;

        #endregion

        #region Constructors

        protected ContainerBase()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeTheme();
        }

        public delegate void BackgroundChangedEventHandler();

        [Category(Localize.EventsCategory.Appearance)]
        public event BackgroundChangedEventHandler BackgroundChanged;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color Background
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
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

        #endregion

        #region Events

        /// <summary>Fires the OnBackgroundChanged event.</summary>
        protected virtual void OnBackgroundChanged()
        {
            ExceptionManager.ApplyContainerBackColorChange(this, backgroundColor);
            BackgroundChanged?.Invoke();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            ExceptionManager.SetControlBackColor(e.Control, backgroundColor, false);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            ExceptionManager.SetControlBackColor(e.Control, backgroundColor, true);
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
                backgroundColor = controlStyle.Background(0);
            }
            else
            {
                backgroundColor = Settings.DefaultValue.Control.Background(0);
            }
        }

        #endregion
    }
}