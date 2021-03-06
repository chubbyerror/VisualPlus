﻿namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Managers;
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RadioButton))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual RadioButton")]
    [Designer(ControlManager.FilterProperties.VisualRadioButton)]
    public class VisualRadioButton : RadioButtonBase
    {
        #region Constructors

        public VisualRadioButton()
        {
            Cursor = Cursors.Hand;
            Size = new Size(125, 23);

            Border = new Border { Rounding = Settings.DefaultValue.Rounding.RoundedRectangle };

            CheckMark = new Checkmark(ClientRectangle)
                {
                    Style = Checkmark.CheckType.Shape,
                    Location = new Point(3, 8),
                    ImageSize = new Size(19, 16),
                    ShapeSize = new Size(8, 8),
                    ShapeRounding = Settings.DefaultValue.Rounding.Default
                };
        }

        #endregion
    }
}