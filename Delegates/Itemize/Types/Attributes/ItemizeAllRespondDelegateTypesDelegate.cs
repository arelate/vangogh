using System;
using Interfaces.Delegates.Itemize;
using Delegates.Itemize.Types;
using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllRespondsToRequestsAttributeTypesDelegate :
        ItemizeAllTypesWithClassAttributeDelegate<RespondsToRequests>
    {
        [Dependencies(
            typeof(ItemizeAllAppDomainTypesDelegate))]
        public ItemizeAllRespondsToRequestsAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate) :
            base(itemizeAllAppDomainTypesDelegate)
        {
            // ...
        }
    }
}