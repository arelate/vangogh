using System;
using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Attributes;

namespace Delegates.Itemize.Types
{
    public abstract class ItemizeAllTypesWithClassAttributeDelegate<AttributeType> : IItemizeAllDelegate<Type>
    {
        private readonly IItemizeAllDelegate<Type> itemizeAllTypesDelegate;

        public ItemizeAllTypesWithClassAttributeDelegate(
            IItemizeAllDelegate<Type> itemizeAllTypesDelegate)
        {
            this.itemizeAllTypesDelegate = itemizeAllTypesDelegate;
        }

        public IEnumerable<Type> ItemizeAll()
        {
            foreach (var type in itemizeAllTypesDelegate.ItemizeAll())
                if (type.IsDefined(typeof(AttributeType), false))
                    yield return type;
        }
    }
}