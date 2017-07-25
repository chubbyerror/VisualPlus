namespace VisualPlus.Toolkit.ActionList
{
    #region Namespace

    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;

    using VisualPlus.Toolkit.Controls;

    #endregion

    internal class VisualTextBoxTasks : ControlDesigner
    {
        #region Variables

        private DesignerActionListCollection actionListCollection;

        #endregion

        #region Properties

        /// <summary>Gets the design-time action lists supported by the component associated with the designer.</summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionListCollection == null)
                {
                    actionListCollection = new DesignerActionListCollection { new VisualTextBoxActionList(Component) };
                }

                return actionListCollection;
            }
        }

        #endregion
    }

    internal class VisualTextBoxActionList : DesignerActionList
    {
        #region Variables

        private IComponentChangeService _service;
        private VisualTextBox buttonControl;
        private DesignerActionUIService designerService;

        #endregion

        #region Constructors

        public VisualTextBoxActionList(IComponent component) : base(component)
        {
            buttonControl = (VisualTextBox)component;
            designerService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        #endregion

        #region Properties

        [Category("Behaviour")]
        [Description("Gets or sets a value indicating whether this is a multiline TextBox control.")]
        [DefaultValue(false)]
        public virtual bool MultiLine
        {
            get
            {
                return buttonControl.MultiLine;
            }

            set
            {
                buttonControl.MultiLine = value;
                buttonControl.Invalidate();
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(false)]
        public virtual string Text
        {
            get
            {
                return buttonControl.Text;
            }

            set
            {
                buttonControl.Text = value;
            }
        }

        #endregion

        #region Events

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection
                {
                    new DesignerActionPropertyItem("MultiLine", "MultiLine"),
                    new DesignerActionPropertyItem("Text", "Edit Text:")
                };

            return items;
        }

        #endregion
    }
}