using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using Controllers.Instances;

using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Delegates.Convert.Requests;

using Models.Requests;

using TestModels.ArgsDefinitions;

namespace Delegates.Convert.Requests.Tests
{
    public class ConvertRequestsDataToResolvedCollectionsDelegateTests
    {
        private readonly IConvertAsyncDelegate<RequestsData, Task<RequestsData>> convertRequestsDataToResolvedCollectionsDelegate;

        public ConvertRequestsDataToResolvedCollectionsDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(DependencyContext.Default | DependencyContext.Test);

            convertRequestsDataToResolvedCollectionsDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertRequestsDataToResolvedCollectionsDelegate))
                as ConvertRequestsDataToResolvedCollectionsDelegate;
        }

        [Theory]
        [InlineData(8, "update")] // no collections - should use all as default
        [InlineData(1, "update", "products")] // applicable collection
        [InlineData(9, "update", "productfiles")] // not applicable collection
        [InlineData(0, "authorize")] // no collection and none expected
        [InlineData(1, "authorize", "products")] // not applicable collection
        public async void CanConvertRequestsDataToResolvedCollections(
            int expectedCollectionsCount,
            string method, 
            params string[] collections)
        {
            var requestsData = new RequestsData();
            requestsData.Methods.Add(method);
            requestsData.Collections.AddRange(collections);

            var requestsDataWithCollections =
                await convertRequestsDataToResolvedCollectionsDelegate.ConvertAsync(
                    requestsData);

            Assert.Equal(expectedCollectionsCount, requestsDataWithCollections.Collections.Count);
        }
    }
}