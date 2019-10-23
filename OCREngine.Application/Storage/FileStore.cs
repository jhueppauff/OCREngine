using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace OCREngine.Application.Storage
{
    /// <summary>
    /// Class to deal with the Azure Blob Storage
    /// </summary>
    public class FileStore
    {
        /// <summary>
        /// The library container
        /// </summary>
        private readonly CloudBlobContainer libraryContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStore"/> class.
        /// </summary>
        /// <param name="blobConnectionString">The BLOB connection string.</param>
        /// <param name="containerName">Name of the container.</param>
        public FileStore(string blobConnectionString, string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            libraryContainer = blobClient.GetContainerReference(containerName);
        }

        /// <summary>
        /// Uploads the file to library.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="name">The name.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>Returns the URL to the Blob with an SAS Token</returns>
        public async Task<string> UploadFileToLibrary(Stream stream, string name, bool overwrite = false, string contentType = "image/jpg")
        {
            CloudBlockBlob blockBlob = libraryContainer.GetBlockBlobReference(name);
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Delete
            };

            if (!await blockBlob.ExistsAsync())
            {
                await blockBlob.UploadFromStreamAsync(stream);
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.SetPropertiesAsync();
            }

            string blobUri = blockBlob.Uri.ToString();
            string sas = blockBlob.GetSharedAccessSignature(sasConstraints);

            return $"{blobUri}{sas}";
        }

        /// <summary>
        /// Uploads to BLOB.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>Returns the Task <see cref="UploadFileToLibrary"/></returns>
        public Task<string> UploadToBlob(byte[] data, string name, bool overwrite = false, string contentType = "image/jpg")
        {
            return UploadFileToLibrary(new MemoryStream(data), name, overwrite, contentType);
        }

        /// <summary>
        /// Uploads to BLOB.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>Returns the Task <see cref="UploadFileToLibrary"/></returns>
        public Task<string> UploadToBlob(string data, string name, bool overwrite = false, string contentType = "image/jpg")
        {
            return UploadFileToLibrary(new MemoryStream(Convert.FromBase64String(data)), name, overwrite, contentType);
        }

        /// <summary>
        /// Deletes the BLOB.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Returns <see cref="Task{VoidResult}"/></returns>
        public async Task DeleteBlob(string uri)
        {
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(uri));
            await blob.DeleteAsync();
        }
    }
}
