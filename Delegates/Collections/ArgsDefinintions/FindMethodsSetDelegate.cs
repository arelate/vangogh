using Attributes;

using Interfaces.Delegates.Collections;

using Models.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindMethodsSetDelegate: FindDelegate<MethodsSet>
    {
        [Dependencies(
            "Delegates.Collections.ArgsDefinitions.FindAllMethodsSetDelegate,Delegates")]
        public FindMethodsSetDelegate(
            IFindAllDelegate<MethodsSet> findAllMethodsSetDelegate):
            base(findAllMethodsSetDelegate)
            {
                // ...
            }
    }
}