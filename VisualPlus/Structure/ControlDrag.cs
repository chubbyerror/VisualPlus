namespace VisualPlus.Framework.Structure
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    public class ControlDrag
    {

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

        private static Size mouseOffset;

        // TKey is control to drag, TValue is a flag used while dragging
        private Dictionary<Control, bool> moveableDictionary = new Dictionary<Control, bool>();
    }
}