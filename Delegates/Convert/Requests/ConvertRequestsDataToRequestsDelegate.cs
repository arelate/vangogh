using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Controllers.Stash;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.Intersect;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ArgsDefinitions;
using Models.Requests;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToRequestsDelegate :
        IConvertAsyncDelegate<RequestsData, IAsyncEnumerable<Request>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private IFindDelegate<Method> findMethodDelegate;
        private IIntersectDelegate<string> intersectStringDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Delegates.Find.ArgsDefinitions.FindMethodDelegate,Delegates",
            "Delegates.Intersect.System.IntersectStringDelegate,Delegates")]
            [Dependencies(
            DependencyContext.Test,
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "",
            "")]            
        public ConvertRequestsDataToRequestsDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            IFindDelegate<Method> findMethodDelegate,
            IIntersectDelegate<string> intersectStringDelegate)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.findMethodDelegate = findMethodDelegate;
            this.intersectStringDelegate = intersectStringDelegate;
        }

        public async IAsyncEnumerable<Request> ConvertAsync(RequestsData requestsData)
        {
            var requests = new List<Request>();
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();

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