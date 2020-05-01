using Attributes;
using Interfaces.Delegates.Collections;
using Models.ArgsDefinitions;
using Delegates.Collections.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindMethodsSetDelegate : FindDelegate<MethodsSet>
    {
        [Dependencies(
            typeof(FindAllMethodsSetDelegate))]
        public FindMethodsSetDelegate(
            IFindAllDelegate<MethodsSet> findAllMethodsSetDelegate) :
            base(findAllMethodsSetDelegate)
        {
            // ...
        }
    }
}