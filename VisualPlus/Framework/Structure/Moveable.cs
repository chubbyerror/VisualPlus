namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    public static class Moveable
    {
        #region Variables

        private static Size mouseOffset;

        // TKey is control to drag, TValue is a flag used while dragging
        private static Dictionary<Control, bool> moveableDictionary = new Dictionary<Control, bool>();

        #endregion

        #region Events

        /// <summary>Toggle move for control.</summary>
        /// <param name="control">The control.</param>
        /// <param name="enable">The toggle.</param>
        public static void MoveToggle(this Control control, bool enable)
        {
            if (enable)
            {
                if (moveableDictionary.ContainsKey(control))
                {
                    // return if control is already draggable
                    return;
                }

                // 'false' - initial state is 'not dragging'
                moveableDictionary.Add(control, false);

                control.MouseDown += ControlMouseDown;
                control.MouseUp += ControlMouseUp;
                control.MouseMove += ControlMouseMove;
            }
            else
            {
                if (!moveableDictionary.ContainsKey(control))
                {
                    // return if control is not draggable
                    return;
                }

                control.MouseDown -= ControlMouseDown;
                control.MouseUp -= ControlMouseUp;
                control.MouseMove -= ControlMouseMove;
                moveableDictionary.Remove(control);
            }
        }

        /// <summary>Control mouse down event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private static void ControlMouseDown(object sender, MouseEventArgs e)
        {
            mouseOffset = new Size(e.Location);

            // Enable move
            moveableDictionary[(Control)sender] = true;
        }

        /// <summary>Control mouse move event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private static void ControlMouseMove(object sender, MouseEventArgs e)
        {
            // If moving is turned on calculate control's new position
            if (moveableDictionary[(Control)sender])
            {
                Point newLocationOffset = e.Location - mouseOffset;
                ((Control)sender).Left += newLocationOffset.X;
                ((Control)sender).Top += newLocationOffset.Y;
            }
        }

        /// <summary>Control mouse up event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private static void ControlMouseUp(object sender, MouseEventArgs e)
        {
            // Disable move
            moveableDictionary[(Control)sender] = false;
        }

        #endregion
    }
}