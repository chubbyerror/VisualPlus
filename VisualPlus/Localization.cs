namespace VisualPlus
{
    internal class Localize
    {
        private const string DefaultCategoryText = "VisualExtension";

        public struct GlobalStrings
        {
            public const string Abort = "Abort";
            public const string Cancel = "Cancel";
            public const string Close = "Close";
            public const string Ignore = "Ignore";
            public const string No = "No";
            public const string OK = "OK";
            public const string Retry = "Retry";
            public const string Yes = "Yes";
        }

        public struct Description
        {
            public struct Border
            {
                public const string Rounding = "Gets or sets the rounding of the border in pixels.";
                public const string Shape = "Gets or sets the shape of the border.";
                public const string Thickness = "Gets or sets the pixel thickness for the border.";
            }

            public struct Checkmark
            {
                public const string Checked = "Indicates whether the component is in the checked state.";
                public const string Character = "The checkmark character.";
                public const string CheckType = "The checkmark type.";
            }

            public struct Common
            {
                public const string Amount = "The total number of objects.";
                public const string Animation = "Toggle the animation state.";
                public const string AnimationSpeed = "The speed of the animation.";
                public const string Alignment = "Gets or sets horizontal alignment of the content.";
                public const string AutoSize = "Gets or sets a value indicating whether the control is automatically resized to display its entire contents.";
                public const string Color = "Gets or sets the color.";
                public const string Cursor = "Represents the image used to paint the mouse pointer.";
                public const string ColorGradient = "Gets or sets the gradient.";
                public const string Diameter = "A unit of linear measurement.";
                public const string Direction = "ndicates whether the Parameter object is used to bind a value to a control, or the control can be used to change the value.";
                public const string Image = "Gets or sets the image.";
                public const string MouseState = "The state of the mouse on the control.";
                public const string Padding = "Represents padding or margin information associated with a user interface (UI) element.";
                public const string Point = "Gets or sets the point.";
                public const string Rectangle = "Gets or set the rectangle.";
                public const string Rotation = "Applies the specified rotation to the transformation matrix of this Graphics.";
                public const string StartIndex = "Gets or sets the starting index.";
                public const string Size = "Gets or sets the size.";
                public const string Spacing = "The spacing between two components.";
                public const string Visible = "Gets or sets whether the control is visible.";
                public const string Opacity = "Gets or sets the opacity level of the object.";
                public const string Type = "Gets or sets the controls type.";
                public const string Orientation = "Defines the different orientations that a control or layout can have.";
                public const string ValueDivisor = "The value division.";
                public const string Outline = "Draws a line around the elements.";
                public const string Toggle = "Toggles the behaviour.";
                public const string TextImageRelation = "Specifies the position of the text and image relative to each other on a control.";
            }

            public struct Progressbar
            {
                public const string Bars = "The amount of bars to divide the progress between.";
            }

            public struct Expander
            {
                public const string ContractedHeight = "The contracted height.";
                public const string Expanded = "The expander toggle.";
            }

            public struct Gradient
            {
                public const string Angle = "The space between two intersecting lines.";
                public const string Colors = "The range of position-dependant colors";
                public const string Positions = "The gradient color positioning.";
            }

            public struct Strings
            {
                public const string Font = "Defines a particular format for text, including font face, size, and style attributes.";
                public const string Text = "Gets or sets the text associated with this control.";
                public const string TextRenderingHint = "Gets or sets the rendering mode for text associated with this Graphics.";
            }
        }

        public struct EventDescription
        {
            public const string ForeColorDisabledChanged = "The foreground disabled color of this compoment, which is used to display the text.";
            public const string MouseStateChanged = "Occours when the state of the mouse on the control changes.";
            public const string StyleManagerChanged = "Occours when the style manager has changed.";
            public const string TextRenderingHintChanged = "Occours when the TextRenderingHint has changed.";
            public const string ToggleExpanderChanged = "Occours when the expander toggle has changed.";
            public const string ControlDragChanged = "Occours when the control is being dragged.";
            public const string ControlDragToggleChanged = "Occours when the control drag toggle has been changed.";
        }

#if DEBUG

        public struct EventsCategory
        {
            public const string Action = DefaultCategoryText;
            public const string Appearance = DefaultCategoryText;
            public const string Behavior = DefaultCategoryText;
            public const string Data = DefaultCategoryText;
            public const string DragDrop = DefaultCategoryText;
            public const string Focus = DefaultCategoryText;
            public const string Key = DefaultCategoryText;
            public const string Layout = DefaultCategoryText;
            public const string Misc = DefaultCategoryText;
            public const string Mouse = DefaultCategoryText;
            public const string PropertyChanged = DefaultCategoryText;
        }

        public struct PropertiesCategory
        {
            public const string Accessibility = DefaultCategoryText;
            public const string Appearance = DefaultCategoryText;
            public const string Behavior = DefaultCategoryText;
            public const string Data = DefaultCategoryText;
            public const string Design = DefaultCategoryText;
            public const string Focus = DefaultCategoryText;
            public const string Layout = DefaultCategoryText;
        }

#else

        public struct EventsCategory
        {
            public const string Action = "Action";
            public const string Appearance = "Appearance";
            public const string Behavior = "Behavior";
            public const string Data = "Data";
            public const string DragDrop = "Drag Drop";
            public const string Focus = "Focus";
            public const string Key = "Key";
            public const string Layout = "Layout";
            public const string Misc = "Misc";
            public const string Mouse = "Mouse";
            public const string PropertyChanged = "Property Changed";
        }

        public struct PropertiesCategory
        {
            public const string Accessibility = "Accessibility";
            public const string Appearance = "Appearance";
            public const string Behavior = "Behavior";
            public const string Layout = "Layout";
            public const string Data = "Data";
            public const string Design = "Design";
            public const string Focus = "Focus";
        }
#endif
    }
}