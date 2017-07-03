namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
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

    public class MouseState
    {
        #region Variables

        private MouseStates mouseState;

        #endregion

        #region Constructors

        public MouseState(Control control)
        {
            mouseState = MouseStates.Normal;

            if (control is VisualComboBox)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
                control.LostFocus += OnLostFocus;
            }

            if (control is VisualForm)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }

            if (control is VisualKnob)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }
            
            // Specific controls might need to ignore some events
            // if (!(control is VisualCheckBox))
            // {
            // // Add here
            // }
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.MouseState)]
        public MouseStates State
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

        protected virtual void OnLostFocus(object sender, EventArgs e)
        {
            mouseState = MouseStates.Normal;
        }

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
}