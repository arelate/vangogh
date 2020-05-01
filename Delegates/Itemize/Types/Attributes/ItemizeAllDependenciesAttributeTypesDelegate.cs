using System;
using Interfaces.Delegates.Itemize;
using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllDependenciesAttributeTypesDelegate :
        ItemizeAllTypesWithConstructorAttributeDelegate<DependenciesAttribute>
    {
        [Dependencies(
            typeof(ItemizeAllAppDomainTypesDelegate))]
        public ItemizeAllDependenciesAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate) :
            base(itemizeAllAppDomainTypesDelegate)
        {
            // ...
        }
    }
}