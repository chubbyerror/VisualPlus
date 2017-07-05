namespace VisualPlus.Framework.Handlers
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    #endregion

    internal class ControlManager
    {
        #region Variables

        private static readonly Dictionary<ToolboxControl, string> ToolBoxControlsContainer = LoadControls();

        #endregion

        #region Constructors

        public enum ToolboxControl
        {
            /// <summary>The visual button.</summary>
            VisualButton = 0,

            /// <summary>The visual check box.</summary>
            VisualCheckBox = 1,

            /// <summary>The visual circle progress bar.</summary>
            VisualCircleProgressBar = 2,

            /// <summary>The visual color picker.</summary>
            VisualColorPicker = 3,

            /// <summary>The visual combo box.</summary>
            VisualComboBox = 4,

            /// <summary>The visual context menu.</summary>
            VisualContextMenu = 5,

            /// <summary>The visual group box.</summary>
            VisualGroupBox = 6,

            /// <summary>The visual knob.</summary>
            VisualKnob = 7,

            /// <summary>The visual label.</summary>
            VisualLabel = 8,

            /// <summary>The visual list box.</summary>
            VisualListBox = 9,

            /// <summary>The visual list view.</summary>
            VisualListView = 10,

            /// <summary>The visual numeric up down.</summary>
            VisualNumericUpDown = 11,

            /// <summary>The visual panel.</summary>
            VisualPanel = 12,

            /// <summary>The visual progress bar.</summary>
            VisualProgressBar = 13,

            /// <summary>The visual progress indicator.</summary>
            VisualProgressIndicator = 14,

            /// <summary>The visual radio button.</summary>
            VisualRadioButton = 15,

            /// <summary>The visual rating.</summary>
            VisualRating = 16,

            /// <summary>The visual rich text box.</summary>
            VisualRichTextBox = 17,

            /// <summary>The visual separator.</summary>
            VisualSeparator = 18,

            /// <summary>The visual shape.</summary>
            VisualShape = 19,

            /// <summary>The visual tab control.</summary>
            VisualTabControl = 20,

            /// <summary>The visual text box.</summary>
            VisualTextBox = 21,

            /// <summary>The visual toggle.</summary>
            VisualToggle = 22,

            /// <summary>The visual track bar.</summary>
            VisualTrackBar = 23
        }

        public enum UnsortedControl
        {
            /// <summary>The visual container.</summary>
            VisualContainer = 0,

            /// <summary>The visual form.</summary>
            VisualForm = 1,

            /// <summary>The visual styles manager.</summary>
            VisualStylesManager = 2,

            /// <summary>The visual tool tip.</summary>
            VisualToolTip = 3
        }

        #endregion

        #region Properties

        [ReadOnly(true)]
        [Description("Contains Control information. <Control, DesignerLocation>")]
        public static Dictionary<ToolboxControl, string> ToolboxControls
        {
            get
            {
                return ToolBoxControlsContainer;
            }

            protected set
            {
                ToolboxControls = ToolBoxControlsContainer;
            }
        }

        #endregion

        #region Events

        /// <summary>Gets the namespace location from the control.</summary>
        /// <param name="controlName">The control Name.</param>
        /// <returns>Returns namespace name.</returns>
        public static string GetControlNamespace(Control controlName)
        {
            return controlName.GetType().Namespace;
        }

        /// <summary>Gets the control type.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="controlName">The control name.</param>
        /// <returns>The control type.</returns>
        private static T ControlType<T>(string controlName)
        {
            return (T)Activator.CreateInstance(Type.GetType(controlName));
        }

        /// <summary>Get the location to the control designer file.</summary>
        /// <param name="control">The control.</param>
        /// <returns>Returns the location.</returns>
        private static string GetControlPropertyFilterPath(ToolboxControl control)
        {
            return NamespaceLocations.FilterPropertiesLocation + control + "Designer";
        }

        /// <summary>Gets a registered control object.</summary>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>Returns the object of the control.</returns>
        private static Control GetControlType(ToolboxControl controlName)
        {
            return ControlType<Control>(controlName.ToString());
        }

        /// <summary>Initializes all the controls.</summary>
        /// <returns>Dictionary with controls.</returns>
        private static Dictionary<ToolboxControl, string> LoadControls()
        {
            ToolboxControl toolboxControl = 0;

            var controlDictionary = new Dictionary<ToolboxControl, string>();
            int controlCount = toolboxControl.Count();

            for (var i = 0; i < controlCount; i++)
            {
                string controlName = toolboxControl.GetValueByIndex<ToolboxControl>(i);
                toolboxControl = (ToolboxControl)controlName.ToEnum<ToolboxControl>();
                controlDictionary.Add(toolboxControl, GetControlPropertyFilterPath(toolboxControl));
            }

            return controlDictionary;
        }

        #endregion

        #region Methods

        /// <summary>Bind visual studio designer files here to controls.</summary>
        /// <summary>Note: If you implement more controls or to bind your own designer styles, just add here.</summary>
        public struct FilterProperties
        {
            private const string DesignerFilterPath = NamespaceLocations.FilterPropertiesLocation + "Visual";
            private const string DesignerSuffix = "Designer";

            public const string VisualButton = DesignerFilterPath + "Button" + DesignerSuffix;
            public const string VisualCheckBox = DesignerFilterPath + "CheckBox" + DesignerSuffix;
            public const string VisualCircleProgressBar = DesignerFilterPath + "CircleProgressBar" + DesignerSuffix;
            public const string VisualComboBox = DesignerFilterPath + "ComboBox" + DesignerSuffix;
            public const string VisualContextMenu = DesignerFilterPath + "ContextMenu" + DesignerSuffix;
            public const string VisualForm = DesignerFilterPath + "Form" + DesignerSuffix;
            public const string VisualGroupBox = DesignerFilterPath + "GroupBox" + DesignerSuffix;
            public const string VisualLabel = DesignerFilterPath + "Label" + DesignerSuffix;
            public const string VisualListBox = DesignerFilterPath + "ListBox" + DesignerSuffix;
            public const string VisualListView = DesignerFilterPath + "ListView" + DesignerSuffix;
            public const string VisualNumericUpDown = DesignerFilterPath + "NumericUpDown" + DesignerSuffix;
            public const string VisualPanel = DesignerFilterPath + "Panel" + DesignerSuffix;
            public const string VisualProgressBar = DesignerFilterPath + "ProgressBar" + DesignerSuffix;
            public const string VisualProgressIndicator = DesignerFilterPath + "ProgressIndicator" + DesignerSuffix;
            public const string VisualRadioButton = DesignerFilterPath + "RadioButton" + DesignerSuffix;
            public const string VisualRating = DesignerFilterPath + "Rating" + DesignerSuffix;
            public const string VisualRichTextBox = DesignerFilterPath + "RichTextBox" + DesignerSuffix;
            public const string VisualSeparator = DesignerFilterPath + "Separator" + DesignerSuffix;
            public const string VisualTabControl = DesignerFilterPath + "TabControl" + DesignerSuffix;
            public const string VisualTextBox = DesignerFilterPath + "TextBox" + DesignerSuffix;
            public const string VisualToggle = DesignerFilterPath + "Toggle" + DesignerSuffix;
            public const string VisualToolTip = DesignerFilterPath + "ToolTip" + DesignerSuffix;
            public const string VisualTrackBar = DesignerFilterPath + "TrackBar" + DesignerSuffix;
        }

        private struct NamespaceLocations
        {
            public const string FilterPropertiesLocation = @"VisualPlus.Toolkit.FilterProperties.";
        }

        #endregion
    }
}