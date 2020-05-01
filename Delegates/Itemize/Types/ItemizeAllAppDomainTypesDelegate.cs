using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Delegates.Itemize;

namespace Delegates.Itemize.Types
{
    public class ItemizeAllAppDomainTypesDelegate : IItemizeAllDelegate<Type>
    {
        public IEnumerable<Type> ItemizeAll()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        }
    }
}