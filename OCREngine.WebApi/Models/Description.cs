using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
{
    public class Description
    {
        /// <summary>
        /// Gets or sets the caption type.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the collection of captions.
        /// </summary>
        public Caption[] Captions { get; set; }
    }
}
