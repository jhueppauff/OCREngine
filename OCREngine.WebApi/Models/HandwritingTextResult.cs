using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
{
    public class HandwritingTextResult
    {
        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public HandwritingTextLine[] Lines { get; set; }
    }
}
