using OCREngine.WebApi.Function.Models;
using System.IO;
using System.Threading.Tasks;


namespace OCREngine.Function.Vision
{
    internal interface IVisionServiceClient
    {
        /// <summary>
        /// Recognizes the text.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="detectOrientation">if set to <c>true</c> [detect orientation].</param>
        /// <returns>The OCR object.</returns>
        Task<OcrResults> RecognizeTextAsync(string imageUrl, string languageCode = LanguageCodes.AutoDetect, bool detectOrientation = true);

        /// <summary>
        /// Recognizes the text.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="detectOrientation">if set to <c>true</c> [detect orientation].</param>
        /// <returns>The OCR object.</returns>
        Task<OcrResults> RecognizeTextAsync(Stream imageStream, string languageCode = LanguageCodes.AutoDetect, bool detectOrientation = true);

        /// <summary>
        /// HandwritingRecognitionOperation
        /// </summary>
        /// <param name="imageUrl">Image url</param>
        /// <returns>HandwritingRecognitionOperation created</returns>
        Task<HandwritingRecognitionOperation> CreateHandwritingRecognitionOperationAsync(string imageUrl);

        /// <summary>
        /// Create HandwritingRecognitionOperation
        /// </summary>
        /// <param name="imageStream">Image content is byte array.</param>
        /// <returns>HandwritingRecognitionOperation created</returns>
        Task<HandwritingRecognitionOperation> CreateHandwritingRecognitionOperationAsync(Stream imageStream);

        /// <summary>
        /// Get HandwritingRecognitionOperationResult
        /// </summary>
        /// <param name="opeartion">HandwritingRecognitionOperation object</param>
        /// <returns>HandwritingRecognitionOperationResult</returns>
        Task<HandwritingRecognitionOperationResult> GetHandwritingRecognitionOperationResultAsync(HandwritingRecognitionOperation opeartion);

        /// <summary>
        /// Gets the tags associated with an image.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns>Analysis result with tags.</returns>
        Task<AnalysisResult> GetTagsAsync(string imageUrl);

        /// <summary>
        /// Gets the tags associated with an image.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <returns>Analysis result with tags.</returns>
        Task<AnalysisResult> GetTagsAsync(Stream imageStream);
    }
}
