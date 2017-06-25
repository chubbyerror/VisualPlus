namespace VisualPlus.Enums
{
    #region Namespace

    using System;
    using System.ComponentModel;

    using VisualPlus.Styles;

    #endregion

    public class Styles
    {
        #region Constructors

        public enum Style
        {
            // Note: If you implement more styles or your own styles, just add enumeration here.

            /// <summary>The visual.</summary>
            [Description("VisualPlus Style")]
            Visual = 0,

            /// <summary>The black and yellow.</summary>
            [Description("In Rush")]
            BlackAndYellow = 1
        }

        #endregion

        #region Events

        /// <summary>Returns the interface style</summary>
        /// <param name="style">The Style.</param>
        /// <returns>The interface style.</returns>
        public static object GetInterfaceObject(Style style)
        {
            object interfaceObject;

            switch (style)
            {
                case Style.Visual:
                    {
                        interfaceObject = new Visual();
                        break;
                    }

                case Style.BlackAndYellow:
                    {
                        interfaceObject = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return interfaceObject;
        }

        #endregion
    }
}