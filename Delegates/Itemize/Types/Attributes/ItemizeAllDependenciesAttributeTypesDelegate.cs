using System;

using Interfaces.Delegates.Itemize;


using Delegates.Itemize.Types;

using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllDependenciesAttributeTypesDelegate :
        ItemizeAllTypesWithConstructorAttributeDelegate<DependenciesAttribute>
    {
        [Dependencies(
            "Delegates.Itemize.Types.ItemizeAllAppDomainTypesDelegate,Delegates")]
        public ItemizeAllDependenciesAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate) :
            base(itemizeAllAppDomainTypesDelegate)
        {
            // ...
        }
    }
}