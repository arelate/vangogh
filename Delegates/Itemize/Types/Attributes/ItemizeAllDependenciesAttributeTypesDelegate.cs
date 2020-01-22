using System;

using Interfaces.Delegates.Itemize;
using Interfaces.Models.Dependencies;

using Delegates.Itemize.Types;

using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllDependenciesAttributeTypesDelegate :
        ItemizeAllTypesWithConstructorAttributeDelegate<DependenciesAttribute>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Itemize.Types.ItemizeAllAppDomainTypesDelegate,Delegates")]
        public ItemizeAllDependenciesAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate) :
            base(itemizeAllAppDomainTypesDelegate)
        {
            // ...
        }
    }
}