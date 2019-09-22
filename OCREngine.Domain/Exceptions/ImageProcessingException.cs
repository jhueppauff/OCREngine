namespace OCREngine.Domain.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class ImageProcessingException : Exception
    {
        public ImageProcessingException()
        {
        }

        public ImageProcessingException(string message) : base(message)
        {
        }

        public ImageProcessingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImageProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
