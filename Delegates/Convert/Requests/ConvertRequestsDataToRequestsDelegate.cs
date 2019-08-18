using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Models.ArgsDefinitions;
using Models.Requests;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToRequestsDelegate :
        IConvertDelegate<RequestsData, IEnumerable<Request>>
    {
        private ArgsDefinition argsDefinitions;
        private ICollectionController collectionController;

        public ConvertRequestsDataToRequestsDelegate(
            ArgsDefinition argsDefinitions,
            ICollectionController collectionController)
        {
            this.argsDefinitions = argsDefinitions;
            this.collectionController = collectionController;
        }

        public IEnumerable<Request> Convert(RequestsData requestsData)
        {
            var requests = new List<Request>();

            foreach (var method in requestsData.Methods)
            {
                var methodDefinition = collectionController.Find(
                    argsDefinitions.Methods,
                    m => m.Title == method);

                if (methodDefinition == null)
                    throw new ArgumentException();

                var methodCollections = collectionController.Intersect(
                    requestsData.Collections,
                    methodDefinition.Collections);

                // parameters
                var methodParameters = new Dictionary<string, IEnumerable<string>>();
                foreach (var parameter in requestsData.Parameters.Keys)
                    if (methodDefinition.Parameters.Contains(parameter))
                        methodParameters.Add(parameter, requestsData.Parameters[parameter]);

                // add method without collections if method definition supports that
                if (methodDefinition.Collections == null ||
                    methodDefinition.Collections.Length == 0)
                {
                    var request = new Request()
                    {
                        Method = method,
                        Collection = string.Empty,
                        Parameters = methodParameters
                    };
                    requests.Add(request);
                }

                foreach (var collection in methodCollections)
                {
                    var request = new Request()
                    {
                        Method = method,
                        Collection = collection,
                        Parameters = methodParameters
                    };
                    requests.Add(request);
                }
            }

            return requests;
        }
    }
}