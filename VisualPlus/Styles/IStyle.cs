namespace VisualPlus.Styles
{
    #region Namespace

    using System.Drawing;

    using VisualPlus.Enums;

    #endregion

    public interface IStyle
    {
        #region Properties

        Color BackgroundProgressCircle { get; }

        Color ButtonDownColor { get; }

        Color ButtonHoverColor { get; }

        Color ButtonNormalColor { get; }

        Color ControlDisabled { get; }

        Color DropDownButtonColor { get; }

        Color DropDownColor { get; }

        Color ForegroundProgressCircle { get; }

        Color HatchColor { get; }

        Color ItemDisableBackgroundColor { get; }

        Color LineColor { get; }

        Color StyleColor { get; }

        Color ProgressColor { get; }

        Color ShadowColor { get; }

        Styles StyleManagement { get; }

        Color TabHover { get; }

        Color TabMenu { get; }

        Color TabNormal { get; }

        Color TabSelected { get; }

        Color TabTextNormal { get; }

        Color TabTextSelected { get; }

        Color TextDisabled { get; }

        FontFamily FontFamily { get; }

        #endregion

        #region Events

        Color BackgroundColor(int depth);

        Color BorderColor(int depth);

        Color ForeColor(int depth);

        Color ItemHover(int depth);

        Color ItemNormal(int depth);

        #endregion
    }
}