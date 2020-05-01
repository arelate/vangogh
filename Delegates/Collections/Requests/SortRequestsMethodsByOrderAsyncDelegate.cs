using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Delegates.Collections.ArgsDefinitions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Collections;
using Models.ArgsDefinitions;
using Delegates.Data.Storage.ArgsDefinitions;

namespace Delegates.Collections.Requests
{
    public class SortRequestsMethodsByOrderAsyncDelegate : ISortAsyncDelegate<string>
    {
        private IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate;
        private IFindDelegate<Method> findMethodDelegate;

        [Dependencies(
            typeof(GetArgsDefinitionsDataFromPathAsyncDelegate),
            typeof(FindMethodDelegate))]
        public SortRequestsMethodsByOrderAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<Method> findMethodDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodDelegate = findMethodDelegate;
        }

        public async Task SortAsync(List<string> methods)
        {
            var argsDefinitions = 
                await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync(string.Empty);

            methods.Sort((x, y) =>
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