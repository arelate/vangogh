using System.Threading.Tasks;
using Delegates.Convert.Requests;
using Interfaces.Delegates.Convert;
using Models.Requests;
using Tests.TestDelegates.Convert.Types;
using Xunit;

namespace Tests.Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedDependenciesDelegateTests
    {
        private readonly IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
            convertRequestsDataToResolvedDependenciesDelegate;

        public ConvertRequestsDataToResolvedDependenciesDelegateTests()
        {
            convertRequestsDataToResolvedDependenciesDelegate = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                    typeof(ConvertRequestsDataToResolvedDependenciesDelegate))
                as ConvertRequestsDataToResolvedDependenciesDelegate;
        }

        [Theory]
        [InlineData(2, 1, "update", "accountproducts")]
        [InlineData(2, 1, "update", "apiproducts")]
        [InlineData(2, 1, "update", "gamedetails")]
        [InlineData(2, 1, "update", "updated")]
        [InlineData(2, 1, "update", "wishlisted")]
        [InlineData(4, 2, "download", "productfiles")] // complex dependency
        [InlineData(1, 1, "update", "products")] // no dependency
        public async void CanConvertRequestsDataToResolvedDependencies(
            int expectedMethodsCount,
            int expectedCollectionsCount,
            string method,
            string collection)
        {
            var requestsData = new RequestsData();
            requestsData.Methods.Add(method);
            requestsData.Collections.Add(collection);

            var requestsDataWithDependencies =
                await convertRequestsDataToResolvedDependenciesDelegate.ConvertAsync(
                    requestsData);

            Assert.Equal(expectedMethodsCount, requestsDataWithDependencies.Methods.Count);
            Assert.Equal(expectedCollectionsCount, requestsDataWithDependencies.Collections.Count);
        }
    }
}