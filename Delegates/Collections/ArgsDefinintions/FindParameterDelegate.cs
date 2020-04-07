using Attributes;

using Interfaces.Delegates.Collections;

using Models.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindParameterDelegate: FindDelegate<Parameter>
    {
        [Dependencies(
            "Delegates.Collections.ArgsDefinitions.FindAllParameterDelegate,Delegates")]
        public FindParameterDelegate(
            IFindAllDelegate<Parameter> findAllParameterDelegate):
            base(findAllParameterDelegate)
            {
                // ...
            }
    }
}