namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Forms;

    using VisualPlus.Controls;
    using VisualPlus.Localization;

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

        private MouseStates mouseState = MouseStates.Normal;

        #endregion

        #region Constructors

        public MouseState(Control control)
        {
            if (control is VisualButton)
            {
                control.MouseDown += OnMouseDown;
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
                control.MouseUp += OnMouseUp;
            }

            if (control is VisualCheckBox)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }

            if (control is VisualColorPicker)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }

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

            if (control is VisualGroupBox)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }

            if (control is VisualKnob)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }

            if (control is VisualNumericUpDown)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
            }

            // Specific controls might need to ignore some events
            //if (!(control is VisualCheckBox))
            //{
            //    // Add here
            //}

            //if (control is VisualNumericUpDown)
            //{
            //    control.MouseEnter += OnMouseEnter;
            //    control.MouseLeave += OnMouseLeave;

            // //   control.Invalidate();

            //    // TODO: Doesn't seem to be registering the OnMouseLeave() event Invalidate() after on the control properly.
            //}

            //if (control is VisualRichTextBox)
            //{
            //    control.Enter += OnMouseEnter;
            //  //  control.Leave += OnMouseLeave;
            //}

            //if (control is VisualTextBox)
            //{
            //    control.Enter += OnMouseEnter;
            //   // control.Leave += OnMouseLeave;
            //}
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

        protected virtual void OnLostFocus(object sender, EventArgs e)
        {
            mouseState = MouseStates.Normal;
        }
        #endregion
    }
}