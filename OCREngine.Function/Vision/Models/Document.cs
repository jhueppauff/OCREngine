using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.Function.Vision.Models
{
    public class Document
    {
        public string Name { get; set; }

        public string FileLocation { get; set; }

        public string LanguageCode { get; set; }
    }
}
