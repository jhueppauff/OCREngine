namespace Tests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OCREngine.Function.Clients;
    using FluentAssertions;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public class FileWebClientTests
    {

        [TestMethod]
        public async Task DownloadFileFromServer()
        {
            string downloadUrl = "https://sdkdev.blob.core.windows.net/unittests/EDDSKJFK.txt?st=2018-12-27T13%3A27%3A40Z&se=2020-12-28T13%3A27%3A00Z&sp=rl&sv=2017-07-29&sr=b&sig=vcNW4vbC9Fxs7Uk4KDfaycgOA91Rhr%2BS9i3%2FVbTDWYU%3D";

            string path = Path.GetTempPath();
            FileWebClient client = new FileWebClient();
            string newPath = await client.DownloadFile(path, downloadUrl).ConfigureAwait(false);

            newPath.Should().NotBeNullOrEmpty("because the Download should return a FilePath");

            newPath.Should().Contain(path, "File Path should contain the provided path");
            newPath.Should().Contain("EDDSKJFK.txt", "File Path should contain the file name of the remote file");

            File.Delete(newPath);
        }

        [TestMethod]
        public async Task Download_Image_Should_be_a_readable_Image()
        {
            string downloadUrl = "https://ocrenginefusa.blob.core.windows.net/demo/vrkIj.png?st=2019-01-08T17%3A08%3A49Z&se=2020-01-09T17%3A08%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=AzrtKxgAx%2FSJaLPD3%2FKRI0Zdk0l0Q5H1zj8unKLgD1A%3D";

            string path = Path.GetTempPath();
            FileWebClient client = new FileWebClient();
            string newPath = await client.DownloadFile(path, downloadUrl).ConfigureAwait(false);

            try
            {
                using (Image test = Image.FromFile(newPath))
                {
                    test.RawFormat.Should().Equals(ImageFormat.Bmp);
                }
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            finally
            {
                File.Delete(newPath);
            }
        }
    }
}
