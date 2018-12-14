namespace OCREngine.Function.Vision.Models
{
    public class Description
    {
        /// <summary>
        /// Gets or sets the caption type.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the collection of captions.
        /// </summary>
        public Caption[] Captions { get; set; }
    }
}
