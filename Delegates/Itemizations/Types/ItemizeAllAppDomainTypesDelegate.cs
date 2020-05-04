using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations.Types
{
    public class ItemizeAllAppDomainTypesDelegate : IItemizeAllDelegate<Type>
    {
        public IEnumerable<Type> ItemizeAll()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        }
    }
}