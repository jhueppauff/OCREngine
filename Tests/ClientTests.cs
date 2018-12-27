namespace Tests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OCREngine.Function.Clients;
    using FluentAssertions;

    [TestClass]
    public class ClientTests
    {

        [TestMethod]
        public void DownloadFileFromServer()
        {
            string downloadUrl = "https://sdkdev.blob.core.windows.net/unittests/EDDSKJFK.txt?st=2018-12-27T13%3A27%3A40Z&se=2020-12-28T13%3A27%3A00Z&sp=rl&sv=2017-07-29&sr=b&sig=vcNW4vbC9Fxs7Uk4KDfaycgOA91Rhr%2BS9i3%2FVbTDWYU%3D";

            string path = Path.GetTempPath();
            FileWebClient client = new FileWebClient();
            string newPath = client.DownloadFile(path, downloadUrl);

            newPath.Should().NotBeNullOrEmpty("because the Download should return a FilePath");

            newPath.Should().Contain(path, "File Path should contain the provided path");
            newPath.Should().Contain("EDDSKJFK.txt", "File Path should contain the file name of the remote file");

            File.Delete(newPath);
        }
    }
}
