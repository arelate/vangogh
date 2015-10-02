using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG.Model;
using GOG.Interfaces;
using GOG.Controllers;
using GOG.SharedControllers;
using GOG.SharedModels;

namespace GoodOfflineGames.Tests
{
    [TestClass]
    public class GameDetailsControllerTests
    {
        private IProductCoreController<GameDetails> gameDetailsController;
        private IProductCoreController<Product> productsController;
        private IProductCoreController<ProductData> productDataController;

        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private IDeserializeDelegate<string> stringDeserializeDelegate;

        private IList<Product> products;
        private IList<ProductData> productsData;
        private IList<GameDetails> gameDetails;

        private Product productWithDLC;
        private Product productWithMultipleLanguages;

        private IList<Product> owned;
        private IList<long> updated;

        private ICollectionController<Product> ownedController;
        private ICollectionController<long> updatedController;

        private string[] supportedLanguages;

        public GameDetailsControllerTests()
        {
            products = new List<Product>();
            productsData = new List<ProductData>();
            gameDetails = new List<GameDetails>();

            supportedLanguages = new string[2] {
                "English",
                "русский"
            };

            productWithDLC = new Product();
            productWithDLC.Id = 1;

            productWithMultipleLanguages = new Product();
            productWithMultipleLanguages.Id = 2;

            uriController = new UriController(); // no need to mock UriController
            stringNetworkController = new MockNetworkController(uriController);
            stringDeserializeDelegate = new JSONStringController();

            productsController = new ProductsController(products);

            productDataController = new ProductDataController(productsData,
                stringNetworkController,
                stringDeserializeDelegate);

            owned = new List<Product>() { productWithDLC, productWithMultipleLanguages };
            ownedController = new OwnedController(owned);

            updated = new List<long>() { productWithDLC.Id, productWithMultipleLanguages.Id };
            updatedController = new UpdatedController(updated);

            gameDetailsController = new GameDetailsController(gameDetails,
                stringNetworkController,
                stringDeserializeDelegate,
                supportedLanguages);
        }

        private void UpdateGameDetailsForProduct(Product product)
        {
            products.Clear();
            productsController.Add(product);

            var items = new List<string>() { product.Id.ToString() };

            gameDetails.Clear();

            gameDetailsController.Update(items).Wait();
        }

        [TestMethod]
        public void UpdateGameDetailsForProductWithDLC()
        {
            UpdateGameDetailsForProduct(productWithDLC);

            Assert.AreEqual(gameDetails.Count, 1);
            Assert.IsNotNull(gameDetails[0]);
            Assert.AreEqual(gameDetails[0].Title, "Darksiders II");
            Assert.IsNotNull(gameDetails[0].DLCs);
            Assert.AreEqual(gameDetails[0].DLCs.Count, 1);
            Assert.AreEqual(gameDetails[0].DLCs[0].Title, "Darksiders II - Complete DLC Pack");
        }

        [TestMethod]
        public void UpdateGameDetailsForProductWithMultipleLanguages()
        {
            UpdateGameDetailsForProduct(productWithMultipleLanguages);

            Assert.AreEqual(gameDetails.Count, 1);
            Assert.IsNotNull(gameDetails[0]);
            Assert.AreEqual(gameDetails[0].Title, "Saints Row: The Third - The Full Package");
            Assert.IsNull(gameDetails[0].DynamicDownloads);
            Assert.IsNotNull(gameDetails[0].LanguageDownloads);
            Assert.AreEqual(gameDetails[0].LanguageDownloads.Count, supportedLanguages.Length);

            foreach (var dwn in gameDetails[0].LanguageDownloads)
            {
                Assert.IsNull(dwn.Linux);
                Assert.IsNull(dwn.Mac);
                Assert.IsNotNull(dwn.Windows);
                Assert.AreEqual(dwn.Windows.Count, 5);
            }
        }
    }
}
