using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Delegates.Sort;

using Interfaces.Status;

using Attributes;

using Models.ArgsDefinitions;

namespace Delegates.Sort.Requests
{
    public class SortRequestsMethodsByOrderAsyncDelegate : ISortAsyncDelegate<string>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private ICollectionController collectionController;

        [Dependencies(
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController",
            "Controllers.Collection.CollectionController")]
        [TestDependenciesOverrides(
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "")]
        public SortRequestsMethodsByOrderAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            ICollectionController collectionController)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.collectionController = collectionController;
        }

        public async Task SortAsync(List<string> methods, IStatus status)
        {
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync(status);

            methods.Sort((string x, string y) =>
            {
                var cx = collectionController.Find(argsDefinitions.Methods, c => c.Title == x);
                var cy = collectionController.Find(argsDefinitions.Methods, c => c.Title == y);

                if (cx == null || cy == null)
                    throw new InvalidOperationException();

                return cx.Order - cy.Order;
            });
        }
    }
}