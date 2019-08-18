using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Models.Requests;
using Models.ArgsDefinitions;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestsDataToResolvedCollectionsDelegate :
        IConvertDelegate<RequestsData, RequestsData>
    {
        private ArgsDefinition argsDefinitions;
        private ICollectionController collectionController;

        public ConvertRequestsDataToResolvedCollectionsDelegate(
            ArgsDefinition argsDefinitions,
            ICollectionController collectionController)
        {
            this.argsDefinitions = argsDefinitions;
            this.collectionController = collectionController;
        }

        public RequestsData Convert(RequestsData requestsData)
        {
            var defaultCollections = new List<string>();
            foreach (var method in requestsData.Methods)
            {
                var methodDefinition = collectionController.Find(
                    argsDefinitions.Methods,
                    m => m.Title == method);

                if (methodDefinition == null) throw new ArgumentException();

                if (collectionController.ConfirmExclusive(
                    methodDefinition.Collections,
                    requestsData.Collections) &&
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