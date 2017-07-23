namespace VisualPlus.Toolkit.ActionList
{
    #region Namespace

    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    using VisualPlus.Toolkit.Controls;

    #endregion

    internal class VisualListBoxTasks : ControlDesigner
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
                    actionListCollection = new DesignerActionListCollection { new VisualListBoxActionList(Component) };
                }

                return actionListCollection;
            }
        }

        #endregion
    }

    internal class VisualListBoxActionList : DesignerActionList
    {
        #region Variables

        private VisualListBox _listBox;
        private IComponentChangeService _service;

        private VisualListBox buttonControl;
        private DesignerActionUIService designerService;

        #endregion

        #region Constructors

        public VisualListBoxActionList(IComponent component) : base(component)
        {
            buttonControl = (VisualListBox)component;
            designerService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        #endregion

        #region Properties

        [Category("Data")]
        [Description("The items in the VisualListBox.")]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [Localizable(true)]
        public virtual ListBox.ObjectCollection Items
        {
            get
            {
                return buttonControl.Items;
            }
        }

        #endregion

        #region Events

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection
                {
                    new DesignerActionHeaderItem("Unbound Mode"),
                    new DesignerActionPropertyItem("Items", "Edit Items...", "Unbound Mode")
                };

            return items;
        }

        #endregion
    }
}