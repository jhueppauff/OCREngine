using System.Threading.Tasks;

namespace OCREngine.Application.Clients
{
    public interface IFileWebClient
    {
        Task<string> DownloadFile(string path, string downloadUrl);
    }
}