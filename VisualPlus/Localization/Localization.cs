namespace VisualPlus.Localization
{
    public class Localize
    {
        public struct Description
        {
            public const string Alignment = "Gets or sets the alignment.";
            public const string Angle = "Applies the specified angle to the transformation matrix of this Graphics.";
            public const string Animation = "Toggle the animation state.";
            public const string AnimationSpeed = "The speed of the animation.";
            public const string AutoSize = "Gets or sets a value indicating whether the control is automatically resized to display its entire contents.";
            public const string BarAmount = "The amount of bars to display in the progress.";
            public const string BarSize = "The progress bar size.";
            public const string BarSpacing = "The progress bar/s spacing.";
            public const string ButtonImage = "Image on the button.";
            public const string ButtonSize = "Size of the button.";
            public const string BorderColor = "The controls border color.";
            public const string BorderHoverColor = "The controls border hover color.";
            public const string BorderHoverVisible = "Indicates whether the hover border is visible.";
            public const string BorderRounding = "Amount of pixels to curve a smooth path.";
            public const string BorderThickness = "The size of the border in pixels.";
            public const string BorderVisible = "Indicates whether the border is visible.";
            public const string ComponentColor = "The color of the component.";
            public const string ComponentDiameter = "The diameter of the component.";
            public const string ComponentFont = "The font used to display the text in the component.";
            public const string ComponentLocation = "The coordinates of the upper-left corner of the component relative to the upper-left corner of its container.";
            public const string ComponentNoName = "Description not available for component style.";
            public const string ComponentShape = "The components border shape.";
            public const string ComponentSize = "The size of the component in pixels.";
            public const string ComponentVisible = "Indicates whether the component is visible.";
            public const string ControlDisabled = "The disabled color for the control.";
            public const string DropDownButton = "Applies the specified drop down button style.";
            public const string FocusVisible = "Sets input focus to the control";
            public const string GradientPosition = "The position of the gradient colors.";
            public const string HatchSize = "The hatch size.";
            public const string HatchStyle = "Applies the specified hatch style.";
            public const string HoverColor = "The controls hover color.";
            public const string Icon = "The icon to be displayed.";
            public const string IconPosition = "The icon position.";
            public const string IconSize = "The icon size.";
            public const string MirrorColor = "The mirrored text color.";
            public const string NormalColor = "The controls normal color.";
            public const string Outline = "Draws a line around the elements.";
            public const string Orientation = "Defines the different orientations that a control or layout can have.";
            public const string PressedColor = "The controls pressed color.";
            public const string ProgressBarStyle = "Applies the specified progressbar style.";
            public const string ProgressSize = "The size of the progressbar.";
            public const string ProgressStyle = "Applies the specified progress style.";
            public const string Reflection = "Draws a reflection to the text.";
            public const string ReflectionSpacing = "Space between the reflection and the text.";
            public const string Rotation = "Applies the specified rotation to the transformation matrix of this Graphics.";
            public const string SeparatorStyle = "Applies the specified separator style.";
            public const string Shadow = "Draws a dropshadow.";
            public const string ShadowColor = "The shadow color.";
            public const string ShadowDirection = "Direction of the shadow.";
            public const string ShadowOpacity = "Opacity of the shadow.";
            public const string StartIndex = "Gets or sets the starting index.";
            public const string StringAlignment = "Specifies the alignment of a text string relative to its layout rectangle.";
            public const string Style = "The visual style.";
            public const string StyleColor = "The visual style color.";
            public const string TextColor = "The text color.";
            public const string TextImageRelation = "Specifies the position of the text and image relative to each other on a control.";
            public const string TextRenderingHint = "Renders the text in different styles.";
            public const string TextVisible = "Indicates whether the text is visible.";
            public const string TitleBoxVisible = "Indicates whether the title box is visible.";
            public const string Toggled = "Toggles the state of the control.";
            public const string ToggleType = "Applies the specified toggle type.";
            public const string TrackBarType = "Sets or gets the specified trackbar type.";
            public const string ValueDivisor = "The value division.";
        }

#if DEBUG

        /// <summary>Property group categories.</summary>
        public struct Category
        {
            public const string Accessibility = "VisualExtension";
            public const string Appearance = "VisualExtension";
            public const string Behavior = "VisualExtension";
            public const string Data = "VisualExtension";
            public const string Design = "VisualExtension";
            public const string Focus = "VisualExtension";
            public const string Layout = "VisualExtension";
        }

#else

/// <summary>Property group categories.</summary>
        public struct Category
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