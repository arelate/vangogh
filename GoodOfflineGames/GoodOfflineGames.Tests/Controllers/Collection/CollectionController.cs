using Microsoft.VisualStudio.TestTools.UnitTesting;

using Interfaces.Collection;

using Controllers.Collection;

namespace GoodOfflineGames.Tests.Controllers.Collection
{
    [TestClass]
    public class CollectionControllerTests: BaseTests
    {
        private ICollectionController collectionController;
        private int[] collection;

        public CollectionControllerTests()
        {
            collectionController = new CollectionController();
        }

        public override void InitializeData()
        {
            collection = new int[5] { 1, 2, 3, 4, 5 };
        }


        [TestMethod]
        public void CanFind()
        {
            InitializeData();

            var target = 1;
            
            var found = collectionController.Find(collection, i => i == target);

            Assert.AreEqual(found, target);
        }

        [TestMethod]
        public void CanMap()
        {
            InitializeData();

            var targetSum = 0;

            foreach (var i in collection)
                targetSum += i;

            var sum = 0;
            collectionController.Map(collection, i => sum += i);

            Assert.AreEqual(targetSum, sum);
        }

        [TestMethod]
        public void CanReduce()
        {
            InitializeData();

            var threshold = 3;

            var targetCount = 0;

            foreach (var i in collection)
                if (i < threshold) ++targetCount;

            var reduced = collectionController.Reduce(collection, i => i < threshold);

            var count = 0;
            foreach (var r in reduced)
                ++count;

            Assert.AreEqual(targetCount, count);
        }
    }
}
