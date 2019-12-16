using System.Threading.Tasks;

using Xunit;

using Controllers.Instances;

using Interfaces.Delegates.Convert;

using Models.Requests;

namespace Delegates.Convert.Requests.Tests
{
    public class ConvertRequestsDataToResolvedDependenciesDelegateTests
    {
        private readonly IConvertAsyncDelegate<RequestsData, Task<RequestsData>> convertRequestsDataToResolvedDependenciesDelegate;
        private readonly Models.Status.Status testStatus;

        public ConvertRequestsDataToResolvedDependenciesDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(true);

            convertRequestsDataToResolvedDependenciesDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertRequestsDataToResolvedDependenciesDelegate))
                as ConvertRequestsDataToResolvedDependenciesDelegate;

            testStatus = new Models.Status.Status();
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
                    requestsData,
                    testStatus);

            Assert.Equal(expectedMethodsCount, requestsDataWithDependencies.Methods.Count);
            Assert.Equal(expectedCollectionsCount, requestsDataWithDependencies.Collections.Count);
        }
    }}