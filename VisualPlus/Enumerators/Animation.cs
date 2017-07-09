namespace VisualPlus.Enumerators
{
    public enum EffectType
    {
        /// <summary>The linear.</summary>
        Linear,

        /// <summary>The ease in out.</summary>
        EaseInOut,

        /// <summary>The ease out.</summary>
        EaseOut,

        /// <summary>The custom quadratic.</summary>
        CustomQuadratic
    }

    public enum AnimationDirection
    {
        /// <summary>In. Stops if finished..</summary>
        In,

        /// <summary>Out. Stops if finished.</summary>
        Out,

        /// <summary>Same as In, but changes to InOutOut if finished.</summary>
        InOutIn,

        /// <summary>Same as Out.</summary>
        InOutOut,

        /// <summary>Same as In, but changes to InOutRepeatingOut if finished.</summary>
        InOutRepeatingIn,

        /// <summary>Same as Out, but changes to InOutRepeatingIn if finished.</summary>
        InOutRepeatingOut
    }
}