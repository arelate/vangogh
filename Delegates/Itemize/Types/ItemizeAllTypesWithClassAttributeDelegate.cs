using System;
using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Attributes;

namespace Delegates.Itemize.Types
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
            foreach (var type in itemizeAllAppDomainTypesDelegate.ItemizeAll())
                if (type.IsDefined(typeof(AttributeType), false))
                    yield return type;
        }
    }
}