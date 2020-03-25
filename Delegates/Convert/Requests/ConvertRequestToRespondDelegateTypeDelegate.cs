using System;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Find;


using Attributes;

using Models.Requests;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestToRespondDelegateTypeDelegate : IConvertDelegate<Request, Type>
    {
        private readonly IFindDelegate<Type> findTypeDelegate;
        private readonly IItemizeAllDelegate<Type> itemizeAllRespondDelegateTypesDelegate;

        [Dependencies(
            "Delegates.Find.System.FindTypeDelegate,Delegates",
            "Delegates.Itemize.Types.Attributes.ItemizeAllRespondsToRequestsAttributeTypesDelegate,Delegates")]
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

                    return (respondsToRequestAttribute.Method == request.Method &&
                        respondsToRequestAttribute.Collection == request.Collection) ||
                        (respondsToRequestAttribute.Method == request.Method &&
                        string.IsNullOrEmpty(respondsToRequestAttribute.Collection) && string.IsNullOrEmpty(request.Collection));
                });

            return respondDelegate;
        }
    }
}