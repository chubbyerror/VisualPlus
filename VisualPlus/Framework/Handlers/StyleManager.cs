namespace VisualPlus.Framework.Handlers
{
    #region Namespace

    using System;
    using System.Drawing;

    using VisualPlus.Enums;
    using VisualPlus.Styles;

    #endregion

    public class StyleManager
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="StyleManager" /> class.</summary>
        /// <param name="style">The style.</param>
        public StyleManager(Styles style)
        {
            DefaultStyle = Settings.DefaultValue.DefaultStyle;

            UpdateStyle(style);

            Font = new Font(FontStyle.FontFamily, FontStyle.FontSize, FontStyle.FontStyle);
        }

        #endregion

        #region Properties

        public IBorder BorderStyle { get; set; }

        public ICheckmark CheckmarkStyle { get; set; }

        public IControlState ControlStatesStyle { get; set; }

        public IControl ControlStyle { get; set; }

        public Styles DefaultStyle { get; }

        public Font Font { get; }

        public IFont FontStyle { get; set; }

        public IProgress ProgressStyle { get; set; }

        public Styles Style { get; set; }

        public ITab TabStyle { get; set; }

        public IWatermark WatermarkStyle { get; set; }

        #endregion

        #region Events

        /// <summary>Updates the style.</summary>
        /// <param name="style">The style.</param>
        public void UpdateStyle(Styles style)
        {
            BorderStyle = (IBorder)GetStyleObject(style);
            CheckmarkStyle = (ICheckmark)GetStyleObject(style);
            ControlStatesStyle = (IControlState)GetStyleObject(style);
            ControlStyle = (IControl)GetStyleObject(style);
            FontStyle = (IFont)GetStyleObject(style);
            ProgressStyle = (IProgress)GetStyleObject(style);
            TabStyle = (ITab)GetStyleObject(style);
            WatermarkStyle = (IWatermark)GetStyleObject(style);
        }

        /// <summary>RGets the style object.</summary>
        /// <param name="styles">The Style.</param>
        /// <returns>The interface style.</returns>
        private static object GetStyleObject(Styles styles)
        {
            object interfaceObject;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        interfaceObject = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
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