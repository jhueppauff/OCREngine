using System.Threading.Tasks;

namespace OCREngine.Function.Clients
{
    public interface IFileWebClient
    {
        Task<string> DownloadFile(string path, string downloadUrl);
    }
}