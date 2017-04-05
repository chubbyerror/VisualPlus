namespace VisualPlus.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Framework;

    /// <summary>The visual ContextMenuStrip.</summary>
    [ToolboxBitmap(typeof(ContextMenuStrip)), Designer(VSDesignerBinding.VisualContextMenu)]
    public partial class VisualContextMenu : Form
    {
        #region  ${0} Variables

        public bool mouseIsHovering;

        #endregion

        #region ${0} Properties

        public VisualContextMenu(Bitmap defaultFile, Bitmap hoverFile)
        {
            Children = new List<MenuItem>(0);
            DefaultImage = defaultFile;
            HoverImage = hoverFile;
            mouseIsHovering = false;

            MouseEnter += VisualContextMenu_MouseEnter;
            MouseLeave += VisualContextMenu_MouseLeave;
            MouseClick += VisualContextMenu_MouseClick;
        }

        public List<MenuItem> Children { get; private set; }

        public Bitmap DefaultImage { get; private set; }

        public Bitmap HoverImage { get; private set; }

        #endregion

        #region ${0} Events

        private void VisualContextMenu_MouseClick(object sender, EventArgs e)
        {
            // draw all the children
        }

        private void VisualContextMenu_MouseEnter(object sender, EventArgs e)
        {
            mouseIsHovering = true;

            // switch to hovering image
        }

        private void VisualContextMenu_MouseLeave(object sender, EventArgs e)
        {
            mouseIsHovering = false;
            var childIsBeingViewed = false;

            foreach (MenuItem mi in Children)
            {
                //if (mi.mouseIsHovering == true)
                //{
                //    childIsBeingViewed = true;
                //    break;
                //}
            }

            if (!childIsBeingViewed)
            {
                // stop displaying all children and this object reverts to defalut image
            }
        }

        #endregion

        #region ${0} Methods

        public void addChild(MenuItem m)
        {
            Children.Add(m);
        }

        #endregion

        // private Color buttonNormal = style.ButtonNormalColor;
        // private Color buttonPressed = ControlPaint.Light(style.ButtonDownColor);
        // private Color itemHover = ControlPaint.Light(style.ButtonNormalColor);

        // private Color textDisabled = style.TextDisabled;
    }
}