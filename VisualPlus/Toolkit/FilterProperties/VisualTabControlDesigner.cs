namespace VisualPlus.Toolkit.FilterProperties
{
    #region Namespace

    using System.Collections;
    using System.Windows.Forms.Design;

    #endregion

    internal class VisualTabControlDesigner : ControlDesigner
    {
        #region Events

        protected override void PreFilterProperties(IDictionary properties)
        {
            // properties.Remove("ImeMode");
            // properties.Remove("Padding");
            // properties.Remove("FlatAppearance");
            // properties.Remove("FlatStyle");

            // properties.Remove("AutoEllipsis");
            // properties.Remove("UseCompatibleTextRendering");

            // properties.Remove("Image");
            // properties.Remove("ImageAlign");
            // properties.Remove("ImageIndex");
            // properties.Remove("ImageKey");
            // properties.Remove("ImageList");
            // properties.Remove("TextImageRelation");

            // properties.Remove("BackColor");
            // properties.Remove("BackgroundImage");
            // properties.Remove("BackgroundImageLayout");
            // properties.Remove("UseVisualStyleBackColor");

            // properties.Remove("ComponentFont");
            // properties.Remove("ForeColor");
            // properties.Remove("RightToLeft");
            base.PreFilterProperties(properties);
        }

        #endregion
    }
}