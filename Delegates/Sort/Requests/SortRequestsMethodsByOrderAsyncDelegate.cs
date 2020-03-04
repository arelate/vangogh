using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Delegates.Sort;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ArgsDefinitions;

namespace Delegates.Sort.Requests
{
    public class SortRequestsMethodsByOrderAsyncDelegate : ISortAsyncDelegate<string>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private ICollectionController collectionController;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Controllers.Collection.CollectionController,Controllers")]
        [Dependencies(
            DependencyContext.Test,
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "")]
        public SortRequestsMethodsByOrderAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            ICollectionController collectionController)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.collectionController = collectionController;
        }

        public async Task SortAsync(List<string> methods)
        {
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();

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