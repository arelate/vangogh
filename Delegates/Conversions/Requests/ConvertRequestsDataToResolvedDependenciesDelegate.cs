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
    public class ConvertRequestsDataToResolvedDependenciesDelegate :
        IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
    {
        private readonly IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate;
        private readonly IFindDelegate<Dependency> findDependencyDelegate;
        private readonly IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExclusiveStringDelegate;

        [Dependencies(
            typeof(Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate),
            typeof(FindDependencyDelegate),
            typeof(ConfirmExclusiveStringDelegate))]
        public ConvertRequestsDataToResolvedDependenciesDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<Dependency> findDependencyDelegate,
            IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExclusiveStringDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findDependencyDelegate = findDependencyDelegate;
            this.confirmExclusiveStringDelegate = confirmExclusiveStringDelegate;
        }

        public async Task<RequestsData> ConvertAsync(RequestsData requestsData)
        {
            var requiredMethods = new List<string>();
            var requiredCollections = new List<string>();
            var argsDefinitions = 
                await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync(string.Empty);

            foreach (var method in requestsData.Methods)
            {
                var dependency = findDependencyDelegate.Find(
                    argsDefinitions.Dependencies,
                    d => d.Method == method);

                if (dependency == null) continue;

                if (!confirmExclusiveStringDelegate.Confirm(
                    (requestsData.Collections, dependency.Collections)))
                    foreach (var requirement in dependency.Requires)
                    {
                        requiredMethods.Add(requirement.Method);
                        requiredCollections.AddRange(requirement.Collections);
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