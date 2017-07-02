namespace VisualPlus.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Controls.Bases;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;

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

            Box = new Shape(new Rectangle(new Point(0, 0), new Size(14, 14)))
                {
                    BackgroundImage =
                        {
                            Size = new Size(19, 16),
                            Point = new Point(-3, 3)
                        }
                };

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