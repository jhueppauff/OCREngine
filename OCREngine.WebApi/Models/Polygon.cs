using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
{
    public class Polygon
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public Polygon()
        {
            Points = new List<Point>();
        }

        /// <summary>
        /// Gets and sets the points of polygon
        /// </summary>
        /// <value>
        /// The points of polygon
        /// </value>
        public List<Point> Points { get; set; }

        /// <summary>
        /// Get a polygon object from boundingbox array
        /// </summary>
        /// <param name="boundingBox"> The boundingBox array</param>
        /// <returns>The polygon</returns>
        public static Polygon FromArray(int[] boundingBox)
        {
            Polygon polygon = new Polygon();

            for (int i = 0; i + 1 < boundingBox.Length; i += 2)
            {
                polygon.Points.Add(new Point() { X = boundingBox[i], Y = boundingBox[i + 1] });
            }

            return polygon;
        }
    }
}
