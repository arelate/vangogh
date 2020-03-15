using System;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.System
{
    public class FindTypeDelegate: FindDelegate<Type>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.System.FindAllTypeDelegate,Delegates")]
        public FindTypeDelegate(
            IFindAllDelegate<Type> findAllTypeDelegate):
            base(findAllTypeDelegate)
            {
                // ...
            }
    }
}