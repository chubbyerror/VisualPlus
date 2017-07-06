namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Toolkit.Controls;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public abstract class ContainerBase : VisualControlBase
    {
        #region Variables

        private Color _backgroundColor;

        #endregion

        #region Constructors

        protected ContainerBase()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            _backgroundColor = StyleManager.ControlStyle.Background(0);
        }

        public delegate void BackgroundChangedEventHandler();

        [Category(Localize.EventsCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public event BackgroundChangedEventHandler BackgroundChanged;

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

        #endregion

        #region Events

        /// <summary>Apply BackColor change on the container and it's child controls.</summary>
        /// <param name="container">The container control.</param>
        /// <param name="backgroundColor">The container backgroundColor.</param>
        protected virtual void ApplyContainerBackColorChange(Control container, Color backgroundColor)
        {
            foreach (object control in container.Controls)
            {
                if (control != null)
                {
                    ((Control)control).BackColor = backgroundColor;
                }
            }
        }

        protected virtual void OnBackgroundChanged()
        {
            ApplyContainerBackColorChange(this, _backgroundColor);
            BackgroundChanged?.Invoke();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            SetControlBackColor(e.Control, _backgroundColor, false);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            SetControlBackColor(e.Control, _backgroundColor, true);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        /// <summary>Set's the container controls BackColor.</summary>
        /// <param name="control">Current control.</param>
        /// <param name="backgroundColor">Container background color.</param>
        /// <param name="onControlRemoved">Control removed?</param>
        protected virtual void SetControlBackColor(Control control, Color backgroundColor, bool onControlRemoved)
        {
            Color backColor;

            if (onControlRemoved)
            {
                backColor = Color.Transparent;

                // Bug: The Control doesn't support transparent background
                if (control is VisualProgressIndicator)
                {
                    backColor = SystemColors.Control;
                }
            }
            else
            {
                backColor = backgroundColor;
            }

            control.BackColor = backColor;
        }

        #endregion
    }
}