using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.Requests;
using Models.ArgsDefinitions;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedDependenciesDelegate :
        IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private ICollectionController collectionController;

        [Dependencies(
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Controllers.Collection.CollectionController,Controllers")]
        [TestDependenciesOverrides(
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "")]
        public ConvertRequestsDataToResolvedDependenciesDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            ICollectionController collectionController)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.collectionController = collectionController;
        }
        public async Task<RequestsData> ConvertAsync(RequestsData requestsData)
        {
            var requiredMethods = new List<string>();
            var requiredCollections = new List<string>();
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();

            foreach (var method in requestsData.Methods)
            {
                var dependency = collectionController.Find(
                    argsDefinitions.Dependencies,
                    d => d.Method == method);

                if (dependency == null) continue;

                if (!collectionController.ConfirmExclusive(
                    requestsData.Collections,
                    dependency.Collections))
                {
                    foreach (var requirement in dependency.Requires)
                    {
                        requiredMethods.Add(requirement.Method);
                        requiredCollections.AddRange(requirement.Collections);
                    }
                }
            }

            foreach (var requiredMethod in requiredMethods)
                if (!requestsData.Methods.Contains(requiredMethod))
                    requestsData.Methods.Add(requiredMethod);

            foreach (var requiredCollection in requiredCollections)
                if (!requestsData.Collections.Contains(requiredCollection))
                    requestsData.Collections.Add(requiredCollection);

            return requestsData;
        }
    }
}