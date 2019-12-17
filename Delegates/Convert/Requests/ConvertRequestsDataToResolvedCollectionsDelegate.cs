using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.Requests;
using Models.ArgsDefinitions;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedCollectionsDelegate :
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
        public ConvertRequestsDataToResolvedCollectionsDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            ICollectionController collectionController)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.collectionController = collectionController;
        }

        public async Task<RequestsData> ConvertAsync(RequestsData requestsData, IStatus status)
        {
            var defaultCollections = new List<string>();
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync(status);

            foreach (var method in requestsData.Methods)
            {
                var methodDefinition = collectionController.Find(
                    argsDefinitions.Methods,
                    m => m.Title == method);

                if (methodDefinition == null) throw new ArgumentException();

                if (collectionController.ConfirmExclusive(
                    methodDefinition.Collections,
                    requestsData.Collections) &&
                    methodDefinition.Collections != null)
                    defaultCollections.AddRange(methodDefinition.Collections);
            }

            foreach (var collection in defaultCollections)
                if (!requestsData.Collections.Contains(collection))
                    requestsData.Collections.Add(collection);

            return requestsData;
        }
    }
}