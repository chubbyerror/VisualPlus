namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using VisualPlus.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ContainedControlBase : VisualControlBase
    {
        #region Events

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