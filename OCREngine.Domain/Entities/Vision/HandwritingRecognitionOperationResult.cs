namespace OCREngine.Domain.Entities.Vision
{
    public class HandwritingRecognitionOperationResult
    {
        /// <summary>
        /// Gets or Sets the status
        /// </summary>
        /// <value>
        /// The status
        /// </value>
        public HandwritingRecognitionOperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the RecognitionResult
        /// </summary>
        /// <value>
        /// The result of recognition
        /// </value>
        public HandwritingTextResult RecognitionResult { get; set; }
    }
}
