using Attributes;
using Interfaces.Delegates.Collections;
using Models.ArgsDefinitions;

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