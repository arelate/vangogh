using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG.Interfaces;
using GOG.Controllers;

namespace GoodOfflineGames.Tests.Controllers
{
    [TestClass]
    public class BytesFormattingControllerTests
    {
        private IFormattingController bytesFormattingController;

        public BytesFormattingControllerTests()
        {
            bytesFormattingController = new BytesFormattingController();
        }

        [TestMethod]
        public void BytesAreFormattedAsExpected()
        {
            Assert.AreEqual(bytesFormattingController.Format(0), "zero bytes");
            Assert.AreEqual(bytesFormattingController.Format(1023), "1023 bytes");
            Assert.AreEqual(bytesFormattingController.Format(1024), "1 KB");
            Assert.AreEqual(bytesFormattingController.Format(1023 * 1024), "1023 KB");
            Assert.AreEqual(bytesFormattingController.Format(1024 * 1024), "1 MB");
            Assert.AreEqual(bytesFormattingController.Format(1023 * 1024 * 1024), "1023 MB");
            Assert.AreEqual(bytesFormattingController.Format(1024 * 1024 * 1024), "1 GB");
            Assert.AreEqual(bytesFormattingController.Format((long)1023 * 1024 * 1024 * 1024), "1023 GB");
            Assert.AreEqual(bytesFormattingController.Format((long)1024 * 1024 * 1024 * 1024), "1 TB");
            Assert.AreEqual(bytesFormattingController.Format((long)1023 * 1024 * 1024 * 1024 * 1024), "1023 TB");
            Assert.AreEqual(bytesFormattingController.Format((long)1024 * 1024 * 1024 * 1024 * 1024), "1024 TB");
        }
    }
}
