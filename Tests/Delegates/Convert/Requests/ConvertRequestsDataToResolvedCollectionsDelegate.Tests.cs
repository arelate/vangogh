using System.Threading.Tasks;
using Delegates.Convert.Requests;
using Interfaces.Delegates.Convert;
using Models.Requests;
using Tests.TestDelegates.Convert.Types;
using Xunit;

namespace Tests.Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedCollectionsDelegateTests
    {
        private readonly IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
            convertRequestsDataToResolvedCollectionsDelegate;

        public ConvertRequestsDataToResolvedCollectionsDelegateTests()
        {
            convertRequestsDataToResolvedCollectionsDelegate = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
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