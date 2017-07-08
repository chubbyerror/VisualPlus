namespace VisualPlus.Framework.Interface
{
    using System.Drawing;

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
}