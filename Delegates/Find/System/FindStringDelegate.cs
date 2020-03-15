using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.System
{
    public class FindStringDelegate: FindDelegate<string>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.System.FindAllStringDelegate,Delegates")]
        public FindStringDelegate(
            IFindAllDelegate<string> findAllStringDelegate):
            base(findAllStringDelegate)
            {
                // ...
            }
    }
}