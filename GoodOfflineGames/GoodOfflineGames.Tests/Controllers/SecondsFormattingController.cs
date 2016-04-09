using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG.Interfaces;
using GOG.Controllers;

namespace GoodOfflineGames.Tests.Controllers
{
    [TestClass]
    public class SecondsFormattingControllerTests
    {
        private IFormattingController secondsFormattingController;

        public SecondsFormattingControllerTests()
        {
            secondsFormattingController = new SecondsFormattingController();
        }

        [TestMethod]
        public void BytesAreFormattedAsExpected()
        {
            Assert.AreEqual(secondsFormattingController.Format(0), "zero seconds");
            Assert.AreEqual(secondsFormattingController.Format(59), "59 second(s)");
            Assert.AreEqual(secondsFormattingController.Format(60), "1 minute(s)");
            Assert.AreEqual(secondsFormattingController.Format(59* 60), "59 minute(s)");
            Assert.AreEqual(secondsFormattingController.Format(60 * 60), "1 hour(s)");
            Assert.AreEqual(secondsFormattingController.Format(23 * 60 * 60), "23 hour(s)");
            Assert.AreEqual(secondsFormattingController.Format(24 * 60 * 60), "1 day(s)");
            Assert.AreEqual(secondsFormattingController.Format(6 * 24 * 60 * 60), "6 day(s)");
            Assert.AreEqual(secondsFormattingController.Format(7 * 24 * 60 * 60), "1 week(s)");
            Assert.AreEqual(secondsFormattingController.Format(10 * 7 * 24 * 60 * 60), "10 week(s)");
        }
    }
}
