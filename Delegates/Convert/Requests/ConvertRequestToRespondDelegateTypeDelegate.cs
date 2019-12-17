using System;

using Interfaces.Controllers.Collection;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;

using Attributes;

using Models.Requests;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestToRespondDelegateTypeDelegate : IConvertDelegate<Request, Type>
    {
        private readonly ICollectionController collectionController;
        private readonly IItemizeAllDelegate<Type> itemizeAllRespondDelegateTypesDelegate;

        [Dependencies(
            "Controllers.Collection.CollectionController,Controllers",
            "GOG.Delegates.Itemize.Types.ItemizeAllRespondDelegateTypesDelegate,GOG.Delegates")]
        public ConvertRequestToRespondDelegateTypeDelegate(
            ICollectionController collectionController,
            IItemizeAllDelegate<Type> itemizeAllRespondDelegateTypesDelegate)
        {
            this.collectionController = collectionController;
            this.itemizeAllRespondDelegateTypesDelegate = itemizeAllRespondDelegateTypesDelegate;
        }

        public Type Convert(Request request)
        {
            var respondDelegateTypes = itemizeAllRespondDelegateTypesDelegate.ItemizeAll();

            var respondDelegate = collectionController.Find(respondDelegateTypes,
                delegateType =>
                {
                    var respondsToRequestAttribute = Attribute.GetCustomAttribute(
                        delegateType,
                        typeof(RespondsToRequests))
                        as RespondsToRequests;

                    return (respondsToRequestAttribute.Method == request.Method &&
                        respondsToRequestAttribute.Collection == request.Collection);
                });

            return respondDelegate;
        }
    }
}