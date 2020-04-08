using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delegates.Convert.Requests;
using Interfaces.Delegates.Convert;
using Models.Requests;
using TestDelegates.Convert.Types;
using TestModels.Requests;
using Xunit;

namespace Tests.Delegates.Convert.Requests
{
    public class ConvertArgsToRequestsDelegateTests
    {
        private readonly IConvertAsyncDelegate<string[], IAsyncEnumerable<Request>> convertArgsToRequestsDelegate;

        public ConvertArgsToRequestsDelegateTests()
        {
            convertArgsToRequestsDelegate = ConvertTypeToInstanceDelegateInstances.Test.Convert(
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