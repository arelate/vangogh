using System;
using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize.Types
{
    public abstract class ItemizeAllTypesWithCustomAttributeInAssemblyDelegate<AttributeType> : IItemizeAllDelegate<Type>
    {
        public IEnumerable<Type> ItemizeAll()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.ExportedTypes)
                    if (type.IsDefined(typeof(AttributeType), false))
                        yield return type;
        }
    }
}