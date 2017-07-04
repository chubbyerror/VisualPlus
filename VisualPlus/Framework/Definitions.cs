namespace VisualPlus.Framework
{
    #region Namespace

    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;

    #endregion

    public interface IContainedInputControl
    {
        #region Properties

        Control ContainedControl { get; }

        #endregion
    }

    public interface IContainerStyle
    {
        StyleManager StyleManager { get; set; }
    }

    // For controls with all the control state support
    public interface IMouseControlStates
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

        bool Hover { get; }

        #endregion
    }
}