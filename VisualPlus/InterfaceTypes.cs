namespace VisualPlus
{
    #region Namespace

    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Handlers;
    using VisualPlus.Structure;

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

    public interface IAnimate
    {
        #region Properties

        bool Animation { get; set; }

        #endregion

        #region Events

        void ConfigureAnimation();

        void DrawAnimation(Graphics graphics);

        #endregion
    }

    public interface IControlState
    {
        #region Properties

        Gradient DisabledGradient { get; }

        Gradient EnabledGradient { get; }

        #endregion
    }

    public interface IControlStates : IControlState
    {
        #region Properties

        Gradient HoverGradient { get; }

        Gradient PressedGradient { get; }

        #endregion
    }
}