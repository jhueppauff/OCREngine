using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Function.Models
{
    public class BoundingBox
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Length { get; set; }

        public int Size { get; set; }
    }
}
