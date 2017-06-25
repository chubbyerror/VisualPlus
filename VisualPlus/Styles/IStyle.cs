namespace VisualPlus.Styles
{
    #region Namespace

    using System.Drawing;

    using VisualPlus.Framework.Structure;

    #endregion

    public interface IBorder
    {
        #region Properties

        Color Color { get; }

        Color HoverColor { get; }

        #endregion
    }

    public interface ICheckmark
    {
        Gradient EnabledGradient { get; }

        Gradient DisabledGradient { get; }
    }

    public interface IControl
    {
        #region Properties

        Gradient BoxDisabled { get; }

        Gradient BoxEnabled { get; }

        Gradient ControlDisabled { get; }

        Gradient ControlEnabled { get; }

        Gradient ControlHover { get; }

        Gradient ControlPressed { get; }

        Color FlatButtonDisabled { get; }

        Color FlatButtonEnabled { get; }

        Color ItemEnabled { get; }

        Color ItemHover { get; }

        Color Line { get; }

        Color Shadow { get; }

        #endregion

        #region Events

        Color Background(int depth);

        #endregion
    }

    public interface IFont
    {
        #region Properties

        FontFamily FontFamily { get; }

        float FontSize { get; }

        FontStyle FontStyle { get; }

        Color ForeColor { get; }

        Color ForeColorDisabled { get; }

        Color ForeColorSelected { get; }

        #endregion
    }

    public interface IProgress
    {
        #region Properties

        Color BackCircle { get; }

        Gradient Background { get; }

        Color ForeCircle { get; }

        Color Hatch { get; }

        Gradient Progress { get; }

        Gradient ProgressDisabled { get; }

        #endregion
    }

    public interface ITab
    {
        #region Properties

        Color Menu { get; }

        Color TabEnabled { get; }

        Color TabHover { get; }

        Color TabSelected { get; }

        #endregion
    }

    public interface IWatermark
    {
        #region Properties

        Color ActiveColor { get; }

        Color InactiveColor { get; }

        #endregion
    }
}