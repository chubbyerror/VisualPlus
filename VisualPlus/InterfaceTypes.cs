namespace VisualPlus
{
    #region Namespace

    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Managers;
    using VisualPlus.Structure;

    #endregion

    /// <summary>Exposes access to content values.</summary>
    public interface IContentValues
    {
        #region Events

        /// <summary>Gets the content long text.</summary>
        /// <returns>String value.</returns>
        string GetLongText();

        /// <summary>Gets the content short text.</summary>
        /// <returns>String value.</returns>
        string GetShortText();

        #endregion
    }

    public interface IInputMethods
    {
        #region Events

        void AppendText(string text);

        void Clear();

        void ClearUndo();

        void Copy();

        void Cut();

        void DeselectAll();

        int GetCharFromPosition(Point pt);

        int GetCharIndexFromPosition(Point pt);

        int GetFirstCharIndexFromLine(int lineNumber);

        int GetLineFromCharIndex(int index);

        Point GetPositionFromCharIndex(int index);

        void Paste();

        void ScrollToCaret();

        void Select(int start, int length);

        void SelectAll();

        void Undo();

        #endregion
    }

    public interface IContainedControl
    {
        #region Properties

        Border Border { get; set; }

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

    public interface IImageControl
    {
        #region Properties

        Image Hover { get; set; }

        Image Normal { get; set; }

        Image Pressed { get; set; }

        #endregion
    }
}