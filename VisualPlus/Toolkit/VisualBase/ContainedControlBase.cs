namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using VisualPlus.Enumerators;
    using VisualPlus.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ContainedControlBase : SimpleBase
    {
        #region Constructors

        protected ContainedControlBase()
        {
            Background = StyleManager.ControlStyle.Background(3);
        }

        #endregion

        #region Events

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            MouseState = MouseStates.Hover;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            MouseState = MouseStates.Normal;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
        }

        /// <summary>Gets the internal control location.</summary>
        /// <param name="shape">The shape of the container control.</param>
        /// <returns>The internal control location.</returns>
        internal Point GetInternalControlLocation(Shape shape)
        {
            return new Point((shape.Rounding / 2) + shape.Thickness + 1, (shape.Rounding / 2) + shape.Thickness + 1);
        }

        /// <summary>Gets the internal control size.</summary>
        /// <param name="size">The size of the container control.</param>
        /// <param name="shape">The shape of the container control.</param>
        /// <returns>The internal control size.</returns>
        internal Size GetInternalControlSize(Size size, Shape shape)
        {
            return new Size(size.Width - shape.Rounding - shape.Thickness - 3, size.Height - shape.Rounding - shape.Thickness - 3);
        }

        #endregion
    }
}