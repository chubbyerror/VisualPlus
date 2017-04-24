namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Localization;

    #endregion

    public class VisualStylesManager : Component
    {
        #region Variables

        public List<string> StylesList = new List<string>();

        #endregion

        #region Variables

        private Styles currentStyle = Settings.DefaultValue.CurrentStyle;

        private Color styleColor = Settings.DefaultValue.Style.StyleColor;

        #endregion

        #region Constructors

        public VisualStylesManager()
        {
            LoadStyles();
        }

        #endregion

        #region Properties

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

        #region Events

        /// <summary>Loads styles to a string list.</summary>
        private void LoadStyles()
        {
            StylesList = Enum.GetValues(typeof(Styles)).Cast<Styles>().Select(v => v.ToString()).ToList();
        }

        #endregion
    }
}