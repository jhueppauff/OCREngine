using OCREngine.Domain.Entities.Vision;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCREngine.DurableFunction.Model
{
    public class ConstructDocumentRequest
    {
        public List<string> Files { get; set; }

        public List<OcrResult> OcrResults { get; set; }
    }
}
