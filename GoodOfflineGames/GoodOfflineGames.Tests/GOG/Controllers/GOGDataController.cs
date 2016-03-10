using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG.Model;
using GOG.Interfaces;
using GOG.Controllers;
using GOG.SharedControllers;
using GOG.SharedModels;

namespace GoodOfflineGames.Tests
{
    [TestClass]
    public class GOGDataControllerTests
    {
        private IGetStringDelegate gogDataController;

        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private IDeserializeDelegate<string> stringDeserializeDelegate;

        public GOGDataControllerTests()
        {
            uriController = new UriController(); // no need to mock UriController
            stringNetworkController = new MockNetworkController(uriController);
            stringDeserializeDelegate = new JSONStringController();

            gogDataController = new GOGDataController(stringNetworkController);
        }

        [TestMethod]
        public void ExtractsGOGDataWhenItIsAvailable()
        {
            var resultString = gogDataController.GetString(Urls.GameProductDataPageTemplate).Result;

            Assert.IsFalse(string.IsNullOrEmpty(resultString));
            Assert.IsFalse(string.IsNullOrWhiteSpace(resultString));

            try
            {
                var result = stringDeserializeDelegate.Deserialize<GOGData>(resultString);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.ProductData);
                Assert.IsNotNull(result.ProductData.Title);

                Assert.AreEqual(result.ProductData.Title, "System Shock: Enhanced Edition");
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
