using System;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Sort;

using Controllers.Instances;

namespace Delegates.Sort.Requests.Tests
{
    public class MethodOrderCompareDelegateTests
    {
        private ISortAsyncDelegate<string> sortRequestsMethodsByOrderAsyncDelegate;
        private readonly Models.Status.Status testStatus;

        public MethodOrderCompareDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(true);

            sortRequestsMethodsByOrderAsyncDelegate = singletonInstancesController.GetInstance(
                typeof(SortRequestsMethodsByOrderAsyncDelegate))
                as SortRequestsMethodsByOrderAsyncDelegate;

            testStatus = new Models.Status.Status();
        }

        [Theory]
        [InlineData(false, "update", "authorize")]
        [InlineData(true, "authorize", "update")]
        public async void CanCompareMethodOrder(bool expectedOrder, string method1, string method2)
        {
            var methods = new List<string>() { method1, method2 };
            await sortRequestsMethodsByOrderAsyncDelegate.SortAsync(
                methods,
                testStatus);

            var firstIndex = expectedOrder ? 0 : 1;
            var secondIndex = expectedOrder ? 1 : 0;

            Assert.Equal(2, methods.Count);
            Assert.Equal(method1, methods[firstIndex]);
            Assert.Equal(method2, methods[secondIndex]);
        }

        [Theory]
        [InlineData("arbitrarystring", "authorize")] // first argument
        [InlineData("update", "arbitrarystring")] // second argument
        [InlineData("arbitrarystring1", "arbitrarystring2")] // both arguments
        [InlineData(null, null)]
        public async void MethodOrderCompareDelegateThrowsForUnknownMethods(string method1, string method2)
        {
            var methods = new List<string>() { method1, method2 };

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                await sortRequestsMethodsByOrderAsyncDelegate.SortAsync(
                    methods,
                    testStatus));
        }
    }
}