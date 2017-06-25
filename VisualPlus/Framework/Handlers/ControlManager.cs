namespace VisualPlus.Framework.Handlers
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    using VisualPlus.Controls;

    #endregion

    internal class ControlManager
    {
        #region Variables

        private static readonly Dictionary<ToolboxControl, string> toolboxControls = LoadControls();

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

            /// <summary>The visual drag.</summary>
            VisualDrag = 6,

            /// <summary>The visual group box.</summary>
            VisualGroupBox = 7,

            /// <summary>The visual knob.</summary>
            VisualKnob = 8,

            /// <summary>The visual label.</summary>
            VisualLabel = 9,

            /// <summary>The visual list box.</summary>
            VisualListBox = 10,

            /// <summary>The visual list view.</summary>
            VisualListView = 11,

            /// <summary>The visual numeric up down.</summary>
            VisualNumericUpDown = 12,

            /// <summary>The visual panel.</summary>
            VisualPanel = 13,

            /// <summary>The visual progress bar.</summary>
            VisualProgressBar = 14,

            /// <summary>The visual progress indicator.</summary>
            VisualProgressIndicator = 15,

            /// <summary>The visual radio button.</summary>
            VisualRadioButton = 16,

            /// <summary>The visual rating.</summary>
            VisualRating = 17,

            /// <summary>The visual rich text box.</summary>
            VisualRichTextBox = 18,

            /// <summary>The visual separator.</summary>
            VisualSeparator = 19,

            /// <summary>The visual shape.</summary>
            VisualShape = 20,

            /// <summary>The visual tab control.</summary>
            VisualTabControl = 21,

            /// <summary>The visual text box.</summary>
            VisualTextBox = 22,

            /// <summary>The visual toggle.</summary>
            VisualToggle = 23,

            /// <summary>The visual track bar.</summary>
            VisualTrackBar = 24
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
                return toolboxControls;
            }

            protected set
            {
                ToolboxControls = toolboxControls;
            }
        }

        #endregion

        #region Events

        /// <summary>Gets the namespace location from the control.</summary>
        /// <param name="controlName">The control Name.</param>
        /// <returns>Returns namespace name.</returns>
        public static string GetControlNamespace(Control controlName)
        {
            Type controlType = controlName.GetType();
            return controlType.Namespace;
        }

        /// <summary>Gets a registered control object.</summary>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>Returns the object of the control.</returns>
        private static Control GetControlObject(ToolboxControl controlName)
        {
            Control controlObject;

            switch (controlName)
            {
                case ToolboxControl.VisualButton:
                    {
                        controlObject = new VisualButton();
                        break;
                    }

                case ToolboxControl.VisualCheckBox:
                    {
                        controlObject = new VisualCheckBox();
                        break;
                    }

                case ToolboxControl.VisualCircleProgressBar:
                    {
                        controlObject = new VisualCircleProgressBar();
                        break;
                    }

                case ToolboxControl.VisualColorPicker:
                    {
                        controlObject = new VisualColorPicker();
                        break;
                    }

                case ToolboxControl.VisualComboBox:
                    {
                        controlObject = new VisualComboBox();
                        break;
                    }

                case ToolboxControl.VisualContextMenu:
                    {
                        controlObject = new VisualContextMenuStrip();
                        break;
                    }

                case ToolboxControl.VisualDrag:
                    {
                        controlObject = new VisualDrag();
                        break;
                    }

                case ToolboxControl.VisualGroupBox:
                    {
                        controlObject = new VisualGroupBox();
                        break;
                    }

                case ToolboxControl.VisualKnob:
                    {
                        controlObject = new VisualKnob();
                        break;
                    }

                case ToolboxControl.VisualLabel:
                    {
                        controlObject = new VisualLabel();
                        break;
                    }

                case ToolboxControl.VisualListBox:
                    {
                        controlObject = new VisualListBox();
                        break;
                    }

                case ToolboxControl.VisualListView:
                    {
                        controlObject = new VisualListView();
                        break;
                    }

                case ToolboxControl.VisualNumericUpDown:
                    {
                        controlObject = new VisualNumericUpDown();
                        break;
                    }

                case ToolboxControl.VisualPanel:
                    {
                        controlObject = new VisualPanel();
                        break;
                    }

                case ToolboxControl.VisualProgressBar:
                    {
                        controlObject = new VisualProgressBar();
                        break;
                    }

                case ToolboxControl.VisualProgressIndicator:
                    {
                        controlObject = new VisualProgressIndicator();
                        break;
                    }

                case ToolboxControl.VisualRadioButton:
                    {
                        controlObject = new VisualRadioButton();
                        break;
                    }

                case ToolboxControl.VisualRating:
                    {
                        controlObject = new VisualRating();
                        break;
                    }

                case ToolboxControl.VisualRichTextBox:
                    {
                        controlObject = new VisualRichTextBox();
                        break;
                    }

                case ToolboxControl.VisualSeparator:
                    {
                        controlObject = new VisualSeparator();
                        break;
                    }

                case ToolboxControl.VisualShape:
                    {
                        controlObject = new VisualShape();
                        break;
                    }

                case ToolboxControl.VisualTabControl:
                    {
                        controlObject = new VisualTabControl();
                        break;
                    }

                case ToolboxControl.VisualTextBox:
                    {
                        controlObject = new VisualTextBox();
                        break;
                    }

                case ToolboxControl.VisualToggle:
                    {
                        controlObject = new VisualToggle();
                        break;
                    }

                case ToolboxControl.VisualTrackBar:
                    {
                        controlObject = new VisualTrackBar();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(controlName), controlName, null);
                    }
            }

            return controlObject;
        }

        /// <summary>Get the location to the control designer file.</summary>
        /// <param name="control">The control.</param>
        /// <returns>Returns the location.</returns>
        private static string GetControlPropertyFilterPath(ToolboxControl control)
        {
            return NamespaceLocations.FilterPropertiesLocation + control + "Designer";
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
            public const string FilterPropertiesLocation = @"VisualPlus.Controls.FilterProperties.";
        }

        #endregion
    }
}