namespace VisualPlus.Delegates
{
    #region Namespace

    using VisualPlus.EventArgs;

    #endregion

    public delegate void ForeColorDisabledChangedEventHandler();

    public delegate void MouseStateChangedEventHandler();

    public delegate void StyleManagerChangedEventHandler();

    public delegate void TextRenderingChangedEventHandler();

    public delegate void BackgroundChangedEventHandler(ColorEventArgs e);
}