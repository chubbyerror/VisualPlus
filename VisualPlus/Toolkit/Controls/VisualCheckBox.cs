namespace VisualPlus.Toolkit.Controls
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
    [ToolboxBitmap(typeof(CheckBox))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual CheckBox")]
    [Designer(ControlManager.FilterProperties.VisualCheckBox)]
    public sealed class VisualCheckBox : CheckBoxBase
    {
        #region Constructors

        public VisualCheckBox()
        {
            Cursor = Cursors.Hand;
            Size = new Size(125, 23);

            Border = new Border { Rounding = Settings.DefaultValue.Rounding.BoxRounding };

            CheckMark = new Checkmark(ClientRectangle)
                {
                    Style = Checkmark.CheckType.Character,
                    Location = new Point(-1, 5),
                    ImageSize = new Size(19, 16),
                    ShapeSize = new Size(8, 8)
                };
        }

        #endregion
    }
}