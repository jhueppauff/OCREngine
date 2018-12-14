using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.Function.Vision.Models
{
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
