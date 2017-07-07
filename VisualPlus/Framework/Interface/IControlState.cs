namespace VisualPlus.Framework.Interface
{
    #region Namespace

    using VisualPlus.Framework.Structure;

    #endregion

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