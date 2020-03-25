using Attributes;

using Interfaces.Delegates.Find;


using Models.ArgsDefinitions;

namespace Delegates.Find.ArgsDefinitions
{
    public class FindDependencyDelegate: FindDelegate<Dependency>
    {
        [Dependencies(
            "Delegates.Find.ArgsDefinitions.FindAllDependencyDelegate,Delegates")]
        public FindDependencyDelegate(
            IFindAllDelegate<Dependency> findAllDependencyDelegate):
            base(findAllDependencyDelegate)
            {
                // ...
            }
    }
}