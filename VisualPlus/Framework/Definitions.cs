namespace VisualPlus.Framework
{
    #region Namespace

    using System.Windows.Forms;

    using VisualPlus.Framework.Handlers;

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
}