namespace OCREngine.Domain.Infrastructure
{
    /// <summary>
    /// Document Request Object
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the document.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>
        /// The download URL of the document to process.
        /// </value>
        public string DownloadUrl { get; set; }
    }
}
