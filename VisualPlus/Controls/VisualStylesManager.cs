namespace VisualPlus.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Localization;

    public class VisualStylesManager : Component
    {
        #region  ${0} Variables

        public List<string> StylesList = new List<string>();

        private Styles currentStyle = Settings.DefaultValue.CurrentStyle;

        private Color styleColor = Settings.DefaultValue.Style.StyleColor;

        #endregion

        #region ${0} Properties

        public VisualStylesManager()
        {
            LoadStyles();
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Style)]
        public Styles Style
        {
            get
            {
                return currentStyle;
            }

            set
            {
                currentStyle = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.StyleColor)]
        public Color StyleColor
        {
            get
            {
                return styleColor;
            }

            set
            {
                styleColor = value;
            }
        }

        #endregion

        #region ${0} Events

        /// <summary>Loads styles to a string list.</summary>
        private void LoadStyles()
        {
            StylesList = Enum.GetValues(typeof(Styles)).Cast<Styles>().Select(v => v.ToString()).ToList();
        }

        #endregion
    }
}