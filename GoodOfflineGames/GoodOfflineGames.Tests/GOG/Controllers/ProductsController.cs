using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG.Interfaces;
using GOG.Model;

namespace GoodOfflineGames.Tests
{
    [TestClass]
    public class ProductsController
    {
        private IProductCoreController<Product> productsController;
        private IList<Product> products;
        Product p;

        public ProductsController()
        {
            products = new List<Product>();
            productsController = new GOG.Controllers.ProductsController(products);
            p = new Product();
            p.Id = 1;
        }

        [TestMethod]
        public void FailsToUpdateWithoutProperSetup()
        {
            try
            {
                productsController.Update().Wait();
            }
            catch
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void DoesNotAddSameElementTwice()
        {
            products.Clear();

            var p = new Product();
            p.Id = 1;

            productsController.Add(p);

            Assert.AreEqual(products.Count, 1);

            productsController.Add(p);

            Assert.AreEqual(products.Count, 1);
        }

        [TestMethod]
        public void DoesNotInsertSameElementTwice()
        {
            products.Clear();

            productsController.Insert(0, p);

            Assert.AreEqual(products.Count, 1);

            productsController.Insert(0, p);

            Assert.AreEqual(products.Count, 1);
        }

        [TestMethod]
        public void ToleratesAddingNull()
        {
            products.Clear();
            productsController.Add(null);

            Assert.AreEqual(products.Count, 0);
        }

        [TestMethod]
        public void ToleratesInsertingNull()
        {
            products.Clear();
            productsController.Insert(0, null);

            Assert.AreEqual(products.Count, 0);
        }

        [TestMethod]
        public void RemovesElement()
        {
            products.Clear();
            productsController.Add(p);

            Assert.AreEqual(products.Count, 1);

            Assert.IsTrue(productsController.Remove(p));

            Assert.AreEqual(products.Count, 0);
        }

        [TestMethod]
        public void DoesNotRemoveNotExistantElement()
        {
            products.Clear();
            productsController.Add(p);

            Assert.AreEqual(products.Count, 1);

            Assert.IsFalse(productsController.Remove(new Product()));

            Assert.AreEqual(products.Count, 1);
        }

        [TestMethod]
        public void ToleratesRemovingNull()
        {
            products.Clear();
            productsController.Add(p);

            Assert.AreEqual(products.Count, 1);

            Assert.IsFalse(productsController.Remove(null));
        }

        [TestMethod]
        public void FindsElement()
        {
            products.Clear();
            productsController.Add(new Product());
            productsController.Add(p);

            Assert.AreEqual(products.Count, 2);

            var found = productsController.Find(p.Id);

            Assert.IsNotNull(found);
            Assert.AreEqual(p, found);
        }
    }
}
