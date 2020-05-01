using Attributes;
using Interfaces.Delegates.Collections;
using Models.ArgsDefinitions;
using Delegates.Collections.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindParameterDelegate : FindDelegate<Parameter>
    {
        [Dependencies(
            typeof(FindAllParameterDelegate))]
        public FindParameterDelegate(
            IFindAllDelegate<Parameter> findAllParameterDelegate) :
            base(findAllParameterDelegate)
        {
            // ...
        }
    }
}