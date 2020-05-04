using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delegates.Conversions.Requests;
using Interfaces.Delegates.Conversions;
using Models.Requests;
using Tests.TestDelegates.Conversions.Types;
using Tests.TestModels.Requests;
using Xunit;

namespace Tests.Delegates.Conversions.Requests
{
    public class ConvertArgsToRequestsDelegateTests
    {
        private readonly IConvertAsyncDelegate<string[], IAsyncEnumerable<Request>> convertArgsToRequestsDelegate;

        public ConvertArgsToRequestsDelegateTests()
        {
            convertArgsToRequestsDelegate = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                    typeof(ConvertArgsToRequestsDelegate))
                as ConvertArgsToRequestsDelegate;
        }

        private async Task<List<Request>> ConvertArgsToRequests(string args)
        {
            var requests = 
                convertArgsToRequestsDelegate.ConvertAsync(args.Split(" "));
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