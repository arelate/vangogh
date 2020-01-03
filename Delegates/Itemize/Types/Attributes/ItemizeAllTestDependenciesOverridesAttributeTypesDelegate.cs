using System;

using Interfaces.Delegates.Itemize;

using Delegates.Itemize.Types;

using Attributes;

namespace Delegates.Itemize.Types.Attributes
{
    public class ItemizeAllTestDependenciesOverridesAttributeTypesDelegate :
        ItemizeAllTypesWithConstructorAttributeDelegate<TestDependenciesOverridesAttribute>
    {
        [Dependencies("Delegates.Itemize.Types.ItemizeAllTypesDelegate,Delegates")]
        public ItemizeAllTestDependenciesOverridesAttributeTypesDelegate(
            IItemizeAllDelegate<Type> itemizeAllTypesDelegate) :
            base(itemizeAllTypesDelegate)
        {
            // ...
        }
    }
}