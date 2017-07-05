namespace VisualPlus.Framework
{
    #region Namespace

    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Styles;

    #endregion

    public interface IContainedInputControl
    {
        #region Properties

        Control ContainedControl { get; }

        #endregion
    }

    public interface IControlStyle
    {
        #region Properties

        StyleManager StyleManager { get; set; }

        #endregion
    }

    // For controls with all the control state support
    public interface IControlMouseStates
    {
        #region Properties

        IControlState ControlStates { get; }

        MouseStates MouseState { get; }

        #endregion
    }

    // For controls like input fields with no down state support, mostly for border
    public interface IMouseState
    {
        #region Properties

        Color BorderColor { get; }

        Color BorderHoverColor { get; }

        bool Hover { get; }

        #endregion
    }
}