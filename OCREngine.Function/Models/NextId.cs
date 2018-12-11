using Microsoft.WindowsAzure.Storage.Table;

namespace OCREngine.Function.Models
{
    public class NextId : TableEntity
    {
         public int Id { get; set; }
    }
}