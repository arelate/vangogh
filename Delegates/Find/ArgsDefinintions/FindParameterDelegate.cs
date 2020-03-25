using Attributes;

using Interfaces.Delegates.Find;


using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindParameterDelegate: FindDelegate<Parameter>
    {
        [Dependencies(
            "Delegates.Find.ArgsDefinitions.FindAllParameterDelegate,Delegates")]
        public FindParameterDelegate(
            IFindAllDelegate<Parameter> findAllParameterDelegate):
            base(findAllParameterDelegate)
            {
                // ...
            }
    }
}