namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;

    #endregion

    /// <summary>Stores a text/extraText/Image triple of values as a content values source.</summary>
    public class FixedContentValue : IContentValues
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="FixedContentValue" /> class.</summary>
        public FixedContentValue() : this(string.Empty, string.Empty, null, Color.Empty)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="FixedContentValue" /> class.</summary>
        /// <param name="shortText">Initial short text value.</param>
        /// <param name="longText">Initial long text value.</param>
        /// <param name="image">Initial image value.</param>
        /// <param name="imageTransparentColor">Initial image transparent color value.</param>
        public FixedContentValue(string shortText, string longText, Image image, Color imageTransparentColor)
        {
            ShortText = shortText;
            LongText = longText;
            Image = image;
            ImageTransparentColor = imageTransparentColor;
        }

        #endregion

        #region Properties

        /// <summary>Gets and sets the image.</summary>
        [Category("Appearance")]
        [Description("Image associated with item.")]
        [Localizable(true)]
        public Image Image { get; set; }

        /// <summary>Gets and sets the image transparent color.</summary>
        [Category("Appearance")]
        [Description("Color to treat as transparent in the Image.")]
        [Localizable(true)]
        public Color ImageTransparentColor { get; set; }

        /// <summary>Gets and sets the long text.</summary>
        [Category("Appearance")]
        [Description("Supplementary text.")]
        [Localizable(true)]
        [DefaultValue("")]
        public string LongText { get; set; }

        /// <summary>Gets and sets the short text.</summary>
        [Category("Appearance")]
        [Description("Main text.")]
        [Localizable(true)]
        [DefaultValue("")]
        public string ShortText { get; set; }

        #endregion

        #region Events

        ///// <summary>
        ///// Gets the content image.
        ///// </summary>
        ///// <param name="state">The state for which the image is needed.</param>
        ///// <returns>Image value.</returns>
        // public Image GetImage(PaletteState state)
        // {
        // return _image;
        // }

        ///// <summary>
        ///// Gets the image color that should be transparent.
        ///// </summary>
        ///// <param name="state">The state for which the image is needed.</param>
        ///// <returns>Color value.</returns>
        // public Color GetImageTransparentColor(PaletteState state)
        // {
        // return _imageTransparentColor;
        // }

        /// <summary>Gets the content long text.</summary>
        /// <returns>String value.</returns>
        public string GetLongText()
        {
            return LongText;
        }

        /// <summary>Gets the content short text.</summary>
        /// <returns>String value.</returns>
        public string GetShortText()
        {
            return ShortText;
        }

        private bool ShouldSerializeImage()
        {
            return Image != null;
        }

        private bool ShouldSerializeImageTransparentColor()
        {
            return ImageTransparentColor != Color.Empty;
        }

        private bool ShouldSerializeLongText()
        {
            return !string.IsNullOrEmpty(LongText);
        }

        private bool ShouldSerializeShortText()
        {
            return !string.IsNullOrEmpty(ShortText);
        }

        #endregion
    }
}