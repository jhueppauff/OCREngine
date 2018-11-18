namespace OCREngine.Domain.Entities
{
    using OCREngine.Domain.Infrastructure;
    using System;
    using System.Collections.Generic;

    public class OcrRequest : ValueObject
    {
        public Guid RequestId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public ProcessingStates ProcessingState { get; set; }

        private OcrRequest()
        {
                
        }

        public static explicit operator OcrRequest(string value)
        {
            return new OcrRequest();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }
    }
}
