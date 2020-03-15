using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.System
{
    public class FindLongDelegate: FindDelegate<long>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.System.FindAllLongDelegate,Delegates")]
        public FindLongDelegate(
            IFindAllDelegate<long> findAllLongDelegate):
            base(findAllLongDelegate)
            {
                // ...
            }
    }
}