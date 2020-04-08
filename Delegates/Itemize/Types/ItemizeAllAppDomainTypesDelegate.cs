using System;
using System.Collections.Generic;
using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize.Types
{
    public class ItemizeAllAppDomainTypesDelegate : IItemizeAllDelegate<Type>
    {
        public IEnumerable<Type> ItemizeAll()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
                yield return type;
        }
    }
}