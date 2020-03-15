using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindDependencyDelegate: FindDelegate<Dependency>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ArgsDefinitions.FindAllDependencyDelegate,Delegates")]
        public FindDependencyDelegate(
            IFindAllDelegate<Dependency> findAllDependencyDelegate):
            base(findAllDependencyDelegate)
            {
                // ...
            }
    }
}