namespace VisualPlus.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using VisualPlus.Enums;
    using VisualPlus.Localization;

    public class VisualStylesManager : Component
    {
        #region  ${0} Variables

        private List<string> stylesList = new List<string>();

        #endregion

        #region ${0} Properties

        public VisualStylesManager()
        {
            LoadStyles();
        }

        [Category(Localize.Category.Appearance), Description("Visual Theme")]
        public List<string> Styles
        {
            get
            {
                return stylesList;
            }

            set
            {
                stylesList = value;
            }
        }

        #endregion

        #region ${0} Events

        /// <summary>Loads styles to a string list.</summary>
        private void LoadStyles()
        {
            stylesList = Enum.GetValues(typeof(Styles)).
                              Cast<Styles>().
                              Select(v => v.ToString()).
                              ToList();
        }

        #endregion
    }
}