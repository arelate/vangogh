using System;
using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Attributes;

namespace Delegates.Itemize.Types
{
    public abstract class ItemizeAllTypesWithConstructorAttributeDelegate<AttributeType> : IItemizeAllDelegate<Type>
    {
        private readonly IItemizeAllDelegate<Type> itemizeAllTypesDelegate;

        public ItemizeAllTypesWithConstructorAttributeDelegate(
            IItemizeAllDelegate<Type> itemizeAllTypesDelegate)
        {
            this.itemizeAllTypesDelegate = itemizeAllTypesDelegate;
        }
        public IEnumerable<Type> ItemizeAll()
        {
            foreach (var type in itemizeAllTypesDelegate.ItemizeAll())
                foreach (var constructorInfo in type.GetConstructors())
                    if (constructorInfo.IsDefined(typeof(AttributeType), true))
                        yield return type;
        }
    }
}