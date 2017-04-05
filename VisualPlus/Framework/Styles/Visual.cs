namespace VisualPlus.Framework.Styles
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    public class Visual : IStyle
    {
        #region  ${0} Variables

        private readonly Color defaultBackgroundColorNoDepth = Color.White;
        private readonly Color defaultBorderColorNoDepth = Color.FromArgb(180, 180, 180);
        private readonly Color defaultForeColorNoDepth = Color.Black;
        private readonly Color defaultItemHoverNoDepth = Color.White;
        private readonly Color defaultItemNormalNoDepth = Color.White;

        #endregion

        #region ${0} Properties

        public Color BackgroundProgressCircle
        {
            get
            {
                return Color.FromArgb(52, 73, 96);
            }
        }

        public SolidBrush BrushFontItemDisable
        {
            get
            {
                return new SolidBrush(Color.DarkGray);
            }
        }

        public Color ButtonDownColor
        {
            get
            {
                return Color.FromArgb(137, 136, 136);
            }
        }

        public Color ButtonHoverColor
        {
            get
            {
                return Color.FromArgb(181, 181, 181);
            }
        }

        public Color ButtonNormalColor
        {
            get
            {
                return Color.FromArgb(226, 226, 226);
            }
        }

        public Color ControlDisabled
        {
            get
            {
                return Color.FromArgb(243, 243, 243);
            }
        }

        public Color DropDownButtonColor
        {
            get
            {
                return Color.FromArgb(119, 119, 118);
            }
        }

        public Color DropDownColor
        {
            get
            {
                return Color.White;
            }
        }

        public Color ForegroundProgressCircle
        {
            get
            {
                return Color.FromArgb(48, 56, 68);
            }
        }

        public Color HatchColor
        {
            get
            {
                return Color.FromArgb(20, Color.Black);
            }
        }

        public Color ItemDisableBackgroundColor
        {
            get
            {
                return Color.LightGray;
            }
        }

        public Color LineColor
        {
            get
            {
                return Color.FromArgb(224, 222, 220);
            }
        }

        public Color MainColor
        {
            get
            {
                return ColorTranslator.FromHtml("#2D882D");
            }
        }

        public Color ProgressColor
        {
            get
            {
                return ColorTranslator.FromHtml("#2D882D");
            }
        }

        public Color ShadowColor
        {
            get
            {
                return Color.FromArgb(250, 249, 249);
            }
        }

        public Style StyleManagement
        {
            get
            {
                return Style.Visual;
            }
        }

        public Color TabHover
        {
            get
            {
                return Color.FromArgb(35, 36, 38);
            }
        }

        public Color TabMenu
        {
            get
            {
                return Color.FromArgb(77, 75, 76);
            }
        }

        public Color TabNormal
        {
            get
            {
                return Color.FromArgb(77, 75, 76);
            }
        }

        public Color TabSelected
        {
            get
            {
                return Color.FromArgb(66, 64, 65);
            }
        }

        public Color TabTextNormal
        {
            get
            {
                return Color.FromArgb(127, 127, 127);
            }
        }

        public Color TabTextSelected
        {
            get
            {
                return ColorTranslator.FromHtml("#2D882D");
            }
        }

        public Color TextDisabled
        {
            get
            {
                return Color.FromArgb(131, 129, 129);
            }
        }

        #endregion

        #region ${0} Events

        private static List<Color> GetBackgroundColor()
        {
            var list = new List<Color>
                {
                    ControlPaint.LightLight(Color.Gainsboro),
                    ControlPaint.Light(Color.Gainsboro),
                    Color.FromArgb(66, 64, 65)
                };

            return list;
        }

        private static List<Color> GetBorderColor()
        {
            var list = new List<Color>
                {
                    Color.FromArgb(180, 180, 180),
                    Color.FromArgb(120, 183, 230),
                    Color.FromArgb(66, 64, 65)
                };

            return list;
        }

        private static List<Color> GetForeColor()
        {
            var list = new List<Color>
                {
                    Color.Black,
                    ControlPaint.LightLight(Color.Silver)
                };

            return list;
        }

        private static List<Color> GetItemHover()
        {
            var list = new List<Color>
                {
                    Color.FromArgb(241, 241, 241),
                    ControlPaint.LightLight(Color.Gainsboro)
                };

            return list;
        }

        private static List<Color> GetItemNormal()
        {
            var list = new List<Color>
                {
                    Color.White,
                    ControlPaint.LightLight(Color.Gainsboro)
                };

            return list;
        }

        #endregion

        #region ${0} Methods

        public Color BackgroundColor(int depth)
        {
            if (depth < GetBackgroundColor().
                    Count)
            {
                return GetBackgroundColor()[depth];
            }

            return defaultBackgroundColorNoDepth;
        }

        public Color BorderColor(int depth)
        {
            if (depth < GetBorderColor().
                    Count)
            {
                return GetBorderColor()[depth];
            }

            return defaultBorderColorNoDepth;
        }

        public Color ForeColor(int depth)
        {
            if (depth < GetForeColor().
                    Count)
            {
                return GetForeColor()[depth];
            }

            return defaultForeColorNoDepth;
        }

        public Color ItemHover(int depth)
        {
            if (depth < GetItemHover().
                    Count)
            {
                return GetItemHover()[depth];
            }

            return defaultItemHoverNoDepth;
        }

        public Color ItemNormal(int depth)
        {
            if (depth < GetItemNormal().
                    Count)
            {
                return GetItemNormal()[depth];
            }

            return defaultItemNormalNoDepth;
        }

        #endregion
    }
}