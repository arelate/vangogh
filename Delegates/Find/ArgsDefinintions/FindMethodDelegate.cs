using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindMethodDelegate: FindDelegate<Method>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ArgsDefinitions.FindAllMethodDelegate,Delegates")]
        public FindMethodDelegate(
            IFindAllDelegate<Method> findAllMethodDelegate):
            base(findAllMethodDelegate)
            {
                // ...
            }
    }
}