using Attributes;

using Interfaces.Delegates.Collections;

using Models.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindDependencyDelegate: FindDelegate<Dependency>
    {
        [Dependencies(
            "Delegates.Collections.ArgsDefinitions.FindAllDependencyDelegate,Delegates")]
        public FindDependencyDelegate(
            IFindAllDelegate<Dependency> findAllDependencyDelegate):
            base(findAllDependencyDelegate)
            {
                // ...
            }
    }
}