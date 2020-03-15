using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Stash;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.Confirm;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Requests;
using Models.ArgsDefinitions;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedDependenciesDelegate :
        IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
    {
        private readonly IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private readonly IFindDelegate<Dependency> findDependencyDelegate;
        private readonly IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExclusiveStringDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Delegates.Find.ArgsDefinitions.FindDependencyDelegate,Delegates",
            "Delegates.Confirm.System.ConfirmExclusiveStringDelegate,Delegates")]
        [Dependencies(
            DependencyContext.Test,
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "",
            "")]
        public ConvertRequestsDataToResolvedDependenciesDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            IFindDelegate<Dependency> findDependencyDelegate,
            IConfirmDelegate<(IEnumerable<string>, IEnumerable<string>)> confirmExclusiveStringDelegate)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.findDependencyDelegate = findDependencyDelegate;
            this.confirmExclusiveStringDelegate = confirmExclusiveStringDelegate;
        }
        public async Task<RequestsData> ConvertAsync(RequestsData requestsData)
        {
            var requiredMethods = new List<string>();
            var requiredCollections = new List<string>();
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();

            foreach (var method in requestsData.Methods)
            {
                var dependency = findDependencyDelegate.Find(
                    argsDefinitions.Dependencies,
                    d => d.Method == method);

                if (dependency == null) continue;

                if (!confirmExclusiveStringDelegate.Confirm(
                    (requestsData.Collections, dependency.Collections)))
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