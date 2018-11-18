using Newtonsoft.Json;

namespace OCREngine.WebApi.Vision.Models
{
    public class Region
    {
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public string BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public Line[] Lines { get; set; }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        [JsonIgnore]
        public Rectangle Rectangle
        {
            get
            {
                return Rectangle.FromString(BoundingBox);
            }
        }
    }
}
