using Attributes;
using Interfaces.Delegates.Collections;
using Models.ArgsDefinitions;

namespace Delegates.Collections.ArgsDefinitions
{
    public class FindDependencyDelegate : FindDelegate<Dependency>
    {
        [Dependencies(
            typeof(FindAllDependencyDelegate))]
        public FindDependencyDelegate(
            IFindAllDelegate<Dependency> findAllDependencyDelegate) :
            base(findAllDependencyDelegate)
        {
            // ...
        }
    }
}