using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using Controllers.Instances;

using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Models.Requests;

using TestModels.ArgsDefinitions;
using TestModels.Requests;

namespace Delegates.Convert.Requests.Tests
{
    public class ConvertArgsToRequestsDelegateTests
    {
        private readonly IConvertAsyncDelegate<string[], IAsyncEnumerable<Request>> convertArgsToRequestsDelegate;

        public ConvertArgsToRequestsDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(DependencyContext.Default | DependencyContext.Test);

            this.convertArgsToRequestsDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertArgsToRequestsDelegate))
                as ConvertArgsToRequestsDelegate;
        }

        private async Task<List<Request>> ConvertArgsToRequests(string args)
        {
            var requests = convertArgsToRequestsDelegate.ConvertAsync(args.Split(" "));
            var requestsList = new List<Request>();

            await foreach (var request in requests)
                requestsList.Add(request);

            return requestsList;
        }

        [Theory]
        [InlineData("sync")]
        [InlineData("download productfiles")]
        public async void CanConvertArgsToRequests(string spaceSeparatedArgs)
        {
            var requests = await ConvertArgsToRequests(spaceSeparatedArgs);

            var referenceRequests = ReferenceRequests.Requests[spaceSeparatedArgs];

            Assert.Equal(requests.Count(), referenceRequests.Count());

            for (var ii = 0; ii < requests.Count(); ii++)
            {
                var request = requests.ElementAt(ii);
                var referenceRequest = referenceRequests.ElementAt(ii);
                Assert.Equal(referenceRequest.Method, request.Method);
                Assert.Equal(referenceRequest.Collection, request.Collection);
            }
        }
    }
}