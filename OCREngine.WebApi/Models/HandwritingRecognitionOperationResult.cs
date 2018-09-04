using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
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
