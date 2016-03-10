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
    public class ProductDataControllerTests
    {
        private IProductCoreController<ProductData> productDataController;
        private IProductCoreController<Product> productsController;

        private IGetStringDelegate gogDataController;

        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private IDeserializeDelegate<string> stringDeserializeDelegate;

        private IList<ProductData> productsData;
        private IList<Product> products;

        public ProductDataControllerTests()
        {
            productsData = new List<ProductData>();
            products = new List<Product>();

            uriController = new UriController(); // no need to mock UriController
            stringNetworkController = new MockNetworkController(uriController);
            stringDeserializeDelegate = new JSONStringController();

            gogDataController = new GOGDataController(stringNetworkController);

            productsController = new ProductsController(products);

            productDataController = new ProductDataController(productsData, 
                gogDataController, 
                stringDeserializeDelegate);
        }

        [TestMethod]
        public void WasCreatedToUpdateWithoutFailing()
        {
            try
            {
                productDataController.Update(new List<string>()).Wait();
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdatesProductData()
        {
            productsData.Clear();

            var items = new List<string>() { "{0}" };

            productDataController.Update(items).Wait();

            Assert.AreEqual(productsData.Count, 1);
            Assert.IsNotNull(productsData[0]);
            Assert.IsNotNull(productsData[0].Id);
            Assert.IsFalse(string.IsNullOrEmpty(productsData[0].Title));
            Assert.IsFalse(string.IsNullOrWhiteSpace(productsData[0].Title));
        }
    }
}
