using System;
using System.Collections.Generic;
using System.Text;

namespace OCREngine.Domain.Entities
{
    /// <summary>
    /// OCR Operation Object
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Gets or sets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public string OperationId { get; set; }

        /// <summary>
        /// Gets or sets the file identifier.
        /// </summary>
        /// <value>
        /// The file identifier.
        /// </value>
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets the file URL.
        /// </summary>
        /// <value>
        /// The file URL of the document.
        /// </value>
        public string FileUrl { get; set; }
    }
}
