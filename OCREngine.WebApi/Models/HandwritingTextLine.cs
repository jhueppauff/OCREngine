﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
{
    public class HandwritingTextLine
    {
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public int[] BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the words.
        /// </summary>
        /// <value>
        /// The words.
        /// </value>
        public HandwritingTextWord[] Words { get; set; }

        /// <summary>
        /// Gets the polygon
        /// </summary>
        /// <value>
        /// The polygon
        /// </value>
        [JsonIgnore]
        public Polygon Polygon
        {
            get
            {
                return Polygon.FromArray(this.BoundingBox);
            }
        }
    }
}
