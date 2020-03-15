using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindParameterDelegate: FindDelegate<Parameter>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ArgsDefinitions.FindAllParameterDelegate,Delegates")]
        public FindParameterDelegate(
            IFindAllDelegate<Parameter> findAllParameterDelegate):
            base(findAllParameterDelegate)
            {
                // ...
            }
    }
}