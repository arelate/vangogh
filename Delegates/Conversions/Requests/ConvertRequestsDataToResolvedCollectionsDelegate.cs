using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Delegates.Collections.ArgsDefinitions;
using Delegates.Confirmations.System;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ArgsDefinitions;
using Models.Requests;

namespace Delegates.Conversions.Requests
{
    public class ConvertRequestsDataToResolvedCollectionsDelegate :
        IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
    {
        private readonly IGetDataAsyncDelegate<ArgsDefinition,string> getArgsDefinitionsDataFromPathAsyncDelegate;
        private readonly IFindDelegate<Method> findMethodDelegate;
        private readonly IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExlusiveStringDelegate;

        [Dependencies(
            typeof(Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate),
            typeof(FindMethodDelegate),
            typeof(ConfirmExclusiveStringDelegate))]
        public ConvertRequestsDataToResolvedCollectionsDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<Method> findMethodDelegate,
            IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExlusiveStringDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodDelegate = findMethodDelegate;
            this.confirmExlusiveStringDelegate = confirmExlusiveStringDelegate;
        }

        public async Task<RequestsData> ConvertAsync(RequestsData requestsData)
        {
            var defaultCollections = new List<string>();
            var argsDefinitions = 
                await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync(string.Empty);

            foreach (var method in requestsData.Methods)
            {
                var methodDefinition = findMethodDelegate.Find(
                    argsDefinitions.Methods,
                    m => m.Title == method);

                if (methodDefinition == null) throw new ArgumentException();

                if (confirmExlusiveStringDelegate.Confirm(
                        (methodDefinition.Collections, requestsData.Collections)) &&
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