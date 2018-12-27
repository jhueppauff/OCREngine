using System;
using System.IO;
using System.Net;

namespace OCREngine.Function.Clients
{
    public class FileWebClient
    {
        public string DownloadFile(string path, string downloadUrl)
        {
            string filePath = string.Empty;

            using (WebClient client = new WebClient())
            {
                using (Stream rawStream = client.OpenRead(downloadUrl))
                {
                    string fileName = string.Empty;
                    string contentDisposition = client.ResponseHeaders["content-disposition"];

                    if (!string.IsNullOrEmpty(contentDisposition))
                    {
                        string lookFor = "filename=";
                        int index = contentDisposition.IndexOf(lookFor, StringComparison.CurrentCultureIgnoreCase);
                        if (index >= 0)
                            fileName = contentDisposition.Substring(index + lookFor.Length);
                    }
                    else
                    {
                        // Fallback
                        fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1, (downloadUrl.Length - downloadUrl.LastIndexOf("/") - 1));
                        fileName?.TrimEnd('/');
                        fileName = fileName.Split("?")[0].Split("#")[0];
                    }

                    if (fileName.Length > 0)
                    {
                        using (StreamReader reader = new StreamReader(rawStream))
                        {
                            filePath = Path.Combine(path, fileName);
                            File.WriteAllText(filePath, reader.ReadToEnd());
                            reader.Close();
                        }
                    }

                    rawStream.Close();
                }
            }

            return filePath;
        }
    }
}
