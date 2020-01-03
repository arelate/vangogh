using System;
using System.Linq;
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
    public class ConvertRequestDataToRequestsDelegateTests
    {
        private readonly IConvertAsyncDelegate<RequestsData, IAsyncEnumerable<Request>> convertRequestsDataToRequestsDelegate;

        public ConvertRequestDataToRequestsDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(DependencyContext.Default | DependencyContext.Test);

            convertRequestsDataToRequestsDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertRequestsDataToRequestsDelegate))
                as ConvertRequestsDataToRequestsDelegate;
        }

        [Theory]
        [InlineData(2, "update", "products", "accountproducts")] // two applicable collections
        [InlineData(1, "update", "products", "productfiles")] // two collections, but only one applicable
        [InlineData(0, "update", "productfiles")] // no applicable collections
        public async void CanConvertRequestDataToRequests(
            int expectedRequestsCount,
            string method,
            params string[] collections)
        {
            var requestsData = new RequestsData();
            requestsData.Methods.Add(method);
            requestsData.Collections.AddRange(collections);
            // Parameters are not part of the test, since they don't affect number of requests            

            var requests = convertRequestsDataToRequestsDelegate.ConvertAsync(
                requestsData);
            
            var requestsCount = 0;
            await foreach (var request in requests)
                requestsCount++;

            Assert.Equal(expectedRequestsCount, requestsCount);
        }
    }
}