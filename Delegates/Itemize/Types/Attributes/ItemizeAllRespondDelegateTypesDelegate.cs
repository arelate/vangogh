using System;

using Interfaces.Delegates.Itemize;

using Delegates.Itemize.Types;

using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllRespondsToRequestsAttributeTypesDelegate :
        ItemizeAllTypesWithClassAttributeDelegate<RespondsToRequests>
    {
        [Dependencies("Delegates.Itemize.Types.ItemizeAllTypesDelegate,Delegates")]
        public ItemizeAllRespondsToRequestsAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllTypesDelegate):
            base(itemizeAllTypesDelegate)
        {
            // ...
        }
    }
}