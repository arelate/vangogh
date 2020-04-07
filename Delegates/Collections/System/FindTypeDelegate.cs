using System;
using Attributes;

using Interfaces.Delegates.Collections;

namespace Delegates.Collections.System
{
    public class FindTypeDelegate: FindDelegate<Type>
    {
        [Dependencies(
            "Delegates.Collections.System.FindAllTypeDelegate,Delegates")]
        public FindTypeDelegate(
            IFindAllDelegate<Type> findAllTypeDelegate):
            base(findAllTypeDelegate)
            {
                // ...
            }
    }
}