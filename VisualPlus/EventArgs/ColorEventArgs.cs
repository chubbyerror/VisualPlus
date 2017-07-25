namespace VisualPlus.EventArgs
{
    #region Namespace

    using System;
    using System.Drawing;

    #endregion

    public class ColorEventArgs : EventArgs
    {
        #region Variables

        private Color _color;

        #endregion

        #region Constructors

        public ColorEventArgs(Color color)
        {
            _color = color;
        }

        #endregion

        #region Properties

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        #endregion
    }
}