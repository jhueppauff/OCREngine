namespace OCREngine.Domain.Entities.Vision
{
    using System;

    public class DocumentProcessingStatus
    {
        public Guid JobId { get; set; }

        public ProcessStatus ProcessStatus { get; set; }

        public string Message { get; set; }

        public Exception ProcessingException { get; set; }

        public string Protocol { get; set; }
    }
}
