//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;

//using GOG.Interfaces;
//using GOG.Controllers;
//using GOG.SharedControllers;
//using GOG.Model;
//using GOG.SharedModels;

//namespace GoodOfflineGames.Tests.GOG.Controllers
//{
//    [TestClass]
//    public class ProductsResultControllerTests
//    {
//        private ProductsResult existingProductsResult;
//        private ProductsController productsResultController;
//        private IUriController uriController;
//        private IStringGetController stringGetController;
//        private IStringifyController serializationController;
//        private IConsoleController consoleController;

//        public ProductsResultControllerTests()
//        {
//            uriController = new UriController(); // no need to mock UriController
//            serializationController = new JSONController(); // no need to mock JSONController
//            stringGetController = new MockNetworkController(uriController);
//            consoleController = new MockConsoleController();

//            productsResultController = new ProductsResultController(
//                stringGetController,
//                serializationController,
//                consoleController);
//        }

//        [TestMethod]
//        public List<Product> ProductsResultControllerCanGetAll()
//        {
//            var products = productsResultController.GetAll(
//                Urls.GamesAjaxFiltered,
//                QueryParameters.GamesAjaxFiltered).Result;

//            Assert.AreEqual(products.Count, 100); // 2 pages * 50 per page

//            return products;
//        }

//        [TestMethod]
//        public void ProductsResultControllerDoesntUpdateExisting()
//        {
//            var products = productsResultController.GetAll(
//                Urls.GamesAjaxFiltered,
//                QueryParameters.GamesAjaxFiltered).Result;

//            var productsResult = new ProductsResult(products);

//            var anotherProductsResult = productsResultController.GetUpdated(
//                Urls.GamesAjaxFiltered,
//                QueryParameters.GamesAjaxFiltered,
//                productsResult).Result;

//            // requesting new shouldn't product new products on the same data set
//            Assert.AreEqual(anotherProductsResult.Count, 0);
//        }

//        [TestMethod]
//        public List<Product> ProductsResultControllerCanGetAccountProducts(bool updated = false)
//        {
//            var accountProductsQuery = new Dictionary<string, string>(QueryParameters.AccountGetFilteredProducts);

//            accountProductsQuery["isUpdated"] = updated ? "1" : "0";

//            var accountProducts = productsResultController.GetAll(
//                Urls.AccountGetFilteredProducts,
//                accountProductsQuery).Result;

//            var expectedProducts = updated ? 5 : 100;

//            Assert.AreEqual(accountProducts.Count, expectedProducts); // 2 pages * 50 per page or 5 updated

//            return accountProducts;
//        }

//        [TestMethod]
//        public void ProductsResultControllerCanUpdateOwned()
//        {
//            var accountProducts = ProductsResultControllerCanGetAccountProducts(false);

//            var accountController = new ProductsResultController(accountProducts);

//            var allOwned = true;

//            foreach (Product product in accountProducts)
//            {
//                allOwned &= product.Owned;
//            }

//            // check that not all are marked owned yet
//            Assert.IsFalse(allOwned);

//            // set all to being owned
//            accountController.SetAllAsOwned(accountProducts);

//            allOwned = true;

//            foreach (Product product in accountProducts)
//            {
//                allOwned &= product.Owned;
//            }

//            // now all should be owned
//            Assert.IsTrue(allOwned);
//        }

//        //[TestMethod]
//        //public void ProductsResultControllerCanResetUpdated()
//        //{
//        //    var accountProducts = ProductsResultControllerCanGetAccountProducts(false);

//        //    var accountController = new ProductsResultController(accountProducts);

//        //    var noneUpdated = true;

//        //    foreach (Product product in accountProducts)
//        //    {
//        //        noneUpdated &= product.Updates == 0;
//        //    }

//        //    // check that there are updates
//        //    Assert.IsFalse(noneUpdated);

//        //    // reset updates
//        //    accountController.ResetUpdated();

//        //    noneUpdated = true;

//        //    foreach (Product product in accountProducts)
//        //    {
//        //        noneUpdated &= product.Updates == 0;
//        //    }

//        //    // now there shouldn't be updates
//        //    Assert.IsTrue(noneUpdated);
//        //}

//        [TestMethod]
//        public void ProductsResultControllerCanGetUpdated()
//        {
//            var updatedProducts = ProductsResultControllerCanGetAccountProducts(true);

//            Assert.AreEqual(updatedProducts.Count, 5);
//        }

//        [TestMethod]
//        public void ProductsResultControllerCanMergeOwned()
//        {
//            var products = ProductsResultControllerCanGetAll();
//            var ownedProducts = ProductsResultControllerCanGetAccountProducts(false);

//            var ownedController = new ProductsResultController(ownedProducts);
//            ownedController.SetAllAsOwned(ownedProducts);

//            var existingController = new ProductsResultController(products);

//            int existingCount = products.Count;
//            int existingOwned = 0;
//            // calculate delta
//            foreach (Product ownedProduct in ownedProducts)
//            {
//                foreach (Product existingProduct in products)
//                {
//                    if (existingProduct.Id == ownedProduct.Id)
//                    {
//                        existingOwned++;
//                        break;
//                    }
//                }
//            }

//            int newOwned = products.Count - existingOwned; // 100 - 29 = 71

//            existingController.MergeOwned(ownedProducts);

//            Assert.AreEqual(products.Count, existingOwned + newOwned);

//            int checkOwned = 0;
//            foreach (Product p in products)
//            {
//                if (p.Owned) checkOwned++;
//            }

//            Assert.AreEqual(checkOwned, existingOwned);
//        }

//        //[TestMethod]
//        //public void ProductsResultControllerCanMergeUpdated()
//        //{
//        //    var products = ProductsResultControllerCanGetAll();
//        //    var updatedProducts = ProductsResultControllerCanGetAccountProducts(true);

//        //    var existingController = new ProductsResultController(products);

//        //    int existingCount = products.Count;
//        //    int existingUpdated = 0;
//        //    // calculate delta
//        //    foreach (Product ownedProduct in updatedProducts)
//        //    {
//        //        foreach (Product existingProduct in products)
//        //        {
//        //            if (existingProduct.Id == ownedProduct.Id)
//        //            {
//        //                existingUpdated++;
//        //                break;
//        //            }
//        //        }
//        //    }

//        //    // not all updated products are owned (per mocked data)
//        //    Assert.IsTrue(existingUpdated < updatedProducts.Count);

//        //    if (existingUpdated > 0)
//        //    {
//        //        try
//        //        {
//        //            existingController.MergeUpdated(updatedProducts);
//        //        }
//        //        catch (System.InvalidOperationException)
//        //        {
//        //            // all is good
//        //        }
//        //    }
//        //    else
//        //    {
//        //        throw new System.InvalidOperationException("Should be at least 1 updated based on mock data.");
//        //    }
//        //}

//        // TODO: Test that GetAll doesn't add new entries
//        // TODO: Test that GetUpdated gets exactly the number of new entries
//    }
//}
