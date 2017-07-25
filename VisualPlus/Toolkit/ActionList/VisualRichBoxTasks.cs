namespace VisualPlus.Toolkit.ActionList
{
    #region Namespace

    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;

    using VisualPlus.Toolkit.Controls;

    #endregion

    internal class VisualRichBoxTasks : ControlDesigner
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
                    actionListCollection = new DesignerActionListCollection { new VisualRichBoxActionList(Component) };
                }

                return actionListCollection;
            }
        }

        #endregion
    }

    internal class VisualRichBoxActionList : DesignerActionList
    {
        #region Variables

        private IComponentChangeService _service;
        private VisualRichTextBox buttonControl;
        private DesignerActionUIService designerService;

        #endregion

        #region Constructors

        public VisualRichBoxActionList(IComponent component) : base(component)
        {
            buttonControl = (VisualRichTextBox)component;
            designerService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        #endregion

        #region Properties

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
                    new DesignerActionPropertyItem("Text", "Edit Text:")
                };

            return items;
        }

        #endregion
    }
}