using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
{
    public class DocumentInput
    {
        public string Name { get; set; }

        public byte[] Content { get; set; }
    }
}
