using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace OCREngine.Function.Clients
{
    public class FileWebClient
    {
        public async Task<string> DownloadFile(string path, string downloadUrl)
        {
            string filePath = string.Empty;

            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;

                //WebRequest request = WebRequest.Create(downloadUrl);
                //WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);

                //var contentDisposition = response.Headers["Content-Disposition"];
                
                string fileName = string.Empty;

                //if (!string.IsNullOrEmpty(contentDisposition))
                //{
                //    string lookFor = "filename=";
                //    int index = contentDisposition.IndexOf(lookFor, StringComparison.CurrentCultureIgnoreCase);
                //    if (index >= 0)
                //        fileName = contentDisposition.Substring(index + lookFor.Length);
                //}
                //else
                //{
                    // Fallback if content-disposition header is not supplied by the server
                    fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1, (downloadUrl.Length - downloadUrl.LastIndexOf("/") - 1));
                    fileName?.TrimEnd('/');
                    fileName = fileName.Split("?")[0].Split("#")[0];
                //}

                if (fileName.Length > 0)
                {
                    filePath = Path.Combine(path, fileName);

                    if (!File.Exists(filePath))
                    {
                        await client.DownloadFileTaskAsync(downloadUrl, filePath);
                    }
                    else
                    {
                        throw new Domain.Exceptions.FileExistsException($"File {filePath} does already exists");
                    }
                }
            }

            return filePath;
        }
    }
}