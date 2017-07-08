namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Toolkit.Controls;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class RadioButtonBase : ToggleButtonBase
    {
        #region Events

        protected override void OnClick(EventArgs e)
        {
            if (!Toggle)
            {
                Toggle = true;
            }

            base.OnClick(e);
        }

        protected override void OnToggleChanged(EventArgs e)
        {
            base.OnToggleChanged(e);
            AutoUpdateOthers();
            Invalidate();
        }

        private void AutoUpdateOthers()
        {
            // Only un-check others if they are checked
            if (Toggle)
            {
                Control parent = Parent;
                if (parent != null)
                {
                    // Search all sibling controls
                    foreach (Control control in parent.Controls)
                    {
                        // If another radio button found, that is not us
                        if ((control != this) && control is VisualRadioButton)
                        {
                            // Cast to correct type
                            VisualRadioButton radioButton = (VisualRadioButton)control;

                            // If target allows auto check changed and is currently checked
                            if (radioButton.Toggle)
                            {
                                // Set back to not checked
                                radioButton.Toggle = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}