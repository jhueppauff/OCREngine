namespace OCREngine.Application.Helper
{
    using OCREngine.Application.Clients;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class FileHelper
    {
        public static async Task<string> DownloadFile(string downloadPath)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            FileWebClient client = new FileWebClient();

            string fileName = await client.DownloadFile(path, downloadPath).ConfigureAwait(false);

            return fileName;
        }
    }
}
