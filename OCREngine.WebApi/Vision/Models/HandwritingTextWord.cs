using Newtonsoft.Json;

namespace OCREngine.WebApi.Vision.Models
{
    public class HandwritingTextWord
    {
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public int[] BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

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
                return Polygon.FromArray(BoundingBox);
            }
        }
    }
}
