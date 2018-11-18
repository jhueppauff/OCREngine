namespace OCREngine.Domain.Entities.Vision
{
    using System;

    public class AnalysisResult
    {
            /// <summary>
            /// Gets or sets the request identifier.
            /// </summary>
            /// <value>
            /// The request identifier.
            /// </value>
            public Guid RequestId { get; set; }

            /// <summary>
            /// Gets or sets the image description.
            /// </summary>
            /// <value>
            /// The description.
            /// </value>
            public Description Description { get; set; }
        }
}
