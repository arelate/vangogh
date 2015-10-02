using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG;
using GOG.Interfaces;
using GOG.Model;

namespace GoodOfflineGames.Tests
{
    [TestClass]
    public class FilterDelegateTests
    {
        IFilterDelegate<Product> filter;

        public FilterDelegateTests()
        {
            filter = new ExistingProductsFilter();
        }

        private IList<Product> CreateCollection(int fromId, int toId)
        {
            var collection = new List<Product>(toId - fromId + 1);
            for (var ii= fromId; ii<=toId; ii++)
            {
                var p = new Product();
                p.Id = ii;
                collection.Add(p);
            }

            return collection;
        }

        [TestMethod]
        public void FiltersAllElements()
        {
            var collection1 = CreateCollection(1, 10);
            var collection2 = CreateCollection(1, 10);

            Assert.AreEqual(collection1.Count, 10);
            Assert.AreEqual(collection2.Count, 10);
            Assert.AreEqual(collection1.Count, collection2.Count);

            var result = filter.Filter(collection1, collection2);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), 0);
        }
    }
}
