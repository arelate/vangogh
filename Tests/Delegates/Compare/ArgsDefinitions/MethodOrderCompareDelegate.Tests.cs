using System;
using System.Collections.Generic;

using Xunit;

using Controllers.Collection;

using TestModels.ArgsDefinitions;

namespace Delegates.Compare.ArgsDefinitions.Tests
{
    public class MethodOrderCompareDelegateTests
    {
        private IComparer<string> methodOrderCompareDelegate;

        public MethodOrderCompareDelegateTests()
        {
            var collectionController = new CollectionController();

            methodOrderCompareDelegate = new MethodOrderCompareDelegate(
                ReferenceArgsDefinition.ArgsDefinition.Methods,
                collectionController);
        }

        [Theory]
        [InlineData(false, "update", "authorize")]
        [InlineData(true, "authorize", "update")]
        public void CanCompareMethodOrder(bool expectedOrder, string method1, string method2)
        {
            var result = methodOrderCompareDelegate.Compare(method1, method2);

            if (expectedOrder) Assert.True(result < 0);
            else Assert.True(result > 0);
        }

        [Theory]
        [InlineData("arbitrarystring", "authorize")] // first argument
        [InlineData("update", "arbitrarystring")] // second argument
        [InlineData("arbitrarystring1", "arbitrarystring2")] // both arguments
        public void MethodOrderCompareDelegateThrowsForUnknownMethods(string method1, string method2)
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                methodOrderCompareDelegate.Compare(method1, method2));
        }        
    }
}