using System;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.System
{
    public class FindTypeDelegate: FindDelegate<Type>
    {
        [Dependencies(
            "Delegates.Find.System.FindAllTypeDelegate,Delegates")]
        public FindTypeDelegate(
            IFindAllDelegate<Type> findAllTypeDelegate):
            base(findAllTypeDelegate)
            {
                // ...
            }
    }
}