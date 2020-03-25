using System;
using System.Linq;
using System.Collections.Generic;


using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.Intersect;


using Attributes;

using Models.ArgsDefinitions;
using Models.Requests;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToRequestsDelegate :
        IConvertAsyncDelegate<RequestsData, IAsyncEnumerable<Request>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate;
        private IFindDelegate<Method> findMethodDelegate;
        private IIntersectDelegate<string> intersectStringDelegate;

        [Dependencies(
            "Delegates.GetData.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Find.ArgsDefinitions.FindMethodDelegate,Delegates",
            "Delegates.Intersect.System.IntersectStringDelegate,Delegates")]           
        public ConvertRequestsDataToRequestsDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<Method> findMethodDelegate,
            IIntersectDelegate<string> intersectStringDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodDelegate = findMethodDelegate;
            this.intersectStringDelegate = intersectStringDelegate;
        }

        public async IAsyncEnumerable<Request> ConvertAsync(RequestsData requestsData)
        {
            var requests = new List<Request>();
            var argsDefinitions = await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync();

            foreach (var method in requestsData.Methods)
            {
                var methodDefinition = findMethodDelegate.Find(
                    argsDefinitions.Methods,
                    m => m.Title == method);

                if (methodDefinition == null)
                    throw new ArgumentException();

                var methodCollections = intersectStringDelegate.Intersect(
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
                    yield return request;
                }

                foreach (var collection in methodCollections)
                {
                    var request = new Request()
                    {
                        Method = method,
                        Collection = collection,
                        Parameters = methodParameters
                    };
                    yield return request;
                }
            }
        }
    }
}