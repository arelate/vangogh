using System;
using System.Collections.Generic;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations.Types
{
    public abstract class ItemizeAllTypesWithConstructorAttributeDelegate<AttributeType> : IItemizeAllDelegate<Type>
    {
        private readonly IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate;

        public ItemizeAllTypesWithConstructorAttributeDelegate(
            IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate)
        {
            this.itemizeAllAppDomainTypesDelegate = itemizeAllAppDomainTypesDelegate;
        }

        public IEnumerable<Type> ItemizeAll()
        {
            foreach (var type in itemizeAllAppDomainTypesDelegate.ItemizeAll())
            foreach (var constructorInfo in type.GetConstructors())
                if (constructorInfo.IsDefined(typeof(AttributeType), true))
                    yield return type;
        }
    }
}