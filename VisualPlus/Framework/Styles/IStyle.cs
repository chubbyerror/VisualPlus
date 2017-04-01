namespace VisualPlus.Framework.Styles
{
    using System.Drawing;

    public interface IStyle
    {
        #region ${0} Properties

        Color BackgroundProgressCircle { get; }

        Color ButtonDownColor { get; }

        Color ButtonHoverColor { get; }

        Color ButtonNormalColor { get; }

        Color DropDownButtonColor { get; }

        Color DropDownColor { get; }

        Color ForegroundProgressCircle { get; }

        Color HatchColor { get; }

        Color ItemDisableBackgroundColor { get; }

        Color LineColor { get; }

        Color MainColor { get; }

        Color ProgressColor { get; }

        Color ShadowColor { get; }

        Style StyleManagement { get; }
        
        Color TabHover { get; }

        Color TabMenu { get; }

        Color TabNormal { get; }

        Color TabSelected { get; }

        Color TabTextNormal { get; }

        Color TabTextSelected { get; }

        Color TextDisabled { get; }

        Color ControlDisabled { get; }

        #endregion

        #region ${0} Methods

        Color ItemNormal(int depth);

        Color ItemHover(int depth);

        Color BackgroundColor(int depth);

        Color BorderColor(int depth);

        #endregion
    }
}