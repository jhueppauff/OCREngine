using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Models
{
    public class DocumentProcessingStatus
    {
        public Guid JobId { get; set; }

        public ProcessStatus ProcessStatus { get; set; }

        public string Message { get; set; }

        public Exception ProcessingException { get; set; }

        public string Protocol { get; set; }
    }
}
