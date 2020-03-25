using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.Confirm;

using Attributes;

using Models.Requests;
using Models.ArgsDefinitions;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedCollectionsDelegate :
        IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
    {
        private readonly IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate;
        private readonly IFindDelegate<Method> findMethodDelegate;
        private readonly IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExlusiveStringDelegate;

        [Dependencies(
            "Delegates.GetData.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Find.ArgsDefinitions.FindMethodDelegate,Delegates",
            "Delegates.Confirm.System.ConfirmExclusiveStringDelegate,Delegates")]
        public ConvertRequestsDataToResolvedCollectionsDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate,
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
            var argsDefinitions = await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync();

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