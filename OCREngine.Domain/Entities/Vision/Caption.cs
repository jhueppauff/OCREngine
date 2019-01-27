namespace OCREngine.Domain.Entities.Vision
{
    public class Caption
    {
        /// <summary>
        /// Gets or sets the caption text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the confidence score for the caption text.
        /// </summary>
        public double Confidence { get; set; }
    }
}
