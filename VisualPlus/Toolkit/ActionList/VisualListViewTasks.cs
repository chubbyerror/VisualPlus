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

    internal class VisualListViewTasks : ControlDesigner
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
                    actionListCollection = new DesignerActionListCollection { new VisualListViewActionList(Component) };
                }

                return actionListCollection;
            }
        }

        #endregion
    }

    internal class VisualListViewActionList : DesignerActionList
    {
        #region Variables

        private IComponentChangeService _service;
        private VisualListView buttonControl;
        private DesignerActionUIService designerService;

        private bool dockState;
        private string dockText;

        #endregion

        #region Constructors

        public VisualListViewActionList(IComponent component) : base(component)
        {
            buttonControl = (VisualListView)component;
            designerService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));

            dockText = "Dock in Parent Container.";
            dockState = false;
        }

        #endregion

        #region Properties

        [Category("Data")]
        [Description("The items in the VisualListView.")]
        [Editor("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [Localizable(true)]
        public virtual ListView.ColumnHeaderCollection Columns
        {
            get
            {
                return buttonControl.Columns;
            }
        }

        [Category("Data")]
        [Description("The items in the VisualListView.")]
        [Editor("System.Windows.Forms.Design.ListViewGroupCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [Localizable(true)]
        public virtual ListViewGroupCollection Groups
        {
            get
            {
                return buttonControl.Groups;
            }
        }

        [Category("Data")]
        [Description("The items in the VisualListView.")]
        [Editor("System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [Localizable(true)]
        public virtual ListView.ListViewItemCollection Items
        {
            get
            {
                return buttonControl.Items;
            }
        }

        [Category("Behavior")]
        [Description("Selects one of five different views that can be shown in.")]
        [DefaultValue(false)]
        public virtual View View
        {
            get
            {
                return buttonControl.View;
            }

            set
            {
                buttonControl.View = value;
            }
        }

        #endregion

        #region Events

        public void DockContainer()
        {
            if (!dockState)
            {
                buttonControl.Dock = DockStyle.None;
                dockText = ContainerText.Docked;
                dockState = true;
            }
            else
            {
                buttonControl.Dock = DockStyle.Fill;
                dockText = ContainerText.Undock;
                dockState = false;
            }

            designerService.Refresh(buttonControl);
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection
                {
                    new DesignerActionPropertyItem("Items", "Edit Items..."),
                    new DesignerActionPropertyItem("Columns", "Edit Columns..."),
                    new DesignerActionPropertyItem("Groups", "Edit Groups..."),
                    new DesignerActionPropertyItem("View", "View:"),
                    new DesignerActionMethodItem(this, "DockContainer", dockText)
                };

            return items;
        }

        #endregion

        #region Methods

        private struct ContainerText
        {
            public const string Docked = "Dock in Parent Container";
            public const string Undock = "Undock in Parent Container.";
        }

        #endregion
    }
}