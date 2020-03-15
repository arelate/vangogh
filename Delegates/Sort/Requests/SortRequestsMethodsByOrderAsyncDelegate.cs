using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Interfaces.Controllers.Stash;
using Interfaces.Delegates.Sort;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ArgsDefinitions;

namespace Delegates.Sort.Requests
{
    public class SortRequestsMethodsByOrderAsyncDelegate : ISortAsyncDelegate<string>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private IFindDelegate<Method> findMethodDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Delegates.Find.ArgsDefinitions.FindMethodDelegate,Delegates")]
        [Dependencies(
            DependencyContext.Test,
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "")]
        public SortRequestsMethodsByOrderAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            IFindDelegate<Method> findMethodDelegate)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.findMethodDelegate = findMethodDelegate;
        }

        public async Task SortAsync(List<string> methods)
        {
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();

            methods.Sort((string x, string y) =>
            {
                var cx = findMethodDelegate.Find(argsDefinitions.Methods, c => c.Title == x);
                var cy = findMethodDelegate.Find(argsDefinitions.Methods, c => c.Title == y);

                if (cx == null || cy == null)
                    throw new InvalidOperationException();

                return cx.Order - cy.Order;
            });
        }
    }
}