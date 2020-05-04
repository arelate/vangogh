using System;
using System.Collections.Generic;
using Delegates.Collections.Requests;
using Interfaces.Delegates.Collections;
using Tests.TestDelegates.Conversions.Types;
using Xunit;

namespace Tests.Delegates.Collections.Requests
{
    public class MethodOrderCompareDelegateTests
    {
        private ISortAsyncDelegate<string> sortRequestsMethodsByOrderAsyncDelegate;

        public MethodOrderCompareDelegateTests()
        {
            sortRequestsMethodsByOrderAsyncDelegate =
                DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                        typeof(SortRequestsMethodsByOrderAsyncDelegate))
                    as SortRequestsMethodsByOrderAsyncDelegate;
        }

        [Theory]
        [InlineData(false, "update", "authorize")]
        [InlineData(true, "authorize", "update")]
        public async void CanCompareMethodOrder(bool expectedOrder, string method1, string method2)
        {
            var methods = new List<string>() {method1, method2};
            await sortRequestsMethodsByOrderAsyncDelegate.SortAsync(
                methods);

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
            var methods = new List<string>() {method1, method2};

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await sortRequestsMethodsByOrderAsyncDelegate.SortAsync(
                        methods));
        }
    }
}