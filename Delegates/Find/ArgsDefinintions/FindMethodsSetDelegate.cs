using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindMethodsSetDelegate: FindDelegate<MethodsSet>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ArgsDefinitions.FindAllMethodsSetDelegate,Delegates")]
        public FindMethodsSetDelegate(
            IFindAllDelegate<MethodsSet> findAllMethodsSetDelegate):
            base(findAllMethodsSetDelegate)
            {
                // ...
            }
    }
}