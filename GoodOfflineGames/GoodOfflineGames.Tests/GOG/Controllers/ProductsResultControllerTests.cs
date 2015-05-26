using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using GOG.Interfaces;
using GOG.Controllers;
using GOG.SharedControllers;
using GOG.Models;
using GOG.SharedModels;

namespace GoodOfflineGames.Tests.GOG.Controllers
{
    [TestClass]
    public class ProductsResultControllerTests
    {
        private ProductsResult existingProductsResult;
        private ProductsResultController productsResultController;
        private IUriController uriController;
        private IStringRequestController stringRequestController;
        private ISerializationController serializationController;
        private IConsoleController consoleController;

        public ProductsResultControllerTests()
        {
            uriController = new UriController(); // no need to mock UriController
            serializationController = new JSONController(); // no need to mock JSONController
            stringRequestController = new MockNetworkController(uriController);
            consoleController = new MockConsoleController();

            existingProductsResult = new ProductsResult();

            productsResultController = new ProductsResultController(
                existingProductsResult,
                stringRequestController,
                serializationController);
        }

        [TestMethod]
        public ProductsResult ProductsResultControllerCanRequestNewSiteProducts()
        {
            var productsResult = productsResultController.RequestNew(existingProductsResult,
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                string.Empty).Result;

            Assert.AreEqual(productsResult.Products.Count, 100); // 2 pages * 50 per page

            return productsResult;
        }

        [TestMethod]
        public void ProductsResultControllerRequestsOnlyNewProducts()
        {
            var productsResult = productsResultController.RequestNew(
                existingProductsResult,
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                string.Empty).Result;

            var anotherProductsResult = productsResultController.RequestNew(
                productsResult,
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                string.Empty).Result;

            // requesting new shouldn't product new products on the same data set
            Assert.AreEqual(anotherProductsResult.Products.Count, productsResult.Products.Count);
        }

        [TestMethod]
        public void ProductsResultControllerRequestNewHandlesNullExisting()
        {
            var productsResult = productsResultController.RequestNew(
                null,
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                string.Empty).Result;

            Assert.AreEqual(productsResult.Products.Count, 100); // 2 pages * 50 per page
        }

        [TestMethod]
        public void ProductsResultControllerRequestNewHandlesMissingPageParameter()
        {
            var missingPageParameter = new Dictionary<string, string>(QueryParameters.GamesAjaxFiltered);
            missingPageParameter.Remove("page");

            var productsResult = productsResultController.RequestNew(
                null,
                Urls.GamesAjaxFiltered,
                missingPageParameter,
                consoleController,
                string.Empty).Result;

            Assert.AreEqual(productsResult.Products.Count, 100); // 2 pages * 50 per page
        }

        [TestMethod]
        public ProductsResult ProductsResultControllerCanRequestAccountProducts(bool updated = false)
        {
            var accountProductsQuery = new Dictionary<string, string>(QueryParameters.AccountGetFilteredProducts);

            accountProductsQuery["isUpdated"] = updated ? "1" : "0";

            var accountProductsResult = productsResultController.RequestNew(null,
                Urls.AccountGetFilteredProducts,
                accountProductsQuery,
                consoleController,
                string.Empty).Result;

            var expectedProducts = updated ? 5 : 100;

            Assert.AreEqual(accountProductsResult.Products.Count, expectedProducts); // 2 pages * 50 per page or 5 updated

            return accountProductsResult;
        }

        [TestMethod]
        public void ProductsResultControllerCanUpdateOwned()
        {
            var accountProductsResult = ProductsResultControllerCanRequestAccountProducts(false);

            var accountController = new ProductsResultController(accountProductsResult);

            var allOwned = true;

            foreach (Product product in accountProductsResult.Products)
            {
                allOwned &= product.Owned;
            }

            // check that not all are marked owned yet
            Assert.IsFalse(allOwned);

            // set all to being owned
            accountController.UpdateOwned();

            allOwned = true;

            foreach (Product product in accountProductsResult.Products)
            {
                allOwned &= product.Owned;
            }

            // now all should be owned
            Assert.IsTrue(allOwned);
        }

        [TestMethod]
        public void ProductsResultControllerCanResetUpdated()
        {
            var accountProductsResult = ProductsResultControllerCanRequestAccountProducts(false);

            var accountController = new ProductsResultController(accountProductsResult);

            var noneUpdated = true;

            foreach (Product product in accountProductsResult.Products)
            {
                noneUpdated &= product.Updates == 0;
            }

            // check that there are updates
            Assert.IsFalse(noneUpdated);

            // reset updates
            accountController.ResetUpdated();

            noneUpdated = true;

            foreach (Product product in accountProductsResult.Products)
            {
                noneUpdated &= product.Updates == 0;
            }

            // now there shouldn't be updates
            Assert.IsTrue(noneUpdated);
        }

        [TestMethod]
        public void ProductsResultControllerCanGetUpdated()
        {
            var updatedProductsResult = ProductsResultControllerCanRequestAccountProducts(true);

            Assert.AreEqual(updatedProductsResult.Products.Count, 5);
        }

        [TestMethod]
        public void ProductsResultControllerCanMergeOwned()
        {
            var productsResult = ProductsResultControllerCanRequestNewSiteProducts();
            var ownedProductsResult = ProductsResultControllerCanRequestAccountProducts(false);

            var ownedController = new ProductsResultController(ownedProductsResult);
            ownedController.UpdateOwned();

            var existingController = new ProductsResultController(productsResult);

            int existingCount = productsResult.Products.Count;
            int existingOwned = 0;
            // calculate delta
            foreach (Product ownedProduct in ownedProductsResult.Products)
            {
                foreach (Product existingProduct in productsResult.Products)
                {
                    if (existingProduct.Id == ownedProduct.Id)
                    {
                        existingOwned++;
                        break;
                    }
                }
            }

            int newOwned = productsResult.Products.Count - existingOwned; // 100 - 29 = 71

            existingController.MergeOwned(ownedProductsResult);

            Assert.AreEqual(productsResult.Products.Count, existingCount + newOwned);

            int checkOwned = 0;
            foreach (Product p in productsResult.Products)
            {
                if (p.Owned) checkOwned++;
            }

            Assert.AreEqual(checkOwned, ownedProductsResult.Products.Count);
        }

        [TestMethod]
        public void ProductsResultControllerCanMergeUpdated()
        {
            var productsResult = ProductsResultControllerCanRequestNewSiteProducts();
            var updatedProductsResult = ProductsResultControllerCanRequestAccountProducts(true);

            var existingController = new ProductsResultController(productsResult);

            int existingCount = productsResult.Products.Count;
            int existingUpdated = 0;
            // calculate delta
            foreach (Product ownedProduct in updatedProductsResult.Products)
            {
                foreach (Product existingProduct in productsResult.Products)
                {
                    if (existingProduct.Id == ownedProduct.Id)
                    {
                        existingUpdated++;
                        break;
                    }
                }
            }

            // not all updated products are owned (per mocked data)
            Assert.IsTrue(existingUpdated < updatedProductsResult.Products.Count);

            if (existingUpdated > 0)
            {
                try
                {
                    existingController.MergeUpdated(updatedProductsResult);
                }
                catch (System.InvalidOperationException)
                {
                    // all is good
                }
            }
            else
            {
                throw new System.InvalidOperationException("Should be at least 1 updated based on mock data.");
            }
        }
    }
}
