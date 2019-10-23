namespace OCREngine.Domain.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class FileExistsException : Exception
    {
        public FileExistsException()
        {
        }

        public FileExistsException(string message) : base(message)
        {
        }

        public FileExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}