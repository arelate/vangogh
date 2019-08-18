using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Models.Requests;
using Models.ArgsDefinitions;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedDependenciesDelegate :
        IConvertDelegate<RequestsData, RequestsData>
    {
        private ArgsDefinition argsDefinitions;
        private ICollectionController collectionController;

        public ConvertRequestsDataToResolvedDependenciesDelegate(
            ArgsDefinition argsDefinitions,
            ICollectionController collectionController)
        {
            this.argsDefinitions = argsDefinitions;
            this.collectionController = collectionController;
        }        
        public RequestsData Convert(RequestsData requestsData)
        {
            var requiredMethods = new List<string>();
            var requiredCollections = new List<string>();
            
            foreach (var method in requestsData.Methods)
            {
                var dependency = collectionController.Find(
                    argsDefinitions.Dependencies,
                    d => d.Method == method);

                if (dependency == null) continue;

                if (!collectionController.ConfirmExclusive(
                    requestsData.Collections,
                    dependency.Collections))
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