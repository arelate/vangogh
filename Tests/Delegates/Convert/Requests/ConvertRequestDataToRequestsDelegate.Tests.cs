using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

using Controllers.Collection;

using Interfaces.Delegates.Convert;

using Delegates.Convert.Requests;

using Models.Requests;

using TestModels.ArgsDefinitions;

namespace Delegates.Convert.Requests.Tests
{
    public class ConvertRequestDataToRequestsDelegateTests
    {
        private IConvertDelegate<RequestsData, IEnumerable<Request>> convertRequestsDataToRequestsDelegate;

        public ConvertRequestDataToRequestsDelegateTests()
        {
            var collectionController = new CollectionController();

            convertRequestsDataToRequestsDelegate = 
                new ConvertRequestsDataToRequestsDelegate(
                    ReferenceArgsDefinition.ArgsDefinition,
                    collectionController);
        }

        [Theory]
        [InlineData(2, "update", "products", "accountproducts")] // two applicable collections
        [InlineData(1, "update", "products", "productfiles")] // two collections, but only one applicable
        [InlineData(0, "update", "productfiles")] // no applicable collections
        public void CanConvertRequestDataToRequests(
            int requestsCount,
            string method,
            params string[] collections)
        {
            var requestsData = new RequestsData();
            requestsData.Methods.Add(method);
            requestsData.Collections.AddRange(collections);
            // Parameters are not part of the test, since they don't affect number of requests            

            var requests = convertRequestsDataToRequestsDelegate.Convert(requestsData);

            Assert.Equal(requestsCount, requests.Count());
        }
    }
}