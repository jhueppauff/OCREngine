using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Vision.Models
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
