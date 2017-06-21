namespace VisualPlus.Framework.Handlers
{ 
    /// <summary>Bind visual studio designer files here to controls.</summary>
    /// <summary>Note: If you implement more controls or to bind your own designer styles, just add string here.</summary>
    public struct DesignManager
    {
        // Designer namespace location
        private const string NamespaceLocation = @"VisualPlus.Controls.FilterProperties.";

        // Binded designer files
        public const string VisualButton = NamespaceLocation + "VisualButtonDesigner";
        public const string VisualCheckBox = NamespaceLocation + "VisualCheckBoxDesigner";
        public const string VisualCircleProgressBar = NamespaceLocation + "VisualCircleProgressBarDesigner";
        public const string VisualComboBox = NamespaceLocation + "VisualComboBoxDesigner";
        public const string VisualContextMenu = NamespaceLocation + "VisualContextMenuDesigner";
        public const string VisualForm = NamespaceLocation + "VisualFormDesigner";
        public const string VisualGroupBox = NamespaceLocation + "VisualGroupBoxDesigner";
        public const string VisualLabel = NamespaceLocation + "VisualLabelDesigner";
        public const string VisualListBox = NamespaceLocation + "VisualListBoxDesigner";
        public const string VisualListView = NamespaceLocation + "VisualListViewDesigner";
        public const string VisualNumericUpDown = NamespaceLocation + "VisualNumericUpDownDesigner";
        public const string VisualPanel = NamespaceLocation + "VisualPanelDesigner";
        public const string VisualProgressBar = NamespaceLocation + "VisualProgressBarDesigner";
        public const string VisualProgressIndicator = NamespaceLocation + "VisualProgressIndicatorDesigner";
        public const string VisualRadioButton = NamespaceLocation + "VisualRadioButtonDesigner";
        public const string VisualRichTextBox = NamespaceLocation + "VisualRichTextBoxDesigner";
        public const string VisualSeparator = NamespaceLocation + "VisualSeparatorDesigner";
        public const string VisualTab = NamespaceLocation + "VisualTabDesigner";
        public const string VisualTextBox = NamespaceLocation + "VisualTextBoxDesigner";
        public const string VisualToggle = NamespaceLocation + "VisualToggleDesigner";
        public const string VisualToolTip = NamespaceLocation + "VisualToolTip";
        public const string VisualTrackBar = NamespaceLocation + "VisualTrackBarDesigner";
    }
}