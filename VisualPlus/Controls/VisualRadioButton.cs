namespace VisualPlus.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Controls.Bases;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RadioButton))]
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [Description("The Visual RadioButton")]
    [Designer(ControlManager.FilterProperties.VisualRadioButton)]
    public sealed class VisualRadioButton : RadioButtonBase
    {
        #region Constructors

        public VisualRadioButton()
        {
            Cursor = Cursors.Hand;
            Size = new Size(125, 23);

            Box = new Shape(new Rectangle(new Point(0, 0), new Size(14, 14)))
                {
                    Border = new Border
                        {
                            Rounding = Settings.DefaultValue.Rounding.RoundedRectangle
                        },
                    BackgroundImage =
                        {
                            Size = new Size(19, 16)
                        }
                };

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