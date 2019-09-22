using Microsoft.WindowsAzure.Storage.Table;

namespace OCREngine.Domain.Entities
{
    public class ImageRequest : TableEntity
    {
        public ImageRequest(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public ImageRequest()
        {

        }

        public string BlobUri { get; set; }

        public string ProcessingState { get; set; }

        public string Exception { get; set; }

        public Vision.OcrResult Response { get; set; }
    }
}
