namespace OCREngine.WebApi.PdfConverter
{
    public class Files
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Vision.Models.OcrResults OcrResultn { get; set; }
    }
}
