using System;
using Attributes;
using Delegates.Itemizations.Types.Attributes;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Itemizations;
using Models.Requests;

namespace Delegates.Conversions.Requests
{
    public class ConvertRequestToRespondDelegateTypeDelegate : IConvertDelegate<Request, Type>
    {
        private readonly IFindDelegate<Type> findTypeDelegate;
        private readonly IItemizeAllDelegate<Type> itemizeAllRespondDelegateTypesDelegate;

        [Dependencies(
            typeof(Delegates.Collections.System.FindTypeDelegate),
            typeof(ItemizeAllRespondsToRequestsAttributeTypesDelegate))]
        public ConvertRequestToRespondDelegateTypeDelegate(
            IFindDelegate<Type> findTypeDelegate,
            IItemizeAllDelegate<Type> itemizeAllRespondDelegateTypesDelegate)
        {
            this.findTypeDelegate = findTypeDelegate;
            this.itemizeAllRespondDelegateTypesDelegate = itemizeAllRespondDelegateTypesDelegate;
        }

        public Type Convert(Request request)
        {
            var respondDelegateTypes = itemizeAllRespondDelegateTypesDelegate.ItemizeAll();

            var respondDelegate = findTypeDelegate.Find(respondDelegateTypes,
                delegateType =>
                {
                    var respondsToRequestAttribute = Attribute.GetCustomAttribute(
                            delegateType,
                            typeof(RespondsToRequests))
                        as RespondsToRequests;

                    return respondsToRequestAttribute.Method == request.Method &&
                           respondsToRequestAttribute.Collection == request.Collection ||
                           respondsToRequestAttribute.Method == request.Method &&
                           string.IsNullOrEmpty(respondsToRequestAttribute.Collection) &&
                           string.IsNullOrEmpty(request.Collection);
                });

            return respondDelegate;
        }
    }
}