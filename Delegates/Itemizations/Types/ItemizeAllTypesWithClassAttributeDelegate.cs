using System;
using System.Collections.Generic;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations.Types
{
    public abstract class ItemizeAllTypesWithClassAttributeDelegate<AttributeType> : IItemizeAllDelegate<Type>
    {
        private readonly IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate;

        public ItemizeAllTypesWithClassAttributeDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate)
        {
            this.itemizeAllAppDomainTypesDelegate = itemizeAllAppDomainTypesDelegate;
        }

        public IEnumerable<Type> ItemizeAll()
        {
            var types = itemizeAllAppDomainTypesDelegate.ItemizeAll();
            foreach (var type in types)
                if (type.IsDefined(typeof(AttributeType), false))
                    yield return type;
        }
    }
}