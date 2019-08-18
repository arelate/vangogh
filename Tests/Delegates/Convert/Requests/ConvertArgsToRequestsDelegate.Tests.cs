using System.Linq;
using System.Collections.Generic;

using Xunit;

using Controllers.Collection;

using Interfaces.Delegates.Convert;

using Creators.Delegates.Convert.Requests;

using Models.Requests;

using TestModels.ArgsDefinitions;
using TestModels.Requests;

namespace Delegates.Convert.Requests.Tests
{
    public class ConvertArgsToRequestsDelegateTests
    {
        private IConvertDelegate<string[], IEnumerable<Request>> convertArgsToRequestsDelegate;

        public ConvertArgsToRequestsDelegateTests()
        {
            var collectionController = new CollectionController();

            var convertArgsToRequestsDelegateCreator = new ConvertArgsToRequestsDelegateCreator(
                ReferenceArgsDefinition.ArgsDefinition,
                collectionController);

            convertArgsToRequestsDelegate = convertArgsToRequestsDelegateCreator.CreateDelegate();
        }

        [Theory]
        [InlineData("sync")]
        [InlineData("download productfiles")]
        public void CanConvertArgsToRequests(string spaceSeparatedArgs)
        {
            var requests = convertArgsToRequestsDelegate.Convert(spaceSeparatedArgs.Split(" "));
            var referenceRequests = ReferenceRequests.Requests[spaceSeparatedArgs];

            Assert.Equal(requests.Count(), referenceRequests.Count());

            for (var ii=0; ii<requests.Count(); ii++)
            {
                var request = requests.ElementAt(ii);
                var referenceRequest = referenceRequests.ElementAt(ii);
                Assert.Equal(referenceRequest.Method, request.Method);
                Assert.Equal(referenceRequest.Collection, request.Collection);
            }
        }
    }
}