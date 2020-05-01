using Attributes;
using Interfaces.Delegates.Collections;
using Models.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindMethodDelegate : FindDelegate<Method>
    {
        [Dependencies(
            typeof(FindAllMethodDelegate))]
        public FindMethodDelegate(
            IFindAllDelegate<Method> findAllMethodDelegate) :
            base(findAllMethodDelegate)
        {
            // ...
        }
    }
}