using System;
using Attributes;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations.Types.Attributes
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