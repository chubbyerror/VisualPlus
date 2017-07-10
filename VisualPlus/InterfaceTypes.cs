namespace VisualPlus
{
    #region Namespace

    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Managers;
    using VisualPlus.Structure;

    #endregion

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