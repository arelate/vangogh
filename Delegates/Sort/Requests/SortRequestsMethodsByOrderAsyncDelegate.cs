using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Interfaces.Delegates.Sort;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;


using Attributes;

using Models.ArgsDefinitions;

namespace Delegates.Sort.Requests
{
    public class SortRequestsMethodsByOrderAsyncDelegate : ISortAsyncDelegate<string>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate;
        private IFindDelegate<Method> findMethodDelegate;

        [Dependencies(
            "Delegates.GetData.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Find.ArgsDefinitions.FindMethodDelegate,Delegates")]
        public SortRequestsMethodsByOrderAsyncDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<Method> findMethodDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodDelegate = findMethodDelegate;
        }

        public async Task SortAsync(List<string> methods)
        {
            var argsDefinitions = await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync();

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