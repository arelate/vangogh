using System;

using Interfaces.Delegates.Itemize;
using Interfaces.Models.Dependencies;

using Delegates.Itemize.Types;

using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllRespondsToRequestsAttributeTypesDelegate :
        ItemizeAllTypesWithClassAttributeDelegate<RespondsToRequests>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Itemize.Types.ItemizeAllAppDomainTypesDelegate,Delegates")]
        public ItemizeAllRespondsToRequestsAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate):
            base(itemizeAllAppDomainTypesDelegate)
        {
            // ...
        }
    }
}